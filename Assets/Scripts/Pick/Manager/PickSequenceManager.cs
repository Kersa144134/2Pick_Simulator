// ======================================================
// PickSequenceManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : ピック順管理クラス
//             現在のピック回数と提示レアリティ順を管理
// ======================================================

using System;
using System.Collections.Generic;
using CardGame.CardSystem.Data;

namespace CardGame.PickSystem.Manager
{
    /// <summary>
    /// ピック順管理クラス
    /// 現在のピック順を管理し、順目に応じた提示レアリティを返す
    /// </summary>
    public class PickSequenceManager
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>最大ピック回数</summary>
        private const int MAX_PICK_COUNT = 19;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>現在のピック順（0開始）</summary>
        private int _currentPickIndex = 0;

        /// <summary>順目ごとの提示レアリティマップ</summary>
        private readonly Dictionary<int, CardData.CardRarity[]> _pickRarityMap;

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>現在のピック順（0開始）</summary>
        public int CurrentPickIndex => _currentPickIndex;

        /// <summary>最大ピック回数</summary>
        public int GetMaxPickCount()
        {
            return MAX_PICK_COUNT;
        }

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// コンストラクタ
        /// 提示レアリティマップを初期化
        /// </summary>
        public PickSequenceManager()
        {
            _pickRarityMap = new Dictionary<int, CardData.CardRarity[]>
            {
                { 1,  new[] { CardData.CardRarity.Bronze } },
                { 2,  new[] { CardData.CardRarity.Silver } },
                { 3,  new[] { CardData.CardRarity.Bronze } },
                { 4,  new[] { CardData.CardRarity.Silver } },
                { 5,  new[] { CardData.CardRarity.Bronze } },
                { 6,  new[] { CardData.CardRarity.Gold } },
                { 7,  new[] { CardData.CardRarity.Bronze } },
                { 8,  new[] { CardData.CardRarity.Silver } },
                { 9,  new[] { CardData.CardRarity.Bronze } },
                { 10, new[] { CardData.CardRarity.Silver } },
                { 11, new[] { CardData.CardRarity.Gold, CardData.CardRarity.Legend } },
                { 12, new[] { CardData.CardRarity.Bronze } },
                { 13, new[] { CardData.CardRarity.Silver } },
                { 14, new[] { CardData.CardRarity.Bronze } },
                { 15, new[] { CardData.CardRarity.Gold} },
                { 16, new[] { CardData.CardRarity.Bronze, CardData.CardRarity.Silver } },
                { 17, new[] { CardData.CardRarity.Silver, CardData.CardRarity.Gold } },
                { 18, new[] { CardData.CardRarity.Bronze, CardData.CardRarity.Silver, CardData.CardRarity.Gold } },
                { 19, new[] { CardData.CardRarity.Legend } }
            };
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 残りピック回数を取得
        /// </summary>
        public int GetRemainingPickCount()
        {
            return Math.Max(0, MAX_PICK_COUNT - _currentPickIndex);
        }

        /// <summary>
        /// 次のピックで提示するレアリティ配列を取得
        /// </summary>
        public CardData.CardRarity[] GetNextPickRarities()
        {
            int nextIndex = _currentPickIndex + 1;

            if (_pickRarityMap.TryGetValue(nextIndex, out CardData.CardRarity[] rarities))
            {
                return rarities;
            }

            // MAX_PICK_COUNTを超えた場合は空配列
            return Array.Empty<CardData.CardRarity>();
        }

        /// <summary>
        /// ピック実行後に順目を進める
        /// </summary>
        public void IncrementPick()
        {
            if (_currentPickIndex < MAX_PICK_COUNT)
            {
                _currentPickIndex++;
            }
        }

        /// <summary>
        /// ピック順をリセット
        /// </summary>
        public void ResetPickSequence()
        {
            _currentPickIndex = 0;
        }
    }
}