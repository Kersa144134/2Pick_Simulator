// ======================================================
// CardClassSelectorPanel.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : 8クラスを選択可能なボタンUIを制御
//             指定クラス＋ニュートラル以外を0枚に設定
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.Data;
using CardGame.Database;

namespace CardGame.UI
{
    /// <summary>
    /// クラス選択ボタン群を表示し、クリックでCardDatabaseへ反映するUI
    /// </summary>
    public class CardClassSelectorPanel : MonoBehaviour
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>ボタン群（ニュートラルを含む8クラス）</summary>
        [SerializeField]
        private Button[] classButtons = new Button[8];

        /// <summary>各クラスボタンの対応Enum</summary>
        private CardData.CardClass[] classes =
        {
            CardData.CardClass.Neutral,
            CardData.CardClass.Elf,
            CardData.CardClass.Royal,
            CardData.CardClass.Witch,
            CardData.CardClass.Dragon,
            CardData.CardClass.Nightmare,
            CardData.CardClass.Bishop,
            CardData.CardClass.Nemesis
        };

        /// <summary>参照するデータベース</summary>
        private CardDatabase database = null;

        // ======================================================
        // 公開メソッド
        // ======================================================

        /// <summary>
        /// データベース参照を受け取り、ボタン動作を設定
        /// </summary>
        public void Initialize(CardDatabase db)
        {
            database = db;

            // 各ボタンにイベント登録
            for (int i = 0; i < classButtons.Length; i++)
            {
                int index = i;
                classButtons[i].onClick.AddListener(() => OnClassSelected(classes[index]));
            }
        }

        // ======================================================
        // 内部メソッド
        // ======================================================

        /// <summary>
        /// クラス選択時に呼ばれ、ニュートラル＋選択クラス以外を0枚に設定
        /// </summary>
        private void OnClassSelected(CardData.CardClass selected)
        {
            database.SetAllToZero(); // 全部0枚に
            database.SetMaxCopiesByClass(CardData.CardClass.Neutral, 3); // ニュートラル有効
            database.SetMaxCopiesByClass(selected, 3); // 選択クラス有効

            Debug.Log($"[CardClassSelector] {selected} クラスを選択。");
        }
    }
}