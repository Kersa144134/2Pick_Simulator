// ======================================================
// CardGroupControlPanel.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : パック・レアリティごとのチェックUIを管理
//             ON/OFFで0枚またはデフォルト枚数に設定
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.Data;
using CardGame.Database;

namespace CardGame.UI
{
    /// <summary>
    /// パック・レアリティ別に一括変更できるチェックボックスUI
    /// </summary>
    public class CardGroupControlPanel : MonoBehaviour
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>データベース参照</summary>
        private CardDatabase database = null;

        /// <summary>対象パック番号</summary>
        [SerializeField]
        private int targetPackNumber = 1;

        /// <summary>対象レアリティ</summary>
        [SerializeField]
        private CardData.CardRarity targetRarity = CardData.CardRarity.Bronze;

        /// <summary>ON時に有効化、OFF時に無効化するトグル</summary>
        [SerializeField]
        private Toggle toggle = null;

        // ======================================================
        // 公開メソッド
        // ======================================================

        public void Initialize(CardDatabase db)
        {
            database = db;
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        // ======================================================
        // 内部メソッド
        // ======================================================

        /// <summary>
        /// チェック状態に応じてカード枚数を変更
        /// </summary>
        private void OnToggleChanged(bool isOn)
        {
            int value = isOn ? 3 : 0;

            // パック指定またはレアリティ指定を適用
            if (targetPackNumber > 0)
            {
                database.SetMaxCopiesByPack(targetPackNumber, value);
            }

            database.SetMaxCopiesByRarity(targetRarity, value);
            Debug.Log($"[CardGroupControl] {targetPackNumber}番パック / {targetRarity} → {value}枚");
        }
    }
}