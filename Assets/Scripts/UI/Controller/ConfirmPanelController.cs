// ======================================================
// ConfirmPanelController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : 汎用確認パネル管理クラス
//             OK / BACKボタン押下時の処理を設定可能
//             遷移先シーン名を指定可能
// ======================================================

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// 汎用確認パネル管理クラス
    /// OK / BACKボタン処理を設定可能
    /// 遷移先シーン名をInspectorで指定できる
    /// </summary>
    public class ConfirmPanelController : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("UI設定")]
        [SerializeField]
        /// <summary>パネル内メッセージ表示用 Text</summary>
        private Text messageText;

        [SerializeField]
        /// <summary>OKボタン</summary>
        private Button okButton;

        [SerializeField]
        /// <summary>BACKボタン</summary>
        private Button backButton;

        [Header("シーン遷移設定")]
        [SerializeField]
        /// <summary>OK押下時に遷移するシーン名（空文字ならコールバックのみ）</summary>
        private string sceneNameOnOk = "";

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// パネル初期化
        /// OK / BACKボタン押下時のコールバックを設定
        /// </summary>
        public void Initialize(Action onOk, Action onBack)
        {
            // --------------------------------------------------
            // OKボタン押下時
            // --------------------------------------------------
            okButton.onClick.AddListener(() =>
            {
                // コールバック実行
                onOk?.Invoke();

                // 遷移先シーンが設定されていればシーン遷移
                if (!string.IsNullOrEmpty(sceneNameOnOk))
                {
                    SceneManager.LoadScene(sceneNameOnOk);
                }

                // パネル非表示
                gameObject.SetActive(false);
            });

            // --------------------------------------------------
            // BACKボタン押下時
            // --------------------------------------------------
            backButton.onClick.AddListener(() =>
            {
                onBack?.Invoke();
                gameObject.SetActive(false);
            });

            // 初期状態は非表示
            gameObject.SetActive(false);
        }

        /// <summary>
        /// パネルを表示
        /// </summary>
        public void Show(string message = "")
        {
            if (messageText == null)
            {
                Debug.LogError("ConfirmPanelController: messageText が null です");
                return;
            }

            if (message != "")
            {
                messageText.text = message;
            }

            gameObject.SetActive(true);
        }

        /// <summary>
        /// パネルを非表示
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}