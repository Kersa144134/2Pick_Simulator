// ======================================================
// CardVisibilityController.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-31
// �X�V����   : 2025-10-31
// �T�v       : CardData�̕\���^��\�����X�g���Ǘ����鐧��N���X
//             CardDisplayManager����Ăяo����A�\����Ԃ̈ꊇ�ύX���s��
// ======================================================

using System.Collections.Generic;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
using static CardGame.CardSystem.Data.CardData;
using UnityEngine;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// �J�[�h�̕\����ԁi�\���E��\���j���ꊇ�Ǘ�����N���X  
    /// �\�����X�g�E��\�����X�g������ŕێ����A�܂Ƃ߂Đ؂�ւ������񋟂���
    /// </summary>
    public class CardVisibilityController
    {
        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>���ݕ\���ΏۂƂ��ėL����CardData���X�g</summary>
        private readonly List<CardData> _visibleCardData = new List<CardData>();

        /// <summary>���ݔ�\���ΏۂƂ��ĕێ������CardData���X�g</summary>
        private readonly List<CardData> _hiddenCardData = new List<CardData>();

        // ======================================================
        // �R���X�g���N�^
        // ======================================================

        /// <summary>
        /// �����̑SCardData���X�g���珉�������s��  
        /// ���ׂẴJ�[�h���ŏ��͕\���ΏۂƂ��ēo�^
        /// </summary>
        /// <param name="allCards">���[�h�ς݂̑SCardData</param>
        public CardVisibilityController(IEnumerable<CardData> allCards)
        {
            foreach (var card in allCards)
            {
                _visibleCardData.Add(card);
            }
        }

        // ======================================================
        // �p�u���b�N���\�b�h
        // ======================================================

        // --------------------------------------------------
        // ��ԕύX
        // --------------------------------------------------

        /// <summary>
        /// �w�肵�������̃J�[�h���\�����X�g�Ɉړ�����  
        /// ���ɔ�\���̏ꍇ�͖��������
        /// </summary>
        /// <param name="cardsToHide">��\���ɂ���CardData�̃R���N�V����</param>
        public void HideCards(IEnumerable<CardData> cardsToHide)
        {
            foreach (var card in cardsToHide)
            {
                if (_visibleCardData.Contains(card))
                {
                    _visibleCardData.Remove(card);
                    _hiddenCardData.Add(card);
                }
            }
        }

        /// <summary>
        /// �w�肵�������̃J�[�h��\�����X�g�ɖ߂�  
        /// ���ɕ\������Ă���ꍇ�͖��������
        /// </summary>
        /// <param name="cardsToShow">�\���Ώۂɖ߂�CardData�̃R���N�V����</param>
        public void ShowCards(IEnumerable<CardData> cardsToShow)
        {
            foreach (var card in cardsToShow)
            {
                if (_hiddenCardData.Contains(card))
                {
                    _hiddenCardData.Remove(card);
                    _visibleCardData.Add(card);
                }
            }

            // �\������CardId�Ń\�[�g�i���肵���`�揇��ۏ؁j
            _visibleCardData.Sort((a, b) => a.CardId.CompareTo(b.CardId));
        }

        // --------------------------------------------------
        // ���X�g�Q��
        // --------------------------------------------------

        /// <summary>
        /// ���ݕ\���ΏۂƂȂ��Ă���CardData���X�g���擾����
        /// </summary>
        public List<CardData> GetVisibleCards()
        {
            return new List<CardData>(_visibleCardData);
        }

        /// <summary>
        /// ���ݔ�\���ΏۂƂȂ��Ă���CardData���X�g���擾����
        /// </summary>
        public List<CardData> GetHiddenCards()
        {
            return new List<CardData>(_hiddenCardData);
        }

        // --------------------------------------------------
        // ���Z�b�g����
        // --------------------------------------------------

        /// <summary>
        /// ���ׂẴJ�[�h��\���Ώۂɖ߂�  
        /// �i�t�B���^�����Ȃǂ̈ꊇ����Ɏg�p�j
        /// </summary>
        public void ShowAll()
        {
            _visibleCardData.AddRange(_hiddenCardData);
            _hiddenCardData.Clear();

            _visibleCardData.Sort((a, b) => a.CardId.CompareTo(b.CardId));
        }

        /// <summary>
        /// ���ׂẴJ�[�h���\���ɂ���
        /// </summary>
        public void HideAll()
        {
            _hiddenCardData.AddRange(_visibleCardData);
            _visibleCardData.Clear();
        }
    }
}