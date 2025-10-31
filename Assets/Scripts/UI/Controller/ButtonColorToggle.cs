// ======================================================
// ButtonColorToggle.cs
// �쐬��   : ��������
// �쐬���� : 2025-10-31
// �X�V���� : 2025-10-31
// �T�v     : �{�^����������Image�F��؂�ւ����MonoBehaviour�N���X
//           Color�ݒ�͍\���̂ł܂Ƃ߁A�O�����珉�������Ďg�p
// ======================================================

using UnityEngine;
using UnityEngine.UI;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// Button������Image�F���I���^�I�t�؂�ւ���N���X
    /// MonoBehaviour���p�������A�O������Button�R���|�[�l���g�ƐF�ݒ��^���Ďg�p
    /// </summary>
    public class ButtonColorToggle
    {
        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>�Ώ�Button�R���|�[�l���g</summary>
        private readonly Button _button;

        /// <summary>Button��Image�R���|�[�l���g</summary>
        private readonly Image _image;

        /// <summary>�I���^�I�t�F�ݒ�</summary>
        private readonly ButtonColorSettings _colorSettings;

        /// <summary>���݂̃I���^�I�t���</summary>
        private bool _isActive;

        // ======================================================
        // �R���X�g���N�^
        // ======================================================

        /// <summary>
        /// ButtonColorToggle�̏�����
        /// </summary>
        /// <param name="button">�Ώ�Button</param>
        /// <param name="settings">�I���^�I�t�F�ݒ�</param>
        /// <param name="defaultOn">������Ԃ��I�����ǂ���</param>
        public ButtonColorToggle(Button button, ButtonColorSettings settings, bool defaultOn = true)
        {
            _button = button;
            _image = _button.GetComponent<Image>();
            _colorSettings = settings;
            _isActive = defaultOn;

            if (_button == null || _image == null)
            {
                Debug.LogWarning("[ButtonColorToggle] Button �܂��� Image ���擾�ł��܂���B");
                return;
            }

            // �����F��ݒ�
            _image.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
        }

        // ======================================================
        // �p�u���b�N���\�b�h
        // ======================================================

        /// <summary>�{�^���������ɐF��؂�ւ���</summary>
        public void ToggleColor()
        {
            if (_image == null) return;

            // ��Ԕ��]
            _isActive = !_isActive;

            // �F�X�V
            _image.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
        }
    }
}