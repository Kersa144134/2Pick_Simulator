// ======================================================
// CardButtonInitializer.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : 各種カードボタン（クラス・パック・レアリティ・コスト）を
//             初期化し、CardButtonManagerへ登録する初期化専用クラス
// ======================================================

using System;
using UnityEngine;
using CardGame.UISystem.Controller;
using CardGame.UISystem.Manager;

namespace CardGame.UISystem.Initializer
{
    /// <summary>
    /// カード関連ボタンを一括初期化する補助クラス  
    /// ボタン配列を受け取り、CardButtonManagerへの登録とイベント設定を行う
    /// </summary>
    public class CardButtonInitializer
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>ボタン状態を統括管理するマネージャ</summary>
        private readonly CardButtonManager _buttonManager;

        /// <summary>カード可視状態を制御するクラス</summary>
        private readonly CardVisibilityController _visibilityController;

        /// <summary>ボタン更新後に呼び出す外部更新処理（カード再描画）</summary>
        private readonly Action _onButtonUpdate;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// CardButtonInitializerを生成  
        /// Managerとイベントを外部から注入して一元化する
        /// </summary>
        /// <param name="manager">CardButtonManager</param>
        /// <param name="visibilityController">カード表示管理クラス</param>
        /// <param name="onUpdate">ボタンクリック後に実行する更新処理</param>
        public CardButtonInitializer(
            CardButtonManager manager,
            CardVisibilityController visibilityController,
            Action onUpdate
        )
        {
            _buttonManager = manager;
            _visibilityController = visibilityController;
            _onButtonUpdate = onUpdate;
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// クラスボタンを初期化
        /// </summary>
        public void InitializeClassButtons(CardDisplayManager.CardClassButtonInfo[] classButtons)
        {
            if (classButtons == null || classButtons.Length == 0)
            {
                Debug.LogWarning("classButtons が未設定です。Inspectorで割り当ててください。");
                return;
            }

            foreach (var cb in classButtons)
            {
                if (cb.Button == null) continue;

                // ボタン生成と登録
                var btn = new CardClassButton(cb.Button, cb.ColorSettings, cb.Class, cb.DefaultOn);
                _buttonManager.RegisterClassButton(btn);

                // 押下時イベント登録
                cb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        /// <summary>
        /// パックボタンを初期化
        /// </summary>
        public void InitializePackButtons(CardDisplayManager.CardPackButtonInfo[] packButtons)
        {
            if (packButtons == null) return;

            foreach (var pb in packButtons)
            {
                if (pb.Button == null) continue;

                var btn = new CardPackButton(pb.Button, pb.ColorSettings, pb.PackId, pb.DefaultOn);
                _buttonManager.RegisterPackButton(btn);

                pb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        /// <summary>
        /// レアリティボタンを初期化
        /// </summary>
        public void InitializeRarityButtons(CardDisplayManager.CardRarityButtonInfo[] rarityButtons)
        {
            if (rarityButtons == null) return;

            foreach (var rb in rarityButtons)
            {
                if (rb.Button == null) continue;

                var btn = new CardRarityButton(rb.Button, rb.ColorSettings, rb.Rarity, rb.DefaultOn);
                _buttonManager.RegisterRarityButton(btn);

                rb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        /// <summary>
        /// コストボタンを初期化
        /// </summary>
        public void InitializeCostButtons(CardDisplayManager.CardCostButtonInfo[] costButtons)
        {
            if (costButtons == null) return;

            foreach (var cb in costButtons)
            {
                if (cb.Button == null) continue;

                var btn = new CardCostButton(cb.Button, cb.ColorSettings, cb.Cost, cb.DefaultOn);
                _buttonManager.RegisterCostButton(btn);

                cb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        /// <summary>
        /// すべてのボタンを一括初期化
        /// </summary>
        public void InitializeAll(
            CardDisplayManager.CardClassButtonInfo[] classButtons,
            CardDisplayManager.CardPackButtonInfo[] packButtons,
            CardDisplayManager.CardRarityButtonInfo[] rarityButtons,
            CardDisplayManager.CardCostButtonInfo[] costButtons
        )
        {
            InitializeClassButtons(classButtons);
            InitializePackButtons(packButtons);
            InitializeRarityButtons(rarityButtons);
            InitializeCostButtons(costButtons);
        }
    }
}