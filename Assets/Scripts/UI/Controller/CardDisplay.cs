// ======================================================
// CardDisplay.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : 単一カードUIの表示／非表示とUI更新を担当するクラス
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// 単一カードのUI要素を制御するクラス  
    /// CardDisplayManagerから生成され、CardDataの内容に基づきUIを更新する
    /// </summary>
    public class CardDisplay : MonoBehaviour
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>このカードが表示しているデータ</summary>
        public CardData CurrentData { get; private set; }

        /// <summary>カード画像を表示するImage</summary>
        private Image cardImage;

        private CardQuantityController _quantityController = new CardQuantityController();

        // ======================================================
        // 初期化
        // ======================================================

        /// <summary>
        /// カードのUI内容を初期化する  
        /// CardDisplayManagerから生成時に呼び出される
        /// </summary>
        public void Initialize(CardData data, CardDatabase database)
        {
            CurrentData = data;
            cardImage = gameObject.GetComponent<Image>();

            _quantityController.Initialize(gameObject, data, database);

            UpdateUI();
        }

        // ======================================================
        // 表示更新
        // ======================================================

        /// <summary>
        /// CardDataの内容に基づいてUIを更新する
        /// </summary>
        public void UpdateUI()
        {
            if (CurrentData == null) return;

            if (cardImage != null)
            {
                cardImage.sprite = CurrentData.CardImage;
            }
        }

        // ======================================================
        // 表示切替
        // ======================================================

        /// <summary>
        /// 表示リストまたは非表示リストへ移動する  
        /// CardDisplayManagerから呼び出される
        /// </summary>
        public void SetParent(RectTransform newParent)
        {
            if (newParent == null) return;
            transform.SetParent(newParent, false);
        }
    }
}