// ======================================================
// CardDataLoader.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-31
// �X�V����   : 2025-10-31
// �T�v       : Resources/Cards�z������CardData�����[�h���ĊǗ����郆�[�e�B���e�B�N���X
//             ����Manager���狤�L�\
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using CardGame.CardSystem.Data;
using static CardGame.CardSystem.Data.CardData;

namespace CardGame.CardSystem.Utility
{
    /// <summary>
    /// CardData�����[�h�E�L���b�V�����郆�[�e�B���e�B�N���X
    /// </summary>
    public class CardDataLoader
    {
        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>���[�h�ς݂�CardData���X�g</summary>
        private readonly List<CardData> _allCardData = new List<CardData>();

        /// <summary>�ǂݎ���p�ŊO���Q�Ɖ\��CardData���X�g</summary>
        public IReadOnlyList<CardData> AllCardData => _allCardData;

        // ======================================================
        // �p�u���b�N���\�b�h
        // ======================================================

        /// <summary>
        /// Resources/Cards�z������CardData�����[�h  
        /// ���łɃ��[�h�ς݂̏ꍇ�͍ă��[�h�����L���b�V�����g�p
        /// </summary>
        public void LoadAllCardData()
        {
            if (_allCardData.Count > 0)
            {
                // ���łɃ��[�h�ς݂Ȃ珈�����X�L�b�v
                return;
            }

            CardData[] loadedCards = Resources.LoadAll<CardData>("Cards");

            if (loadedCards != null && loadedCards.Length > 0)
            {
                _allCardData.Clear();
                _allCardData.AddRange(loadedCards);
            }
            else
            {
                Debug.LogWarning("[CardDataLoader] Resources/Cards �z����CardData��������܂���B");
            }
        }

        /// <summary>
        /// �w��ID��CardData���擾
        /// </summary>
        /// <param name="cardId">��������J�[�hID</param>
        /// <returns>������Ȃ����null</returns>
        public CardData GetCardById(int cardId)
        {
            return _allCardData.Find(c => c.CardId == cardId);
        }

        /// <summary>
        /// �w������Ɉ�v����J�[�h���X�g���擾
        /// </summary>
        /// <param name="predicate">��������</param>
        public List<CardData> FindCards(System.Predicate<CardData> predicate)
        {
            return _allCardData.FindAll(predicate);
        }

        // ======================================================
        // �ǉ��������\�b�h
        // ======================================================

        /// <summary>
        /// �w��N���X�ɏ�������J�[�h���擾
        /// </summary>
        /// <param name="className">�����Ώۂ̃N���X��</param>
        /// <returns>�Y���J�[�h�̃��X�g</returns>
        public List<CardData> FindCardsByClass(CardClass className)
        {
            return _allCardData.FindAll(c => c.ClassType == className);
        }

        /// <summary>
        /// �w��p�b�N�ɏ�������J�[�h���擾
        /// </summary>
        /// <param name="packName">�����Ώۂ̃p�b�N��</param>
        /// <returns>�Y���J�[�h�̃��X�g</returns>
        public List<CardData> FindCardsByPack(int packNumber)
        {
            return _allCardData.FindAll(c => c.PackNumber == packNumber);
        }

        /// <summary>
        /// �w�背�A���e�B�̃J�[�h���擾
        /// </summary>
        /// <param name="rarity">�����Ώۂ̃��A���e�B</param>
        /// <returns>�Y���J�[�h�̃��X�g</returns>
        public List<CardData> FindCardsByRarity(CardData.CardRarity rarity)
        {
            return _allCardData.FindAll(c => c.Rarity == rarity);
        }

        /// <summary>
        /// �w��R�X�g�̃J�[�h���擾
        /// </summary>
        /// <param name="cost">�����Ώۂ̃R�X�g�l</param>
        /// <returns>�Y���J�[�h�̃��X�g</returns>
        public List<CardData> FindCardsByCost(int cost)
        {
            return _allCardData.FindAll(c => c.CardCost == cost);
        }
    }
}