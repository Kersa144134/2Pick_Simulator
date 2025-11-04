// ======================================================
// ClassCardDisplayController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-04
// 概要       : クラスボタン上のカード画像を設定・管理するクラス
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;

namespace CardGame.UISystem.Controller
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
        /// クラスボタン上にカード画像を反映する
        /// </summary>
        /// <param name="classButton">カードを表示する対象のクラスボタン</param>
        /// <param name="cardList">表示対象となる2枚のカードデータ</param>
        public void DisplayPickedCards(Button classButton, List<CardData> cardList)
        {
            // --------------------------------------------------
            // ボタンの子オブジェクトからImageコンポーネントを取得
            // ボタン本体のImageは除外し、子オブジェクトのImageのみを使用する
            // --------------------------------------------------
            List<Image> childImages = new List<Image>();
            Image buttonImage = classButton.GetComponent<Image>();

            // 非アクティブな子も含めて取得
            Image[] allImages = classButton.GetComponentsInChildren<Image>(true);
            foreach (Image img in allImages)
            {
                // ボタン自身のImageは除外
                if (img != buttonImage)
                {
                    childImages.Add(img);
                }
            }

            // --------------------------------------------------
            // 子Imageが2枚未満なら描画不能のため終了
            // --------------------------------------------------
            if (childImages.Count < 2)
            {
                Debug.LogWarning("クラスボタンの子Imageが2枚未満のため、カードを表示できません。");
                return;
            }

            // --------------------------------------------------
            // 渡されたカードリストが有効であるか確認
            // --------------------------------------------------
            if (cardList == null || cardList.Count == 0)
            {
                Debug.LogWarning("カードリストが空またはnullのため、カード画像を表示できません。");
                return;
            }

            // --------------------------------------------------
            // 各子Imageに対応するカード画像を設定
            // 2枚未満の場合でも存在する分のみ表示
            // --------------------------------------------------
            for (int i = 0; i < childImages.Count && i < cardList.Count; i++)
            {
                CardData card = cardList[i];

                // カード画像が存在すれば設定し、有効化
                if (card != null && card.CardImage != null)
                {
                    childImages[i].sprite = card.CardImage;
                    childImages[i].enabled = true;
                }
                else
                {
                    // カード画像がない場合は非表示にする
                    childImages[i].enabled = false;
                }
            }
        }
    }
}