// ======================================================
// CardVisibilityController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-10-31
// 概要       : CardDataの表示／非表示リストを管理する制御クラス
//             CardDisplayManagerから呼び出され、表示状態の一括変更を行う
// ======================================================

using System.Collections.Generic;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
using static CardGame.CardSystem.Data.CardData;
using UnityEngine;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// カードの表示状態（表示・非表示）を一括管理するクラス  
    /// 表示リスト・非表示リストを内部で保持し、まとめて切り替え操作を提供する
    /// </summary>
    public class CardVisibilityController
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>現在表示対象として有効なCardDataリスト</summary>
        private readonly List<CardData> _visibleCardData = new List<CardData>();

        /// <summary>現在非表示対象として保持されるCardDataリスト</summary>
        private readonly List<CardData> _hiddenCardData = new List<CardData>();

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// 既存の全CardDataリストから初期化を行う  
        /// すべてのカードを最初は表示対象として登録
        /// </summary>
        /// <param name="allCards">ロード済みの全CardData</param>
        public CardVisibilityController(IEnumerable<CardData> allCards)
        {
            foreach (var card in allCards)
            {
                _visibleCardData.Add(card);
            }
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        // --------------------------------------------------
        // 状態変更
        // --------------------------------------------------

        /// <summary>
        /// 指定した複数のカードを非表示リストに移動する  
        /// 既に非表示の場合は無視される
        /// </summary>
        /// <param name="cardsToHide">非表示にするCardDataのコレクション</param>
        public void HideCards(IEnumerable<CardData> cardsToHide)
        {
            foreach (var card in cardsToHide)
            {
                if (_visibleCardData.Contains(card))
                {
                    _visibleCardData.Remove(card);
                    _hiddenCardData.Add(card);
                }
            }
        }

        /// <summary>
        /// 指定した複数のカードを表示リストに戻す  
        /// 既に表示されている場合は無視される
        /// </summary>
        /// <param name="cardsToShow">表示対象に戻すCardDataのコレクション</param>
        public void ShowCards(IEnumerable<CardData> cardsToShow)
        {
            foreach (var card in cardsToShow)
            {
                if (_hiddenCardData.Contains(card))
                {
                    _hiddenCardData.Remove(card);
                    _visibleCardData.Add(card);
                }
            }

            // 表示順をCardIdでソート（安定した描画順を保証）
            _visibleCardData.Sort((a, b) => a.CardId.CompareTo(b.CardId));
        }

        // --------------------------------------------------
        // リスト参照
        // --------------------------------------------------

        /// <summary>
        /// 現在表示対象となっているCardDataリストを取得する
        /// </summary>
        public List<CardData> GetVisibleCards()
        {
            return new List<CardData>(_visibleCardData);
        }

        /// <summary>
        /// 現在非表示対象となっているCardDataリストを取得する
        /// </summary>
        public List<CardData> GetHiddenCards()
        {
            return new List<CardData>(_hiddenCardData);
        }

        // --------------------------------------------------
        // リセット制御
        // --------------------------------------------------

        /// <summary>
        /// すべてのカードを表示対象に戻す  
        /// （フィルタ解除などの一括操作に使用）
        /// </summary>
        public void ShowAll()
        {
            _visibleCardData.AddRange(_hiddenCardData);
            _hiddenCardData.Clear();

            _visibleCardData.Sort((a, b) => a.CardId.CompareTo(b.CardId));
        }

        /// <summary>
        /// すべてのカードを非表示にする
        /// </summary>
        public void HideAll()
        {
            _hiddenCardData.AddRange(_visibleCardData);
            _visibleCardData.Clear();
        }
    }
}