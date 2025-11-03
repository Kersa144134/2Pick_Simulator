// ======================================================
// CardDisplayRefresher.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : CardDisplayManagerから分離した表示更新制御クラス
//             表示中カードの並び替え・スクロール範囲調整を担当
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// カード表示領域の更新を担当するクラス  
    /// 表示カードを整列し、スクロール範囲と位置を自動補正する
    /// </summary>
    public class CardDisplayRefresher
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>表示コンテナ</summary>
        private readonly RectTransform _visibleParent;

        /// <summary>非表示コンテナ</summary>
        private readonly RectTransform _hiddenParent;

        /// <summary>カード整列間隔（px）</summary>
        private readonly float _horizontalSpacing;

        /// <summary>スクロール制御コンポーネント</summary>
        private readonly CardScrollController _scrollController;

        /// <summary>生成済みカード表示オブジェクト群</summary>
        private readonly List<CardDisplay> _cardDisplays;

        // ======================================================
        // 定数
        // ======================================================

        /// <summary>スクロール範囲計算で全体幅補正に使う枚数補正値</summary>
        private const int SCROLL_WIDTH_OFFSET = 2;

        /// <summary>スクロールの左端位置</summary>
        private const float SCROLL_MIN_X_DEFAULT = 0f;

        /// <summary>スクロールの右端最大位置</summary>
        private const float SCROLL_MAX_X_DEFAULT = 0f;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// CardDisplayRefresherのコンストラクタ
        /// </summary>
        /// <param name="visibleParent">可視カードの親Transform</param>
        /// <param name="hiddenParent">非表示カードの親Transform</param>
        /// <param name="horizontalSpacing">カード間の水平間隔</param>
        /// <param name="scrollController">スクロール制御クラス</param>
        /// <param name="cardDisplays">生成済みカード表示リスト</param>
        public CardDisplayRefresher(
            RectTransform visibleParent,
            RectTransform hiddenParent,
            float horizontalSpacing,
            CardScrollController scrollController,
            List<CardDisplay> cardDisplays)
        {
            _visibleParent = visibleParent;
            _hiddenParent = hiddenParent;
            _horizontalSpacing = horizontalSpacing;
            _scrollController = scrollController;
            _cardDisplays = cardDisplays;
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 可視カードリストに基づいてカード表示を更新
        /// </summary>
        /// <param name="visibleCardData">表示対象のCardDataリスト</param>
        public void Refresh(List<CardData> visibleCardData)
        {
            // --------------------------------------
            // 表示対象カードがない場合は全て非表示
            // --------------------------------------
            if (visibleCardData == null || visibleCardData.Count == 0)
            {
                HideAllCards();
                _scrollController.SetScrollRange(SCROLL_MIN_X_DEFAULT);
                return;
            }

            // --------------------------------------
            // 可視カードセットを構築（高速検索用）
            // --------------------------------------
            HashSet<CardData> visibleSet = new HashSet<CardData>(visibleCardData);

            int visibleIndex = 0; // 可視カード配置用インデックス
            int hiddenIndex = 0;  // 非表示カード配置用インデックス

            // --------------------------------------
            // カードごとに表示/非表示を振り分け
            // --------------------------------------
            foreach (CardDisplay card in _cardDisplays)
            {
                RectTransform rect = (RectTransform)card.transform;

                if (visibleSet.Contains(card.CurrentData))
                {
                    // 可視カードはvisibleParent配下に移動
                    card.SetParent(_visibleParent);
                    rect.anchoredPosition = new Vector2(visibleIndex * _horizontalSpacing, 0f);
                    visibleIndex++;
                }
                else
                {
                    // 非表示カードはhiddenParent配下に移動
                    card.SetParent(_hiddenParent);
                    rect.anchoredPosition = new Vector2(hiddenIndex * _horizontalSpacing, 0f);
                    hiddenIndex++;
                }
            }

            // --------------------------------------
            // スクロール範囲を算出して設定
            // --------------------------------------
            float minScrollX = CalculateScrollRange(visibleIndex);
            _scrollController.SetScrollRange(minScrollX);
            ClampScrollPosition(minScrollX);
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// すべてのカードを非表示コンテナに移動
        /// </summary>
        private void HideAllCards()
        {
            int index = 0;
            foreach (CardDisplay card in _cardDisplays)
            {
                card.SetParent(_hiddenParent);
                RectTransform rect = (RectTransform)card.transform;
                rect.anchoredPosition = new Vector2(index * _horizontalSpacing, 0f);
                index++;
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

            // 表示領域幅
            float viewWidth = _visibleParent.rect.width;

            // サンプルカードの幅取得
            RectTransform sampleCard = _cardDisplays[0].GetComponent<RectTransform>();
            float cardWidth = sampleCard.rect.width;

            // --------------------------------------------------
            // 全カード横並び幅の算出
            // 最後のカード右端が表示領域右端に揃うように補正
            // --------------------------------------------------
            float totalWidth = (visibleCount - SCROLL_WIDTH_OFFSET) * _horizontalSpacing + cardWidth;

            // 表示領域より小さい場合はスクロール不要
            if (totalWidth <= viewWidth)
            {
                return SCROLL_MIN_X_DEFAULT;
            }

            // 左端を0とし、右端が見切れる直前で止まる負値を返す
            float minScrollX = -(totalWidth - viewWidth);
            return minScrollX;
        }

        /// <summary>
        /// 現在のスクロール位置が範囲外に出ないように補正
        /// </summary>
        /// <param name="minScrollX">スクロール可能最小X座標</param>
        private void ClampScrollPosition(float minScrollX)
        {
            Vector2 pos = _visibleParent.anchoredPosition;

            if (pos.x < minScrollX)
            {
                pos.x = minScrollX; // 左端補正
            }
            else if (pos.x > SCROLL_MAX_X_DEFAULT)
            {
                pos.x = SCROLL_MAX_X_DEFAULT; // 右端補正
            }

            _visibleParent.anchoredPosition = pos;
        }
    }
}