// ======================================================
// CardDisplayManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-10-31
// 概要       : CardDataのCardImageをCanvas上に横一列で表示する管理クラス
//             クラスボタン押下で所属カードの表示/非表示を一括切替可能
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Utility;
using CardGame.UISystem.Controller;
using static CardGame.CardSystem.Data.CardData;

namespace CardGame.UISystem.Manager
{
    /// <summary>
    /// カード画像の表示制御およびクラスボタンによる一括切替管理クラス
    /// </summary>
    public class CardDisplayManager : MonoBehaviour
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

        /// <summary>CardDataをロード、キャッシュするクラス</summary>
        private CardDataLoader _loader;

        /// <summary>スクロール制御用コントローラ（RectTransform移動版）</summary>
        private CardScrollController _scrollController;

        /// <summary>カードの表示/非表示を一括管理するクラス</summary>
        private CardVisibilityController _visibilityController;

        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("スクロール設定")]
        /// <summary>カード画像のスクロール速度およびスクロール範囲設定</summary>
        [SerializeField]
        private CardScrollSettings scrollSettings;

        [Header("表示設定")]
        /// <summary>カードを横に並べる際の間隔（px）</summary>
        [SerializeField, Min(0f)]
        private float horizontalSpacing = 100f;

        /// <summary>カード表示用のPrefab（Image付きUIオブジェクト）</summary>
        [SerializeField]
        private GameObject cardPrefab = null;

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

        /// <summary>Canvas上に生成されたカードオブジェクトのリスト</summary>
        private readonly List<GameObject> _displayedCards = new List<GameObject>();

        /// <summary>現在表示対象となるCardDataリスト</summary>
        private List<CardData> _visibleCardData = new List<CardData>();

        /// <summary>カード生成親RectTransform</summary>
        private RectTransform _parentTransform;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            // RectTransform取得
            _parentTransform = GetComponent<RectTransform>();

            // ScrollController初期化（RectTransform直接移動版）
            _scrollController = new CardScrollController(_parentTransform, scrollSettings);

            // CardDataロード
            _loader = new CardDataLoader();
            _loader.LoadAllCardData();

            // 表示管理初期化
            _visibilityController = new CardVisibilityController(_loader.AllCardData);
            _visibleCardData = _visibilityController.GetVisibleCards();

            // CardButtonManager初期化
            _buttonManager = new CardButtonManager(_visibilityController, _loader);

            // ボタン設定
            SetupAllButtons();

            // カード生成
            RefreshDisplay();
        }

        private void Update()
        {
            _scrollController?.Update();
        }

        // ======================================================
        // ボタン初期化処理
        // ======================================================

        /// <summary>すべてのボタンを初期化してCardButtonManagerに登録</summary>
        private void SetupAllButtons()
        {
            SetupClassButtons();
            SetupPackButtons();
            SetupRarityButtons();
            SetupCostButtons();

            // CardButtonManagerの更新イベントにCanvas更新処理を登録
            // これでどのフィルターでも変更後にRefreshDisplayが呼ばれる
            _buttonManager.OnCardsUpdated += () =>
            {
                _visibleCardData = _visibilityController.GetVisibleCards();
                RefreshDisplay();
            };
        }

        /// <summary>クラスボタン初期化</summary>
        private void SetupClassButtons()
        {
            if (classButtons == null || classButtons.Length == 0)
            {
                Debug.LogWarning("classButtons が未設定です。Inspectorで割り当ててください。");
                return;
            }

            foreach (var cb in classButtons)
            {
                if (cb.Button != null)
                {
                    var classBtnInstance = new CardClassButton(cb.Button, cb.ColorSettings, cb.Class, cb.DefaultOn);
                    _buttonManager.RegisterClassButton(classBtnInstance);

                    // ボタンクリック時に表示を即更新
                    cb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        /// <summary>パックボタン初期化</summary>
        private void SetupPackButtons()
        {
            if (packButtons == null) return;

            foreach (var pb in packButtons)
            {
                if (pb.Button != null)
                {
                    var btn = new CardPackButton(pb.Button, pb.ColorSettings, pb.PackId, pb.DefaultOn);
                    _buttonManager.RegisterPackButton(btn);

                    pb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        /// <summary>レアリティボタン初期化</summary>
        private void SetupRarityButtons()
        {
            if (rarityButtons == null) return;

            foreach (var rb in rarityButtons)
            {
                if (rb.Button != null)
                {
                    var btn = new CardRarityButton(rb.Button, rb.ColorSettings, rb.Rarity, rb.DefaultOn);
                    _buttonManager.RegisterRarityButton(btn);

                    rb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        /// <summary>コストボタン初期化</summary>
        private void SetupCostButtons()
        {
            if (costButtons == null) return;

            foreach (var cb in costButtons)
            {
                if (cb.Button != null)
                {
                    var btn = new CardCostButton(cb.Button, cb.ColorSettings, cb.Cost, cb.DefaultOn);
                    _buttonManager.RegisterCostButton(btn);

                    cb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        // ======================================================
        // 表示更新処理
        // ======================================================

        /// <summary>
        /// 現在表示対象のCardDataリストに基づき、カードを生成・配置
        /// ScrollRect非使用、RectTransform直接移動で配置
        /// </summary>
        public void RefreshDisplay()
        {
            // 既存カード削除
            foreach (GameObject cardGO in _displayedCards)
            {
                GameObject.Destroy(cardGO);
            }
            _displayedCards.Clear();

            // カード生成
            for (int i = 0; i < _visibleCardData.Count; i++)
            {
                CardData cardData = _visibleCardData[i];

                if (cardData.CardImage == null || cardPrefab == null) continue;

                GameObject cardGO = GameObject.Instantiate(cardPrefab, _parentTransform);

                Image img = cardGO.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = cardData.CardImage;
                    img.preserveAspect = true;
                }

                RectTransform rt = cardGO.GetComponent<RectTransform>();
                if (rt != null) rt.anchoredPosition = new Vector2(i * horizontalSpacing, 0f);

                _displayedCards.Add(cardGO);
            }

            // スクロール位置リセット
            _scrollController?.ResetScrollPosition();
        }
    }
}