// ======================================================
// DeckManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-04
// 更新日時   : 2025-11-04
// 概要       : デッキ確認シーンの管理クラス
//              Exitボタン押下時に指定されたシーンへ遷移する
// ======================================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CardGame.DeckSystem.Manager
{
    /// <summary>
    /// デッキ確認シーン全体を管理するクラス  
    /// 主にシーン遷移（Exitボタン押下）を担当する。
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("ボタン設定")]
        [SerializeField]
        /// <summary>シーン遷移を行うExitボタン</summary>
        private Button exitButton;

        [Header("シーン設定")]
        [SerializeField]
        /// <summary>Exitボタン押下後に遷移するターゲットシーン名</summary>
        private string targetSceneName;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // Exitボタンが未設定の場合は警告を出して処理を中断
            if (exitButton == null)
            {
                Debug.LogWarning("Exitボタンが未設定です。");
                return;
            }

            // シーン名が空の場合も警告
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogWarning("Targetシーン名が未設定です。");
                return;
            }

            // ボタンイベントを登録
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        // ======================================================
        // ボタンイベント処理
        // ======================================================

        /// <summary>
        /// Exitボタン押下時の処理  
        /// 設定されたシーン名へ遷移する。
        /// </summary>
        private void OnExitButtonClicked()
        {
            // 指定されたシーンへ遷移
            SceneManager.LoadScene(targetSceneName);
        }
    }
}