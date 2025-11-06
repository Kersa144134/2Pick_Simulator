// ======================================================
// CardRandomPicker.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-06
// 概要       : カードのランダム抽選を管理するクラス
//              レアリティ不足時は低レアリティから順に範囲を拡張して再抽選を行う
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;

namespace CardGame.PickSystem.Controller
{
    /// <summary>
    /// カードのランダム抽選処理を担当するクラス  
    /// CardDatabase 内の重みデータを参照して抽選を行う
    /// </summary>
    public class CardRandomPicker
    {
        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 指定したクラスとレアリティに基づいてカードをランダム抽選する。  
        /// 指定枚数に満たない場合は、未指定の低レアリティを順に追加して再抽選を行う。
        /// </summary>
        /// <param name="db">カードデータベース参照</param>
        /// <param name="count">抽選枚数</param>
        /// <param name="classes">対象クラス配列</param>
        /// <param name="rarities">対象レアリティ配列</param>
        /// <returns>抽選結果のカードリスト</returns>
        public List<CardData> GetRandomCardsByClassAndRarity(
            CardDatabase db,
            int count,
            CardData.CardClass[] classes,
            CardData.CardRarity[] rarities
        )
        {
            // 抽選結果を格納するリスト
            List<CardData> result = new List<CardData>();

            // 不正パラメータの場合は空のリストを返す
            if (db == null || count <= 0 || classes == null || classes.Length == 0 || rarities == null || rarities.Length == 0)
            {
                return result;
            }

            // --------------------------------------------------
            // 指定クラスに該当するカードのみを抽出
            // --------------------------------------------------
            List<CardData> classFiltered = CardQueryManager.GetCardsByClass(db, classes);

            // 残り抽選枚数を追跡
            int remaining = count;

            // 抽選対象のレアリティ候補リスト
            List<CardData.CardRarity> remainingRarities = new List<CardData.CardRarity>(rarities);

            // --------------------------------------------------
            // 必要枚数が揃うまで抽選を繰り返す
            // --------------------------------------------------
            while (remaining > 0)
            {
                // レアリティ候補が尽きた場合は次のレアリティ層を追加して再抽選
                if (remainingRarities.Count == 0)
                {
                    CardData.CardRarity[] nextRarities = GetNextAvailableRarities(rarities);

                    if (nextRarities != null)
                    {
                        return GetRandomCardsByClassAndRarity(db, count, classes, nextRarities);
                    }

                    // すべてのレアリティを試しても不足する場合は終了
                    return result;
                }

                // --------------------------------------------------
                // データベース内のレアリティ重みに基づき1つ選択（重み0は除外）
                // --------------------------------------------------
                CardData.CardRarity? selectedRarity = PickRarityWeighted(db, remainingRarities.ToArray());

                // null の場合は残り候補からすべて重み0なので除外
                if (!selectedRarity.HasValue)
                {
                    // 重み0のレアリティをすべて除外
                    remainingRarities.RemoveAll(r => db.RarityWeights.ContainsKey(r) && db.RarityWeights[r] <= 0f);
                    continue;
                }

                // 選ばれたレアリティのカードを抽出
                List<CardData> rarityFiltered = CardQueryManager.GetCardsByRarity(db, selectedRarity.Value);

                // クラス条件と重複を考慮した候補リストを作成
                List<CardData> candidateCards = new List<CardData>();
                foreach (CardData card in rarityFiltered)
                {
                    if (classFiltered.Contains(card) && !result.Contains(card))
                    {
                        candidateCards.Add(card);
                    }
                }

                // 対象カードが存在しない場合は該当レアリティを除外し、次のループへ
                if (candidateCards.Count == 0)
                {
                    remainingRarities.Remove(selectedRarity.Value);
                    continue;
                }

                // --------------------------------------------------
                // 重み付け抽選でカードを1枚選出
                // --------------------------------------------------
                CardData picked = PickCardWeighted(db, candidateCards);

                // 抽選成功時に結果へ追加し、残り数を減算
                if (picked != null)
                {
                    result.Add(picked);
                    remaining--;
                }
            }

            // 完了した抽選結果を返却
            return result;
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// 現在指定されているレアリティ配列から  
        /// 未指定の低レアリティを1つ追加して新しい配列を返す。
        /// </summary>
        /// <param name="current">現在のレアリティ配列</param>
        /// <returns>拡張されたレアリティ配列、または null</returns>
        private CardData.CardRarity[] GetNextAvailableRarities(CardData.CardRarity[] current)
        {
            // 全レアリティ順リストを定義
            CardData.CardRarity[] all = new CardData.CardRarity[]
            {
                CardData.CardRarity.Bronze,
                CardData.CardRarity.Silver,
                CardData.CardRarity.Gold,
                CardData.CardRarity.Legend
            };

            // すでに全レアリティを使用している場合は終了
            if (current.Length >= 4)
            {
                return null;
            }

            // 未指定のレアリティを探索
            List<CardData.CardRarity> available = new List<CardData.CardRarity>();
            foreach (CardData.CardRarity rarity in all)
            {
                bool exists = false;

                // 現在の配列に存在するかをチェック
                foreach (CardData.CardRarity c in current)
                {
                    if (c == rarity)
                    {
                        exists = true;
                        break;
                    }
                }

                // 存在しないものを候補に追加
                if (!exists)
                {
                    available.Add(rarity);
                }
            }

            // 最も低い未指定レアリティを追加して返す
            if (available.Count > 0)
            {
                CardData.CardRarity next = available[0];
                List<CardData.CardRarity> expanded = new List<CardData.CardRarity>(current);
                expanded.Add(next);
                return expanded.ToArray();
            }

            return null;
        }

        /// <summary>
        /// データベース内のレアリティ重みを参照して、  
        /// 複数レアリティから1つをランダムに選出する（重み0は除外）。
        /// </summary>
        /// <param name="db">カードデータベース</param>
        /// <param name="rarities">レアリティ候補配列</param>
        /// <returns>選ばれたレアリティ、全て0なら null を返す</returns>
        private CardData.CardRarity? PickRarityWeighted(CardDatabase db, CardData.CardRarity[] rarities)
        {
            // 各レアリティごとの重みを取得し、辞書に格納
            Dictionary<CardData.CardRarity, float> weights = new Dictionary<CardData.CardRarity, float>();
            foreach (CardData.CardRarity rarity in rarities)
            {
                float weight = 1f;
                if (db.RarityWeights.ContainsKey(rarity))
                {
                    weight = db.RarityWeights[rarity];
                }

                // 重み0のレアリティは抽選対象外
                if (weight > 0f)
                {
                    weights[rarity] = weight;
                }
            }

            // 有効なレアリティがなければ null を返す
            if (weights.Count == 0)
            {
                Debug.Log("[CardRandomPicker] 重み0以上のレアリティが存在しないため抽選不可");
                return null;
            }

            // 合計重みを計算
            float totalWeight = 0f;
            foreach (var w in weights.Values)
            {
                totalWeight += w;
            }

            // ランダム抽選
            float randomValue = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var pair in weights)
            {
                cumulative += pair.Value;
                if (randomValue <= cumulative)
                {
                    return pair.Key;
                }
            }

            // 万一選ばれなかった場合、最後の要素を返す
            return new List<CardData.CardRarity>(weights.Keys)[^1];
        }

        /// <summary>
        /// カードリストから重み付けに基づき1枚を選択する。  
        /// パック番号やクラスに応じて抽選確率を変動させる。
        /// </summary>
        /// <param name="db">カードデータベース</param>
        /// <param name="cards">抽選候補カードリスト</param>
        /// <returns>選ばれたカード</returns>
        private CardData PickCardWeighted(CardDatabase db, List<CardData> cards)
        {
            // 全体の重み合計を追跡
            float total = 0f;

            // 各カードごとの重み値を格納
            Dictionary<CardData, float> weights = new Dictionary<CardData, float>();

            // 各カードの特性に応じて重みを設定
            foreach (CardData card in cards)
            {
                float weight = 1.0f;

                // 最新パックのカードは出やすくする
                if (card.PackNumber == db.LatestPackNumber)
                {
                    weight *= db.LatestPackWeight;
                }

                // ニュートラルカードはやや出にくくする
                if (card.ClassType == CardData.CardClass.Neutral)
                {
                    weight *= db.NeutralCardWeight;
                }

                weights[card] = weight;
                total += weight;
            }

            // 重み合計に基づくランダム選択処理
            float randomValue = Random.Range(0f, total);
            float cumulative = 0f;

            foreach (KeyValuePair<CardData, float> pair in weights)
            {
                cumulative += pair.Value;

                // 累積値が閾値を超えたカードを返す
                if (randomValue <= cumulative)
                {
                    return pair.Key;
                }
            }

            // 万一選ばれなかった場合、最初のカードを返す
            if (cards.Count > 0)
            {
                return cards[0];
            }

            // 候補が存在しない場合は null を返す
            return null;
        }
    }
}