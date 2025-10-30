// ======================================================
// CardDatabaseEditorUI.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : カードデータベース編集UIの統括クラス
//             クラス選択・一括設定・個別設定の各UIを制御
// ======================================================

using UnityEngine;

namespace CardGame.UI
{
    /// <summary>
    /// カードデータベース編集UIの中核クラス  
    /// 各種UIパネルをまとめ、CardDatabaseとの同期を管理する
    /// </summary>
    public class CardDatabaseEditorUI : MonoBehaviour
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>CardDatabaseインスタンス参照（外部で生成済みを渡す）</summary>
        [SerializeField]
        private CardGame.Database.CardDatabase database = null;

        /// <summary>クラス選択UIパネル</summary>
        [SerializeField]
        private CardClassSelectorPanel classSelectorPanel = null;

        /// <summary>パック・レアリティ制御パネル</summary>
        [SerializeField]
        private CardGroupControlPanel groupControlPanel = null;

        /// <summary>全カード一覧編集パネル</summary>
        [SerializeField]
        private CardListEditorPanel listEditorPanel = null;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // 各UIにデータベース参照を渡す
            if (classSelectorPanel != null)
            {
                classSelectorPanel.Initialize(database);
            }

            if (groupControlPanel != null)
            {
                groupControlPanel.Initialize(database);
            }

            if (listEditorPanel != null)
            {
                listEditorPanel.Initialize(database);
            }
        }
    }
}