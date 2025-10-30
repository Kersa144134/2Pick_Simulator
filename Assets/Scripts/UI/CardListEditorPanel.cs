// ======================================================
// CardListEditorPanel.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : 全カードを横1列に並べて、個別に+/-で枚数変更できるUI
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardGame.Data;
using CardGame.Database;

namespace CardGame.UI
{
    /// <summary>
    /// 全カードを1列に並べ、各カードの枚数をボタンで調整できるUI
    /// </summary>
    public class CardListEditorPanel : MonoBehaviour
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>カード1枚分のUIプレハブ</summary>
        [SerializeField]
        private GameObject cardUIPrefab = null;

        /// <summary>カードを並べる親Transform</summary>
        [SerializeField]
        private Transform contentRoot = null;

        /// <summary>内部で参照するCardDatabase</summary>
        private CardDatabase database = null;

        /// <summary>表示中のカードUI群</summary>
        private readonly List<GameObject> spawnedCards = new List<GameObject>();

        // ======================================================
        // 公開メソッド
        // ======================================================

        /// <summary>
        /// 初期化してカードを全表示
        /// </summary>
        public void Initialize(CardDatabase db)
        {
            database = db;
            CreateCardUIs();
        }

        // ======================================================
        // 内部メソッド
        // ======================================================

        /// <summary>
        /// 各CardDataに対応するUIを生成し、+/-ボタンと連動
        /// </summary>
        private void CreateCardUIs()
        {
            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }

            // CardDatabaseに登録されたカード群をUI化
            foreach (var dataField in typeof(CardDatabase)
                     .GetField("cardList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                     ?.GetValue(database) as List<CardData>)
            {
                GameObject ui = Instantiate(cardUIPrefab, contentRoot);
                spawnedCards.Add(ui);

                // UI要素取得
                Image image = ui.transform.Find("CardImage").GetComponent<Image>();
                Text countText = ui.transform.Find("CountText").GetComponent<Text>();
                Button plusButton = ui.transform.Find("PlusButton").GetComponent<Button>();
                Button minusButton = ui.transform.Find("MinusButton").GetComponent<Button>();

                // 表示内容設定
                image.sprite = dataField.cardImage;
                countText.text = database.GetMaxCopies(dataField.CardId).ToString();

                // ボタン動作登録
                plusButton.onClick.AddListener(() =>
                {
                    int current = database.GetMaxCopies(dataField.CardId);
                    database.SetMaxCopies(dataField.CardId, current + 1);
                    countText.text = database.GetMaxCopies(dataField.CardId).ToString();
                });

                minusButton.onClick.AddListener(() =>
                {
                    int current = database.GetMaxCopies(dataField.CardId);
                    database.SetMaxCopies(dataField.CardId, current - 1);
                    countText.text = database.GetMaxCopies(dataField.CardId).ToString();
                });
            }
        }
    }
}