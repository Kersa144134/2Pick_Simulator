// ======================================================
// CardWeightUIController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-06
// 概要       : カード出現率関連の倍率をUI経由で動的に変更する制御クラス
//              数値入力制限（0〜9と小数点のみ、.は1回まで）に対応
// ======================================================

using UnityEngine;
using TMPro;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;

namespace CardGame.CardSystem.UI
{
    /// <summary>
    /// 出現倍率関連の入力UIを管理するクラス  
    /// TMP_InputField 経由で CardDatabase 内の倍率を動的に変更する。
    /// </summary>
    public class CardWeightUIController : MonoBehaviour
    {
        // ======================================================
        // インスペクタ設定
        // ======================================================

        /// <summary>レアリティ倍率入力欄（Bronze）</summary>
        [SerializeField]
        private TMP_InputField bronzeWeightInput;

        /// <summary>レアリティ倍率入力欄（Silver）</summary>
        [SerializeField]
        private TMP_InputField silverWeightInput;

        /// <summary>レアリティ倍率入力欄（Gold）</summary>
        [SerializeField]
        private TMP_InputField goldWeightInput;

        /// <summary>レアリティ倍率入力欄（Legend）</summary>
        [SerializeField]
        private TMP_InputField legendWeightInput;

        /// <summary>最新パック倍率入力欄</summary>
        [SerializeField]
        private TMP_InputField latestPackInput;

        /// <summary>ニュートラル倍率入力欄</summary>
        [SerializeField]
        private TMP_InputField neutralWeightInput;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>操作対象のカードデータベース</summary>
        private CardDatabase _database;

        // ======================================================
        // Unityイベント
        // ======================================================

        /// <summary>
        /// 起動時に初期値を反映し、入力制限イベントを登録する。
        /// </summary>
        private void Start()
        {
            _database = CardDatabaseManager.Instance.GetCardDatabase();

            // 現在の倍率をUIに反映
            bronzeWeightInput.text = _database.RarityWeights[CardData.CardRarity.Bronze].ToString("F2");
            silverWeightInput.text = _database.RarityWeights[CardData.CardRarity.Silver].ToString("F2");
            goldWeightInput.text = _database.RarityWeights[CardData.CardRarity.Gold].ToString("F2");
            legendWeightInput.text = _database.RarityWeights[CardData.CardRarity.Legend].ToString("F2");

            latestPackInput.text = _database.LatestPackWeight.ToString("F2");
            neutralWeightInput.text = _database.NeutralCardWeight.ToString("F2");

            // 入力制限イベント登録
            bronzeWeightInput.onValueChanged.AddListener(ValidateNumericInput);
            silverWeightInput.onValueChanged.AddListener(ValidateNumericInput);
            goldWeightInput.onValueChanged.AddListener(ValidateNumericInput);
            legendWeightInput.onValueChanged.AddListener(ValidateNumericInput);
            latestPackInput.onValueChanged.AddListener(ValidateNumericInput);
            neutralWeightInput.onValueChanged.AddListener(ValidateNumericInput);

            // 値確定時に反映イベント登録
            bronzeWeightInput.onEndEdit.AddListener((v) => ApplyRarityWeight(CardData.CardRarity.Bronze, v));
            silverWeightInput.onEndEdit.AddListener((v) => ApplyRarityWeight(CardData.CardRarity.Silver, v));
            goldWeightInput.onEndEdit.AddListener((v) => ApplyRarityWeight(CardData.CardRarity.Gold, v));
            legendWeightInput.onEndEdit.AddListener((v) => ApplyRarityWeight(CardData.CardRarity.Legend, v));
            latestPackInput.onEndEdit.AddListener((v) => ApplyGlobalWeights());
            neutralWeightInput.onEndEdit.AddListener((v) => ApplyGlobalWeights());
        }

        // ======================================================
        // 入力制御
        // ======================================================

        /// <summary>
        /// 数字と小数点のみを許可し、小数点は1回までに制限。  
        /// さらに小数点を含めて最大5桁までしか入力できないよう制限する。
        /// </summary>
        private void ValidateNumericInput(string value)
        {
            string filtered = string.Empty;
            bool hasDecimal = false;

            // 文字を1つずつチェックして構築
            foreach (char c in value)
            {
                // 数字は常に許可
                if (char.IsDigit(c))
                {
                    filtered += c;
                    continue;
                }

                // 小数点は1回のみ許可
                if (c == '.' && !hasDecimal)
                {
                    filtered += c;
                    hasDecimal = true;
                    continue;
                }
            }

            // 桁数制限：全体で5文字まで
            if (filtered.Length > 5)
            {
                filtered = filtered.Substring(0, 5);
            }

            // フィルタ後の結果を反映（不正文字を即時除外）
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
        /// 特定レアリティの重み係数を変更する（空入力時は直前値を復活）。
        /// </summary>
        private void ApplyRarityWeight(CardData.CardRarity rarity, string input)
        {
            float value;

            if (string.IsNullOrWhiteSpace(input))
            {
                // 空入力なら直前値を復活
                value = _database.RarityWeights[rarity];
                // フィールドを更新してUIに反映
                TMP_InputField field = GetInputFieldByRarity(rarity);
                if (field != null)
                {
                    field.text = value.ToString("F2");
                }
            }
            else
            {
                // 入力値を解析
                value = ParseInput(input, _database.RarityWeights[rarity]);
            }

            // データベースに反映
            _database.SetRarityWeight(rarity, value);

            // 変更内容をログ出力
        }

        /// <summary>
        /// レアリティに対応する入力フィールドを返す。
        /// </summary>
        private TMP_InputField GetInputFieldByRarity(CardData.CardRarity rarity)
        {
            return rarity switch
            {
                CardData.CardRarity.Bronze => bronzeWeightInput,
                CardData.CardRarity.Silver => silverWeightInput,
                CardData.CardRarity.Gold => goldWeightInput,
                CardData.CardRarity.Legend => legendWeightInput,
                _ => null,
            };
        }

        /// <summary>
        /// 最新パックとニュートラル補正を更新する（空入力時は直前値を復活）。
        /// </summary>
        private void ApplyGlobalWeights()
        {
            // 最新パック倍率の処理
            string latestText = latestPackInput.text;
            float latest;

            if (string.IsNullOrWhiteSpace(latestText))
            {
                // 空入力なら直前値を復活
                latest = _database.LatestPackWeight;
                latestPackInput.text = latest.ToString("F2");
            }
            else
            {
                latest = ParseInput(latestText, _database.LatestPackWeight);
            }

            _database.SetLatestPackWeight(latest);

            // ニュートラル倍率の処理
            string neutralText = neutralWeightInput.text;
            float neutral;

            if (string.IsNullOrWhiteSpace(neutralText))
            {
                // 空入力なら直前値を復活
                neutral = _database.NeutralCardWeight;
                neutralWeightInput.text = neutral.ToString("F2");
            }
            else
            {
                neutral = ParseInput(neutralText, _database.NeutralCardWeight);
            }

            _database.SetNeutralCardWeight(neutral);
        }

        // ======================================================
        // 補助関数
        // ======================================================

        /// <summary>
        /// 数値入力が不正な場合は既定値を返す。
        /// </summary>
        private float ParseInput(string input, float fallback)
        {
            if (float.TryParse(input, out float result))
            {
                return result;
            }
            return fallback;
        }
    }
}