// ======================================================
// TitleManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : タイトル画面の管理クラス
//             START / OPTION ボタン押下に応じたキャンバス表示制御
//             OPTIONキャンバス用の戻るボタンでタイトル画面に復帰
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.DeckSystem.Manager;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// タイトル画面管理クラス
    /// START / OPTION ボタンの押下に応じて対応キャンバスを表示し、
    /// OPTIONキャンバスの戻るボタンでタイトル画面に復帰
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

        // ======================================================
        // Unityイベント
        // ======================================================

        /// <summary>
        /// 初期化処理
        /// ボタンのクリックイベントを登録
        /// </summary>
        private void Start()
        {
            // STARTボタン押下時のイベント登録
            startButton.onClick.AddListener(OnStartButtonClicked);

            // OPTIONボタン押下時のイベント登録
            optionButton.onClick.AddListener(OnOptionButtonClicked);

            // OPTIONキャンバス用戻るボタン押下時のイベント登録
            if (optionBackButton != null)
            {
                optionBackButton.onClick.AddListener(OnOptionBackButtonClicked);
            }

            // 起動時はタイトル以外のキャンバスを非表示
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
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// STARTボタン押下時処理
        /// タイトルキャンバスを非表示にしてClassSelectCanvasを表示
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

            DeckListManager.Instance.ResetDeck();
        }

        /// <summary>
        /// OPTIONボタン押下時処理
        /// タイトルキャンバスを非表示にしてOptionCanvasを表示
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
        /// OPTIONキャンバスの戻るボタン押下時処理
        /// OptionCanvasを非表示にしてタイトル画面用キャンバスを表示
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
    }
}