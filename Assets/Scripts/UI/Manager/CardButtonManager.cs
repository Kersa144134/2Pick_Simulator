// ======================================================
// CardButtonManager.cs
// 作成者 : 高橋一翔
// 作成日時 : 2025-10-31
// 更新日時 : 2025-10-31
// 概要 : カードフィルタボタン（クラス・パック・レアリティ・コスト）を管理
// ボタン押下による状態変更を統合し、カード表示/非表示更新を通知
// ======================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
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

        /// <summary>コンストラクタ：ボタン初期化、クリックイベント登録</summary>  
        /// <param name="button">対象Button</param>  
        /// <param name="colorSettings">オン／オフ色設定</param>  
        /// <param name="value">このボタンのフィルタ値</param>  
        /// <param name="defaultOn">初期状態をオンにするか</param>  
        public CardFilterButton(Button button, ButtonColorSettings colorSettings, T value, bool defaultOn)
        {
            _button = button;
            _colorSettings = colorSettings;
            _isActive = defaultOn;
            FilterValue = value;

            if (_button != null)
            {
                Image img = _button.GetComponent<Image>();
                if (img != null) img.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;

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

        /// <summary>ボタン押下時にオン／オフ色を切り替える</summary>  
        public void ToggleColor()
        {
            _isActive = !_isActive;

            Image img = _button?.GetComponent<Image>();
            if (img != null) img.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
        }
    }

    /// <summary>カードクラス用フィルタボタン</summary>  
    public class CardClassButton : CardFilterButton<CardClass>
    {
        public CardClassButton(Button button, ButtonColorSettings colorSettings, CardClass value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>カードパック用フィルタボタン</summary>  
    public class CardPackButton : CardFilterButton<int>
    {
        public CardPackButton(Button button, ButtonColorSettings colorSettings, int value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>カードレアリティ用フィルタボタン</summary>  
    public class CardRarityButton : CardFilterButton<CardRarity>
    {
        public CardRarityButton(Button button, ButtonColorSettings colorSettings, CardRarity value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>カードコスト用フィルタボタン</summary>  
    public class CardCostButton : CardFilterButton<int>
    {
        public CardCostButton(Button button, ButtonColorSettings colorSettings, int value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>複数カードフィルタボタンを統合管理するクラス</summary>  
    public class CardButtonManager
    {
        // ======================================================  
        // フィールド  
        // ======================================================  

        private readonly CardVisibilityController _visibilityController;
        private readonly CardDataLoader _loader;

        /// <summary>カードクラスボタンリスト</summary>  
        public List<CardClassButton> ClassButtons { get; private set; } = new List<CardClassButton>();

        /// <summary>カードパックボタンリスト</summary>  
        public List<CardPackButton> PackButtons { get; private set; } = new List<CardPackButton>();

        /// <summary>カードレアリティボタンリスト</summary>  
        public List<CardRarityButton> RarityButtons { get; private set; } = new List<CardRarityButton>();

        /// <summary>カードコストボタンリスト</summary>  
        public List<CardCostButton> CostButtons { get; private set; } = new List<CardCostButton>();

        // ======================================================  
        // イベント  
        // ======================================================  

        /// <summary>カード表示更新時に通知されるイベント</summary>  
        public event Action OnCardsUpdated;

        // ======================================================  
        // コンストラクタ  
        // ======================================================  

        /// <summary>CardButtonManager初期化</summary>  
        /// <param name="visibilityController">表示／非表示管理クラス</param>  
        /// <param name="loader">CardDataロードクラス</param>  
        public CardButtonManager(CardVisibilityController visibilityController, CardDataLoader loader)
        {
            _visibilityController = visibilityController;
            _loader = loader;
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

        /// <summary>全ボタンのフィルタを適用し表示/非表示を更新</summary>  
        /// <typeparam name="T">押下されたボタンの型</typeparam>  
        /// <param name="changedButton">押下されたボタン</param>  
        private void ApplyFilters<T>(CardFilterButton<T> changedButton)
        {
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
                        // ボタンの値が10なら、カードコストが10以上を対象
                        if (b.FilterValue == 10 && cd.CardCost >= 10)
                        {
                            return true;
                        }
                        // それ以外は通常の一致判定
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
    }
}