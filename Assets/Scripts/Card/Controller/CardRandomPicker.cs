// ======================================================
// CardRandomPicker.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : カードのランダム抽選を管理（複数クラス＋複数レアリティ対応）
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// カードのランダム抽選を管理
    /// </summary>
    public static class CardRandomPicker
    {
        /// <summary>
        /// 指定複数クラスかつ複数レアリティのカードからランダムに複数枚取得（重複なし）
        /// </summary>
        /// <param name="db">カードデータベース</param>
        /// <param name="count">抽選枚数</param>
        /// <param name="classes">抽選対象のクラス配列</param>
        /// <param name="rarities">抽選対象のレアリティ配列</param>
        /// <returns>ランダムに選ばれたカードリスト（該当なしは空リスト）</returns>
        public static List<CardData> GetRandomCardsByClassAndRarity(
            CardDatabase db,
            int count,
            CardData.CardClass[] classes,
            CardData.CardRarity[] rarities
        )
        {
            List<CardData> result = new List<CardData>();

            // 引数チェック
            if (db == null || count <= 0 || classes == null || classes.Length == 0 ||
                rarities == null || rarities.Length == 0)
            {
                // 不正引数は空リスト返却
                return result;
            }

            // ------------------------------
            // 1. クラスでフィルタリング
            // ------------------------------
            List<CardData> classFiltered = CardQueryManager.GetCardsByClass(db, classes);

            // ------------------------------
            // 2. レアリティでフィルタリング
            // ------------------------------
            List<CardData> filtered = new List<CardData>();
            foreach (CardData.CardRarity rarity in rarities)
            {
                List<CardData> rarityFiltered = CardQueryManager.GetCardsByRarity(db, rarity);
                foreach (CardData card in rarityFiltered)
                {
                    if (classFiltered.Contains(card))
                    {
                        // クラス＋レアリティ両方に該当するカードのみ追加
                        filtered.Add(card);
                    }
                }
            }

            if (filtered.Count == 0)
            {
                // 条件に合うカードなし
                return result;
            }

            // ------------------------------
            // 3. 抽選枚数が総カード数を超える場合は制限
            // ------------------------------
            int drawCount = Mathf.Min(count, filtered.Count);

            // ------------------------------
            // 4. 重複なしランダム抽選
            // ------------------------------
            for (int i = 0; i < drawCount; i++)
            {
                int index = Random.Range(0, filtered.Count);
                result.Add(filtered[index]);

                // 抽出済みカードをリストから削除
                filtered.RemoveAt(index);
            }

            return result;
        }
    }
}