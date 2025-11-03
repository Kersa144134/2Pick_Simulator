// ======================================================
// CardScrollController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-11-03
// 概要       : RectTransformのX座標を直接操作するスクロール制御クラス
//             移動速度のみインスペクタで設定し、範囲は外部から更新可能
// ======================================================

using UnityEngine;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// RectTransform の位置を直接操作してスクロールを行うクラス
    /// </summary>
    public class CardScrollController
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>移動対象となるRectTransform</summary>
        private readonly RectTransform targetRect;

        /// <summary>スクロール速度（単位：px/フレーム）</summary>
        private readonly float moveSpeed;

        /// <summary>スクロール可能範囲の最小値（左端）</summary>
        private float minScrollX;

        /// <summary>スクロール可能範囲の最大値（右端）</summary>
        private float maxScrollX = 0f;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// CardScrollControllerのインスタンスを生成
        /// </summary>
        /// <param name="target">移動対象のRectTransform</param>
        /// <param name="speed">移動速度</param>
        public CardScrollController(RectTransform target, float speed)
        {
            targetRect = target;
            moveSpeed = speed;
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// 範囲を外部から設定する
        /// </summary>
        /// <param name="minX">左端X座標</param>
        public void SetScrollRange(float minX)
        {
            minScrollX = minX;
        }

        /// <summary>
        /// スクロール位置をリセット（右端に戻す）
        /// </summary>
        public void ResetScrollPosition()
        {
            if (targetRect == null)
            {
                return;
            }

            Vector2 anchoredPos = targetRect.anchoredPosition;
            anchoredPos.x = maxScrollX;
            targetRect.anchoredPosition = anchoredPos;
        }

        /// <summary>
        /// マウスホイール入力に基づいてRectTransformを移動
        /// </summary>
        public void Update()
        {
            if (targetRect == null)
            {
                return;
            }

            // 入力取得
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) < 0.001f)
            {
                return;
            }

            // 移動量適用
            Vector2 anchoredPos = targetRect.anchoredPosition;
            anchoredPos.x += scrollInput * moveSpeed;

            // 範囲内にClamp
            anchoredPos.x = Mathf.Clamp(anchoredPos.x, minScrollX, maxScrollX);

            targetRect.anchoredPosition = anchoredPos;
        }
    }
}