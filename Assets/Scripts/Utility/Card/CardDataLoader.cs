// ======================================================
// CardDataLoader.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-10-31
// 概要       : Resources/Cards配下からCardDataをロードして管理するユーティリティクラス
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;
using static CardGame.CardSystem.Data.CardData;

namespace CardGame.CardSystem.Utility
{
    /// <summary>
    /// CardDataをロード・キャッシュするユーティリティクラス
    /// </summary>
    public class CardDataLoader
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>ロード済みのCardDataリスト</summary>
        private readonly List<CardData> _allCardData = new List<CardData>();

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>ロード済みのCardDataリスト</summary>
        public List<CardData> AllCardData => _allCardData;

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// Resources/Cards配下からCardDataをロード  
        /// すでにロード済みの場合は再ロードせずキャッシュを使用
        /// </summary>
        public void LoadAllCardData()
        {
            if (_allCardData.Count > 0)
            {
                // すでにロード済みなら処理をスキップ
                return;
            }

            CardData[] loadedCards = Resources.LoadAll<CardData>("Cards");

            if (loadedCards != null && loadedCards.Length > 0)
            {
                _allCardData.Clear();
                _allCardData.AddRange(loadedCards);
            }
            else
            {
                Debug.LogWarning("[CardDataLoader] Resources/Cards 配下にCardDataが見つかりません。");
            }
        }

        /// <summary>
        /// 指定IDのCardDataを取得
        /// </summary>
        /// <param name="cardId">検索するカードID</param>
        /// <returns>見つからなければnull</returns>
        public CardData GetCardById(int cardId)
        {
            return _allCardData.Find(c => c.CardId == cardId);
        }

        /// <summary>
        /// 指定条件に一致するカードリストを取得
        /// </summary>
        /// <param name="predicate">検索条件</param>
        public List<CardData> FindCards(System.Predicate<CardData> predicate)
        {
            return _allCardData.FindAll(predicate);
        }

        // ======================================================
        // 追加検索メソッド
        // ======================================================

        /// <summary>
        /// 指定クラスに所属するカードを取得
        /// </summary>
        /// <param name="className">検索対象のクラス名</param>
        /// <returns>該当カードのリスト</returns>
        public List<CardData> FindCardsByClass(CardClass className)
        {
            return _allCardData.FindAll(c => c.ClassType == className);
        }

        /// <summary>
        /// 指定パックに所属するカードを取得
        /// </summary>
        /// <param name="packName">検索対象のパック名</param>
        /// <returns>該当カードのリスト</returns>
        public List<CardData> FindCardsByPack(int packNumber)
        {
            return _allCardData.FindAll(c => c.PackNumber == packNumber);
        }

        /// <summary>
        /// 指定レアリティのカードを取得
        /// </summary>
        /// <param name="rarity">検索対象のレアリティ</param>
        /// <returns>該当カードのリスト</returns>
        public List<CardData> FindCardsByRarity(CardData.CardRarity rarity)
        {
            return _allCardData.FindAll(c => c.Rarity == rarity);
        }

        /// <summary>
        /// 指定コストのカードを取得
        /// </summary>
        /// <param name="cost">検索対象のコスト値</param>
        /// <returns>該当カードのリスト</returns>
        public List<CardData> FindCardsByCost(int cost)
        {
            return _allCardData.FindAll(c => c.CardCost == cost);
        }
    }
}