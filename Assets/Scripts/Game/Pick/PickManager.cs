// ======================================================
// PickManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : PickScene管理クラス
//             PickCanvas / DeckCanvas の切り替え
//             Exitボタン押下時の確認パネル処理
//             ピック回数上限時にシーン遷移
// ======================================================

using CardGame.CardSystem.Data;
using CardGame.DeckSystem.Manager;
using CardGame.PickSystem.Manager;
using CardGame.UISystem.Controller;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// PickScene の Canvas 切り替えおよび Exit 確認処理を管理するクラス
    /// ピック回数管理と上限チェックも行う
    /// </summary>
    public class PickManager : MonoBehaviour
    {
        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>ピック順管理オブジェクト</summary>
        private PickSequenceManager _pickSequenceManager = new PickSequenceManager();

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
        /// <summary>背景変更用 Image</summary>
        private Image bgImage;

        [Header("確認パネル")]

        [SerializeField]
        /// <summary>Exit確認用パネルコントローラ</summary>
        private ConfirmPanelController confirmPanel;

        [Header("シーン設定")]
        [SerializeField]
        /// <summary>ピック上限到達時に遷移するシーン名</summary>
        private string targetSceneName;

        [SerializeField]
        /// <summary>Exitボタン押下後、確認パネルのOKで遷移するシーン名</summary>
        private string exitSceneName;

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
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// ピック実行処理
        /// ピック回数を進め、上限到達時に指定シーンへ遷移
        /// </summary>
        public void ExecutePick()
        {
            // ピック上限チェック
            if (_pickSequenceManager.GetRemainingPickCount() <= 0)
            {
                Debug.Log("ピック上限に到達しました。シーン遷移します。");
                SceneManager.LoadScene(targetSceneName);
                return;
            }

            // ピック順を進める
            _pickSequenceManager.IncrementPick();
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// デッキボタン押下時の処理
        /// ピック画面を非表示にし、デッキ画面を表示する。
        /// さらに DeckListManager 内のピック済みカードリスト内容をログに出力する。
        /// </summary>
        private void OnDeckButtonClicked()
        {
            // ピック画面を非表示にする
            pickCanvas.SetActive(false);

            // デッキ画面を表示する
            deckCanvas.SetActive(true);

            // インスタンスが存在する場合のみ処理を行う
            if (DeckListManager.Instance != null)
            {
                // ピック済みカードリストを取得
                List<CardData> pickedCards = DeckListManager.Instance.GetPickedCards();

                // ログ出力開始
                UnityEngine.Debug.Log($"[DeckListManager] 現在のピック済みカード数：{pickedCards.Count}");

                // 各カードの情報を列挙出力
                for (int i = 0; i < pickedCards.Count; i++)
                {
                    CardData card = pickedCards[i];
                    Debug.Log($"[{i + 1}] {card.CardName}（{card.Rarity} / {card.ClassType}）");
                }
            }
            else
            {
                // DeckListManagerが存在しない場合の警告
                Debug.LogWarning("DeckListManager がシーン上に存在しません。ピック済みカード情報を取得できません。");
            }
        }

        private void OnBackButtonClicked()
        {
            deckCanvas.SetActive(false);
            pickCanvas.SetActive(true);
        }

        private void OnExitButtonClicked()
        {
            confirmPanel.Show();
        }

        private void OnConfirmExitOk()
        {
            SceneManager.LoadScene(exitSceneName);
        }

        private void OnConfirmExitBack()
        {
            confirmPanel.Hide();
        }
    }
}