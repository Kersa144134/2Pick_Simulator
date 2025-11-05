// ======================================================
// CardQuantityController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : 各カードオブジェクト内の「＋」「−」ボタンを管理し
//             CardDatabase の最大編成枚数を変更するコントローラ
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardGame.CardSystem.Data;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// カード1枚に対応する＋／−ボタンを制御するクラス  
    /// ボタン押下時に CardDatabase を更新して最大枚数を増減する
    /// </summary>
    public class CardQuantityController
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>このUIが対応するカードデータ</summary>
        private CardData _cardData;

        /// <summary>ゲーム全体で共有されるカードデータベース</summary>
        private CardDatabase _database;

        /// <summary>現在の枚数を表示するテキスト</summary>
        private TextMeshProUGUI _quantityText;

        // ======================================================
        // 初期化
        // ======================================================

        /// <summary>
        /// カード1枚分の情報をもとにUIを初期化する  
        /// CardDisplayManager から生成時に呼び出す
        /// </summary>
        public void Initialize(GameObject obj,
            TextMeshProUGUI text,
            Button plusButton,
            Button minusButton,
            CardData cardData,
            CardDatabase database)
        {
            _quantityText = text;
            _cardData = cardData;
            _database = database;

            plusButton.onClick.AddListener(OnPlusClicked);
            minusButton.onClick.AddListener(OnMinusClicked);

            // 初期表示更新
            UpdateQuantityText();
        }

        // ======================================================
        // ボタンイベント
        // ======================================================

        /// <summary>＋ボタン押下時の処理</summary>
        private void OnPlusClicked()
        {
            int current = _database.GetDeckableCopies(_cardData.CardId);

            _database.SetDeckableCopies(_cardData.CardId, current + 1);
            UpdateQuantityText();
        }

        /// <summary>−ボタン押下時の処理</summary>
        private void OnMinusClicked()
        {
            int current = _database.GetDeckableCopies(_cardData.CardId);

            _database.SetDeckableCopies(_cardData.CardId, current - 1);
            UpdateQuantityText();
        }

        // ======================================================
        // UI更新
        // ======================================================

        /// <summary>
        /// 現在の枚数をUI上に反映する
        /// </summary>
        public void UpdateQuantityText()
        {
            int current = _database.GetDeckableCopies(_cardData.CardId);
            _quantityText.text = $"{current}";
        }
    }
}