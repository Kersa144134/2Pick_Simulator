// ======================================================
// DeckListManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-04
// 概要       : 選択クラスとピックカードリストを管理するシングルトンクラス
//              同種カードを複数枚保持可能（カード＋枚数構造）
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;

namespace CardGame.DeckSystem.Manager
{
    /// <summary>
    /// デッキ情報管理クラス  
    /// 選択クラスとピック済みカードリストを保持する（同一カードの重複も管理）
    /// </summary>
    public class DeckListManager : MonoBehaviour
    {
        // ======================================================
        // サブクラス
        // ======================================================

        /// <summary>
        /// ピックカード情報サブクラス
        /// </summary>
        [System.Serializable]
        public class PickedCardEntry
        {
            public CardData Card;
            public int Count;

            public PickedCardEntry(CardData card, int count)
            {
                Card = card;
                Count = count;
            }
        }

        // ======================================================
        // シングルトン
        // ======================================================

        /// <summary>インスタンス</summary>
        public static DeckListManager Instance { get; private set; }

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>選択されたクラス</summary>
        private CardData.CardClass _selectedClass = CardData.CardClass.Elf;

        /// <summary>ピック済みカードリスト（カード＋枚数）</summary>
        private List<PickedCardEntry> _pickedCards = new List<PickedCardEntry>();

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>選択クラスの取得・設定</summary>
        public CardData.CardClass SelectedClass
        {
            get { return _selectedClass; }
            set { _selectedClass = value; }
        }

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// ピック済みカードリストを返す
        /// </summary>
        public List<PickedCardEntry> GetPickedCardEntries()
        {
            return _pickedCards;
        }

        /// <summary>
        /// カードをピック済みリストに追加  
        /// すでに同じカードが存在する場合は枚数を加算する  
        /// 追加後はコスト昇順、次いで CardID 昇順でソートする
        /// </summary>
        /// <param name="card">追加対象カード</param>
        public void AddPickedCard(CardData card)
        {
            if (card == null)
            {
                Debug.LogWarning("AddPickedCard：cardがnullです。");
                return;
            }

            // 既存カード検索
            int index = _pickedCards.FindIndex(entry => entry.Card == card);

            if (index >= 0)
            {
                // 既存カードがある場合 → 枚数加算
                PickedCardEntry existing = _pickedCards[index];
                existing.Count++;
                _pickedCards[index] = existing;
            }
            else
            {
                // 新規カード追加
                _pickedCards.Add(new PickedCardEntry(card, 1));
            }

            // ソート処理を呼び出し
            SortPickedCards();
        }

        /// <summary>
        /// ピックカードリストをクリア
        /// </summary>
        public void ResetPickedCards()
        {
            _pickedCards.Clear();
        }

        /// <summary>
        /// デッキ全体をリセット
        /// </summary>
        public void ResetDeck()
        {
            _selectedClass = default;
            ResetPickedCards();
        }

        // ======================================================
        // ソート処理
        // ======================================================

        /// <summary>
        /// ピック済みカードリストをコスト → CardID の昇順で並び替える。  
        /// コストが同値の場合は CardID で安定化させる。
        /// </summary>
        private void SortPickedCards()
        {
            if (_pickedCards == null || _pickedCards.Count <= 1)
            {
                return;
            }

            // --------------------------------------------
            // 第1条件：コスト昇順、第2条件：CardID昇順
            // --------------------------------------------
            _pickedCards.Sort((a, b) =>
            {
                int costCompare = a.Card.CardCost.CompareTo(b.Card.CardCost);
                if (costCompare != 0)
                {
                    return costCompare;
                }
                return a.Card.CardId.CompareTo(b.Card.CardId);
            });
        }
    }
}