// ======================================================
// CardRandomPicker.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : カードのランダム抽選を管理
//             複数クラス・複数レアリティ・ピック率対応（累積型レアリティ先行抽選）
// ======================================================

using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

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

        /// <summary>最新パックカードのピック率補正値</summary>
        private const float LATEST_PACK_WEIGHT = 1.2f;

        /// <summary>ニュートラルカードのピック率補正値</summary>
        private const float NEUTRAL_CARD_WEIGHT = 0.1f;

        /// <summary>ブロンズカードの抽選ウェイト</summary>
        private const float BRONZE_WEIGHT = 1.0f;

        /// <summary>シルバーカードの抽選ウェイト</summary>
        private const float SILVER_WEIGHT = 1.0f;

        /// <summary>ゴールドカードの抽選ウェイト</summary>
        private const float GOLD_WEIGHT = 1.5f;

        /// <summary>レジェンドカードの抽選ウェイト</summary>
        private const float LEGEND_WEIGHT = 2.0f;
        
        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 指定複数クラスかつ複数レアリティのカードからランダムに複数枚取得（重複なし）
        /// レアリティ先行抽選＋カードウェイト考慮
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
            // 抽選枚数分、レアリティ→カード抽選
            // ------------------------------
            int remaining = count;

            // レアリティ候補をコピーして管理（抽選失敗レアリティを除外するため）
            List<CardData.CardRarity> remainingRarities = new List<CardData.CardRarity>(rarities);

            while (remaining > 0)
            {
                // --------------------------------------------------
                // 候補レアリティがなくなった場合は抽選失敗として空リスト返却
                // --------------------------------------------------
                if (remainingRarities.Count == 0)
                {
                    return new List<CardData>();
                }

                // --------------------------------------------------
                // レアリティ抽選
                // --------------------------------------------------
                CardData.CardRarity selectedRarity = PickRarityWeighted(remainingRarities.ToArray());

                // --------------------------------------------------
                // 選ばれたレアリティのカードをフィルタ
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

                // --------------------------------------------------
                // 該当カードなしの場合は、このレアリティを候補から除外して再抽選
                // --------------------------------------------------
                if (candidateCards.Count == 0)
                {
                    remainingRarities.Remove(selectedRarity);
                    continue;
                }

                // --------------------------------------------------
                // カードウェイト付き抽選
                // --------------------------------------------------
                CardData picked = PickCardWeighted(candidateCards, db);

                if (picked != null)
                {
                    result.Add(picked);
                    remaining--;
                }
            }

            return result;
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// レアリティ配列からウェイト付きで1つ選択
        /// </summary>
        private static CardData.CardRarity PickRarityWeighted(CardData.CardRarity[] rarities)
        {
            // ------------------------------
            // 各レアリティのウェイト計算
            // ------------------------------
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

            // ------------------------------
            // 累積確率型でランダム抽選
            // ------------------------------
            float random = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (KeyValuePair<CardData.CardRarity, float> keyValue in weights)
            {
                cumulative += keyValue.Value;
                if (random <= cumulative)
                {
                    // 抽選されたレアリティを返す
                    return keyValue.Key;
                }
            }

            // ループを抜けた場合は最初のレアリティを返す
            return rarities[0];
        }

        /// <summary>
        /// ウェイト付きで1枚のカードを選択
        /// </summary>
        private static CardData PickCardWeighted(List<CardData> cards, CardDatabase db)
        {
            // ------------------------------
            // 累積確率型ウェイト抽選
            // ------------------------------

            // ウェイトの総和を計算するための変数
            float total = 0f;

            // 各カードのウェイトを格納する辞書
            Dictionary<CardData, float> weights = new Dictionary<CardData, float>();

            // ------------------------------
            // 各カードのウェイト計算
            // ------------------------------
            foreach (CardData card in cards)
            {
                // 基本ウェイトの設定
                float weight = 1.0f;

                // 最新パックのカードのピック率代入
                if (card.PackNumber == db.LatestPackNumber)
                {
                    weight *= LATEST_PACK_WEIGHT;
                }

                // ニュートラルカードのピック率代入
                if (card.ClassType == CardData.CardClass.Neutral)
                {
                    weight *= NEUTRAL_CARD_WEIGHT;
                }

                // カードごとのウェイトを保存
                weights[card] = weight;

                // 総ウェイトに加算
                total += weight;
            }

            // 0～総ウェイトの範囲でランダム値を生成
            float random = Random.Range(0f, total);

            // ------------------------------
            // 累積ウェイトを使ってカードを決定
            // ------------------------------
            float cumulative = 0f;

            foreach (KeyValuePair<CardData, float> keyValue in weights)
            {
                // 現在のカードのウェイトを累積
                cumulative += keyValue.Value; 
                if (random <= cumulative)
                {
                    // ランダム値が累積ウェイト以下になった時点でそのカードを選択
                    return keyValue.Key;
                }
            }

            // 選択できなかった場合は先頭カードを返す
            return cards[0];
        }
    }
}