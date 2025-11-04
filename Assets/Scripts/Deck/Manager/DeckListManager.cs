// ======================================================
// DeckListManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : 選択クラスとピックカードリストを管理するシングルトンクラス
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;

namespace CardGame.DeckSystem.Manager
{
    /// <summary>
    /// デッキ情報管理クラス
    /// 選択クラスとピック済みカードリストを保持する
    /// </summary>
    public class DeckListManager : MonoBehaviour
    {
        // ======================================================
        // シングルトン
        // ======================================================

        /// <summary>インスタンス</summary>
        public static DeckListManager Instance { get; private set; }

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>選択されたクラス</summary>
        private CardData.CardClass _selectedClass;

        /// <summary>ピック済みカードリスト</summary>
        private List<CardData> _pickedCards = new List<CardData>();

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>選択クラスの取得・設定</summary>
        public CardData.CardClass SelectedClass
        {
            get { return _selectedClass; }
            set { _selectedClass = value; }
        }

        /// <summary>ピックカードリストの取得</summary>
        public List<CardData> PickedCards => _pickedCards;

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
        /// 現在ピック済みのカードリストを返す
        /// </summary>
        public List<CardData> GetPickedCards()
        {
            return _pickedCards;
        }

        /// <summary>
        /// カードをピック済みリストに追加
        /// </summary>
        /// <param name="card">ピックされたカード</param>
        public void AddPickedCard(CardData card)
        {
            if (card != null && !_pickedCards.Contains(card))
            {
                _pickedCards.Add(card);
            }
        }

        /// <summary>
        /// ピックカードリストをクリア
        /// </summary>
        public void ResetPickedCards()
        {
            _pickedCards.Clear();
        }

        /// <summary>
        /// デッキ情報をリセット
        /// </summary>
        public void ResetDeck()
        {
            _selectedClass = default;
            ResetPickedCards();
        }
    }
}