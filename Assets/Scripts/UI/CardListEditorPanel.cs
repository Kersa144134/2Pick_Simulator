// ======================================================
// CardListEditorPanel.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-30
// �X�V����   : 2025-10-30
// �T�v       : �S�J�[�h����1��ɕ��ׂāA�ʂ�+/-�Ŗ����ύX�ł���UI
// ======================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardGame.Data;
using CardGame.Database;

namespace CardGame.UI
{
    /// <summary>
    /// �S�J�[�h��1��ɕ��ׁA�e�J�[�h�̖������{�^���Œ����ł���UI
    /// </summary>
    public class CardListEditorPanel : MonoBehaviour
    {
        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>�J�[�h1������UI�v���n�u</summary>
        [SerializeField]
        private GameObject cardUIPrefab = null;

        /// <summary>�J�[�h����ׂ�eTransform</summary>
        [SerializeField]
        private Transform contentRoot = null;

        /// <summary>�����ŎQ�Ƃ���CardDatabase</summary>
        private CardDatabase database = null;

        /// <summary>�\�����̃J�[�hUI�Q</summary>
        private readonly List<GameObject> spawnedCards = new List<GameObject>();

        // ======================================================
        // ���J���\�b�h
        // ======================================================

        /// <summary>
        /// ���������ăJ�[�h��S�\��
        /// </summary>
        public void Initialize(CardDatabase db)
        {
            database = db;
            CreateCardUIs();
        }

        // ======================================================
        // �������\�b�h
        // ======================================================

        /// <summary>
        /// �eCardData�ɑΉ�����UI�𐶐����A+/-�{�^���ƘA��
        /// </summary>
        private void CreateCardUIs()
        {
            foreach (Transform child in contentRoot)
            {
                Destroy(child.gameObject);
            }

            // CardDatabase�ɓo�^���ꂽ�J�[�h�Q��UI��
            foreach (var dataField in typeof(CardDatabase)
                     .GetField("cardList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                     ?.GetValue(database) as List<CardData>)
            {
                GameObject ui = Instantiate(cardUIPrefab, contentRoot);
                spawnedCards.Add(ui);

                // UI�v�f�擾
                Image image = ui.transform.Find("CardImage").GetComponent<Image>();
                Text countText = ui.transform.Find("CountText").GetComponent<Text>();
                Button plusButton = ui.transform.Find("PlusButton").GetComponent<Button>();
                Button minusButton = ui.transform.Find("MinusButton").GetComponent<Button>();

                // �\�����e�ݒ�
                image.sprite = dataField.cardImage;
                countText.text = database.GetMaxCopies(dataField.CardId).ToString();

                // �{�^������o�^
                plusButton.onClick.AddListener(() =>
                {
                    int current = database.GetMaxCopies(dataField.CardId);
                    database.SetMaxCopies(dataField.CardId, current + 1);
                    countText.text = database.GetMaxCopies(dataField.CardId).ToString();
                });

                minusButton.onClick.AddListener(() =>
                {
                    int current = database.GetMaxCopies(dataField.CardId);
                    database.SetMaxCopies(dataField.CardId, current - 1);
                    countText.text = database.GetMaxCopies(dataField.CardId).ToString();
                });
            }
        }
    }
}