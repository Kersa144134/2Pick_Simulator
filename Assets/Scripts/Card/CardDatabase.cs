// ======================================================
// CardDatabase.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : カードの最大編成枚数および画像を管理するクラス
//              各カードのクラス・パック・レアリティごとの制御に対応
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.Data;

namespace CardGame.Database
{
    /// <summary>
    /// ゲーム中にカードの最大編成枚数および画像を動的に管理するクラス  
    /// ScriptableObjectを使用せず、ランタイムで生成・制御される
    /// </summary>
    public class CardDatabase
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>カード画像のルートディレクトリ</summary>
        private const string ImageRootPath = "Images/Cards/";

        /// <summary>カード画像のファイル名先頭の文字数（5桁ID）</summary>
        private const int CardIdLength = 5;

        /// <summary>最大編成枚数のデフォルト値</summary>
        private const int DefaultMaxCopies = 3;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>全カードデータのリスト</summary>
        private List<CardData> cardList = new List<CardData>();

        /// <summary>カードごとの現在最大枚数テーブル（Key: カードID）</summary>
        private Dictionary<int, int> maxCopiesTable = new Dictionary<int, int>();

        /// <summary>ロード済み画像のキャッシュ（Key: ファイル名）</summary>
        private Dictionary<string, Sprite> imageCache = new Dictionary<string, Sprite>();

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// カードリストを初期化して最大枚数テーブルを生成する
        /// </summary>
        public CardDatabase(List<CardData> sourceList)
        {
            // 渡されたデータをコピー
            cardList = new List<CardData>(sourceList);

            // 各カードに初期値を設定
            foreach (CardData data in cardList)
            {
                maxCopiesTable[data.CardId] = data.MaxCopies;
            }
        }

        // ======================================================
        // メソッド（基本操作）
        // ======================================================

        /// <summary>
        /// 指定IDの最大編成枚数を取得する
        /// </summary>
        public int GetMaxCopies(int cardId)
        {
            if (!maxCopiesTable.ContainsKey(cardId))
            {
                return DefaultMaxCopies;
            }

            return maxCopiesTable[cardId];
        }

        /// <summary>
        /// 指定IDの最大編成枚数を設定する
        /// </summary>
        public void SetMaxCopies(int cardId, int newValue)
        {
            if (newValue < 0)
            {
                newValue = 0;
            }
            else if (newValue > 3)
            {
                newValue = 3;
            }

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
        /// 特定クラスのカード全ての最大枚数を一括変更する
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
        /// 特定パックのカード全ての最大枚数を一括変更する
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
        /// 特定レアリティのカード全ての最大枚数を一括変更する
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
        /// 全カードをデフォルト枚数にリセットする
        /// </summary>
        public void ResetAllToDefault()
        {
            foreach (CardData data in cardList)
            {
                SetMaxCopies(data.CardId, DefaultMaxCopies);
            }
        }

        /// <summary>
        /// 全カードを使用不可（0枚）に設定する
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
        /// 現在のカード設定をコンソール出力する
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