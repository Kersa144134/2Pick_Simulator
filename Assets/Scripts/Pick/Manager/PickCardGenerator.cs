// ======================================================
// PickCardGenerator.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-04
// 概要       : ピックカード抽選クラス
//             PickSequenceManagerで指定されたレアリティをもとに
//             CardDatabaseから該当カードを抽選する
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;
using CardGame.PickSystem.Controller;

namespace CardGame.PickSystem.Manager
{
    /// <summary>
    /// ピックカード抽選を担当するクラス  
    /// 現在のピック順に応じたレアリティ・クラスからカードを抽選する。
    /// </summary>
    public class PickCardGenerator
    {
        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 指定レアリティとクラスに基づいてカードを抽選する
        /// </summary>
        /// <param name="database">カードデータベース</param>
        /// <param name="rarities">提示対象のレアリティ配列</param>
        /// <param name="targetClass">現在のクラス</param>
        /// <param name="pickCount">抽選枚数（デフォルト4枚）</param>
        /// <returns>抽選されたカードリスト</returns>
        public List<CardData> GeneratePickCards(
            CardDatabase database,
            CardData.CardRarity[] rarities,
            CardData.CardClass targetClass,
            int pickCount = 4)
        {
            // --------------------------------------------------
            // 有効なレアリティがない場合は空リストを返す
            // --------------------------------------------------
            if (rarities == null || rarities.Length == 0)
            {
                return new List<CardData>();
            }

            // --------------------------------------------------
            // ニュートラル＋指定クラスを対象に抽選
            // --------------------------------------------------
            CardData.CardClass[] targetClasses = new[]
            {
                CardData.CardClass.Neutral,
                targetClass
            };

            // --------------------------------------------------
            // CardRandomPickerを使用してランダム抽選
            // --------------------------------------------------
            List<CardData> pickedCards = CardRandomPicker.GetRandomCardsByClassAndRarity(
                database,
                pickCount,
                targetClasses,
                rarities
            );

            return pickedCards;
        }
    }
}