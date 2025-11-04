// ======================================================
// PickManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-04
// 概要       : PickScene管理クラス
//             PickCanvas / DeckCanvas の切り替え
// ======================================================

using CardGame.CardSystem.Data;
using CardGame.DeckSystem.Manager;
using CardGame.PickSystem.Manager;
using CardGame.UISystem.Controller;
using CardGame.UISystem.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// PickScene の Canvas 切り替えおよび Exit 確認処理を管理するクラス  
    /// ピック回数管理・上限チェック・背景切り替えも担当する。
    /// </summary>
    public class PickManager : MonoBehaviour
    {

        // ======================================================
        // 列挙体
        // ======================================================

        /// <summary>カードピック時の選択方向を示す列挙体</summary>
        private enum PickSide
        {
            Left,
            Right
        }
        
        // ======================================================
        // 構造体定義
        // ======================================================

        /// <summary>クラス種別と背景スプライトの対応データ構造体</summary>
        [Serializable]
        private struct ClassBackgroundData
        {
            [Tooltip("対応するクラス種別")]
            public CardData.CardClass ClassType;

            [Tooltip("クラスに対応する背景スプライト")]
            public Sprite BackgroundSprite;
        }

        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("コンポーネント参照")]

        [SerializeField]
        /// <summary>カード表示を管理するコントローラ</summary>
        private PickCardDisplayManager _pickCardDisplayManager;
        
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

        [SerializeField]
        /// <summary>左選択ボタン（ピック実行）</summary>
        private Button leftPickButton;

        [SerializeField]
        /// <summary>右選択ボタン（ピック実行）</summary>
        private Button rightPickButton;

        [SerializeField]
        /// <summary>カード再抽選ボタン</summary>
        private Button redrawingButton;

        [Header("UI表示設定")]
        [SerializeField]
        /// <summary>現在のピック回数を表示する TextMeshPro テキスト</summary>
        private TMP_Text pickCountText;

        [SerializeField]
        /// <summary>現在の総カード枚数を表示する TextMeshPro テキスト</summary>
        private TMP_Text deckCountText;

        [SerializeField]
        [Tooltip("コストごとのカード枚数表示用 Text リスト（0～10）")]
        private List<Text> costCountsTexts = new List<Text>();

        [Header("背景設定")]

        [SerializeField]
        /// <summary>背景変更対象の Image コンポーネント</summary>
        private Image bgImage;

        [SerializeField]
        /// <summary>クラスごとの背景スプライト対応リスト</summary>
        private List<ClassBackgroundData> classBackgroundList = new List<ClassBackgroundData>();

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
        // コンポーネント参照
        // ======================================================

        /// <summary>カード抽選を管理するコントローラ</summary>
        private readonly PickSequenceManager _pickSequenceManager = new PickSequenceManager();

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>選択方向ごとの抽選結果（2枚ずつ保持）</summary>
        private Dictionary<PickSide, List<CardData>> _pickedCards = new Dictionary<PickSide, List<CardData>>();

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // 初期Canvas状態設定
            pickCanvas.SetActive(true);
            deckCanvas.SetActive(false);

            // ボタンイベント登録
            deckButton.onClick.AddListener(OnDeckButtonClicked);
            backButton.onClick.AddListener(OnBackButtonClicked);
            exitButton.onClick.AddListener(OnExitButtonClicked);
            leftPickButton.onClick.AddListener(() => ExecutePick(PickSide.Left));
            rightPickButton.onClick.AddListener(() => ExecutePick(PickSide.Right));
            redrawingButton.onClick.AddListener(ExecuteRedraw);

            // 確認パネル初期化
            confirmPanel.Initialize(OnConfirmExitOk, OnConfirmExitBack);

            // 選択クラスに応じて背景を設定
            UpdateBackgroundBySelectedClass();

            // 抽選回数のリセット
            _pickSequenceManager.ResetPickSequence();
            
            // 初回抽選実行
            ExecuteDraw();

            // 初回UI更新
            UpdatePickAndCardCountText();
            UpdateCostCountsTexts();
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// 通常の抽選
        /// </summary>
        private void ExecuteDraw()
        {
            DrawCards(() => _pickSequenceManager.GetNextPickRarities());
        }

        /// <summary>
        /// 再抽選
        /// </summary>
        private void ExecuteRedraw()
        {
            DrawCards(() => _pickSequenceManager.GetRedrawPickRarities());
        }

        /// <summary>
        /// ピック実行処理  
        /// 指定方向のカードを DeckListManager に登録し、ピック進行と再抽選を行う。  
        /// 上限到達時は指定シーンへ遷移する。
        /// </summary>
        /// <param name="pick">選択されたピック方向（左 or 右）</param>
        private void ExecutePick(PickSide pick)
        {
            // 指定方向に対応するカードリストを取得
            List<CardData> cards = _pickedCards[pick];

            // 各カードを DeckListManager に登録
            foreach (CardData card in cards)
            {
                if (card != null)
                {
                    DeckListManager.Instance.AddPickedCard(card);
                }
            }

            // ピック回数を進める
            _pickSequenceManager.IncrementPick();

            // ピック上限チェック
            if (_pickSequenceManager.GetRemainingPickCount() <= 0)
            {
                SceneManager.LoadScene(targetSceneName);
                return;
            }

            // UI更新
            UpdatePickAndCardCountText();
            UpdateCostCountsTexts();

            // 新たなピック候補を再抽選
            ExecuteDraw();
        }

        /// <summary>
        /// デッキボタン押下時の処理  
        /// ピック画面を非表示にし、デッキ画面を表示する。  
        /// さらに DeckListManager 内のピック済みカードリスト内容をログに出力する。
        /// </summary>
        private void OnDeckButtonClicked()
        {
            pickCanvas.SetActive(false);
            deckCanvas.SetActive(true);

            if (DeckListManager.Instance != null)
            {
                // ピック済みカード情報を取得
                List<DeckListManager.PickedCardEntry> pickedEntries = DeckListManager.Instance.GetPickedCardEntries();

                // 総カード枚数を計算（重複を含めた合計）
                int totalCardCount = 0;
                foreach (DeckListManager.PickedCardEntry entry in pickedEntries)
                {
                    totalCardCount += entry.Count;
                }

                // 各カード情報を枚数付きで列挙出力
                for (int i = 0; i < pickedEntries.Count; i++)
                {
                    DeckListManager.PickedCardEntry entry = pickedEntries[i];
                    CardData card = entry.Card;
                    int count = entry.Count;
                }
            }
            else
            {
                Debug.LogWarning("DeckListManager がシーン上に存在しません。ピック済みカード情報を取得できません。");
            }
        }

        /// <summary>
        /// 戻るボタン押下時の処理  
        /// デッキキャンバスを閉じ、ピックキャンバスを再表示する。
        /// </summary>
        private void OnBackButtonClicked()
        {
            deckCanvas.SetActive(false);
            pickCanvas.SetActive(true);
        }

        /// <summary>
        /// Exitボタン押下時の処理  
        /// 確認パネルを表示する。
        /// </summary>
        private void OnExitButtonClicked()
        {
            confirmPanel.Show();
        }

        /// <summary>
        /// Exit確認パネルのOK押下時処理  
        /// Exitシーンへ遷移する。
        /// </summary>
        private void OnConfirmExitOk()
        {
            SceneManager.LoadScene(exitSceneName);
        }

        /// <summary>
        /// Exit確認パネルのBack押下時処理  
        /// 確認パネルを閉じる。
        /// </summary>
        private void OnConfirmExitBack()
        {
            confirmPanel.Hide();
        }

        // ======================================================
        // 背景切り替え関連
        // ======================================================

        /// <summary>
        /// 現在のピック回数と総カード枚数を TextMeshPro に反映する
        /// </summary>
        private void UpdatePickAndCardCountText()
        {
            // ピック回数を更新（例： "Pick 3 / 10"）
            int currentIndex = _pickSequenceManager.CurrentPickIndex + 1;
            int maxPick = _pickSequenceManager.GetMaxPickCount();
            pickCountText.text = $"{currentIndex.ToString("00")} / {maxPick}   Pick";

            // 総カード枚数を DeckListManager から集計
            int totalCardCount = 0;
            if (DeckListManager.Instance != null)
            {
                List<DeckListManager.PickedCardEntry> pickedEntries = DeckListManager.Instance.GetPickedCardEntries();
                foreach (DeckListManager.PickedCardEntry entry in pickedEntries)
                {
                    totalCardCount += entry.Count;
                }
            }

            deckCountText.text = $"{totalCardCount.ToString("00")} / 40";
        }

        /// <summary>
        /// デッキ内のコストごとのカード枚数を更新して TextMeshPro に反映する
        /// </summary>
        private void UpdateCostCountsTexts()
        {
            if (DeckListManager.Instance == null || costCountsTexts.Count == 0)
            {
                return;
            }

            // コストごとの枚数を初期化
            int[] costCounts = new int[costCountsTexts.Count];

            // ピック済みカードリストを取得
            List<DeckListManager.PickedCardEntry> pickedEntries = DeckListManager.Instance.GetPickedCardEntries();

            foreach (var entry in pickedEntries)
            {
                int cost = entry.Card != null ? entry.Card.CardCost : -1;

                if (cost >= 0 && cost < costCounts.Length)
                {
                    // 同コストのカード枚数を加算
                    costCounts[cost] += entry.Count;
                }
            }

            // Text に反映
            for (int i = 0; i < costCounts.Length; i++)
            {
                Text text = costCountsTexts[i];
                if (text != null)
                {
                    text.text = costCounts[i].ToString();
                }
            }
        }

        /// <summary>
        /// DeckListManager の SelectedClass に対応する背景画像を設定する
        /// </summary>
        private void UpdateBackgroundBySelectedClass()
        {
            if (DeckListManager.Instance == null)
            {
                Debug.LogWarning("DeckListManager が存在しません。背景変更をスキップします。");
                return;
            }

            CardData.CardClass selectedClass = DeckListManager.Instance.SelectedClass;

            foreach (ClassBackgroundData data in classBackgroundList)
            {
                if (data.ClassType == selectedClass && data.BackgroundSprite != null)
                {
                    bgImage.sprite = data.BackgroundSprite;
                    return;
                }
            }

            Debug.LogWarning($"選択クラス {selectedClass} に対応する背景スプライトが設定されていません。");
        }

        // ======================================================
        // ヘルパーメソッド
        // ======================================================

        /// <summary>
        /// カード抽選処理共通メソッド  
        /// 指定のレアリティ配列取得関数を使って4枚のカードを抽選し、左右2枚ずつに振り分ける。
        /// </summary>
        /// <param name="getRarities">抽選に使うレアリティ配列を返す関数</param>
        private void DrawCards(Func<CardData.CardRarity[]> getRarities)
        {
            if (getRarities == null)
            {
                Debug.LogWarning("DrawCards: getRarities が null です。");
                return;
            }

            // レアリティ配列を取得
            CardData.CardRarity[] rarities = getRarities();

            // メインクラス候補カードを4枚抽選
            List<CardData> pickCards = _pickCardDisplayManager.PickMainClassCards(rarities);

            // 前回の抽選結果を初期化
            _pickedCards.Clear();

            // 左側に2枚登録
            _pickedCards.Add(PickSide.Left, new List<CardData> { pickCards[0], pickCards[1] });

            // 右側に2枚登録
            _pickedCards.Add(PickSide.Right, new List<CardData> { pickCards[2], pickCards[3] });
        }
    }
}