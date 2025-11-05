// ======================================================
// CardRandomPicker.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-05
// 概要       : カードのランダム抽選を管理
//              レアリティ不足時は未指定レアリティを低い順に追加して再抽選
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;

namespace CardGame.PickSystem.Controller
{
    /// <summary>
    /// カードのランダム抽選を管理
    /// </summary>
    public static class CardRandomPicker
    {
        // ======================================================
        // 定数
        // ======================================================

        private const float LATEST_PACK_WEIGHT = 1.2f;
        private const float NEUTRAL_CARD_WEIGHT = 0.1f;
        private const float BRONZE_WEIGHT = 1.0f;
        private const float SILVER_WEIGHT = 1.0f;
        private const float GOLD_WEIGHT = 1.5f;
        private const float LEGEND_WEIGHT = 2.0f;

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 指定クラス・レアリティのカードを複数枚抽選（不足時はレアリティを拡張して再抽選）
        /// </summary>
        public static List<CardData> GetRandomCardsByClassAndRarity(
            CardDatabase db,
            int count,
            CardData.CardClass[] classes,
            CardData.CardRarity[] rarities
        )
        {
            List<CardData> result = new List<CardData>();

            if (db == null || count <= 0 || classes == null || classes.Length == 0 || rarities == null || rarities.Length == 0)
            {
                return result;
            }

            // ------------------------------
            // クラスでフィルタリング
            // ------------------------------
            List<CardData> classFiltered = CardQueryManager.GetCardsByClass(db, classes);

            // ------------------------------
            // 抽選処理
            // ------------------------------
            int remaining = count;
            List<CardData.CardRarity> remainingRarities = new List<CardData.CardRarity>(rarities);

            while (remaining > 0)
            {
                // --------------------------------------------------
                // 候補が尽きた場合は、再抽選を試行
                // --------------------------------------------------
                if (remainingRarities.Count == 0)
                {
                    // 未指定レアリティを低い順で取得
                    CardData.CardRarity[] nextRarities = GetNextAvailableRarities(rarities);

                    if (nextRarities != null)
                    {
                        // 再帰呼び出しで再抽選
                        return GetRandomCardsByClassAndRarity(db, count, classes, nextRarities);
                    }

                    // Legendまで試しても失敗したら空
                    return new List<CardData>();
                }

                // --------------------------------------------------
                // レアリティ抽選
                // --------------------------------------------------
                CardData.CardRarity selectedRarity = PickRarityWeighted(remainingRarities.ToArray());

                // --------------------------------------------------
                // 選ばれたレアリティのカードを抽出
                // --------------------------------------------------
                List<CardData> rarityFiltered = CardQueryManager.GetCardsByRarity(db, selectedRarity);
                List<CardData> candidateCards = new List<CardData>();

                foreach (CardData card in rarityFiltered)
                {
                    if (classFiltered.Contains(card) && !result.Contains(card))
                    {
                        candidateCards.Add(card);
                    }
                }

                // 該当カードが存在しない場合は候補から除外
                if (candidateCards.Count == 0)
                {
                    remainingRarities.Remove(selectedRarity);
                    continue;
                }

                // --------------------------------------------------
                // ウェイト抽選で1枚選択
                // --------------------------------------------------
                CardData picked = PickCardWeighted(candidateCards, db);

                if (picked != null)
                {
                    result.Add(picked);
                    remaining--;
                }
            }

            // --------------------------------------------------
            // 枚数不足の場合も低い順に上位レアリティで再抽選
            // --------------------------------------------------
            if (result.Count < count)
            {
                CardData.CardRarity[] nextRarities = GetNextAvailableRarities(rarities);
                if (nextRarities != null)
                {
                    return GetRandomCardsByClassAndRarity(db, count, classes, nextRarities);
                }
            }

            return result;
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// 現在指定されていないレアリティを低い順で1段階ずつ追加して返す
        /// </summary>
        private static CardData.CardRarity[] GetNextAvailableRarities(CardData.CardRarity[] current)
        {
            // 全レアリティを昇順で定義
            CardData.CardRarity[] all = new CardData.CardRarity[]
            {
                CardData.CardRarity.Bronze,
                CardData.CardRarity.Silver,
                CardData.CardRarity.Gold,
                CardData.CardRarity.Legend
            };

            // 既にLegendまで含んでいる場合は終了
            if (current.Length >= 4)
            {
                return null;
            }

            // 現在指定されていないレアリティを抽出
            List<CardData.CardRarity> available = new List<CardData.CardRarity>();

            foreach (CardData.CardRarity rarity in all)
            {
                bool exists = false;
                foreach (CardData.CardRarity c in current)
                {
                    if (c == rarity)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    available.Add(rarity);
                }
            }

            // 新たに1段階上位のレアリティを追加
            if (available.Count > 0)
            {
                // 最も低い未指定レアリティを1つ追加
                CardData.CardRarity next = available[0];
                List<CardData.CardRarity> expanded = new List<CardData.CardRarity>(current);
                expanded.Add(next);
                return expanded.ToArray();
            }

            return null;
        }

        /// <summary>
        /// レアリティ配列からウェイト付きで1つ選択
        /// </summary>
        private static CardData.CardRarity PickRarityWeighted(CardData.CardRarity[] rarities)
        {
            Dictionary<CardData.CardRarity, float> weights = new Dictionary<CardData.CardRarity, float>();
            float totalWeight = 0f;

            foreach (CardData.CardRarity rarity in rarities)
            {
                float weight = 1f;
                switch (rarity)
                {
                    case CardData.CardRarity.Bronze: weight = BRONZE_WEIGHT; break;
                    case CardData.CardRarity.Silver: weight = SILVER_WEIGHT; break;
                    case CardData.CardRarity.Gold: weight = GOLD_WEIGHT; break;
                    case CardData.CardRarity.Legend: weight = LEGEND_WEIGHT; break;
                }
                weights[rarity] = weight;
                totalWeight += weight;
            }

            float random = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (KeyValuePair<CardData.CardRarity, float> keyValue in weights)
            {
                cumulative += keyValue.Value;
                if (random <= cumulative)
                {
                    return keyValue.Key;
                }
            }

            return rarities[0];
        }

        /// <summary>
        /// ウェイト付きで1枚のカードを選択
        /// </summary>
        private static CardData PickCardWeighted(List<CardData> cards, CardDatabase db)
        {
            float total = 0f;
            Dictionary<CardData, float> weights = new Dictionary<CardData, float>();

            foreach (CardData card in cards)
            {
                float weight = 1.0f;

                if (card.PackNumber == db.LatestPackNumber)
                {
                    weight *= LATEST_PACK_WEIGHT;
                }

                if (card.ClassType == CardData.CardClass.Neutral)
                {
                    weight *= NEUTRAL_CARD_WEIGHT;
                }

                weights[card] = weight;
                total += weight;
            }

            float random = Random.Range(0f, total);
            float cumulative = 0f;

            foreach (KeyValuePair<CardData, float> kv in weights)
            {
                cumulative += kv.Value;
                if (random <= cumulative)
                {
                    return kv.Key;
                }
            }

            return cards.Count > 0 ? cards[0] : null;
        }
    }
}