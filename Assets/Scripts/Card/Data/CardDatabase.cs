// ======================================================
// CardDatabase.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-11-05
// 概要       : カードの編成可能枚数および画像を管理するクラス
//              各カードのクラス・パック・レアリティごとの制御に対応
// ======================================================

using System.Collections.Generic;

namespace CardGame.CardSystem.Data
{
    /// <summary>
    /// ゲーム中にカードの編成可能枚数および画像を動的に管理するクラス  
    /// ScriptableObjectを使用せず、ランタイムで生成・制御される
    /// </summary>
    public class CardDatabase
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>編成可能枚数のデフォルト値</summary>
        private const int DEFAULT_MAX_DECKABLE_COPIES = 3;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>全カードデータのリスト</summary>
        private List<CardData> cardList = new List<CardData>();

        /// <summary>カードごとの現在の編成可能枚数テーブル（Key: カードID）</summary>
        private Dictionary<int, int> deckableCopiesTable = new Dictionary<int, int>();

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>すべてのカードデータリストを取得（読み取り専用）</summary>
        public List<CardData> AllCards
        {
            get { return new List<CardData>(cardList); }
        }

        /// <summary>最新パック番号を取得（カードリスト内で最大の PackNumber）</summary>
        public int LatestPackNumber
        {
            get
            {
                int maxPack = 0;
                foreach (CardData data in cardList)
                {
                    if (data.PackNumber > maxPack)
                    {
                        maxPack = data.PackNumber;
                    }
                }
                return maxPack;
            }
        }

        // ======================================================
        // 辞書
        // ======================================================

        /// <summary>クラス列挙値からカタカナ名への変換辞書</summary>
        public readonly Dictionary<CardData.CardClass, string> ClassNameMap = new Dictionary<CardData.CardClass, string>()
        {
            { CardData.CardClass.Neutral, "ニュートラル" },
            { CardData.CardClass.Elf, "エルフ" },
            { CardData.CardClass.Royal, "ロイヤル" },
            { CardData.CardClass.Witch, "ウィッチ" },
            { CardData.CardClass.Dragon, "ドラゴン" },
            { CardData.CardClass.Nightmare, "ナイトメア" },
            { CardData.CardClass.Bishop, "ビショップ" },
            { CardData.CardClass.Nemesis, "ネメシス" }
        };

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// カードリストを初期化して編成可能枚数テーブルを生成する
        /// </summary>
        public CardDatabase(List<CardData> sourceList)
        {
            cardList = new List<CardData>(sourceList);

            foreach (CardData data in cardList)
            {
                deckableCopiesTable[data.CardId] = data.MaxCopies;
            }
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        // --------------------------------------------------
        // 基本操作
        // --------------------------------------------------

        /// <summary>
        /// 指定IDの編成可能枚数を取得する
        /// </summary>
        public int GetDeckableCopies(int cardId)
        {
            if (!deckableCopiesTable.ContainsKey(cardId))
            {
                return DEFAULT_MAX_DECKABLE_COPIES;
            }

            return deckableCopiesTable[cardId];
        }

        /// <summary>
        /// 指定IDの編成可能枚数を設定する
        /// </summary>
        public void SetDeckableCopies(int cardId, int newValue)
        {
            if (newValue < 0)
            {
                newValue = 0;
            }
            else if (newValue > 3)
            {
                newValue = 3;
            }

            if (deckableCopiesTable.ContainsKey(cardId))
            {
                deckableCopiesTable[cardId] = newValue;
            }
            else
            {
                deckableCopiesTable.Add(cardId, newValue);
            }
        }

        // --------------------------------------------------
        // グループ操作
        // --------------------------------------------------

        /// <summary>
        /// 特定パックのカード全ての提示可能枚数を一括変更する
        /// </summary>
        public void SetAvailableByPack(int packNumber, int newValue)
        {
            foreach (CardData data in cardList)
            {
                if (data.PackNumber == packNumber)
                {
                    SetDeckableCopies(data.CardId, newValue);
                }
            }
        }

        /// <summary>
        /// 特定レアリティのカード全ての提示可能枚数を一括変更する
        /// </summary>
        public void SetAvailableByRarity(CardData.CardRarity rarity, int newValue)
        {
            foreach (CardData data in cardList)
            {
                if (data.Rarity == rarity)
                {
                    SetDeckableCopies(data.CardId, newValue);
                }
            }
        }

        /// <summary>
        /// 特定コストのカード全ての提示可能枚数を一括変更する
        /// </summary>
        public void SetAvailableByCost(int cost, int newValue)
        {
            foreach (CardData data in cardList)
            {
                if (data.CardCost == cost)
                {
                    SetDeckableCopies(data.CardId, newValue);
                }
            }
        }
    }
}