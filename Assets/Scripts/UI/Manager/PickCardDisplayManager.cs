// ======================================================
// PickCardDisplayManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-04
// 更新日時   : 2025-11-04
// 概要       : ピックシーンにおけるカード表示管理クラス
//             クラスごとの4枚のカードを抽選し、UIに表示する
// ======================================================

using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using CardGame.DeckSystem.Manager;
using CardGame.PickSystem.Controller;
using CardGame.UISystem.Controller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UISystem.Manager
{
    /// <summary>
    /// ピックシーンのカード表示管理クラス
    /// 4枚のカードを抽選してUIに反映する
    /// </summary>
    public class PickCardDisplayManager : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("カード表示設定")]
        [SerializeField]
        /// <summary>左側カード画像（2枚分）</summary>
        private List<Image> leftCardImages = new List<Image>();

        [SerializeField]
        /// <summary>右側カード画像（2枚分）</summary>
        private List<Image> rightCardImages = new List<Image>();

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>カードデータベース（抽選元データ）</summary>
        private CardDatabase _cardDatabase;

        /// <summary>カード表示を管理するコントローラ</summary>
        private readonly ClassCardDisplayController _cardDisplayController = new ClassCardDisplayController();

        /// <summary>カード抽選を管理するコントローラ</summary>
        private readonly ClassCardPickController _cardPickController = new ClassCardPickController();

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // --------------------------------------------------
            // カードデータベース取得
            // --------------------------------------------------
            CardDatabaseManager manager = CardDatabaseManager.Instance;
            if (manager == null)
            {
                Debug.LogError("CardDatabaseManagerが存在しません。");
                return;
            }

            _cardDatabase = manager.GetCardDatabase();
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 指定クラスのメインピックカード（4枚）を抽選してUIに反映する
        /// </summary>
        public List<CardData> PickMainClassCards(CardData.CardRarity[] cardRarities)
        {
            // 抽選処理
            List<CardData> pickedCards = _cardPickController.PickMainClassCards(_cardDatabase, DeckListManager.Instance.SelectedClass, cardRarities);

            if (pickedCards == null || pickedCards.Count == 0)
            {
                Debug.LogWarning($"[{nameof(PickCardDisplayManager)}] カード抽選結果が空です。");
                return pickedCards;
            }

            // 表示処理
            _cardDisplayController.DisplayMainPickedCards(leftCardImages, new List<CardData> { pickedCards[0], pickedCards[1] });
            _cardDisplayController.DisplayMainPickedCards(rightCardImages, new List<CardData> { pickedCards[2], pickedCards[3] });

            return pickedCards;
        }
    }
}