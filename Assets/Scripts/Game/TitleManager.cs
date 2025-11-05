// ======================================================
// TitleManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-05
// 概要       : タイトル画面の管理クラス
//              START / OPTION / FILTER ボタン押下に応じたキャンバスおよび表示制御
//              OPTIONキャンバス用の戻るボタンでタイトル画面に復帰
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.DeckSystem.Manager;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// タイトル画面管理クラス  
    /// START / OPTION / FILTER ボタン押下に応じて各キャンバス・UIを制御する。
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("キャンバス設定")]
        [SerializeField]
        /// <summary>タイトル画面用メインキャンバス</summary>
        private GameObject titleCanvas;

        [SerializeField]
        /// <summary>クラス選択用キャンバス</summary>
        private GameObject classSelectCanvas;

        [SerializeField]
        /// <summary>オプション設定用キャンバス</summary>
        private GameObject optionCanvas;

        [SerializeField]
        /// <summary>フィルター用オブジェクト群（表示ON/OFF対象）</summary>
        private GameObject[] filterGroup;

        [Header("ボタン設定")]
        [SerializeField]
        /// <summary>Startボタン</summary>
        private Button startButton;

        [SerializeField]
        /// <summary>Optionボタン</summary>
        private Button optionButton;

        [SerializeField]
        /// <summary>Optionキャンバス用の戻るボタン</summary>
        private Button optionBackButton;

        [SerializeField]
        /// <summary>Filterボタン</summary>
        private Button filterButton;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>フィルターグループが現在表示中かどうかを示すフラグ</summary>
        private bool _isFilterActive = false;

        // ======================================================
        // Unityイベント
        // ======================================================

        /// <summary>
        /// 初期化処理  
        /// ボタンのクリックイベント登録および初期表示設定を行う。
        /// </summary>
        private void Start()
        {
            // STARTボタン押下イベント登録
            startButton.onClick.AddListener(OnStartButtonClicked);

            // OPTIONボタン押下イベント登録
            optionButton.onClick.AddListener(OnOptionButtonClicked);

            // FILTERボタン押下イベント登録
            filterButton.onClick.AddListener(OnFilterButtonClicked);

            // OPTIONキャンバス用戻るボタン押下イベント登録
            if (optionBackButton != null)
            {
                optionBackButton.onClick.AddListener(OnOptionBackButtonClicked);
            }

            // 起動時はタイトル画面のみ表示
            if (titleCanvas != null)
            {
                titleCanvas.SetActive(true);
            }

            if (classSelectCanvas != null)
            {
                classSelectCanvas.SetActive(false);
            }

            if (optionCanvas != null)
            {
                optionCanvas.SetActive(false);
            }

            // 起動時はフィルター対象をすべて非表示にする
            SetFilterGroupActive(false);
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// STARTボタン押下時処理  
        /// タイトルキャンバスを非表示にしてクラス選択画面を表示する。
        /// </summary>
        private void OnStartButtonClicked()
        {
            if (titleCanvas != null)
            {
                titleCanvas.SetActive(false);
            }

            if (classSelectCanvas != null)
            {
                classSelectCanvas.SetActive(true);
            }

            // デッキ情報をリセット
            DeckListManager.Instance.ResetDeck();
        }

        /// <summary>
        /// OPTIONボタン押下時処理  
        /// タイトルキャンバスを非表示にしてオプション画面を表示する。
        /// </summary>
        private void OnOptionButtonClicked()
        {
            if (titleCanvas != null)
            {
                titleCanvas.SetActive(false);
            }

            if (optionCanvas != null)
            {
                optionCanvas.SetActive(true);
            }
        }

        /// <summary>
        /// FILTERボタン押下時処理  
        /// filterGroup 内の全オブジェクトを一括で ON / OFF 切り替える。
        /// </summary>
        private void OnFilterButtonClicked()
        {
            // 現在の状態を反転
            _isFilterActive = !_isFilterActive;

            // 状態に応じて一括反映
            SetFilterGroupActive(_isFilterActive);
        }

        /// <summary>
        /// OPTIONキャンバスの戻るボタン押下時処理  
        /// OptionCanvas を非表示にしてタイトルキャンバスを再表示する。
        /// </summary>
        private void OnOptionBackButtonClicked()
        {
            if (optionCanvas != null)
            {
                optionCanvas.SetActive(false);
            }

            if (titleCanvas != null)
            {
                titleCanvas.SetActive(true);
            }
        }

        /// <summary>
        /// フィルター対象群の表示状態を一括設定する。
        /// </summary>
        /// <param name="isActive">表示状態（trueで表示 / falseで非表示）</param>
        private void SetFilterGroupActive(bool isActive)
        {
            if (filterGroup == null || filterGroup.Length == 0)
            {
                return;
            }

            // 各オブジェクトのアクティブ状態を一括設定
            foreach (GameObject obj in filterGroup)
            {
                if (obj != null)
                {
                    obj.SetActive(isActive);
                }
            }
        }
    }
}