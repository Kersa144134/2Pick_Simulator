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
        private TextMeshProUGUI quantityText;

        /// <summary>＋ボタン</summary>
        private Button _plusButton;

        /// <summary>−ボタン</summary>
        private Button _minusButton;

        // ======================================================
        // 初期化
        // ======================================================

        /// <summary>
        /// カード1枚分の情報をもとにUIを初期化する  
        /// CardDisplayManager から生成時に呼び出す
        /// </summary>
        public void Initialize(GameObject obj, CardData cardData, CardDatabase database)
        {
            _cardData = cardData;
            _database = database;

            // 子階層からテキスト、ボタンを取得
            Transform quantityTransform = obj.transform.Find("Quantity");
            Transform plusTransform = obj.transform.Find("Plus");
            Transform minusTransform = obj.transform.Find("Minus");

            if (quantityTransform != null)
            {
                quantityText = quantityTransform.GetComponent<TextMeshProUGUI>();
            }

            if (plusTransform != null)
            {
                _plusButton = plusTransform.GetComponent<Button>();
                if (_plusButton != null)
                {
                    _plusButton.onClick.AddListener(OnPlusClicked);
                }
            }

            if (minusTransform != null)
            {
                _minusButton = minusTransform.GetComponent<Button>();
                if (_minusButton != null)
                {
                    _minusButton.onClick.AddListener(OnMinusClicked);
                }
            }

            // 初期表示更新
            UpdateQuantityText();
        }

        // ======================================================
        // ボタンイベント
        // ======================================================

        /// <summary>＋ボタン押下時の処理</summary>
        private void OnPlusClicked()
        {
            int current = _database.GetMaxCopies(_cardData.CardId);

            _database.SetMaxCopies(_cardData.CardId, current + 1);
            UpdateQuantityText();
        }

        /// <summary>−ボタン押下時の処理</summary>
        private void OnMinusClicked()
        {
            int current = _database.GetMaxCopies(_cardData.CardId);

            _database.SetMaxCopies(_cardData.CardId, current - 1);
            UpdateQuantityText();
        }

        // ======================================================
        // UI更新
        // ======================================================

        /// <summary>
        /// 現在の枚数をUI上に反映する
        /// </summary>
        private void UpdateQuantityText()
        {
            int current = _database.GetMaxCopies(_cardData.CardId);
            quantityText.text = $"{current}";
        }
    }
}