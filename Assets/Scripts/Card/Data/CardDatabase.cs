// ======================================================
// CardDatabase.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-11-06
// 概要       : カードの編成可能枚数および画像を管理するクラス
//              各カードのクラス・パック・レアリティごとの制御に対応
// ======================================================

using System.Collections.Generic;
using UnityEngine;

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

        /// <summary>クラスごとの再抽選回数のデフォルト値</summary>
        private const int DEFAULT_REDRAW_COUNT = 3;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>全カードデータのリスト</summary>
        private List<CardData> _cardList = new List<CardData>();

        /// <summary>カードごとの現在の編成可能枚数テーブル（Key: カードID）</summary>
        private Dictionary<int, int> _deckableCopiesTable = new Dictionary<int, int>();

        /// <summary>レアリティごとの重み係数</summary>
        private Dictionary<CardData.CardRarity, float> _rarityWeights =
            new Dictionary<CardData.CardRarity, float>()
            {
                { CardData.CardRarity.Bronze, 1.0f },
                { CardData.CardRarity.Silver, 1.0f },
                { CardData.CardRarity.Gold, 1.5f },
                { CardData.CardRarity.Legend, 2.0f }
            };
        
        /// <summary>最新パックカードの出現補正倍率</summary>
        private float _latestPackWeight = 1.2f;

        /// <summary>ニュートラルカードの出現補正倍率</summary>
        private float _neutralCardWeight = 0.1f;

        /// <summary>クラスごとの再抽選可能回数テーブル</summary>
        private Dictionary<CardData.CardClass, int> _redrawCountTable = new Dictionary<CardData.CardClass, int>();

        /// <summary>プレイ中に消費する再抽選回数</summary>
        private int _currentRedrawCount;

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>全カードデータを取得</summary>
        public List<CardData> AllCards
        {
            get { return new List<CardData>(_cardList); }
        }

        /// <summary>登録されている全てのパック番号を取得</summary>
        public List<int> GetAllPackIds()
        {
            // パック番号の重複を除外するためのリストを作成
            List<int> packIds = new List<int>();

            // 全カードを走査してパック番号を抽出
            foreach (CardData card in AllCards)
            {
                // 未登録のパック番号のみ追加
                if (!packIds.Contains(card.PackNumber))
                {
                    packIds.Add(card.PackNumber);
                }
            }

            // 登録済みの全パック番号を返す
            return packIds;
        }

        /// <summary>最新パック番号（カードリスト中の最大PackNumber）</summary>
        public int LatestPackNumber
        {
            get
            {
                int maxPack = 0;

                foreach (CardData data in _cardList)
                {
                    if (data.PackNumber > maxPack)
                    {
                        maxPack = data.PackNumber;
                    }
                }

                return maxPack;
            }
        }

        /// <summary>レアリティごとの重み係数</summary>
        public Dictionary<CardData.CardRarity, float> RarityWeights => _rarityWeights;

        /// <summary>最新パック補正倍率</summary>
        public float LatestPackWeight => _latestPackWeight;

        /// <summary>ニュートラル補正倍率</summary>
        public float NeutralCardWeight => _neutralCardWeight;

        // ======================================================
        // 辞書
        // ======================================================

        /// <summary>クラス列挙値からカタカナ名への変換辞書</summary>
        public readonly Dictionary<CardData.CardClass, string> ClassNameMap =
            new Dictionary<CardData.CardClass, string>()
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
        /// コンストラクタ
        /// </summary>
        public CardDatabase(List<CardData> sourceList)
        {
            _cardList = new List<CardData>(sourceList);

            // カードごとの編成可能枚数を初期化
            foreach (CardData data in _cardList)
            {
                _deckableCopiesTable[data.CardId] = data.MaxCopies;
            }

            // クラスごとの再抽選回数を初期化
            foreach (CardData.CardClass cardClass in System.Enum.GetValues(typeof(CardData.CardClass)))
            {
                _redrawCountTable[cardClass] = DEFAULT_REDRAW_COUNT;
            }
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        // --------------------------------------------------
        // 基本操作
        // --------------------------------------------------

        /// <summary>
        /// 指定IDの編成可能枚数を取得する。
        /// </summary>
        public int GetDeckableCopies(int cardId)
        {
            if (!_deckableCopiesTable.ContainsKey(cardId))
            {
                return DEFAULT_MAX_DECKABLE_COPIES;
            }

            return _deckableCopiesTable[cardId];
        }

        /// <summary>
        /// 指定IDの編成可能枚数を設定する。
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

            if (_deckableCopiesTable.ContainsKey(cardId))
            {
                _deckableCopiesTable[cardId] = newValue;
            }
            else
            {
                _deckableCopiesTable.Add(cardId, newValue);
            }
        }

        // --------------------------------------------------
        // グループ操作
        // --------------------------------------------------

        /// <summary>
        /// 特定パックのカード全ての提示可能枚数を一括変更する。
        /// </summary>
        public void SetAvailableByPack(int packNumber, int newValue)
        {
            foreach (CardData data in _cardList)
            {
                if (data.PackNumber == packNumber)
                {
                    SetDeckableCopies(data.CardId, newValue);
                }
            }
        }

        /// <summary>
        /// 特定レアリティのカード全ての提示可能枚数を一括変更する。
        /// </summary>
        public void SetAvailableByRarity(CardData.CardRarity rarity, int newValue)
        {
            foreach (CardData data in _cardList)
            {
                if (data.Rarity == rarity)
                {
                    SetDeckableCopies(data.CardId, newValue);
                }
            }
        }

        /// <summary>
        /// 特定コストのカード全ての提示可能枚数を一括変更する。
        /// </summary>
        public void SetAvailableByCost(int cost, int newValue)
        {
            foreach (CardData data in _cardList)
            {
                if (data.CardCost == cost)
                {
                    SetDeckableCopies(data.CardId, newValue);
                }
            }
        }

        /// <summary>
        /// 全てのカードの提示可能枚数を初期値（カードごとの最大値）にリセットする。
        /// </summary>
        public void ResetAllDeckableCopies()
        {
            foreach (CardData data in   _cardList)
            {
                int resetValue = data.MaxCopies;

                if (_deckableCopiesTable.ContainsKey(data.CardId))
                {
                    _deckableCopiesTable[data.CardId] = resetValue;
                }
                else
                {
                    _deckableCopiesTable.Add(data.CardId, resetValue);
                }
            }
        }

        // --------------------------------------------------
        // 出現率設定操作
        // --------------------------------------------------

        /// <summary>
        /// レアリティごとの重み係数を変更する。
        /// UI入力などでリアルタイムに更新可能。
        /// </summary>
        public void SetRarityWeight(CardData.CardRarity rarity, float newValue)
        {
            // 値が負数の場合は 0 に補正
            if (newValue < 0f)
            {
                newValue = 0f;
            }

            // 各レアリティ倍率を反映
            if (RarityWeights.ContainsKey(rarity))
            {
                RarityWeights[rarity] = newValue;
            }
        }

        /// <summary>
        /// 最新パック補正倍率を更新する。
        /// </summary>
        /// <param name="latestWeight">設定する最新パック倍率</param>
        public void SetLatestPackWeight(float latestWeight)
        {
            // 値が負数の場合は 0 に補正
            if (latestWeight < 0f)
            {
                latestWeight = 0f;
            }

            // 最新パック倍率を反映
            _latestPackWeight = latestWeight;
        }

        /// <summary>
        /// ニュートラル補正倍率を更新する。
        /// </summary>
        /// <param name="neutralWeight">設定するニュートラル倍率</param>
        public void SetNeutralCardWeight(float neutralWeight)
        {
            // 値が負数の場合は 0 に補正
            if (neutralWeight < 0f)
            {
                neutralWeight = 0f;
            }

            // ニュートラル倍率を反映
            _neutralCardWeight = neutralWeight;
        }

        // --------------------------------------------------
        // 再抽選回数操作
        // --------------------------------------------------

        /// <summary>
        /// 指定クラスの残り再抽選回数を取得する。
        /// </summary>
        /// <param name="cardClass">対象のカードクラス</param>
        /// <returns>設定済みの再抽選回数、未登録ならデフォルト値</returns>
        public int GetRedrawCount(CardData.CardClass cardClass)
        {
            if (_redrawCountTable.ContainsKey(cardClass))
            {
                return _redrawCountTable[cardClass];
            }

            return DEFAULT_REDRAW_COUNT;
        }

        /// <summary>
        /// 指定クラスの再抽選回数を設定する。
        /// </summary>
        /// <param name="cardClass">対象のカードクラス</param>
        /// <param name="newValue">設定する回数（負数は0に補正）</param>
        public void SetRedrawCount(CardData.CardClass cardClass, int newValue)
        {
            if (newValue < 0)
            {
                newValue = 0;
            }

            _redrawCountTable[cardClass] = newValue;
        }

        /// <summary>
        /// プレイ中の再抽選回数を、指定クラスに対応する初期値で設定する。
        /// </summary>
        /// <param name="cardClass">対象のカードクラス</param>
        public void SetCurrentRedrawCount(CardData.CardClass cardClass)
        {
            if (_redrawCountTable.ContainsKey(cardClass))
            {
                _currentRedrawCount = _redrawCountTable[cardClass];
            }
            else
            {
                _currentRedrawCount = DEFAULT_REDRAW_COUNT;
            }
        }

        /// <summary>
        /// プレイ中の再抽選回数を1減らす（最小0）。
        /// </summary>
        public void ConsumeRedraw()
        {
            _currentRedrawCount = Mathf.Max(0, _currentRedrawCount - 1);
        }

        /// <summary>
        /// プレイ中の再抽選回数を取得する。
        /// </summary>
        /// <returns>現在の再抽選回数</returns>
        public int GetCurrentRedrawCount()
        {
            return _currentRedrawCount;
        }

        /// <summary>
        /// 全クラスの再抽選回数を指定値にリセットする。
        /// </summary>
        /// <param name="newValue">設定する回数（負数は0に補正）</param>
        public void ResetAllRedrawCounts(int newValue)
        {
            if (newValue < 0)
            {
                newValue = 0;
            }

            foreach (CardData.CardClass cardClass in System.Enum.GetValues(typeof(CardData.CardClass)))
            {
                _redrawCountTable[cardClass] = newValue;
            }
        }
    }
}