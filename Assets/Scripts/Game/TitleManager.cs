// ======================================================
// TitleManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-06
// 概要       : タイトル画面の管理クラス
//              START / OPTION / FILTER / MASSCHANGE / WEIGHTCHANGE / REDRAWCHANGE
//              各ボタン押下に応じたキャンバスおよび詳細オプション表示を制御
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Manager;
using CardGame.DeckSystem.Manager;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// タイトル画面管理クラス  
    /// START / OPTION / FILTER / MASSCHANGE / WEIGHTCHANGE / REDRAWCHANGE ボタン押下時の
    /// 各キャンバス・UIの表示状態を統一管理する。
    /// </summary>
    public class TitleManager : MonoBehaviour
    {
        // ======================================================
        // 列挙体定義
        // ======================================================

        /// <summary>
        /// 詳細オプションの状態  
        /// 同時に一つのみ有効（None状態時は全て非表示）
        /// </summary>
        private enum DetailedOptionState
        {
            None,
            Filter,
            MassChange,
            WeightChange,
            RedrawChange
        }

        [Header("キャンバス設定")]

        [SerializeField]
        /// <summary>タイトル画面用のメインキャンバス</summary>
        private GameObject titleCanvas;

        [SerializeField]
        /// <summary>クラス選択画面用キャンバス</summary>
        private GameObject classSelectCanvas;

        [SerializeField]
        /// <summary>オプション設定画面用キャンバス</summary>
        private GameObject optionCanvas;


        [Header("詳細オプションオブジェクト")]

        [SerializeField]
        /// <summary>詳細オプション全体を覆うパネル（None以外の状態で表示）</summary>
        private GameObject detailedOptionPanel;
        
        [SerializeField]
        /// <summary>フィルター設定UIをまとめたオブジェクト</summary>
        private GameObject filterObj;

        [SerializeField]
        /// <summary>一括変更設定UIをまとめたオブジェクト</summary>
        private GameObject massChangeObj;

        [SerializeField]
        /// <summary>抽選倍率変更UIをまとめたオブジェクト</summary>
        private GameObject weightChangeObj;

        [SerializeField]
        /// <summary>再抽選回数変更UIをまとめたオブジェクト</summary>
        private GameObject redrawChangeObj;


        [Header("ボタン設定")]

        [SerializeField]
        /// <summary>ゲーム開始用の Start ボタン</summary>
        private Button startButton;

        [SerializeField]
        /// <summary>オプション画面を開く Option ボタン</summary>
        private Button optionButton;

        [SerializeField]
        /// <summary>オプション画面からタイトル画面へ戻る Back ボタン</summary>
        private Button optionBackButton;

        [SerializeField]
        /// <summary>フィルター設定を開閉する Filter ボタン</summary>
        private Button filterButton;

        [SerializeField]
        /// <summary>一括変更設定を開閉する MassChange ボタン</summary>
        private Button massChangeButton;

        [SerializeField]
        /// <summary>抽選倍率変更UIを開閉する WeightChange ボタン</summary>
        private Button weightChangeButton;

        [SerializeField]
        /// <summary>再抽選回数変更UIを開閉する RedrawChange ボタン</summary>
        private Button redrawChangeButton;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>現在有効な詳細オプション状態</summary>
        private DetailedOptionState _currentOptionState = DetailedOptionState.None;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // 各種ボタンにクリックイベントを登録
            startButton.onClick.AddListener(OnStartButtonClicked);
            optionButton.onClick.AddListener(OnOptionButtonClicked);
            optionBackButton.onClick.AddListener(OnOptionBackButtonClicked);
            filterButton.onClick.AddListener(() => OnDetailedOptionButtonClicked(DetailedOptionState.Filter));
            massChangeButton.onClick.AddListener(() => OnDetailedOptionButtonClicked(DetailedOptionState.MassChange));
            weightChangeButton.onClick.AddListener(() => OnDetailedOptionButtonClicked(DetailedOptionState.WeightChange));
            redrawChangeButton.onClick.AddListener(() => OnDetailedOptionButtonClicked(DetailedOptionState.RedrawChange));

            // 起動時のキャンバス表示設定
            if (titleCanvas != null) titleCanvas.SetActive(true);
            if (classSelectCanvas != null) classSelectCanvas.SetActive(false);
            if (optionCanvas != null) optionCanvas.SetActive(false);

            // パネルを非表示
            detailedOptionPanel.SetActive(false);

            // 全詳細オプションを初期非表示
            SetAllDetailedOptionsActive(false);

            // 初期化処理：デッキ可能枚数リセット
            CardDatabaseManager.Instance.GetCardDatabase().ResetAllDeckableCopies();
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// STARTボタン押下時処理  
        /// タイトル画面を閉じ、クラス選択画面を表示。
        /// </summary>
        private void OnStartButtonClicked()
        {
            titleCanvas?.SetActive(false);
            classSelectCanvas?.SetActive(true);

            // デッキ情報リセット
            DeckListManager.Instance.ResetDeck();
        }

        /// <summary>
        /// OPTIONボタン押下時処理  
        /// タイトル画面を閉じ、オプション画面を表示。
        /// </summary>
        private void OnOptionButtonClicked()
        {
            titleCanvas?.SetActive(false);
            optionCanvas?.SetActive(true);
        }

        /// <summary>
        /// OPTION戻るボタン押下時処理  
        /// オプション画面を閉じ、タイトル画面を再表示。
        /// </summary>
        private void OnOptionBackButtonClicked()
        {
            optionCanvas?.SetActive(false);
            titleCanvas?.SetActive(true);
        }

        /// <summary>
        /// 詳細オプションボタン押下時処理  
        /// 既に同じ状態なら解除、異なる場合は切り替えを行う。
        /// </summary>
        /// <param name="nextState">押下されたボタンに対応する状態</param>
        private void OnDetailedOptionButtonClicked(DetailedOptionState nextState)
        {
            // 同じボタンを再度押した場合は解除
            if (_currentOptionState == nextState)
            {
                _currentOptionState = DetailedOptionState.None;
                UpdateDetailedOptionDisplay();
                return;
            }

            // 状態を更新
            _currentOptionState = nextState;

            // 状態に応じて表示切替
            UpdateDetailedOptionDisplay();
        }

        /// <summary>
        /// 詳細オプション表示を状態に応じて切り替える。
        /// </summary>
        private void UpdateDetailedOptionDisplay()
        {
            // パネルを表示
            detailedOptionPanel.SetActive(true);
            
            // すべてのグループをいったん非表示
            SetAllDetailedOptionsActive(false);

            // 現在の状態に応じて対象グループのみ表示
            switch (_currentOptionState)
            {
                case DetailedOptionState.Filter:
                    filterObj.SetActive(true);
                    break;
                case DetailedOptionState.MassChange:
                    massChangeObj.SetActive(true);
                    break;
                case DetailedOptionState.WeightChange:
                    weightChangeObj.SetActive(true);
                    break;
                case DetailedOptionState.RedrawChange:
                    redrawChangeObj.SetActive(true);
                    break;
                case DetailedOptionState.None:
                default:
                    detailedOptionPanel.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// 全ての詳細オプショングループを非表示に設定。
        /// </summary>
        /// <param name="isActive">表示状態（通常false）</param>
        private void SetAllDetailedOptionsActive(bool isActive)
        {
            filterObj.SetActive(isActive);
            massChangeObj.SetActive(isActive);
            weightChangeObj.SetActive(isActive);
            redrawChangeObj.SetActive(isActive);
        }
    }
}