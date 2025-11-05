// ======================================================
// CardButtonManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-11-05
// 概要       : カードフィルタボタン（クラス・パック・レアリティ・コスト）を管理
//              ボタン押下による状態変更を統合し、カード表示/非表示更新および
//              提示可能枚数制御を行う
// ======================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using CardGame.CardSystem.Utility;
using static CardGame.CardSystem.Data.CardData;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// ボタン押下時のオン／オフ色を保持する構造体
    /// </summary>
    [Serializable]
    public struct ButtonColorSettings
    {
        /// <summary>ボタン押下時の色</summary>
        public Color OnColor;

        /// <summary>ボタン非押下時の色</summary>
        public Color OffColor;
    }

    /// <summary>
    /// ジェネリックなカードフィルタボタンの基底クラス
    /// </summary>
    /// <typeparam name="T">フィルタ対象の値（クラス、パック、レアリティ、コスト）</typeparam>
    public class CardFilterButton<T>
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>対象となるUIボタンコンポーネント</summary>
        protected Button _button;

        /// <summary>ボタン押下時のオン／オフ色を保持する構造体</summary>
        protected ButtonColorSettings _colorSettings;

        /// <summary>現在のボタン押下状態（オン＝true、オフ＝false）</summary>
        protected bool _isActive;

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>ボタンの現在押下状態</summary>
        public bool IsActive => _isActive;

        /// <summary>このボタンが保持するフィルタ値</summary>
        public T FilterValue { get; protected set; }

        // ======================================================
        // イベント
        // ======================================================

        /// <summary>ボタン押下時に状態が変化した際に通知されるイベント</summary>
        public event Action<CardFilterButton<T>> OnFilterToggled;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// コンストラクタ：ボタン初期化、クリックイベント登録
        /// </summary>
        public CardFilterButton(Button button, ButtonColorSettings colorSettings, T value, bool defaultOn)
        {
            _button = button;
            _colorSettings = colorSettings;
            _isActive = defaultOn;
            FilterValue = value;

            if (_button != null)
            {
                Image img = _button.GetComponent<Image>();
                if (img != null)
                {
                    img.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
                }

                _button.onClick.AddListener(() =>
                {
                    ToggleColor();
                    OnFilterToggled?.Invoke(this);
                });
            }
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// ボタン押下時にオン／オフ色を切り替える
        /// </summary>
        public void ToggleColor()
        {
            _isActive = !_isActive;

            Image img = _button?.GetComponent<Image>();
            if (img != null)
            {
                img.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
            }
        }
    }

    /// <summary>カードクラス用フィルタボタン</summary>
    public class CardClassButton : CardFilterButton<CardClass>
    {
        public CardClassButton(Button button, ButtonColorSettings colorSettings, CardClass value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn) { }
    }

    /// <summary>カードパック用フィルタボタン</summary>
    public class CardPackButton : CardFilterButton<int>
    {
        public CardPackButton(Button button, ButtonColorSettings colorSettings, int value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn) { }
    }

    /// <summary>カードレアリティ用フィルタボタン</summary>
    public class CardRarityButton : CardFilterButton<CardRarity>
    {
        public CardRarityButton(Button button, ButtonColorSettings colorSettings, CardRarity value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn) { }
    }

    /// <summary>カードコスト用フィルタボタン</summary>
    public class CardCostButton : CardFilterButton<int>
    {
        public CardCostButton(Button button, ButtonColorSettings colorSettings, int value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn) { }
    }

    // ======================================================
    // メインクラス：カードフィルタ・提示枚数管理
    // ======================================================

    public class CardButtonManager
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>カード1種あたりの最大提示可能枚数</summary>
        private const int MAX_AVAILABLE_COUNT = 3;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>カードデータベース参照</summary>
        private readonly CardDatabase _database;

        /// <summary>カードロードヘルパー</summary>
        private readonly CardDataLoader _loader;

        /// <summary>カード可視制御コンポーネント</summary>
        private readonly CardVisibilityController _visibilityController;

        /// <summary>クラスボタンリスト</summary>
        public List<CardClassButton> ClassButtons = new List<CardClassButton>();

        /// <summary>パックボタンリスト</summary>
        public List<CardPackButton> PackButtons = new List<CardPackButton>();

        /// <summary>レアリティボタンリスト</summary>
        public List<CardRarityButton> RarityButtons = new List<CardRarityButton>();

        /// <summary>コストボタンリスト</summary>
        public List<CardCostButton> CostButtons = new List<CardCostButton>();

        /// <summary>パックごとの提示枚数</summary>
        private readonly Dictionary<int, int> _packAvailableCounts = new Dictionary<int, int>();

        /// <summary>レアリティごとの提示枚数</summary>
        private readonly Dictionary<CardRarity, int> _rarityAvailableCounts = new Dictionary<CardRarity, int>();

        /// <summary>コストごとの提示枚数</summary>
        private readonly Dictionary<int, int> _costAvailableCounts = new Dictionary<int, int>();

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>
        /// 全パックの提示可能枚数辞書を取得する（読み取り専用）
        /// </summary>
        public IReadOnlyDictionary<int, int> GetAllPackAvailableCounts()
        {
            // null対策と安全な参照渡し
            if (_packAvailableCounts == null)
            {
                return new Dictionary<int, int>();
            }

            // DictionaryはIReadOnlyDictionaryとしてキャスト可能
            return _packAvailableCounts;
        }

        /// <summary>
        /// 全レアリティの提示可能枚数辞書を取得する（読み取り専用）
        /// </summary>
        public IReadOnlyDictionary<CardRarity, int> GetAllRarityAvailableCounts()
        {
            if (_rarityAvailableCounts == null)
            {
                return new Dictionary<CardRarity, int>();
            }

            return _rarityAvailableCounts;
        }

        /// <summary>
        /// 全コストの提示可能枚数辞書を取得する（読み取り専用）
        /// </summary>
        public IReadOnlyDictionary<int, int> GetAllCostAvailableCounts()
        {
            if (_costAvailableCounts == null)
            {
                return new Dictionary<int, int>();
            }

            return _costAvailableCounts;
        }

        // ======================================================
        // イベント
        // ======================================================

        /// <summary>カード表示更新時に通知されるイベント</summary>
        public event Action OnCardsUpdated;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CardButtonManager(CardVisibilityController visibilityController, CardDatabase database)
        {
            _visibilityController = visibilityController;
            _database = database;
            _loader = CardDatabaseManager.Instance.GetCardDataLoader();
        }

        // ======================================================  
        // ボタン登録メソッド  
        // ======================================================  

        /// <summary>カードクラスボタンを登録</summary>  
        /// <param name="button">登録対象ボタン</param>  
        public void RegisterClassButton(CardClassButton button)
        {
            ClassButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }

        /// <summary>カードパックボタンを登録</summary>  
        public void RegisterPackButton(CardPackButton button)
        {
            PackButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }

        /// <summary>カードレアリティボタンを登録</summary>  
        public void RegisterRarityButton(CardRarityButton button)
        {
            RarityButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }

        /// <summary>カードコストボタンを登録</summary>  
        public void RegisterCostButton(CardCostButton button)
        {
            CostButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }
        
        // ======================================================
        // フィルタ適用処理
        // ======================================================

        /// <summary>
        /// 全ボタンのフィルタを適用し表示/非表示を更新
        /// </summary>
        /// <typeparam name="T">押下されたボタンの型</typeparam>
        /// <param name="changedButton">押下されたボタン</param>
        private void ApplyFilters<T>(CardFilterButton<T> changedButton)
        {
            if (_loader == null || _visibilityController == null)
            {
                return;
            }

            // まず全カードをベースにリスト作成
            List<CardData> filteredCards = new List<CardData>(_loader.AllCardData);

            // --------------------------------------------------
            // クラスフィルタ適用
            // --------------------------------------------------
            List<CardClassButton> activeClassButtons = ClassButtons.FindAll(b => b.IsActive);
            if (activeClassButtons.Count > 0)
            {
                filteredCards = filteredCards.FindAll(cd => activeClassButtons.Exists(b => cd.ClassType == b.FilterValue));
            }

            // --------------------------------------------------
            // パックフィルタ適用
            // --------------------------------------------------
            List<CardPackButton> activePackButtons = PackButtons.FindAll(b => b.IsActive);
            if (activePackButtons.Count > 0)
            {
                filteredCards = filteredCards.FindAll(cd => activePackButtons.Exists(b => cd.PackNumber == b.FilterValue));
            }

            // --------------------------------------------------
            // レアリティフィルタ適用
            // --------------------------------------------------
            List<CardRarityButton> activeRarityButtons = RarityButtons.FindAll(b => b.IsActive);
            if (activeRarityButtons.Count > 0)
            {
                filteredCards = filteredCards.FindAll(cd => activeRarityButtons.Exists(b => cd.Rarity == b.FilterValue));
            }

            // --------------------------------------------------
            // コストフィルタ適用
            // --------------------------------------------------
            List<CardCostButton> activeCostButtons = CostButtons.FindAll(b => b.IsActive);
            if (activeCostButtons.Count > 0)
            {
                filteredCards = filteredCards.FindAll(cd =>
                {
                    foreach (CardCostButton b in activeCostButtons)
                    {
                        // ボタン値が10ならコスト10以上
                        if (b.FilterValue == 10 && cd.CardCost >= 10)
                        {
                            return true;
                        }
                        // 通常一致
                        else if (cd.CardCost == b.FilterValue)
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }

            // --------------------------------------------------
            // 表示更新
            // --------------------------------------------------
            _visibilityController.HideAll();
            _visibilityController.ShowCards(filteredCards);

            OnCardsUpdated?.Invoke();
        }

        // ======================================================
        // 提示枚数制御共通ヘルパー
        // ======================================================

        /// <summary>
        /// 指定辞書の値を更新し、上限3・下限0にクランプして返す
        /// </summary>
        private int UpdateAvailableCount<TKey>(Dictionary<TKey, int> dict, TKey key, int delta)
        {
            int currentValue = MAX_AVAILABLE_COUNT;
            if (dict.ContainsKey(key))
            {
                currentValue = dict[key];
            }

            int newValue = Mathf.Clamp(currentValue + delta, 0, MAX_AVAILABLE_COUNT);
            dict[key] = newValue;
            return newValue;
        }

        // ======================================================
        // 一括操作：パック
        // ======================================================

        /// <summary>
        /// 特定パックの提示可能枚数を増減させる
        /// </summary>
        public void SetAvailableByPack(int packNumber, int delta)
        {
            if (_database == null)
            {
                return;
            }

            int newValue = UpdateAvailableCount(_packAvailableCounts, packNumber, delta);
            _database.SetAvailableByPack(packNumber, newValue);
            OnCardsUpdated?.Invoke();
        }

        // ======================================================
        // 一括操作：レアリティ
        // ======================================================

        /// <summary>
        /// 特定レアリティの提示可能枚数を増減させる
        /// </summary>
        public void SetAvailableByRarity(CardRarity rarity, int delta)
        {
            if (_database == null)
            {
                return;
            }

            int newValue = UpdateAvailableCount(_rarityAvailableCounts, rarity, delta);
            _database.SetAvailableByRarity(rarity, newValue);
            OnCardsUpdated?.Invoke();
        }

        // ======================================================
        // 一括操作：コスト
        // ======================================================

        /// <summary>
        /// 特定コストの提示可能枚数を増減させる
        /// </summary>
        public void SetAvailableByCost(int cost, int delta)
        {
            if (_database == null)
            {
                return;
            }

            int newValue = UpdateAvailableCount(_costAvailableCounts, cost, delta);
            _database.SetAvailableByCost(cost, newValue);
            OnCardsUpdated?.Invoke();
        }
    }
}