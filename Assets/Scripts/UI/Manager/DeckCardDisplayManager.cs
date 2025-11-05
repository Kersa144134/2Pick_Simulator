// ======================================================
// DeckCardDisplayManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-04 (改訂: 要素交互配置対応)
// 更新日時   : 2025-11-04
// 概要       : DeckListManagerの内容をUI上に2行（上・下）で横方向に交互に配置する管理クラス。
//              要素インデックスが偶数なら上段、奇数なら下段へ配置する。
//              各行はそれぞれX=0から始め、行内は horizontalSpacing 間隔で右方向へ並べる。
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using CardGame.CardSystem.Utility;
using CardGame.DeckSystem.Manager;
using CardGame.UISystem.Controller;

namespace CardGame.UISystem.Manager
{
    /// <summary>
    /// デッキ内カードの一覧をUI上で2行構成（上・下段）に並べて表示するクラス
    /// </summary>
    public class DeckCardDisplayManager : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("スクロール設定")]
        [SerializeField, Min(1f)]
        private float scrollMoveSpeed = 100f;
        
        [Header("レイアウト設定")]

        [SerializeField]
        /// <summary>上段カードの親オブジェクト</summary>
        private RectTransform upRowRoot;

        [SerializeField]
        /// <summary>下段カードの親オブジェクト</summary>
        private RectTransform downRowRoot;

        [SerializeField]
        /// <summary>生成済みカードを一時退避する非表示用親オブジェクト</summary>
        private RectTransform hiddenParent;

        [SerializeField]
        /// <summary>カード表示用のプレハブ</summary>
        private GameObject cardPrefab;

        [Header("整列設定")]

        [SerializeField, Min(1f)]
        /// <summary>カード間の水平間隔（px）</summary>
        private float horizontalSpacing = 150f;

        // ======================================================
        // コンポーネント参照
        // ======================================================

        /// <summary>カードデータベース参照（初期化時に取得）</summary>
        private CardDatabase _cardDatabase;

        /// <summary>カードデータローダ参照（初期化時に取得）</summary>
        private CardDataLoader _loader;

        /// <summary>スクロール制御用コントローラ（RectTransform移動版）</summary>
        private CardScrollController _scrollController;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>生成済み全カードの表示コンポーネントを保持するリスト</summary>
        private readonly List<CardDisplay> _cardDisplays = new List<CardDisplay>();

        // ======================================================
        // 定数
        // ======================================================

        /// <summary>スクロール範囲計算で全体幅補正に使う枚数補正値</summary>
        private const int SCROLL_WIDTH_OFFSET = 1;

        /// <summary>スクロールの左端位置</summary>
        private const float SCROLL_MIN_X_DEFAULT = 0f;

        /// <summary>スクロールの右端最大位置</summary>
        private const float SCROLL_MAX_X_DEFAULT = 0f;
        
        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            InitializeCardData();
            GenerateAllCardObjects();

            _scrollController = new CardScrollController(upRowRoot, scrollMoveSpeed);
            
            RefreshDeckDisplay();
        }

        private void OnEnable()
        {
            RefreshDeckDisplay();
        }

        private void Update()
        {
            _scrollController?.Update();

            downRowRoot.anchoredPosition = new Vector2(upRowRoot.anchoredPosition.x, downRowRoot.anchoredPosition.y);
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        // --------------------------------------------------
        // 初期化処理
        // --------------------------------------------------

        /// <summary>
        /// カードデータベースとローダを初期化する。
        /// </summary>
        private void InitializeCardData()
        {
            // CardDatabaseManager のシングルトン参照を取得
            CardDatabaseManager manager = CardDatabaseManager.Instance;

            // manager が null の場合は処理を中止する（以降の処理で参照が必要なため）
            if (manager == null)
            {
                Debug.LogError("CardDatabaseManager が存在しません。デッキ表示を中止します。");
                return;
            }

            // CardDatabase と CardDataLoader を取得してフィールドへ保持する
            _cardDatabase = manager.GetCardDatabase();
            _loader = manager.GetCardDataLoader();
        }

        /// <summary>
        /// 全カードをプレハブ化して hiddenParent 配下で非表示保持する。
        /// </summary>
        private void GenerateAllCardObjects()
        {
            // ローダが未初期化、またはデータが存在しない場合は処理を中止する
            if (_loader == null || _loader.AllCardData == null)
            {
                Debug.LogError("CardDataLoader が初期化されていません。カードオブジェクト生成を中止します。");
                return;
            }

            // 全カードデータに対してプレハブ生成を行う
            foreach (CardData data in _loader.AllCardData)
            {
                // プレハブからカードオブジェクトを hiddenParent 配下に生成する
                GameObject cardObj = Instantiate(cardPrefab, hiddenParent);

                // 生成オブジェクトから CardDisplay コンポーネントを取得する
                CardDisplay display = cardObj.GetComponent<CardDisplay>();

                // CardDisplay が存在しない場合は警告を出力し、次のデータへ
                if (display == null)
                {
                    Debug.LogWarning("cardPrefab に CardDisplay コンポーネントがアタッチされていません。");
                    Destroy(cardObj);
                    continue;
                }

                // 取得した CardData を CardDisplay に初期化バインドする
                display.Initialize(data, _cardDatabase);

                // 生成した表示コンポーネントをリストへ登録して管理する
                _cardDisplays.Add(display);
            }
        }

        // --------------------------------------------------
        // デッキ表示更新処理
        // --------------------------------------------------

        /// <summary>
        /// DeckListManager のカード内容を UI に反映し、要素インデックスの偶奇で上下を交互に配置する。
        /// 上段・下段それぞれは独立して X=0 から開始し、右方向へ horizontalSpacing 間隔で並べる。
        /// </summary>
        private void RefreshDeckDisplay()
        {
            // DeckListManager の存在確認
            if (DeckListManager.Instance == null)
            {
                Debug.LogWarning("DeckListManager が存在しません。デッキ表示を更新できません。");
                return;
            }

            if (_cardDisplays == null || _cardDisplays.Count <= 0)
            {
                return;
            }

            // ピック済みカードの一覧を取得する
            List<DeckListManager.PickedCardEntry> pickedEntries = DeckListManager.Instance.GetPickedCardEntries();

            // 既存表示をすべて非表示化して hiddenParent へ退避する
            ClearDisplay();

            // 上段・下段それぞれの列インデックスを独立してカウント
            int upRowColumnCounter = 0;
            int downRowColumnCounter = 0;

            // 全要素を走査し、インデックスの偶奇に応じて上段/下段へ配置
            for (int i = 0; i < pickedEntries.Count; i++)
            {
                DeckListManager.PickedCardEntry entry = pickedEntries[i];
                CardDisplay display = FindCardDisplay(entry.Card);

                if (display == null)
                {
                    Debug.LogWarning($"カード {entry.Card.CardName} の表示オブジェクトが見つかりません。");
                    continue;
                }

                display.SetCountText(entry.Count);

                bool isEvenIndex = (i % 2 == 0);
                RectTransform targetParent = isEvenIndex ? upRowRoot : downRowRoot;
                display.transform.SetParent(targetParent, false);

                RectTransform rect = display.GetComponent<RectTransform>();

                int columnIndex;
                if (isEvenIndex)
                {
                    columnIndex = upRowColumnCounter;
                    upRowColumnCounter++;
                }
                else
                {
                    columnIndex = downRowColumnCounter;
                    downRowColumnCounter++;
                }

                // spacing のみで位置を決定
                float xPos = columnIndex * horizontalSpacing;

                Vector2 currentAnchored = rect.anchoredPosition;
                rect.anchoredPosition = new Vector2(xPos, currentAnchored.y);
            }

            // --------------------------------------------------
            // スクロール範囲を上段のカード枚数をもとに算出
            // --------------------------------------------------
            float minScrollX = CalculateScrollRange(upRowColumnCounter);
            _scrollController.SetScrollRange(minScrollX);
            ClampScrollPosition(minScrollX);
        }

        // --------------------------------------------------
        // ユーティリティ
        // --------------------------------------------------

        /// <summary>
        /// 指定した CardData に対応する CardDisplay を検索して返す。
        /// 同一参照でなくても CardId が一致すれば一致とみなす。
        /// </summary>
        private CardDisplay FindCardDisplay(CardData target)
        {
            // 登録済み全カードを走査し、CardId 一致で判定する
            foreach (CardDisplay display in _cardDisplays)
            {
                if (display != null && display.TargetData != null)
                {
                    if (display.TargetData.CardId == target.CardId)
                    {
                        return display;
                    }
                }
            }

            // 一致なしの場合は null
            return null;
        }

        /// <summary>
        /// 全カードを非表示（hiddenParent 配下）へ戻し、UIから退避させる
        /// </summary>
        private void ClearDisplay()
        {
            // 管理リスト内のすべての CardDisplay を hiddenParent 配下へ移動する
            foreach (CardDisplay display in _cardDisplays)
            {
                // Null の場合はスキップする
                if (display == null)
                {
                    continue;
                }

                // 親を hiddenParent に設定して非表示領域へ退避させる
                display.transform.SetParent(hiddenParent, false);
            }
        }

        /// <summary>
        /// 可視カード数からスクロール可能範囲を自動計算
        /// </summary>
        /// <param name="visibleCount">表示中カードの枚数</param>
        /// <returns>スクロール可能最小X座標</returns>
        private float CalculateScrollRange(int visibleCount)
        {
            // カードが存在しない場合はスクロール不要
            if (visibleCount == 0 || _cardDisplays.Count == 0)
            {
                return SCROLL_MIN_X_DEFAULT;
            }

            // --------------------------------------------------
            // 全カード横並び幅の算出
            // 最後のカード右端が表示領域右端に揃うように補正
            // --------------------------------------------------
            float totalWidth = (visibleCount - SCROLL_WIDTH_OFFSET) * horizontalSpacing;

            // 左端を0とし、右端が見切れる直前で止まる負値を返す
            float minScrollX = -totalWidth;
            return minScrollX;
        }

        /// <summary>
        /// 現在のスクロール位置が範囲外に出ないように補正
        /// </summary>
        /// <param name="minScrollX">スクロール可能最小X座標</param>
        private void ClampScrollPosition(float minScrollX)
        {
            Vector2 pos = upRowRoot.anchoredPosition;

            if (pos.x < minScrollX)
            {
                pos.x = minScrollX; // 左端補正
            }
            else if (pos.x > SCROLL_MAX_X_DEFAULT)
            {
                pos.x = SCROLL_MAX_X_DEFAULT; // 右端補正
            }

            upRowRoot.anchoredPosition = pos;
        }
    }
}