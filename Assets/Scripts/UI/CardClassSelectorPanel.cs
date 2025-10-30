// ======================================================
// CardClassSelectorPanel.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-30
// �X�V����   : 2025-10-30
// �T�v       : 8�N���X��I���\�ȃ{�^��UI�𐧌�
//             �w��N���X�{�j���[�g�����ȊO��0���ɐݒ�
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using CardGame.Data;
using CardGame.Database;

namespace CardGame.UI
{
    /// <summary>
    /// �N���X�I���{�^���Q��\�����A�N���b�N��CardDatabase�֔��f����UI
    /// </summary>
    public class CardClassSelectorPanel : MonoBehaviour
    {
        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>�{�^���Q�i�j���[�g�������܂�8�N���X�j</summary>
        [SerializeField]
        private Button[] classButtons = new Button[8];

        /// <summary>�e�N���X�{�^���̑Ή�Enum</summary>
        private CardData.CardClass[] classes =
        {
            CardData.CardClass.Neutral,
            CardData.CardClass.Elf,
            CardData.CardClass.Royal,
            CardData.CardClass.Witch,
            CardData.CardClass.Dragon,
            CardData.CardClass.Nightmare,
            CardData.CardClass.Bishop,
            CardData.CardClass.Nemesis
        };

        /// <summary>�Q�Ƃ���f�[�^�x�[�X</summary>
        private CardDatabase database = null;

        // ======================================================
        // ���J���\�b�h
        // ======================================================

        /// <summary>
        /// �f�[�^�x�[�X�Q�Ƃ��󂯎��A�{�^�������ݒ�
        /// </summary>
        public void Initialize(CardDatabase db)
        {
            database = db;

            // �e�{�^���ɃC�x���g�o�^
            for (int i = 0; i < classButtons.Length; i++)
            {
                int index = i;
                classButtons[i].onClick.AddListener(() => OnClassSelected(classes[index]));
            }
        }

        // ======================================================
        // �������\�b�h
        // ======================================================

        /// <summary>
        /// �N���X�I�����ɌĂ΂�A�j���[�g�����{�I���N���X�ȊO��0���ɐݒ�
        /// </summary>
        private void OnClassSelected(CardData.CardClass selected)
        {
            database.SetAllToZero(); // �S��0����
            database.SetMaxCopiesByClass(CardData.CardClass.Neutral, 3); // �j���[�g�����L��
            database.SetMaxCopiesByClass(selected, 3); // �I���N���X�L��

            Debug.Log($"[CardClassSelector] {selected} �N���X��I���B");
        }
    }
}