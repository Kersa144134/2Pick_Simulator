// ======================================================
// ButtonColorToggle.cs
// 作成者   : 高橋一翔
// 作成日時 : 2025-10-31
// 更新日時 : 2025-10-31
// 概要     : ボタン押下時にImage色を切り替える非MonoBehaviourクラス
//           Color設定は構造体でまとめ、外部から初期化して使用
// ======================================================

using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// Button押下でImage色をオン／オフ切り替えるクラス
    /// MonoBehaviourを継承せず、外部からButtonコンポーネントと色設定を与えて使用
    /// </summary>
    public class ButtonColorToggle
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>対象Buttonコンポーネント</summary>
        private readonly Button _button;

        /// <summary>ButtonのImageコンポーネント</summary>
        private readonly Image _image;

        /// <summary>オン／オフ色設定</summary>
        private readonly ButtonColorSettings _colorSettings;

        /// <summary>現在のオン／オフ状態</summary>
        private bool _isActive;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// ButtonColorToggleの初期化
        /// </summary>
        /// <param name="button">対象Button</param>
        /// <param name="settings">オン／オフ色設定</param>
        /// <param name="defaultOn">初期状態がオンかどうか</param>
        public ButtonColorToggle(Button button, ButtonColorSettings settings, bool defaultOn = true)
        {
            _button = button;
            _image = _button.GetComponent<Image>();
            _colorSettings = settings;
            _isActive = defaultOn;

            if (_button == null || _image == null)
            {
                Debug.LogWarning("[ButtonColorToggle] Button または Image が取得できません。");
                return;
            }

            // 初期色を設定
            _image.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>ボタン押下時に色を切り替える</summary>
        public void ToggleColor()
        {
            if (_image == null) return;

            // 状態反転
            _isActive = !_isActive;

            // 色更新
            _image.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
        }
    }
}