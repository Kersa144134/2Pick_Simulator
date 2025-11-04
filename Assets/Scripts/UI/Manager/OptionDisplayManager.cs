// ======================================================
// OptionDisplayManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-10-31
// 概要       : Option画面でCardImageをCanvas上に横一列で表示する管理クラス
//             クラスボタン押下で所属カードの表示/非表示を一括切替可能
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using CardGame.CardSystem.Utility;
using CardGame.UISystem.Controller;
using CardGame.UISystem.Initializer;
using static CardGame.CardSystem.Data.CardData;

namespace CardGame.UISystem.Manager
{
    /// <summary>
    /// カード画像の表示制御およびクラスボタンによる一括切替管理クラス
    /// </summary>
    public class OptionDisplayManager : MonoBehaviour
    {
        // ======================================================
        // ラッパー構造体（Inspector表示用）
        // ======================================================

        /// <summary>
        /// クラスボタン情報ラッパー
        /// InspectorでUIボタンと初期状態を設定可能
        /// </summary>
        [System.Serializable]
        public struct CardClassButtonInfo
        {
            public Button Button;
            public CardClass Class;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        [System.Serializable]
        public struct CardPackButtonInfo
        {
            public Button Button;
            public int PackId;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        [System.Serializable]
        public struct CardRarityButtonInfo
        {
            public Button Button;
            public CardRarity Rarity;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        [System.Serializable]
        public struct CardCostButtonInfo
        {
            public Button Button;
            public int Cost;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>カードフィルタボタンの状態管理と押下イベントを統合したクラス</summary>
        private CardButtonManager _buttonManager;

        /// <summary>カードデータベース（最大枚数管理用）</summary>
        private CardDatabase _cardDatabase;
        
        /// <summary>CardDataをロード、キャッシュするクラス</summary>
        private CardDataLoader _loader = new CardDataLoader();

        /// <summary></summary>
        private CardFilterGroupController _filterGroupController;

        /// <summary></summary>
        private CardDisplayRefresher _displayRefresher;

        /// <summary>スクロール制御用コントローラ（RectTransform移動版）</summary>
        private CardScrollController _scrollController;

        /// <summary>カードの表示/非表示を一括管理するクラス</summary>
        private CardVisibilityController _visibilityController;

        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("スクロール設定")]
        [SerializeField, Min(1f)]
        private float scrollMoveSpeed = 1000f;

        [Header("表示設定")]
        /// <summary>カードを横に並べる際の間隔（px）</summary>
        [SerializeField, Min(0f)]
        private float horizontalSpacing = 100f;

        [SerializeField]
        /// <summary>表示中カードのRectTransform</summary>
        private RectTransform _visibleParent;

        [SerializeField]
        /// <summary>非表示カードの一時待避先RectTransform</summary>
        private RectTransform _hiddenParent;

        /// <summary>カード表示用のPrefab（Image付きUIオブジェクト）</summary>
        [SerializeField]
        private GameObject cardPrefab = null;

        [Header("フィルターボタングループ")]
        /// <summary>クラスボタン群の親</summary>
        [SerializeField] private GameObject classButtonGroup;

        /// <summary>パックボタン群の親</summary>
        [SerializeField] private GameObject packButtonGroup;

        /// <summary>レアリティボタン群の親</summary>
        [SerializeField] private GameObject rarityButtonGroup;

        /// <summary>コストボタン群の親</summary>
        [SerializeField] private GameObject costButtonGroup;

        /// <summary>フィルターモード時に表示するパネル</summary>
        [SerializeField] private GameObject filterPanel;

        [Header("クラスボタン設定")]
        /// <summary>各カードクラス用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardClassButtonInfo[] classButtons;

        [Header("パックボタン設定")]
        /// <summary>各カードパック用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardPackButtonInfo[] packButtons;

        [Header("レアリティボタン設定")]
        /// <summary>各カードレアリティ用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardRarityButtonInfo[] rarityButtons;

        [Header("コストボタン設定")]
        /// <summary>各カードコスト用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardCostButtonInfo[] costButtons;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>生成済みのカード表示コンポーネントリスト</summary>
        private readonly List<CardDisplay> _cardDisplays = new List<CardDisplay>();

        /// <summary>現在表示対象となるCardDataリスト</summary>
        private List<CardData> _visibleCardData = new List<CardData>();

        /// <summary>すべてのフィルターグループをまとめた一括制御用配列</summary>
        private GameObject[] _filterGroups;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            InitializeCardData();
            InitializeButtonManager();
            InitializeFilterGroups();
            GenerateCardObjects();

            _scrollController = new CardScrollController(_visibleParent, scrollMoveSpeed);
            _displayRefresher = new CardDisplayRefresher(
                _visibleParent,
                _hiddenParent,
                horizontalSpacing,
                _scrollController,
                _cardDisplays
            );

            RefreshDisplay();
        }

        private void OnEnable()
        {
            _filterGroupController?.SetAllGroupsActive(false);
        }

        private void Update()
        {
            _scrollController?.Update();
        }

        // ======================================================
        // ボタン初期化処理
        // ======================================================

        /// <summary>
        /// すべてのボタンを初期化してCardButtonManagerに登録する  
        /// 各ボタンの押下時に表示更新を行う
        /// </summary>
        private void SetupAllButtons()
        {
            // 初期化ヘルパー生成
            CardButtonInitializer initializer = new CardButtonInitializer(
                _buttonManager,
                _visibilityController,
                () =>
                {
                    _visibleCardData = _visibilityController.GetVisibleCards();
                    RefreshDisplay();
                }
            );

            // 各ボタン群の初期化を一括実行
            initializer.InitializeAll(classButtons, packButtons, rarityButtons, costButtons);

            // 外部からの更新通知イベントに対応
            _buttonManager.OnCardsUpdated += () =>
            {
                _visibleCardData = _visibilityController.GetVisibleCards();
                RefreshDisplay();
            };
        }

        // ======================================================
        // 初期化系メソッド群
        // ======================================================

        /// <summary>カードデータのロードと初期化を行う</summary>
        private void InitializeCardData()
        {
            // シングルトンからデータ取得
            CardDatabaseManager manager = CardDatabaseManager.Instance;
            if (manager == null)
            {
                Debug.LogError("CardDatabaseManagerが存在しません。");
                return;
            }

            _cardDatabase = manager.GetCardDatabase();
            CardDataLoader loader = manager.GetCardDataLoader();

            _loader = loader;
            _visibilityController = new CardVisibilityController(_loader.AllCardData);
            _visibleCardData = _visibilityController.GetVisibleCards();
        }

        /// <summary>ボタン管理クラスとイベントを初期化する</summary>
        private void InitializeButtonManager()
        {
            _buttonManager = new CardButtonManager(_visibilityController, _loader);
            SetupAllButtons();
        }

        /// <summary>カードPrefabを生成し、CardDisplayを初期化する</summary>
        private void GenerateCardObjects()
        {
            for (int i = 0; i < _loader.AllCardData.Count; i++)
            {
                CardData data = _loader.AllCardData[i];
                GameObject cardObject = Instantiate(cardPrefab, _hiddenParent);
                CardDisplay display = cardObject.GetComponent<CardDisplay>();
                display.Initialize(data, _cardDatabase);
                _cardDisplays.Add(display);
            }
        }

        /// <summary>フィルタグループ配列を構築し、初期非表示にする</summary>
        private void InitializeFilterGroups()
        {
            _filterGroups = new GameObject[]
            {
                classButtonGroup,
                packButtonGroup,
                rarityButtonGroup,
                costButtonGroup,
                filterPanel
            };

            _filterGroupController = new CardFilterGroupController(_filterGroups);
        }

        // ======================================================
        // 表示更新処理
        // ======================================================

        /// <summary>
        /// 現在の可視カードデータに基づいてUI表示を再構成する
        /// </summary>
        private void RefreshDisplay()
        {
            _displayRefresher.Refresh(_visibleCardData);
        }

        // ======================================================
        // フィルターボタンパネル制御
        // ======================================================

        /// <summary>
        /// フィルター群全体の表示/非表示をトグル切替する  
        /// オプションボタンから呼び出される
        /// </summary>
        public void ToggleAllOptionButtons()
        {
            if (_filterGroupController == null)
            {
                return;
            }

            _filterGroupController.ToggleAllGroups();
        }
    }
}