// ======================================================
// CardButtonManager.cs
// �쐬�� : ��������
// �쐬���� : 2025-10-31
// �X�V���� : 2025-10-31
// �T�v : �J�[�h�t�B���^�{�^���i�N���X�E�p�b�N�E���A���e�B�E�R�X�g�j���Ǘ�
// �{�^�������ɂ���ԕύX�𓝍����A�J�[�h�\��/��\���X�V��ʒm
// ======================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Utility;
using static CardGame.CardSystem.Data.CardData;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// �{�^���������̃I���^�I�t�F��ێ�����\����
    /// </summary>
    [Serializable]
    public struct ButtonColorSettings
    {
        /// <summary>�{�^���������̐F</summary>
        public Color OnColor;

        /// <summary>�{�^���񉟉����̐F</summary>  
        public Color OffColor;
    }

    /// <summary>  
    /// �W�F�l���b�N�ȃJ�[�h�t�B���^�{�^���̊��N���X  
    /// </summary>  
    /// <typeparam name="T">�t�B���^�Ώۂ̒l�i�N���X�A�p�b�N�A���A���e�B�A�R�X�g�j</typeparam>  
    public class CardFilterButton<T>
    {
        // ======================================================  
        // �t�B�[���h  
        // ======================================================  

        /// <summary>�ΏۂƂȂ�UI�{�^���R���|�[�l���g</summary>
        protected Button _button;

        /// <summary>�{�^���������̃I���^�I�t�F��ێ�����\����</summary>
        protected ButtonColorSettings _colorSettings;

        /// <summary>���݂̃{�^��������ԁi�I����true�A�I�t��false�j</summary>
        protected bool _isActive;

        // ======================================================  
        // �v���p�e�B  
        // ======================================================  

        /// <summary>�{�^���R���|�[�l���g�ւ̓ǂݎ���p�A�N�Z�X</summary>
        public Button Button => _button;

        /// <summary>�{�^���̃I���^�I�t�F�ݒ�ւ̓ǂݎ���p�A�N�Z�X</summary>
        public ButtonColorSettings ColorSettings => _colorSettings;

        /// <summary>�{�^���̌��݉������</summary>  
        public bool IsActive => _isActive;

        /// <summary>���̃{�^�����ێ�����t�B���^�l</summary>  
        public T FilterValue { get; protected set; }

        // ======================================================  
        // �C�x���g  
        // ======================================================  

        /// <summary>�{�^���������ɏ�Ԃ��ω������ۂɒʒm�����C�x���g</summary>  
        public event Action<CardFilterButton<T>> OnFilterToggled;

        // ======================================================  
        // �R���X�g���N�^  
        // ======================================================  

        /// <summary>�R���X�g���N�^�F�{�^���������A�N���b�N�C�x���g�o�^</summary>  
        /// <param name="button">�Ώ�Button</param>  
        /// <param name="colorSettings">�I���^�I�t�F�ݒ�</param>  
        /// <param name="value">���̃{�^���̃t�B���^�l</param>  
        /// <param name="defaultOn">������Ԃ��I���ɂ��邩</param>  
        public CardFilterButton(Button button, ButtonColorSettings colorSettings, T value, bool defaultOn)
        {
            _button = button;
            _colorSettings = colorSettings;
            _isActive = defaultOn;
            FilterValue = value;

            if (_button != null)
            {
                Image img = _button.GetComponent<Image>();
                if (img != null) img.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;

                _button.onClick.AddListener(() =>
                {
                    ToggleColor();
                    OnFilterToggled?.Invoke(this);
                });
            }
        }

        // ======================================================  
        // �p�u���b�N���\�b�h  
        // ======================================================  

        /// <summary>�{�^���������ɃI���^�I�t�F��؂�ւ���</summary>  
        public void ToggleColor()
        {
            _isActive = !_isActive;

            Image img = _button?.GetComponent<Image>();
            if (img != null) img.color = _isActive ? _colorSettings.OnColor : _colorSettings.OffColor;
        }
    }

    /// <summary>�J�[�h�N���X�p�t�B���^�{�^��</summary>  
    public class CardClassButton : CardFilterButton<CardClass>
    {
        public CardClassButton(Button button, ButtonColorSettings colorSettings, CardClass value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>�J�[�h�p�b�N�p�t�B���^�{�^��</summary>  
    public class CardPackButton : CardFilterButton<int>
    {
        public CardPackButton(Button button, ButtonColorSettings colorSettings, int value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>�J�[�h���A���e�B�p�t�B���^�{�^��</summary>  
    public class CardRarityButton : CardFilterButton<CardRarity>
    {
        public CardRarityButton(Button button, ButtonColorSettings colorSettings, CardRarity value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>�J�[�h�R�X�g�p�t�B���^�{�^��</summary>  
    public class CardCostButton : CardFilterButton<int>
    {
        public CardCostButton(Button button, ButtonColorSettings colorSettings, int value, bool defaultOn)
            : base(button, colorSettings, value, defaultOn)
        { }
    }

    /// <summary>�����J�[�h�t�B���^�{�^���𓝍��Ǘ�����N���X</summary>  
    public class CardButtonManager
    {
        // ======================================================  
        // �t�B�[���h  
        // ======================================================  

        private readonly CardVisibilityController _visibilityController;
        private readonly CardDataLoader _loader;

        /// <summary>�J�[�h�N���X�{�^�����X�g</summary>  
        public List<CardClassButton> ClassButtons { get; private set; } = new List<CardClassButton>();

        /// <summary>�J�[�h�p�b�N�{�^�����X�g</summary>  
        public List<CardPackButton> PackButtons { get; private set; } = new List<CardPackButton>();

        /// <summary>�J�[�h���A���e�B�{�^�����X�g</summary>  
        public List<CardRarityButton> RarityButtons { get; private set; } = new List<CardRarityButton>();

        /// <summary>�J�[�h�R�X�g�{�^�����X�g</summary>  
        public List<CardCostButton> CostButtons { get; private set; } = new List<CardCostButton>();

        // ======================================================  
        // �C�x���g  
        // ======================================================  

        /// <summary>�J�[�h�\���X�V���ɒʒm�����C�x���g</summary>  
        public event Action OnCardsUpdated;

        // ======================================================  
        // �R���X�g���N�^  
        // ======================================================  

        /// <summary>CardButtonManager������</summary>  
        /// <param name="visibilityController">�\���^��\���Ǘ��N���X</param>  
        /// <param name="loader">CardData���[�h�N���X</param>  
        public CardButtonManager(CardVisibilityController visibilityController, CardDataLoader loader)
        {
            _visibilityController = visibilityController;
            _loader = loader;
        }

        // ======================================================  
        // �{�^���o�^���\�b�h  
        // ======================================================  

        /// <summary>�J�[�h�N���X�{�^����o�^</summary>  
        /// <param name="button">�o�^�Ώۃ{�^��</param>  
        public void RegisterClassButton(CardClassButton button)
        {
            ClassButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }

        /// <summary>�J�[�h�p�b�N�{�^����o�^</summary>  
        public void RegisterPackButton(CardPackButton button)
        {
            PackButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }

        /// <summary>�J�[�h���A���e�B�{�^����o�^</summary>  
        public void RegisterRarityButton(CardRarityButton button)
        {
            RarityButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }

        /// <summary>�J�[�h�R�X�g�{�^����o�^</summary>  
        public void RegisterCostButton(CardCostButton button)
        {
            CostButtons.Add(button);
            button.OnFilterToggled += ApplyFilters;
        }

        // ======================================================  
        // �t�B���^�K�p����  
        // ======================================================  

        /// <summary>�S�{�^���̃t�B���^��K�p���\��/��\�����X�V</summary>  
        /// <typeparam name="T">�������ꂽ�{�^���̌^</typeparam>  
        /// <param name="changedButton">�������ꂽ�{�^��</param>  
        private void ApplyFilters<T>(CardFilterButton<T> changedButton)
        {
            // �܂��S�J�[�h���x�[�X�Ƀ��X�g�쐬
            List<CardData> cardsToShow = new List<CardData>(_loader.AllCardData);

            // �N���X�ōi�荞��
            var activeClasses = ClassButtons.FindAll(b => b.IsActive).ConvertAll(b => b.FilterValue);
            if (activeClasses.Count > 0)
                cardsToShow = cardsToShow.FindAll(cd => activeClasses.Contains(cd.ClassType));

            // �p�b�N�ōi�荞��
            var activePacks = PackButtons.FindAll(b => b.IsActive).ConvertAll(b => b.FilterValue);
            if (activePacks.Count > 0)
                cardsToShow = cardsToShow.FindAll(cd => activePacks.Contains(cd.PackNumber));

            // ���A���e�B�ōi�荞��
            var activeRarities = RarityButtons.FindAll(b => b.IsActive).ConvertAll(b => b.FilterValue);
            if (activeRarities.Count > 0)
                cardsToShow = cardsToShow.FindAll(cd => activeRarities.Contains(cd.Rarity));

            // �R�X�g�ōi�荞��
            var activeCosts = CostButtons.FindAll(b => b.IsActive).ConvertAll(b => b.FilterValue);
            if (activeCosts.Count > 0)
                cardsToShow = cardsToShow.FindAll(cd => activeCosts.Contains(cd.CardCost));

            // �\���X�V
            _visibilityController.ShowCards(cardsToShow);
        }
    }
}