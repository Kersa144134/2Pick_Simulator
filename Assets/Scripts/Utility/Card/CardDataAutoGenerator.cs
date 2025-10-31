// ======================================================
// CardDataAutoGenerator.cs
// �쐬��     : ��������
// �쐬����   : 2025-10-31
// �X�V����   : 2025-10-31
// �T�v       : �摜�t�@�C�������� CardData ScriptableObject ����������
//              �V�K�쐬�Ɗ����J�[�h�̉摜�X�V�𕪂��ď����\
// ======================================================

using UnityEngine;
using UnityEditor;
using System.IO;
using CardGame.Data;

namespace CardGame.Editor
{
    /// <summary>
    /// Editor �p�ɃJ�[�h�f�[�^ ScriptableObject �����������E�摜���蓖�Ă���N���X
    /// </summary>
    public class CardDataAutoGenerator
    {
        // ======================================================
        // �萔
        // ======================================================

        /// <summary>�摜�̃��[�g�t�H���_�iAssets��j</summary>
        private const string ImagesRootPath = "Assets/Resources/Images/Cards/";

        /// <summary>ScriptableObject �o�͐�t�H���_�iAssets��j</summary>
        private const string ResourcesRootPath = "Assets/Resources/Cards/";

        /// <summary>Resources.Load �p�̃��[�g�p�X</summary>
        private const string ResourcesLoadRoot = "Images/Cards/";

        /// <summary>�J�[�hID �̌����i�t�@�C�����擪5�����j</summary>
        private const int CardIdLength = 5;

        /// <summary>�����ő喇��</summary>
        private const int DefaultMaxCopies = 3;

        // ======================================================
        // ���j���[�R�}���h�i�V�K�쐬�{�摜���蓖�āj
        // ======================================================

        [MenuItem("Tools/Card/Create All CardData With Images")]
        private static void GenerateAllCardDataWithImages()
        {
            ProcessAllCardFiles(isUpdateOnly: false);
        }

        // ======================================================
        // ���j���[�R�}���h�i�摜�̂ݏ㏑���j
        // ======================================================

        [MenuItem("Tools/Card/Update All Card Images Only")]
        private static void UpdateAllCardImages()
        {
            ProcessAllCardFiles(isUpdateOnly: true);
        }

        // ======================================================
        // ��������
        // ======================================================

        /// <summary>
        /// �J�[�h�t�H���_�𑖍����ASO�쐬�E�X�V���s�����ʏ���
        /// </summary>
        /// <param name="isUpdateOnly">true: �摜�̂ݍX�V, false: �V�K�쐬�{�摜���蓖��</param>
        private static void ProcessAllCardFiles(bool isUpdateOnly)
        {
            // �N���X�t�H���_���擾
            string[] classFolders = Directory.GetDirectories(ImagesRootPath);

            foreach (string classFolder in classFolders)
            {
                // �t�H���_������N���X�ԍ��ƃN���X�����擾
                string folderName = Path.GetFileName(classFolder);
                string[] parts = folderName.Split('_');

                if (parts.Length < 2)
                {
                    Debug.LogWarning($"[CardDataGenerator] �t�H���_���`���s��: {folderName}");
                    continue;
                }

                string className = parts[1];

                // CardClass �񋓌^�ɕϊ�
                if (!System.Enum.TryParse<CardData.CardClass>(className, out CardData.CardClass cardClass))
                {
                    Debug.LogWarning($"[CardDataGenerator] �N���X�ϊ����s: {className}");
                    continue;
                }

                // �N���X�t�H���_���� PNG �t�@�C�����擾
                string[] files = Directory.GetFiles(classFolder, "*.png");

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    // �t�@�C�������Z������ꍇ�̓X�L�b�v
                    if (fileName.Length < CardIdLength + 1)
                    {
                        Debug.LogWarning($"[CardDataGenerator] �t�@�C�����Z����: {fileName}");
                        continue;
                    }

                    // �J�[�hID���擾
                    string idStr = fileName.Substring(0, CardIdLength);
                    if (!int.TryParse(idStr, out int cardId))
                    {
                        Debug.LogWarning($"[CardDataGenerator] ID�ϊ����s: {fileName}");
                        continue;
                    }

                    // �J�[�h�����擾
                    int underscoreIndex = fileName.IndexOf('_');
                    if (underscoreIndex < 0 || underscoreIndex >= fileName.Length - 1)
                    {
                        Debug.LogWarning($"[CardDataGenerator] �J�[�h���擾���s: {fileName}");
                        continue;
                    }
                    string cardName = fileName.Substring(underscoreIndex + 1);

                    // �ۑ���t�H���_�쐬
                    string outputFolder = Path.Combine(ResourcesRootPath, folderName);
                    if (!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);
                    }

                    // SO �̃t���p�X
                    string assetPath = Path.Combine(outputFolder, $"{cardId:D5}.asset");

                    // ���� SO �����[�h
                    CardData cardSO = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);

                    if (isUpdateOnly)
                    {
                        // �摜�̂ݍX�V���[�h
                        if (cardSO != null)
                        {
                            ApplyCardImage(cardSO, folderName, fileName, cardName, cardId);
                        }
                        else
                        {
                            Debug.LogWarning($"[CardDataGenerator] �X�V�ΏۂȂ�: {cardName} ({cardId:D5})");
                        }
                        continue;
                    }

                    // �V�K�쐬���[�h
                    if (cardSO != null)
                    {
                        // ���� SO �̓X�L�b�v
                        Debug.Log($"[CardDataGenerator] SKIP: ���łɑ��� {cardName} ({cardId:D5})");
                        continue;
                    }

                    // SO �V�K�쐬
                    cardSO = ScriptableObject.CreateInstance<CardData>();

                    // ID�ݒ�i����J�t�B�[���h�j
                    cardSO.GetType().GetField("cardId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(cardSO, cardId);

                    // �J�[�h���ݒ�i����J�t�B�[���h�j
                    cardSO.GetType().GetField("cardName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(cardSO, cardName);

                    // �ő喇���ݒ�i����J�t�B�[���h�j
                    cardSO.GetType().GetField("maxCopies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(cardSO, DefaultMaxCopies);

                    // AssetDatabase �ɕۑ�
                    AssetDatabase.CreateAsset(cardSO, assetPath);

                    // �摜���蓖��
                    ApplyCardImage(cardSO, folderName, fileName, cardName, cardId);

                    // ���O�o��
                    Debug.Log($"[CardDataGenerator] Created: {cardName} ({cardId:D5})");
                }
            }

            // AssetDatabase �X�V
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[CardDataGenerator] ��������");
        }

        // ======================================================
        // �摜���蓖�ċ��ʃ��\�b�h
        // ======================================================

        /// <summary>
        /// CardData �ɉ摜�� Resources ���烍�[�h���Ċ��蓖��
        /// </summary>
        private static void ApplyCardImage(CardData cardSO, string folderName, string fileName, string cardName, int cardId)
        {
            string resourcePath = $"{ResourcesLoadRoot}{folderName}/{fileName}";
            Sprite sprite = Resources.Load<Sprite>(resourcePath);

            if (sprite != null)
            {
                cardSO.CardImage = sprite; // CardImage �啶��
                Debug.Log($"[CardDataGenerator] Image Applied: {cardName} ({cardId:D5})");
            }
            else
            {
                Debug.LogWarning($"[CardDataGenerator] Sprite���[�h���s: {resourcePath}");
            }
        }
    }
}