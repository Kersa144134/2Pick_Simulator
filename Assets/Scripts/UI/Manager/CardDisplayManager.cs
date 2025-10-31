// ======================================================
// CardDisplayManager.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-31
// �X�V����   : 2025-10-31
// �T�v       : CardData��CardImage��Canvas��ɉ����ŕ\������Ǘ��N���X
//             �N���X�{�^�������ŏ����J�[�h�̕\��/��\�����ꊇ�ؑ։\
// ======================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Utility;
using CardGame.UISystem.Controller;
using static CardGame.CardSystem.Data.CardData;

namespace CardGame.UISystem.Manager
{
    /// <summary>
    /// �J�[�h�摜�̕\�����䂨��уN���X�{�^���ɂ��ꊇ�ؑ֊Ǘ��N���X
    /// </summary>
    public class CardDisplayManager : MonoBehaviour
    {
        // ======================================================
        // ���b�p�[�\���́iInspector�\���p�j
        // ======================================================

        /// <summary>
        /// �N���X�{�^����񃉃b�p�[
        /// Inspector��UI�{�^���Ə�����Ԃ�ݒ�\
        /// </summary>
        [System.Serializable]
        public struct CardClassButtonInfo
        {
            public Button Button;
            public CardClass Class;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        [System.Serializable]
        public struct CardPackButtonInfo
        {
            public Button Button;
            public int PackId;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        [System.Serializable]
        public struct CardRarityButtonInfo
        {
            public Button Button;
            public CardRarity Rarity;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        [System.Serializable]
        public struct CardCostButtonInfo
        {
            public Button Button;
            public int Cost;
            public bool DefaultOn;
            public ButtonColorSettings ColorSettings;
        }

        // ======================================================
        // �R���|�[�l���g�Q��
        // ======================================================

        /// <summary>�J�[�h�t�B���^�{�^���̏�ԊǗ��Ɖ����C�x���g�𓝍������N���X</summary>
        private CardButtonManager _buttonManager;

        /// <summary>CardData�����[�h�A�L���b�V������N���X</summary>
        private CardDataLoader _loader;

        /// <summary>�X�N���[������p�R���g���[���iRectTransform�ړ��Łj</summary>
        private CardScrollController _scrollController;

        /// <summary>�J�[�h�̕\��/��\�����ꊇ�Ǘ�����N���X</summary>
        private CardVisibilityController _visibilityController;

        // ======================================================
        // �C���X�y�N�^�ݒ�
        // ======================================================

        [Header("�X�N���[���ݒ�")]
        /// <summary>�J�[�h�摜�̃X�N���[�����x����уX�N���[���͈͐ݒ�</summary>
        [SerializeField]
        private CardScrollSettings scrollSettings;

        [Header("�\���ݒ�")]
        /// <summary>�J�[�h�����ɕ��ׂ�ۂ̊Ԋu�ipx�j</summary>
        [SerializeField, Min(0f)]
        private float horizontalSpacing = 100f;

        /// <summary>�J�[�h�\���p��Prefab�iImage�t��UI�I�u�W�F�N�g�j</summary>
        [SerializeField]
        private GameObject cardPrefab = null;

        [Header("�N���X�{�^���ݒ�")]
        /// <summary>�e�J�[�h�N���X�p�{�^���Ə����\����Ԃ�ݒ肷��z��</summary>
        [SerializeField]
        private CardClassButtonInfo[] classButtons;

        [Header("�p�b�N�{�^���ݒ�")]
        /// <summary>�e�J�[�h�p�b�N�p�{�^���Ə����\����Ԃ�ݒ肷��z��</summary>
        [SerializeField]
        private CardPackButtonInfo[] packButtons;

        [Header("���A���e�B�{�^���ݒ�")]
        /// <summary>�e�J�[�h���A���e�B�p�{�^���Ə����\����Ԃ�ݒ肷��z��</summary>
        [SerializeField]
        private CardRarityButtonInfo[] rarityButtons;

        [Header("�R�X�g�{�^���ݒ�")]
        /// <summary>�e�J�[�h�R�X�g�p�{�^���Ə����\����Ԃ�ݒ肷��z��</summary>
        [SerializeField]
        private CardCostButtonInfo[] costButtons;

        // ======================================================
        // �t�B�[���h
        // ======================================================

        /// <summary>Canvas��ɐ������ꂽ�J�[�h�I�u�W�F�N�g�̃��X�g</summary>
        private readonly List<GameObject> _displayedCards = new List<GameObject>();

        /// <summary>���ݕ\���ΏۂƂȂ�CardData���X�g</summary>
        private List<CardData> _visibleCardData = new List<CardData>();

        /// <summary>�J�[�h�����eRectTransform</summary>
        private RectTransform _parentTransform;

        // ======================================================
        // Unity�C�x���g
        // ======================================================

        private void Start()
        {
            // RectTransform�擾
            _parentTransform = GetComponent<RectTransform>();

            // ScrollController�������iRectTransform���ڈړ��Łj
            _scrollController = new CardScrollController(_parentTransform, scrollSettings);

            // CardData���[�h
            _loader = new CardDataLoader();
            _loader.LoadAllCardData();

            // �\���Ǘ�������
            _visibilityController = new CardVisibilityController(_loader.AllCardData);
            _visibleCardData = _visibilityController.GetVisibleCards();

            // CardButtonManager������
            _buttonManager = new CardButtonManager(_visibilityController, _loader);

            // �{�^���ݒ�
            SetupAllButtons();

            // �J�[�h����
            RefreshDisplay();
        }

        private void Update()
        {
            _scrollController?.Update();
        }

        // ======================================================
        // �{�^������������
        // ======================================================

        /// <summary>���ׂẴ{�^��������������CardButtonManager�ɓo�^</summary>
        private void SetupAllButtons()
        {
            SetupClassButtons();
            SetupPackButtons();
            SetupRarityButtons();
            SetupCostButtons();

            // CardButtonManager�̍X�V�C�x���g��Canvas�X�V������o�^
            // ����łǂ̃t�B���^�[�ł��ύX���RefreshDisplay���Ă΂��
            _buttonManager.OnCardsUpdated += () =>
            {
                _visibleCardData = _visibilityController.GetVisibleCards();
                RefreshDisplay();
            };
        }

        /// <summary>�N���X�{�^��������</summary>
        private void SetupClassButtons()
        {
            if (classButtons == null || classButtons.Length == 0)
            {
                Debug.LogWarning("classButtons �����ݒ�ł��BInspector�Ŋ��蓖�ĂĂ��������B");
                return;
            }

            foreach (var cb in classButtons)
            {
                if (cb.Button != null)
                {
                    var classBtnInstance = new CardClassButton(cb.Button, cb.ColorSettings, cb.Class, cb.DefaultOn);
                    _buttonManager.RegisterClassButton(classBtnInstance);

                    // �{�^���N���b�N���ɕ\���𑦍X�V
                    cb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        /// <summary>�p�b�N�{�^��������</summary>
        private void SetupPackButtons()
        {
            if (packButtons == null) return;

            foreach (var pb in packButtons)
            {
                if (pb.Button != null)
                {
                    var btn = new CardPackButton(pb.Button, pb.ColorSettings, pb.PackId, pb.DefaultOn);
                    _buttonManager.RegisterPackButton(btn);

                    pb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        /// <summary>���A���e�B�{�^��������</summary>
        private void SetupRarityButtons()
        {
            if (rarityButtons == null) return;

            foreach (var rb in rarityButtons)
            {
                if (rb.Button != null)
                {
                    var btn = new CardRarityButton(rb.Button, rb.ColorSettings, rb.Rarity, rb.DefaultOn);
                    _buttonManager.RegisterRarityButton(btn);

                    rb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        /// <summary>�R�X�g�{�^��������</summary>
        private void SetupCostButtons()
        {
            if (costButtons == null) return;

            foreach (var cb in costButtons)
            {
                if (cb.Button != null)
                {
                    var btn = new CardCostButton(cb.Button, cb.ColorSettings, cb.Cost, cb.DefaultOn);
                    _buttonManager.RegisterCostButton(btn);

                    cb.Button.onClick.AddListener(() =>
                    {
                        _visibleCardData = _visibilityController.GetVisibleCards();
                        RefreshDisplay();
                    });
                }
            }
        }

        // ======================================================
        // �\���X�V����
        // ======================================================

        /// <summary>
        /// ���ݕ\���Ώۂ�CardData���X�g�Ɋ�Â��A�J�[�h�𐶐��E�z�u
        /// ScrollRect��g�p�ARectTransform���ڈړ��Ŕz�u
        /// </summary>
        public void RefreshDisplay()
        {
            // �����J�[�h�폜
            foreach (GameObject cardGO in _displayedCards)
            {
                GameObject.Destroy(cardGO);
            }
            _displayedCards.Clear();

            // �J�[�h����
            for (int i = 0; i < _visibleCardData.Count; i++)
            {
                CardData cardData = _visibleCardData[i];

                if (cardData.CardImage == null || cardPrefab == null) continue;

                GameObject cardGO = GameObject.Instantiate(cardPrefab, _parentTransform);

                Image img = cardGO.GetComponent<Image>();
                if (img != null)
                {
                    img.sprite = cardData.CardImage;
                    img.preserveAspect = true;
                }

                RectTransform rt = cardGO.GetComponent<RectTransform>();
                if (rt != null) rt.anchoredPosition = new Vector2(i * horizontalSpacing, 0f);

                _displayedCards.Add(cardGO);
            }

            // �X�N���[���ʒu���Z�b�g
            _scrollController?.ResetScrollPosition();
        }
    }
}