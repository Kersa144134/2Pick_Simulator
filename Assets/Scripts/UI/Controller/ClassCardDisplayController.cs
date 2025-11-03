// ======================================================
// ClassCardDisplayController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : クラスボタン上のカード画像を設定・管理するクラス
// ======================================================

using CardGame.CardSystem.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// クラスボタンにカード画像を表示する管理クラス
    /// </summary>
    public class ClassCardDisplayController
    {
        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 各クラスボタン上のカードImageにランダムカードを設定
        /// ゴールドまたはレジェンド、かつニュートラル＋クラスカードから抽選
        /// ボタン自身の Image は無視し、子オブジェクトのみ使用
        /// </summary>
        /// <param name="classButtons">クラスボタン配列</param>
        /// <param name="db">カードデータベース</param>
        public void GenerateCards(Button classButton, CardData.CardClass cardclass, CardDatabase db)
        {
            // --------------------------------------------------
            // ボタンの子オブジェクトのみからImageコンポーネント取得
            // GetComponentsInChildren(true)で非アクティブも含める
            // ボタン自身のImageは除外する
            // --------------------------------------------------
            List<Image> childImages = new List<Image>();
            Image buttonImage = classButton.GetComponent<Image>();

            Image[] allImages = classButton.GetComponentsInChildren<Image>(true);
            foreach (Image img in allImages)
            {
                if (img != buttonImage) childImages.Add(img);
            }

            // 子Imageが2枚未満なら処理終了
            if (childImages.Count < 2)
            {
                return;
            }

            // --------------------------------------------------
            // 2枚重複なし抽選（ニュートラル + クラスカードのゴールド or レジェンドカード）
            // --------------------------------------------------
            List<CardData> drawnCards = CardRandomPicker.GetRandomCardsByClassAndRarity(
                db,
                2,
                new CardData.CardClass[] { CardData.CardClass.Neutral, cardclass },
                new CardData.CardRarity[] { CardData.CardRarity.Gold, CardData.CardRarity.Legend }
            );

            // --------------------------------------------------
            // Imageにカード画像を反映
            // --------------------------------------------------
            if (drawnCards.Count > 0 && drawnCards[0].CardImage != null)
            {
                childImages[0].sprite = drawnCards[0].CardImage;
                childImages[0].enabled = true;
            }

            if (drawnCards.Count > 1 && drawnCards[1].CardImage != null)
            {
                childImages[1].sprite = drawnCards[1].CardImage;
                childImages[1].enabled = true;
            }
        }
    }
}