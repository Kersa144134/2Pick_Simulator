// ======================================================
// ConfirmPanelController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : 汎用確認パネル管理クラス
//             OK / BACKボタン押下時の処理を設定可能
// ======================================================

using System;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// 汎用確認パネル管理クラス
    /// OK / BACKボタン処理を設定可能
    /// </summary>
    public class ConfirmPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text messageText;
        [SerializeField] private Button okButton;
        [SerializeField] private Button backButton;

        /// <summary>
        /// パネル初期化
        /// </summary>
        public void Initialize(Action onOk, Action onBack)
        {
            okButton.onClick.AddListener(() => onOk.Invoke());
            backButton.onClick.AddListener(() => onBack.Invoke());
            panel.SetActive(false);
        }

        /// <summary>
        /// パネルを表示
        /// </summary>
        public void Show(string message)
        {
            messageText.text = message;
            panel.SetActive(true);
        }

        /// <summary>
        /// パネルを非表示
        /// </summary>
        public void Hide()
        {
            panel.SetActive(false);
        }
    }
}