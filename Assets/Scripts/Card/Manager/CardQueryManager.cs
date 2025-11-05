// ======================================================
// CardQueryManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-05
// 概要       : カードデータベースから特定条件に合致するカードを取得するユーティリティクラス
//             最大枚数が1以上かつ提示可能なカードを基本条件とし、クラスやレアリティで抽出可能
// ======================================================

using System.Collections.Generic;
using CardGame.CardSystem.Data;

namespace CardGame.CardSystem.Manager
{
    /// <summary>
    /// カードデータベースから条件に合致するカードリストを取得する管理クラス
    /// </summary>
    public static class CardQueryManager
    {
        /// <summary>
        /// 指定クラスのカードを取得（最大枚数1以上かつ提示可能なカードのみ）
        /// </summary>
        public static List<CardData> GetCardsByClass(CardDatabase db, params CardData.CardClass[] targetClasses)
        {
            List<CardData> result = new List<CardData>();
            if (targetClasses == null || targetClasses.Length == 0) return result;

            foreach (CardData card in db.AllCards)
            {
                // 最大枚数が0以下または提示不可カードはスキップ
                if (db.GetDeckableCopies(card.CardId) <= 0 || !card.IsAvailable) continue;

                foreach (CardData.CardClass cls in targetClasses)
                {
                    if (card.ClassType == cls)
                    {
                        result.Add(card);
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 指定レアリティのカードを取得（最大枚数1以上かつ提示可能なカードのみ）
        /// </summary>
        public static List<CardData> GetCardsByRarity(CardDatabase db, params CardData.CardRarity[] targetRarities)
        {
            List<CardData> result = new List<CardData>();
            if (targetRarities == null || targetRarities.Length == 0) return result;

            foreach (CardData card in db.AllCards)
            {
                // 最大枚数が0以下または提示不可カードはスキップ
                if (db.GetDeckableCopies(card.CardId) <= 0 || !card.IsAvailable) continue;

                foreach (CardData.CardRarity rarity in targetRarities)
                {
                    if (card.Rarity == rarity)
                    {
                        result.Add(card);
                        break;
                    }
                }
            }

            return result;
        }
    }
}