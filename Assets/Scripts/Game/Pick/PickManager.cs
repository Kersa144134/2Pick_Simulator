// ======================================================
// PickManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : PickScene管理クラス
//             PickCanvas / DeckCanvas の切り替え
//             Exitボタン押下時の確認パネル処理
// ======================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// PickScene の Canvas 切り替えおよび Exit 確認処理を管理するクラス
    /// </summary>
    public class PickManager : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("キャンバス設定")]
        
        [SerializeField]
        /// <summary>カードピック用キャンバス</summary>
        private GameObject pickCanvas;

        [SerializeField]
        /// <summary>デッキ確認用キャンバス</summary>
        private GameObject deckCanvas;

        [Header("ボタン設定")]
        [SerializeField]
        /// <summary>Deckボタン</summary>
        private Button deckButton;

        [SerializeField]
        /// <summary>Deckキャンバス用の戻るボタン</summary>
        private Button backButton;

        [SerializeField]
        /// <summary>Exitボタン</summary>
        private Button exitButton;

        [Header("背景設定")]
        [SerializeField]
        /// <summary>背景変更用Image</summary>
        private Image bgImage;

        [Header("確認パネル")]
        [SerializeField]
        /// <summary>Exit確認用パネルオブジェクト</summary>
        private ConfirmPanelController confirmPanel;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // PickCanvas を最初に表示
            pickCanvas.SetActive(true);
            deckCanvas.SetActive(false);

            // ボタンイベント登録
            deckButton.onClick.AddListener(OnDeckButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);

            // 確認パネル初期化
            confirmPanel.Initialize(OnConfirmExitOk, OnConfirmExitBack);
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// Deckボタン押下時処理
        /// DeckCanvasを表示し、PickCanvasを非表示
        /// </summary>
        private void OnDeckButtonClicked()
        {
            pickCanvas.SetActive(false);
            deckCanvas.SetActive(true);
        }

        /// <summary>
        /// Backボタン押下時処理
        /// PickCanvasを表示し、DeckCanvasを非表示
        /// </summary>
        private void OnBackButtonClicked()
        {
            deckCanvas.SetActive(false);
            pickCanvas.SetActive(true);
        }

        /// <summary>
        /// Exitボタン押下時処理
        /// 確認パネルを表示
        /// </summary>
        private void OnExitButtonClicked()
        {
            confirmPanel.Show();
        }

        /// <summary>
        /// 確認パネル OK 押下時処理
        /// タイトルシーンへ遷移
        /// </summary>
        private void OnConfirmExitOk()
        {
            SceneManager.LoadScene("TitleScene");
        }

        /// <summary>
        /// 確認パネル Back 押下時処理
        /// 確認パネルを非表示にする
        /// </summary>
        private void OnConfirmExitBack()
        {
            confirmPanel.Hide();
        }
    }
}