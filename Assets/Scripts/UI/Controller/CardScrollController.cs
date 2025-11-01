// ======================================================
// CardScrollController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-10-31
// 概要       : RectTransformのX座標を直接操作するスクロール制御クラス
//             移動速度と範囲を構造体から受け取り、純粋な位置移動を実行
// ======================================================

using UnityEngine;

namespace CardGame.UISystem.Controller
{
    // ======================================================
    // 構造体
    // ======================================================

    /// <summary>
    /// スクロール範囲と速度を設定する構造体  
    /// CardDisplayManager からインスペクタ経由で設定可能
    /// </summary>
    [System.Serializable]
    public struct CardScrollSettings
    {
        [Tooltip("スクロール移動速度（単位：ピクセル/フレーム）")]
        public float moveSpeed;

        [Tooltip("スクロール可能範囲（左端と右端のX座標）")]
        public Vector2 scrollRange;
    }

    /// <summary>
    /// RectTransform の位置を直接操作してスクロールを行う純粋クラス  
    /// MonoBehaviour を継承せず、任意のクラスから利用可能
    /// </summary>
    public class CardScrollController
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>移動対象となるRectTransform</summary>
        private readonly RectTransform targetRect;

        /// <summary>スクロール設定（速度・範囲）</summary>
        private readonly CardScrollSettings settings;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// CardScrollControllerのインスタンスを生成
        /// </summary>
        /// <param name="target">移動対象のRectTransform</param>
        /// <param name="scrollSettings">速度・範囲設定</param>
        public CardScrollController(RectTransform target, CardScrollSettings scrollSettings)
        {
            targetRect = target;
            settings = scrollSettings;
        }

        // ======================================================
        // Unityイベント
        // ======================================================

        /// <summary>
        /// マウスホイール入力に基づいてRectTransformを移動させる  
        /// MonoBehaviourのUpdate内から明示的に呼び出す
        /// </summary>
        public void Update()
        {
            if (targetRect == null)
            {
                return;
            }

            // --------------------------------------------------
            // 入力検出（マウスホイール）
            // --------------------------------------------------
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) < 0.001f)
            {
                return;
            }

            // --------------------------------------------------
            // 移動量算出と適用
            // --------------------------------------------------
            Vector2 anchoredPos = targetRect.anchoredPosition;
            anchoredPos.x += scrollInput * settings.moveSpeed;

            // 範囲内にClamp
            anchoredPos.x = Mathf.Clamp(anchoredPos.x, settings.scrollRange.x, settings.scrollRange.y);

            targetRect.anchoredPosition = anchoredPos;
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// スクロール位置をリセット
        /// </summary>
        public void ResetScrollPosition()
        {
            if (targetRect == null)
            {
                return;
            }

            Vector2 anchoredPos = targetRect.anchoredPosition;
            anchoredPos.x = settings.scrollRange.y;
            targetRect.anchoredPosition = anchoredPos;
        }
    }
}