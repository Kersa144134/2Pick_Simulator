// ======================================================
// RedrawCountUIController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-06
// 概要       : 各クラスの再抽選回数をUI経由で動的に変更する制御クラス
//              数値入力制限（0〜99、整数のみ）に対応
// ======================================================

using UnityEngine;
using TMPro;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;

namespace CardGame.CardSystem.UI
{
    /// <summary>
    /// 各クラスの再抽選回数をTMP_InputField経由で管理するクラス
    /// </summary>
    public class RedrawCountUIController : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        [SerializeField] private TMP_InputField elfInput;
        [SerializeField] private TMP_InputField royalInput;
        [SerializeField] private TMP_InputField witchInput;
        [SerializeField] private TMP_InputField dragonInput;
        [SerializeField] private TMP_InputField nightmareInput;
        [SerializeField] private TMP_InputField bishopInput;
        [SerializeField] private TMP_InputField nemesisInput;

        // ======================================================
        // フィールド
        // ======================================================

        private CardDatabase _database;

        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            _database = CardDatabaseManager.Instance.GetCardDatabase();

            // 現在の再抽選回数をUIに反映
            elfInput.text = _database.GetRedrawCount(CardData.CardClass.Elf).ToString();
            royalInput.text = _database.GetRedrawCount(CardData.CardClass.Royal).ToString();
            witchInput.text = _database.GetRedrawCount(CardData.CardClass.Witch).ToString();
            dragonInput.text = _database.GetRedrawCount(CardData.CardClass.Dragon).ToString();
            nightmareInput.text = _database.GetRedrawCount(CardData.CardClass.Nightmare).ToString();
            bishopInput.text = _database.GetRedrawCount(CardData.CardClass.Bishop).ToString();
            nemesisInput.text = _database.GetRedrawCount(CardData.CardClass.Nemesis).ToString();

            // 入力制限イベント登録
            elfInput.onValueChanged.AddListener(ValidateIntegerInput);
            royalInput.onValueChanged.AddListener(ValidateIntegerInput);
            witchInput.onValueChanged.AddListener(ValidateIntegerInput);
            dragonInput.onValueChanged.AddListener(ValidateIntegerInput);
            nightmareInput.onValueChanged.AddListener(ValidateIntegerInput);
            bishopInput.onValueChanged.AddListener(ValidateIntegerInput);
            nemesisInput.onValueChanged.AddListener(ValidateIntegerInput);

            // 値確定時に反映イベント登録
            elfInput.onEndEdit.AddListener((v) => ApplyRedrawCount(CardData.CardClass.Elf, v));
            royalInput.onEndEdit.AddListener((v) => ApplyRedrawCount(CardData.CardClass.Royal, v));
            witchInput.onEndEdit.AddListener((v) => ApplyRedrawCount(CardData.CardClass.Witch, v));
            dragonInput.onEndEdit.AddListener((v) => ApplyRedrawCount(CardData.CardClass.Dragon, v));
            nightmareInput.onEndEdit.AddListener((v) => ApplyRedrawCount(CardData.CardClass.Nightmare, v));
            bishopInput.onEndEdit.AddListener((v) => ApplyRedrawCount(CardData.CardClass.Bishop, v));
            nemesisInput.onEndEdit.AddListener((v) => ApplyRedrawCount(CardData.CardClass.Nemesis, v));
        }

        // ======================================================
        // 入力制御
        // ======================================================

        /// <summary>
        /// 整数のみを許可し、最大2桁までに制限
        /// </summary>
        private void ValidateIntegerInput(string value)
        {
            string filtered = string.Empty;

            foreach (char c in value)
            {
                if (char.IsDigit(c))
                {
                    filtered += c;
                }
            }

            if (filtered.Length > 2)
            {
                filtered = filtered.Substring(0, 2);
            }

            TMP_InputField field = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<TMP_InputField>();
            if (field != null && field.text != filtered)
            {
                field.text = filtered;
            }
        }

        // ======================================================
        // 値反映処理
        // ======================================================

        /// <summary>
        /// 入力値をパースして再抽選回数をデータベースに反映
        /// </summary>
        private void ApplyRedrawCount(CardData.CardClass cardClass, string input)
        {
            int value;

            if (string.IsNullOrWhiteSpace(input))
            {
                value = _database.GetRedrawCount(cardClass);
                TMP_InputField field = GetInputFieldByClass(cardClass);
                if (field != null)
                {
                    field.text = value.ToString();
                }
            }
            else
            {
                value = ParseInput(input, _database.GetRedrawCount(cardClass));
            }

            _database.SetRedrawCount(cardClass, value);
        }

        /// <summary>
        /// クラスに対応する入力フィールドを返す
        /// </summary>
        private TMP_InputField GetInputFieldByClass(CardData.CardClass cardClass)
        {
            return cardClass switch
            {
                CardData.CardClass.Elf => elfInput,
                CardData.CardClass.Royal => royalInput,
                CardData.CardClass.Witch => witchInput,
                CardData.CardClass.Dragon => dragonInput,
                CardData.CardClass.Nightmare => nightmareInput,
                CardData.CardClass.Bishop => bishopInput,
                CardData.CardClass.Nemesis => nemesisInput,
                _ => null,
            };
        }

        /// <summary>
        /// 入力が不正な場合は既定値を返す
        /// </summary>
        private int ParseInput(string input, int fallback)
        {
            if (int.TryParse(input, out int result))
            {
                return result;
            }
            return fallback;
        }
    }
}