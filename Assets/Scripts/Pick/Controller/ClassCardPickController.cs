// ======================================================
// ClassCardPickController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-04
// 更新日時   : 2025-11-04
// 概要       : クラスごとの初回カード抽選を管理するクラス
//             ニュートラル＋対象クラスからゴールド/レジェンド2枚を抽選
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;

namespace CardGame.PickSystem.Controller
{
    /// <summary>
    /// クラス別カード抽選を管理するクラス
    /// </summary>
    public class ClassCardPickController
    {
        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 指定されたクラスから2枚のカードをランダム抽選する
        /// 条件：ニュートラル＋指定クラス、かつゴールド or レジェンド
        /// </summary>
        /// <param name="db">カードデータベース</param>
        /// <param name="cardClass">抽選対象クラス</param>
        /// <returns>抽選されたカードデータのリスト（最大2枚）</returns>
        public List<CardData> PickClassCards(CardDatabase db, CardData.CardClass cardClass)
        {
            List<CardData> cardDatas = CardRandomPicker.GetRandomCardsByClassAndRarity(
                db,
                2,
                new CardData.CardClass[] { CardData.CardClass.Neutral, cardClass },
                new CardData.CardRarity[] { CardData.CardRarity.Gold, CardData.CardRarity.Legend }
            );

            return cardDatas;
        }
    }
}