// ======================================================
// CardGroupControlUI.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : パック・レアリティ一括変更用UIを生成・管理するクラス
//             CardGroupControlPanelを自動生成して配置
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.Database;
using CardGame.Data;

namespace CardGame.UI
{
    /// <summary>
    /// パック・レアリティのトグルUIをまとめて表示・管理するクラス
    /// </summary>
    public class CardGroupControlUI : MonoBehaviour
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>カードデータベース参照</summary>
        [SerializeField]
        private CardDatabase database = null;

        /// <summary>CardGroupControlPanelプレハブ</summary>
        [SerializeField]
        private CardGroupControlPanel panelPrefab = null;

        /// <summary>配置先のUIコンテナ</summary>
        [SerializeField]
        private Transform contentParent = null;

        /// <summary>戻るボタン</summary>
        [SerializeField]
        private Button backButton = null;

        /// <summary>パック数（動的に設定可）</summary>
        [SerializeField, Range(1, 20)]
        private int packCount = 10;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // 戻るボタン押下でタイトルに戻る処理を登録
            backButton.onClick.AddListener(OnBackButtonPressed);

            // UIの動的生成を開始
            GenerateGroupControls();
        }

        // ======================================================
        // 内部メソッド
        // ======================================================

        /// <summary>
        /// パック×レアリティ分のCardGroupControlPanelを生成
        /// </summary>
        private void GenerateGroupControls()
        {
            // 各パックごとにループ
            for (int pack = 1; pack <= packCount; pack++)
            {
                // 各レアリティ（0〜3）をループ
                foreach (CardData.CardRarity rarity in System.Enum.GetValues(typeof(CardData.CardRarity)))
                {
                    // パネルを生成して親に配置
                    CardGroupControlPanel panel = Instantiate(panelPrefab, contentParent);

                    // パネル名を明示（デバッグ・階層管理用）
                    panel.name = $"Pack{pack}_Rarity_{rarity}";

                    // パネルを初期化（データベース・対象設定）
                    panel.Initialize(database);

                    // 対象パックとレアリティを設定
                    var panelField = panel.GetType().GetField("targetPackNumber", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (panelField != null) panelField.SetValue(panel, pack);

                    var rarityField = panel.GetType().GetField("targetRarity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (rarityField != null) rarityField.SetValue(panel, rarity);
                }
            }
        }

        /// <summary>
        /// タイトルシーンに戻る（SceneManagerを使用）
        /// </summary>
        private void OnBackButtonPressed()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        }
    }
}