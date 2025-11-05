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
using TMPro;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using CardGame.CardSystem.Utility;
using CardGame.UISystem.Controller;
using CardGame.UISystem.Initializer;

namespace CardGame.UISystem.Manager
{
    /// <summary>
    /// カード画像の表示制御およびクラスボタンによる一括切替管理クラス
    /// </summary>
    public class OptionDisplayManager : MonoBehaviour
    {
        // ======================================================
        // ラッパークラス
        // ======================================================

        /// <summary>
        /// フィルター対象の種類
        /// </summary>
        public enum TargetEnum
        {
            Class,
            Pack,
            Rarity,
            Cost
        }

        /// <summary>
        /// 対象値の汎用構造体
        /// </summary>
        [System.Serializable]
        public class TargetValue
        {
            public CardData.CardClass ClassType;
            public int PackId;
            public CardData.CardRarity Rarity;
            public int Cost;
        }

        /// <summary>
        /// フィルター用ボタン共通インターフェース
        /// </summary>
        public interface IFilterButton
        {
            Button Button { get; }
            TargetValue Value { get; }
            bool DefaultOn { get; }
            ButtonColorSettings ColorSettings { get; }
        }

        /// <summary>
        /// カードフィルターボタン情報
        /// </summary>
        [System.Serializable]
        public class CardFilterButtonInfo : IFilterButton
        {
            public Button Button;
            public TargetEnum TargetType;
            public TargetValue Value;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;

            // IFilterButton 実装
            Button IFilterButton.Button => Button;
            TargetValue IFilterButton.Value => Value;
            bool IFilterButton.DefaultOn => DefaultOn;
            ButtonColorSettings IFilterButton.ColorSettings => ColorSettings;
        }

        /// <summary>
        /// Deckable 一括変更ボタン情報
        /// </summary>
        [System.Serializable]
        public class DeckableButtonInfo
        {
            public Button PlusButton;
            public Button MinusButton;
            public TargetEnum TargetType;
            public TargetValue Value;
            public TextMeshProUGUI CountText;
        }

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

        [Header("フィルターボタン設定")]
        /// <summary>各カードクラス用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardFilterButtonInfo[] classFilterButtons;

        /// <summary>各カードパック用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardFilterButtonInfo[] packFilterButtons;

        /// <summary>各カードレアリティ用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardFilterButtonInfo[] rarityFilterButtons;

        /// <summary>各カードコスト用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private CardFilterButtonInfo[] costFilterButtons;

        [Header("一括提示枚数変更ボタン設定")]
        /// <summary>各カードパック用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private DeckableButtonInfo[] packDeckableButtons;

        /// <summary>各カードレアリティ用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private DeckableButtonInfo[] rarityDeckableButtons;

        /// <summary>各カードコスト用ボタンと初期表示状態を設定する配列</summary>
        [SerializeField]
        private DeckableButtonInfo[] costDeckableButtons;

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
        private CardDisplayRefresher _displayRefresher;

        /// <summary>スクロール制御用コントローラ（RectTransform移動版）</summary>
        private CardScrollController _scrollController;

        /// <summary>カードの表示/非表示を一括管理するクラス</summary>
        private CardVisibilityController _visibilityController;

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

        private void Update()
        {
            _scrollController?.Update();
        }

        // ======================================================
        // ボタン初期化処理
        // ======================================================

        /// <summary>
        /// すべてのボタンを初期化して CardButtonManager に登録
        /// </summary>
        private void SetupAllButtons()
        {
            CardButtonInitializer initializer = new CardButtonInitializer(
                _buttonManager,
                _visibilityController,
                () =>
                {
                    _visibleCardData = _visibilityController.GetVisibleCards();
                    RefreshDisplay();
                }
            );

            // フィルター用ボタン変換
            CardFilterButtonInfo[] classTargets = ConvertToFilterButtonInfo(classFilterButtons, TargetEnum.Class);
            CardFilterButtonInfo[] packTargets = ConvertToFilterButtonInfo(packFilterButtons, TargetEnum.Pack);
            CardFilterButtonInfo[] rarityTargets = ConvertToFilterButtonInfo(rarityFilterButtons, TargetEnum.Rarity);
            CardFilterButtonInfo[] costTargets = ConvertToFilterButtonInfo(costFilterButtons, TargetEnum.Cost);

            // Deckableボタン
            DeckableButtonInfo[] packDeckables = packDeckableButtons;
            DeckableButtonInfo[] rarityDeckables = rarityDeckableButtons;
            DeckableButtonInfo[] costDeckables = costDeckableButtons;
            
            // 一括初期化
            initializer.InitializeAll(
                classTargets,
                packTargets,
                rarityTargets,
                costTargets,
                packDeckables,
                rarityDeckables,
                costDeckables
            );

            // 外部からのカード更新イベント
            _buttonManager.OnCardsUpdated += () =>
            {
                _visibleCardData = _visibilityController.GetVisibleCards();
                RefreshDisplay();
            };
        }

        /// <summary>
        /// 任意のフィルターボタン配列を CardFilterButtonInfo 共通型に変換
        /// </summary>
        private CardFilterButtonInfo[] ConvertToFilterButtonInfo<T>(T[] sourceButtons, TargetEnum targetType)
            where T : IFilterButton
        {
            CardFilterButtonInfo[] result = new CardFilterButtonInfo[sourceButtons.Length];

            for (int i = 0; i < sourceButtons.Length; i++)
            {
                T btn = sourceButtons[i];

                result[i] = new CardFilterButtonInfo
                {
                    Button = btn.Button,
                    TargetType = targetType,
                    Value = btn.Value,
                    DefaultOn = btn.DefaultOn,
                    ColorSettings = btn.ColorSettings
                };
            }

            return result;
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
            _buttonManager = new CardButtonManager(_visibilityController, _cardDatabase);
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
    }
}