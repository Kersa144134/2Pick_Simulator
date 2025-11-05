// ======================================================
// CardDisplay.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-04
// 概要       : 単一カードUIの表示／非表示とUI更新を担当するクラス
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProUGUIを使用するために追加
using CardGame.CardSystem.Data;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// 単一カードのUI要素を制御するクラス  
    /// CardDisplayManagerから生成され、CardDataの内容に基づきUIを更新する
    /// </summary>
    public class CardDisplay : MonoBehaviour
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>このカードが表示しているデータ</summary>
        public CardData CurrentData { get; private set; }

        /// <summary>対象データ（枚数付きの構造体を外部から受け取る場合に使用）</summary>
        public CardData TargetData { get; private set; }

        /// <summary>カード画像を表示するImage</summary>
        private Image cardImage;

        /// <summary>現在の枚数を表示するテキスト</summary>
        private TextMeshProUGUI countText;

        /// <summary>枚数管理・UI同期制御クラス</summary>
        private CardQuantityController _quantityController = new CardQuantityController();

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>枚数管理・UI同期制御クラス</summary>
        public CardQuantityController QuantityController => _quantityController;

        // ======================================================
        // 初期化
        // ======================================================

        /// <summary>
        /// カードのUI内容を初期化する  
        /// CardDisplayManagerから生成時に呼び出される
        /// </summary>
        public void Initialize(CardData data, CardDatabase database)
        {
            // カードデータと参照コンポーネントを取得
            CurrentData = data;
            TargetData = data;
            cardImage = gameObject.GetComponent<Image>();

            Transform textTransform = transform.Find("Text");
            Transform plusTransform = transform.Find("Plus");
            Transform minusTransform = transform.Find("Minus");
            
            if (textTransform != null)
            {
                countText = textTransform.GetComponent<TextMeshProUGUI>();
            }

            if (plusTransform != null && minusTransform != null)
            {
                Button plusButton = plusTransform.GetComponent<Button>();
                Button minusButton = minusTransform.GetComponent<Button>();

                // 枚数管理クラスを初期化
                _quantityController.Initialize(gameObject, countText, plusButton, minusButton, data, database);
            }

            // UIを初期更新
            UpdateUI();
        }

        // ======================================================
        // 表示更新
        // ======================================================

        /// <summary>
        /// CardDataの内容に基づいてUIを更新する
        /// </summary>
        public void UpdateUI()
        {
            // CardDataが設定されていない場合は処理しない
            if (CurrentData == null)
            {
                return;
            }

            // カード画像を更新
            if (cardImage != null)
            {
                cardImage.sprite = CurrentData.CardImage;
            }
        }

        // ======================================================
        // 枚数表示
        // ======================================================

        /// <summary>
        /// 現在のカード枚数をテキストで表示する  
        /// 外部のDeckListManagerから枚数情報を受け取り更新する
        /// </summary>
        /// <param name="count">表示する枚数</param>
        public void SetCountText(int count)
        {
            // テキスト参照が未設定なら取得を試みる
            if (countText == null)
            {
                countText = GetComponentInChildren<TextMeshProUGUI>();
            }

            // 枚数テキストを更新
            if (countText != null)
            {
                countText.text = $"×{count.ToString()}";
            }
        }

        // ======================================================
        // 表示切替
        // ======================================================

        /// <summary>
        /// 表示リストまたは非表示リストへ移動する  
        /// CardDisplayManagerから呼び出される
        /// </summary>
        /// <param name="newParent">移動先のRectTransform</param>
        public void SetParent(RectTransform newParent)
        {
            // nullチェック
            if (newParent == null)
            {
                return;
            }

            // 親を変更してUI階層を再構築
            transform.SetParent(newParent, false);
        }
    }
}