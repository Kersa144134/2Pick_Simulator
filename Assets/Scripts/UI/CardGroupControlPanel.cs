// ======================================================
// CardGroupControlPanel.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-30
// �X�V����   : 2025-10-30
// �T�v       : �p�b�N�E���A���e�B���Ƃ̃`�F�b�NUI���Ǘ�
//             ON/OFF��0���܂��̓f�t�H���g�����ɐݒ�
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.Data;
using CardGame.Database;

namespace CardGame.UI
{
    /// <summary>
    /// �p�b�N�E���A���e�B�ʂɈꊇ�ύX�ł���`�F�b�N�{�b�N�XUI
    /// </summary>
    public class CardGroupControlPanel : MonoBehaviour
    {
        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>�f�[�^�x�[�X�Q��</summary>
        private CardDatabase database = null;

        /// <summary>�Ώۃp�b�N�ԍ�</summary>
        [SerializeField]
        private int targetPackNumber = 1;

        /// <summary>�Ώۃ��A���e�B</summary>
        [SerializeField]
        private CardData.CardRarity targetRarity = CardData.CardRarity.Bronze;

        /// <summary>ON���ɗL�����AOFF���ɖ���������g�O��</summary>
        [SerializeField]
        private Toggle toggle = null;

        // ======================================================
        // ���J���\�b�h
        // ======================================================

        public void Initialize(CardDatabase db)
        {
            database = db;
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        // ======================================================
        // �������\�b�h
        // ======================================================

        /// <summary>
        /// �`�F�b�N��Ԃɉ����ăJ�[�h������ύX
        /// </summary>
        private void OnToggleChanged(bool isOn)
        {
            int value = isOn ? 3 : 0;

            // �p�b�N�w��܂��̓��A���e�B�w���K�p
            if (targetPackNumber > 0)
            {
                database.SetMaxCopiesByPack(targetPackNumber, value);
            }

            database.SetMaxCopiesByRarity(targetRarity, value);
            Debug.Log($"[CardGroupControl] {targetPackNumber}�ԃp�b�N / {targetRarity} �� {value}��");
        }
    }
}