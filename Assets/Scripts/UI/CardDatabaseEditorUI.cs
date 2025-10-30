// ======================================================
// CardDatabaseEditorUI.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-30
// �X�V����   : 2025-10-30
// �T�v       : �J�[�h�f�[�^�x�[�X�ҏWUI�̓����N���X
//             �N���X�I���E�ꊇ�ݒ�E�ʐݒ�̊eUI�𐧌�
// ======================================================

using UnityEngine;

namespace CardGame.UI
{
    /// <summary>
    /// �J�[�h�f�[�^�x�[�X�ҏWUI�̒��j�N���X  
    /// �e��UI�p�l�����܂Ƃ߁ACardDatabase�Ƃ̓������Ǘ�����
    /// </summary>
    public class CardDatabaseEditorUI : MonoBehaviour
    {
        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>CardDatabase�C���X�^���X�Q�Ɓi�O���Ő����ς݂�n���j</summary>
        [SerializeField]
        private CardGame.Database.CardDatabase database = null;

        /// <summary>�N���X�I��UI�p�l��</summary>
        [SerializeField]
        private CardClassSelectorPanel classSelectorPanel = null;

        /// <summary>�p�b�N�E���A���e�B����p�l��</summary>
        [SerializeField]
        private CardGroupControlPanel groupControlPanel = null;

        /// <summary>�S�J�[�h�ꗗ�ҏW�p�l��</summary>
        [SerializeField]
        private CardListEditorPanel listEditorPanel = null;

        // ======================================================
        // Unity�C�x���g
        // ======================================================

        private void Start()
        {
            // �eUI�Ƀf�[�^�x�[�X�Q�Ƃ�n��
            if (classSelectorPanel != null)
            {
                classSelectorPanel.Initialize(database);
            }

            if (groupControlPanel != null)
            {
                groupControlPanel.Initialize(database);
            }

            if (listEditorPanel != null)
            {
                listEditorPanel.Initialize(database);
            }
        }
    }
}