// ======================================================
// CardData.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-30
// �X�V����   : 2025-10-30
// �T�v       : �J�[�h�̊�{����ێ����� ScriptableObject �N���X
//             �J�[�hID����N���X�E�p�b�N�E���A���e�B����������
// ======================================================

using UnityEngine;

namespace CardGame.CardSystem.Data
{
    /// <summary>
    /// �J�[�h1�����̊�{�����`���� ScriptableObject �N���X
    /// ID�\���Ɋ�Â��ăN���X�E�p�b�N�E���A���e�B�𕪗��\
    /// </summary>
    [CreateAssetMenu(fileName = "CardData", menuName = "CardGame/Card Data")]
    public class CardData : ScriptableObject
    {
        // ======================================================
        // �񋓌^
        // ======================================================

        /// <summary>
        /// �J�[�h�̏����N���X
        /// 0:�j���[�g����, 1:�G���t, 2:���C����, 3:�E�B�b�`,
        /// 4:�h���S��, 5:�i�C�g���A, 6:�r�V���b�v, 7:�l���V�X
        /// </summary>
        public enum CardClass
        {
            Neutral = 0,
            Elf = 1,
            Royal = 2,
            Witch = 3,
            Dragon = 4,
            Nightmare = 5,
            Bishop = 6,
            Nemesis = 7
        }

        /// <summary>
        /// �J�[�h�̃��A���e�B
        /// 0:�u�����Y, 1:�V���o�[, 2:�S�[���h, 3:���W�F���h
        /// </summary>
        public enum CardRarity
        {
            Bronze = 0,
            Silver = 1,
            Gold = 2,
            Legend = 3
        }

        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>
        /// �J�[�h�摜�i�����G��A�[�g�Ȃǁj  
        /// UI�\���ɒ��ڎg�p�\��Sprite��ێ�����
        /// </summary>
        public Sprite CardImage = null;

        /// <summary>
        /// �J�[�h����ӂɎ��ʂ���5������ID
        /// ��F21342 �� ���C���� / �p�b�N13 / �S�[���h / �ԍ�2
        /// </summary>
        [SerializeField]
        private int cardId = 0;

        /// <summary>
        /// �J�[�h�̃R�X�g
        /// </summary>
        [SerializeField, Min(0)]
        private int cardCost = 0;

        /// <summary>
        /// �J�[�h�̕\����
        /// </summary>
        [SerializeField]
        private string cardName = string.Empty;

        /// <summary>
        /// �J�[�h�̌��ʃe�L�X�g�܂��͐�����
        /// </summary>
        [SerializeField, TextArea(2, 5)]
        private string cardText = string.Empty;

        /// <summary>
        /// �f�b�L�ɕҐ��ł���ő喇��
        /// </summary>
        [SerializeField, Range(0, 3)]
        private int maxCopies = 3;

        // ======================================================
        // �v���p�e�B
        // ======================================================

        /// <summary>�J�[�hID���擾</summary>
        public int CardId
        {
            get { return cardId; }
        }

        /// <summary>�J�[�h�R�X�g���擾</summary>
        public int CardCost
        {
            get { return cardCost; }
        }

        /// <summary>�J�[�h�����擾</summary>
        public string CardName
        {
            get { return cardName; }
        }

        /// <summary>�J�[�h�e�L�X�g���擾</summary>
        public string CardText
        {
            get { return cardText; }
        }

        /// <summary>�ő喇�����擾</summary>
        public int MaxCopies
        {
            get { return maxCopies; }
        }

        /// <summary>�J�[�h�̏����N���X��ID����Z�o���Ď擾</summary>
        public CardClass ClassType
        {
            get { return (CardClass)(cardId / 10000); }
        }

        /// <summary>�J�[�h�̑�����p�b�N�ԍ��i2���j��ID����擾</summary>
        public int PackNumber
        {
            get { return (cardId / 100) % 100; }
        }

        /// <summary>�J�[�h�̃��A���e�B��ID����Z�o���Ď擾</summary>
        public CardRarity Rarity
        {
            get { return (CardRarity)((cardId / 10) % 10); }
        }

        /// <summary>�J�[�hID�̖����ԍ��i�d���h�~�p�j���擾</summary>
        public int SubId
        {
            get { return cardId % 10; }
        }

        // ======================================================
        // �p�u���b�N���\�b�h
        // ======================================================

        /// <summary>
        /// �J�[�h���𐮌`���ăR���\�[���o�́i�f�o�b�O�p�j
        /// </summary>
        public void PrintCardInfo()
        {
            // ��͂����e�����o�͂��ăf�[�^�m�F��e�Ղɂ���
            Debug.Log(
                $"[Card Info]\n" +
                $"ID: {cardId}\n" +
                $"Class: {ClassType}\n" +
                $"Pack: {PackNumber}\n" +
                $"Rarity: {Rarity}\n" +
                $"Name: {cardName}\n" +
                $"Text: {cardText}\n" +
                $"Max Copies: {maxCopies}"
            );
        }
    }
}