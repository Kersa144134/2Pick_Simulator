// ======================================================
// CardQueryManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : カードデータベースから特定条件に合致するカードを取得するユーティリティクラス
//             最大枚数が1以上のカードを基本条件とし、クラスやレアリティで抽出可能
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
        /// 指定クラスのカードを取得（最大枚数1以上のみ）
        /// </summary>
        public static List<CardData> GetCardsByClass(CardDatabase db, params CardData.CardClass[] targetClasses)
        {
            List<CardData> result = new List<CardData>();
            if (targetClasses == null || targetClasses.Length == 0) return result;

            foreach (CardData card in db.AllCards)
            {
                if (db.GetMaxCopies(card.CardId) <= 0) continue;

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
        /// 指定レアリティのカードを取得（最大枚数1以上のみ）
        /// </summary>
        public static List<CardData> GetCardsByRarity(CardDatabase db, params CardData.CardRarity[] targetRarities)
        {
            List<CardData> result = new List<CardData>();
            if (targetRarities == null || targetRarities.Length == 0) return result;

            foreach (CardData card in db.AllCards)
            {
                if (db.GetMaxCopies(card.CardId) <= 0) continue;

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