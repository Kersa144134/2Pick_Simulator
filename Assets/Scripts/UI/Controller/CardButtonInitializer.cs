// ======================================================
// CardButtonInitializer.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-05
// 更新日時   : 2025-11-05
// 概要       : 各種カードボタン（フィルター・Deckable）を初期化し
//             CardButtonManagerへ登録する初期化専用クラス
// ======================================================

using CardGame.UISystem.Controller;
using CardGame.UISystem.Manager;
using System;
using System.Collections.Generic;
using static CardGame.CardSystem.Data.CardData;
using static CardGame.UISystem.Manager.OptionDisplayManager;

namespace CardGame.UISystem.Initializer
{
    /// <summary>
    /// カード関連ボタンを一括初期化する補助クラス  
    /// フィルター用・Deckable用ボタンを登録し、押下時に表示や枚数を更新
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
        // フィルター用ボタン初期化
        // ======================================================

        /// <summary>
        /// クラスフィルターボタンを初期化してCardButtonManagerに登録する
        /// </summary>
        /// <param name="classButtons">初期化対象のクラスボタン配列</param>
        public void InitializeFilterClassButtons(OptionDisplayManager.CardFilterButtonInfo[] classButtons)
        {
            if (classButtons == null || classButtons.Length == 0) return;

            for (int i = 0; i < classButtons.Length; i++)
            {
                OptionDisplayManager.CardFilterButtonInfo cb = classButtons[i];
                if (cb.Button == null) continue;

                CardClassButton btn = new CardClassButton(
                    cb.Button,
                    cb.ColorSettings,
                    cb.Value.ClassType,
                    cb.DefaultOn
                );
                _buttonManager.RegisterClassButton(btn);
                cb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        /// <summary>
        /// パックフィルターボタンを初期化してCardButtonManagerに登録する
        /// </summary>
        /// <param name="packButtons">初期化対象のパックボタン配列</param>
        public void InitializeFilterPackButtons(OptionDisplayManager.CardFilterButtonInfo[] packButtons)
        {
            if (packButtons == null || packButtons.Length == 0) return;

            for (int i = 0; i < packButtons.Length; i++)
            {
                OptionDisplayManager.CardFilterButtonInfo pb = packButtons[i];
                if (pb.Button == null) continue;

                CardPackButton btn = new CardPackButton(
                    pb.Button,
                    pb.ColorSettings,
                    pb.Value.PackId,
                    pb.DefaultOn
                );
                _buttonManager.RegisterPackButton(btn);
                pb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        /// <summary>
        /// レアリティフィルターボタンを初期化してCardButtonManagerに登録する
        /// </summary>
        /// <param name="rarityButtons">初期化対象のレアリティボタン配列</param>
        public void InitializeFilterRarityButtons(OptionDisplayManager.CardFilterButtonInfo[] rarityButtons)
        {
            if (rarityButtons == null || rarityButtons.Length == 0) return;

            for (int i = 0; i < rarityButtons.Length; i++)
            {
                OptionDisplayManager.CardFilterButtonInfo rb = rarityButtons[i];
                if (rb.Button == null) continue;

                CardRarityButton btn = new CardRarityButton(
                    rb.Button,
                    rb.ColorSettings,
                    rb.Value.Rarity,
                    rb.DefaultOn
                );
                _buttonManager.RegisterRarityButton(btn);
                rb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        /// <summary>
        /// コストフィルターボタンを初期化してCardButtonManagerに登録する
        /// </summary>
        /// <param name="costButtons">初期化対象のコストボタン配列</param>
        public void InitializeFilterCostButtons(OptionDisplayManager.CardFilterButtonInfo[] costButtons)
        {
            if (costButtons == null || costButtons.Length == 0) return;

            for (int i = 0; i < costButtons.Length; i++)
            {
                OptionDisplayManager.CardFilterButtonInfo cb = costButtons[i];
                if (cb.Button == null) continue;

                CardCostButton btn = new CardCostButton(
                    cb.Button,
                    cb.ColorSettings,
                    cb.Value.Cost,
                    cb.DefaultOn
                );
                _buttonManager.RegisterCostButton(btn);
                cb.Button.onClick.AddListener(() => _onButtonUpdate?.Invoke());
            }
        }

        // ======================================================
        // Deckable一括変更ボタン初期化
        // ======================================================

        /// <summary>
        /// Deckableボタンを初期化（Plus/Minusボタンと枚数Text）  
        /// 対象はパック・レアリティ・コスト
        /// </summary>
        /// <param name="deckableButtons">初期化対象のDeckableボタン配列</param>
        public void InitializeDeckableButtons(DeckableButtonInfo[] deckableButtons)
        {
            // Nullチェックと要素数確認
            if (deckableButtons == null || deckableButtons.Length == 0)
            {
                return;
            }

            // 各Deckableボタンに対して初期化処理を実行
            for (int i = 0; i < deckableButtons.Length; i++)
            {
                DeckableButtonInfo db = deckableButtons[i];

                // どちらのボタンも未設定の場合はスキップ
                if (db.PlusButton == null && db.MinusButton == null)
                {
                    continue;
                }

                // --------------------------------------------------
                // ＋ボタン押下時処理
                // --------------------------------------------------
                if (db.PlusButton != null)
                {
                    db.PlusButton.onClick.AddListener(() =>
                    {
                        // タイプに応じて増加処理
                        switch (db.TargetType)
                        {
                            case TargetEnum.Pack:
                                _buttonManager.SetAvailableByPack(db.Value.PackId, 1);
                                break;

                            case TargetEnum.Rarity:
                                _buttonManager.SetAvailableByRarity(db.Value.Rarity, 1);
                                break;

                            case TargetEnum.Cost:
                                _buttonManager.SetAvailableByCost(db.Value.Cost, 1);
                                break;

                            default:
                                return;
                        }

                        // 表示更新
                        UpdateDeckableCountText(db);
                        _onButtonUpdate?.Invoke();
                    });
                }

                // --------------------------------------------------
                // −ボタン押下時処理
                // --------------------------------------------------
                if (db.MinusButton != null)
                {
                    db.MinusButton.onClick.AddListener(() =>
                    {
                        // タイプに応じて減算処理
                        switch (db.TargetType)
                        {
                            case TargetEnum.Pack:
                                _buttonManager.SetAvailableByPack(db.Value.PackId, -1);
                                break;

                            case TargetEnum.Rarity:
                                _buttonManager.SetAvailableByRarity(db.Value.Rarity, -1);
                                break;

                            case TargetEnum.Cost:
                                _buttonManager.SetAvailableByCost(db.Value.Cost, -1);
                                break;

                            default:
                                return;
                        }

                        // 表示更新
                        UpdateDeckableCountText(db);
                        _onButtonUpdate?.Invoke();
                    });
                }

                // --------------------------------------------------
                // 初期表示更新
                // --------------------------------------------------
                if (db.CountText != null)
                {
                    UpdateDeckableCountText(db);
                }
            }
        }

        /// <summary>
        /// Deckableボタンに対応するCountTextを最新状態に更新する（簡易ログ版）
        /// </summary>
        /// <param name="db">更新対象の DeckableButtonInfo（単体）</param>
        private void UpdateDeckableCountText(DeckableButtonInfo db)
        {
            // 無効チェック：対象またはText未設定なら処理中断
            if (db == null || db.CountText == null)
            {
                return;
            }

            // 現在値初期化
            int current = 0;

            // 全辞書を取得（例外想定なし）
            IReadOnlyDictionary<int, int> packCounts = _buttonManager.GetAllPackAvailableCounts();
            IReadOnlyDictionary<CardRarity, int> rarityCounts = _buttonManager.GetAllRarityAvailableCounts();
            IReadOnlyDictionary<int, int> costCounts = _buttonManager.GetAllCostAvailableCounts();

            // --------------------------------------------------
            // 対象タイプ別にカウント値を決定
            // --------------------------------------------------
            switch (db.TargetType)
            {
                case TargetEnum.Pack:
                    if (db.Value != null && packCounts.ContainsKey(db.Value.PackId))
                    {
                        current = packCounts[db.Value.PackId];
                    }
                    break;

                case TargetEnum.Rarity:
                    if (db.Value != null && rarityCounts.ContainsKey(db.Value.Rarity))
                    {
                        current = rarityCounts[db.Value.Rarity];
                    }
                    break;

                case TargetEnum.Cost:
                    if (db.Value != null && costCounts.ContainsKey(db.Value.Cost))
                    {
                        current = costCounts[db.Value.Cost];
                    }
                    break;
            }

            // --------------------------------------------------
            // テキスト反映
            // --------------------------------------------------
            db.CountText.text = current.ToString();
        }

        // ======================================================
        // 一括初期化
        // ======================================================

        /// <summary>
        /// フィルター用ボタンとDeckable用ボタンをまとめて初期化する
        /// </summary>
        public void InitializeAll(
            OptionDisplayManager.CardFilterButtonInfo[] classButtons,
            OptionDisplayManager.CardFilterButtonInfo[] packButtons,
            OptionDisplayManager.CardFilterButtonInfo[] rarityButtons,
            OptionDisplayManager.CardFilterButtonInfo[] costButtons,
            OptionDisplayManager.DeckableButtonInfo[] packDeckables,
            OptionDisplayManager.DeckableButtonInfo[] rarityDeckables,
            OptionDisplayManager.DeckableButtonInfo[] costDeckables
        )
        {
            InitializeFilterClassButtons(classButtons);
            InitializeFilterPackButtons(packButtons);
            InitializeFilterRarityButtons(rarityButtons);
            InitializeFilterCostButtons(costButtons);

            _buttonManager.InitializeAvailableCountDictionaries();

            InitializeDeckableButtons(packDeckables);
            InitializeDeckableButtons(rarityDeckables);
            InitializeDeckableButtons(costDeckables);
        }
    }
}