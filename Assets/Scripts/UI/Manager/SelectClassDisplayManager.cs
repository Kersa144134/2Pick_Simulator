// ======================================================
// SelectClassDisplayManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-04
// 概要       : クラス選択キャンバス管理クラス
//             クラスボタン押下で確認パネルを表示し、
//             選択されたクラスの2枚のカードをDeckListに登録してPickSceneへ遷移
// ======================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using CardGame.DeckSystem.Manager;
using CardGame.UISystem.Controller;
using CardGame.PickSystem.Controller;

namespace CardGame.UISystem.Manager
{
    /// <summary>
    /// クラス選択キャンバスの管理クラス  
    /// クラスごとの初回抽選を行い、選択されたクラスのカードをデッキに登録してシーン遷移する。
    /// </summary>
    public class SelectClassDisplayManager : MonoBehaviour
    {
        // ======================================================
        // 構造体
        // ======================================================

        /// <summary>
        /// クラスボタンとクラス種別の対応構造体
        /// </summary>
        [Serializable]
        private struct ClassButtonData
        {
            public Button Button;
            public CardData.CardClass Class;
        }

        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("クラスボタン設定")]
        [SerializeField]
        /// <summary>8クラスのボタン配列</summary>
        private ClassButtonData[] classButtonDataArray;

        [Header("確認パネル設定")]
        [SerializeField]
        /// <summary>選択確認用パネル</summary>
        private ConfirmPanelController confirmPanel;

        [Header("シーン設定")]
        [SerializeField]
        /// <summary>ピック上限到達時に遷移するシーン名</summary>
        private string targetSceneName = "PickScene";

        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>カードデータベース（抽選元データ）</summary>
        private CardDatabase _cardDatabase;

        /// <summary>カード表示を管理するコントローラ</summary>
        private readonly ClassCardDisplayController _cardDisplayController = new ClassCardDisplayController();

        /// <summary>カード抽選を管理するコントローラ</summary>
        private readonly ClassCardPickController _cardPickController = new ClassCardPickController();

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>選択中のクラスインデックス</summary>
        private int _selectedClassIndex = -1;

        /// <summary>各クラスごとの抽選結果（2枚ずつ保持）</summary>
        private Dictionary<CardData.CardClass, List<CardData>> _classPickedCards = new Dictionary<CardData.CardClass, List<CardData>>();

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // --------------------------------------------------
            // カードデータベース取得
            // --------------------------------------------------
            CardDatabaseManager manager = CardDatabaseManager.Instance;
            if (manager == null)
            {
                Debug.LogError("CardDatabaseManagerが存在しません。");
                return;
            }

            _cardDatabase = manager.GetCardDatabase();

            // --------------------------------------------------
            // 初回抽選を実施（全クラス分）
            // --------------------------------------------------
            UpdateClassCards();

            // --------------------------------------------------
            // ボタン押下イベントを登録
            // --------------------------------------------------
            for (int i = 0; i < classButtonDataArray.Length; i++)
            {
                int index = i;
                classButtonDataArray[i].Button.onClick.AddListener(() => OnClassButtonClicked(index));
            }

            // --------------------------------------------------
            // 確認パネル初期化
            // --------------------------------------------------
            confirmPanel.Initialize(OnConfirmOk, OnConfirmBack);
        }

        private void OnEnable()
        {
            // 再有効化時にデータベースが存在し、未抽選なら抽選を行う
            if (_cardDatabase != null && _classPickedCards.Count == 0)
            {
                UpdateClassCards();
            }
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// 各クラスボタン上のカード画像を更新し、抽選結果を保持する
        /// </summary>
        private void UpdateClassCards()
        {
            _classPickedCards.Clear();

            foreach (ClassButtonData btnData in classButtonDataArray)
            {
                if (btnData.Button == null)
                {
                    continue;
                }

                // 最初の抽選処理
                List<CardData> pickedCards = _cardPickController.PickInitialClassCards(_cardDatabase, btnData.Class);

                // 表示処理（抽選結果をUIに反映）
                _cardDisplayController.DisplayInitialPickedCards(btnData.Button, pickedCards);

                // 結果を保持（クラスごとに格納）
                if (!_classPickedCards.ContainsKey(btnData.Class))
                {
                    _classPickedCards.Add(btnData.Class, pickedCards);
                }
            }
        }

        /// <summary>
        /// クラスボタン押下時に確認パネルを表示する
        /// </summary>
        /// <param name="classIndex">選択されたクラスのインデックス</param>
        private void OnClassButtonClicked(int classIndex)
        {
            _selectedClassIndex = classIndex;

            if (classIndex < 0 || classIndex >= classButtonDataArray.Length)
            {
                Debug.LogError($"不正なクラスインデックス: {classIndex}");
                return;
            }

            CardData.CardClass selectedClass = classButtonDataArray[classIndex].Class;

            // クラス名をDBのマップから取得
            string className = _cardDatabase.ClassNameMap.ContainsKey(selectedClass)
                ? _cardDatabase.ClassNameMap[selectedClass]
                : "不明";

            // 確認パネル表示
            confirmPanel.Show($"{className} を選択しますか？");
        }

        /// <summary>
        /// 確認パネルのOKボタン押下時に選択されたクラスのカードを登録しシーン遷移
        /// </summary>
        private void OnConfirmOk()
        {
            if (_selectedClassIndex < 0 || _selectedClassIndex >= classButtonDataArray.Length)
            {
                Debug.LogError("不正なクラスインデックスでDeckListManagerに登録できません。");
                return;
            }

            // --------------------------------------------------
            // クラス情報を取得
            // --------------------------------------------------
            CardData.CardClass selectedClass = classButtonDataArray[_selectedClassIndex].Class;

            // --------------------------------------------------
            // DeckListManagerへ登録
            // --------------------------------------------------
            DeckListManager.Instance.SelectedClass = selectedClass;

            if (_classPickedCards.ContainsKey(selectedClass))
            {
                List<CardData> cards = _classPickedCards[selectedClass];
                foreach (CardData card in cards)
                {
                    DeckListManager.Instance.AddPickedCard(card);
                }
            }
            else
            {
                Debug.LogWarning($"クラス {selectedClass} の抽選データが存在しません。");
            }

            // --------------------------------------------------
            // 確認パネルを閉じ、PickSceneへ遷移
            // --------------------------------------------------
            confirmPanel.Hide();
            SceneManager.LoadScene(targetSceneName);
        }

        /// <summary>
        /// 確認パネルのBACKボタン押下時  
        /// パネルを閉じるのみ
        /// </summary>
        private void OnConfirmBack()
        {
            confirmPanel.Hide();
        }
    }
}