// ======================================================
// CardDatabase.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : カードの最大編成枚数を管理するクラス
//              個別カードまたはクラス・パック・レアリティ単位で設定変更が可能
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.Data;

namespace CardGame.Database
{
    /// <summary>
    /// ゲーム中にカードの最大編成枚数を管理・変更するクラス  
    /// ScriptableObjectではなく、動的に管理されるランタイムデータ
    /// </summary>
    public class CardDatabase
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>
        /// 登録されたカードデータの一覧  
        /// 通常はScriptableObjectからロードされて保持される
        /// </summary>
        private List<CardData> cardList = new List<CardData>();

        /// <summary>
        /// 各カードの現在設定されている最大枚数を動的に保持する辞書  
        /// Key: カードID, Value: 最大枚数
        /// </summary>
        private Dictionary<int, int> maxCopiesTable = new Dictionary<int, int>();

        /// <summary>
        /// デフォルトの最大枚数（未設定時に使用される基準値）  
        /// 通常は3枚
        /// </summary>
        private const int DefaultMaxCopies = 3;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// カードリストを初期化して最大枚数テーブルを生成
        /// </summary>
        public CardDatabase(List<CardData> sourceList)
        {
            // 渡されたカードデータ群を内部リストに保持
            cardList = new List<CardData>(sourceList);

            // 各カードごとに初期最大枚数を登録
            foreach (CardData data in cardList)
            {
                maxCopiesTable[data.CardId] = data.MaxCopies;
            }
        }

        // ======================================================
        // メソッド（基本操作）
        // ======================================================

        /// <summary>
        /// 指定したカードIDの現在設定されている最大枚数を取得
        /// </summary>
        public int GetMaxCopies(int cardId)
        {
            // 未登録カードの場合はデフォルト値を返す
            if (!maxCopiesTable.ContainsKey(cardId))
            {
                return DefaultMaxCopies;
            }

            return maxCopiesTable[cardId];
        }

        /// <summary>
        /// 指定カードの最大枚数を直接設定（UI操作などで使用）
        /// </summary>
        public void SetMaxCopies(int cardId, int newValue)
        {
            // 値が1〜3の範囲外なら制限
            if (newValue < 0)
            {
                newValue = 0;
            }
            else if (newValue > 3)
            {
                newValue = 3;
            }

            // 登録済みなら更新、未登録なら追加
            if (maxCopiesTable.ContainsKey(cardId))
            {
                maxCopiesTable[cardId] = newValue;
            }
            else
            {
                maxCopiesTable.Add(cardId, newValue);
            }
        }

        // ======================================================
        // メソッド（グループ操作）
        // ======================================================

        /// <summary>
        /// 特定クラスのカードすべてを指定値に一括変更  
        /// 例: ロイヤル以外を0枚にする等
        /// </summary>
        public void SetMaxCopiesByClass(CardData.CardClass targetClass, int newValue)
        {
            foreach (CardData data in cardList)
            {
                if (data.ClassType == targetClass)
                {
                    SetMaxCopies(data.CardId, newValue);
                }
            }
        }

        /// <summary>
        /// 特定パックのカードすべてを指定値に一括変更
        /// </summary>
        public void SetMaxCopiesByPack(int packNumber, int newValue)
        {
            foreach (CardData data in cardList)
            {
                if (data.PackNumber == packNumber)
                {
                    SetMaxCopies(data.CardId, newValue);
                }
            }
        }

        /// <summary>
        /// 特定レアリティのカードすべてを指定値に一括変更
        /// </summary>
        public void SetMaxCopiesByRarity(CardData.CardRarity rarity, int newValue)
        {
            foreach (CardData data in cardList)
            {
                if (data.Rarity == rarity)
                {
                    SetMaxCopies(data.CardId, newValue);
                }
            }
        }

        // ======================================================
        // メソッド（一括初期化）
        // ======================================================

        /// <summary>
        /// すべてのカードを「デフォルトの最大枚数」に戻す
        /// </summary>
        public void ResetAllToDefault()
        {
            foreach (CardData data in cardList)
            {
                SetMaxCopies(data.CardId, DefaultMaxCopies);
            }
        }

        /// <summary>
        /// すべてのカードを「使用不可（0枚）」に設定する
        /// </summary>
        public void SetAllToZero()
        {
            foreach (CardData data in cardList)
            {
                SetMaxCopies(data.CardId, 0);
            }
        }

        // ======================================================
        // メソッド（デバッグ）
        // ======================================================

        /// <summary>
        /// 現在のカード枚数設定をコンソール出力
        /// </summary>
        public void PrintAllCardLimits()
        {
            foreach (CardData data in cardList)
            {
                int current = GetMaxCopies(data.CardId);
                Debug.Log($"[{data.CardName}] ({data.ClassType}) => Max: {current}");
            }
        }
    }
}