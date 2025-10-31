// ======================================================
// CardDataAutoGenerator.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-31
// 更新日時   : 2025-10-31
// 概要       : 画像ファイル名から CardData ScriptableObject を自動生成
//              新規作成と既存カードの画像更新を分けて処理可能
// ======================================================

using UnityEngine;
using UnityEditor;
using System.IO;
using CardGame.Data;

namespace CardGame.Editor
{
    /// <summary>
    /// Editor 用にカードデータ ScriptableObject を自動生成・画像割り当てするクラス
    /// </summary>
    public class CardDataAutoGenerator
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>画像のルートフォルダ（Assets上）</summary>
        private const string ImagesRootPath = "Assets/Resources/Images/Cards/";

        /// <summary>ScriptableObject 出力先フォルダ（Assets上）</summary>
        private const string ResourcesRootPath = "Assets/Resources/Cards/";

        /// <summary>Resources.Load 用のルートパス</summary>
        private const string ResourcesLoadRoot = "Images/Cards/";

        /// <summary>カードID の桁数（ファイル名先頭5文字）</summary>
        private const int CardIdLength = 5;

        /// <summary>初期最大枚数</summary>
        private const int DefaultMaxCopies = 3;

        // ======================================================
        // メニューコマンド（新規作成＋画像割り当て）
        // ======================================================

        [MenuItem("Tools/Card/Create All CardData With Images")]
        private static void GenerateAllCardDataWithImages()
        {
            ProcessAllCardFiles(isUpdateOnly: false);
        }

        // ======================================================
        // メニューコマンド（画像のみ上書き）
        // ======================================================

        [MenuItem("Tools/Card/Update All Card Images Only")]
        private static void UpdateAllCardImages()
        {
            ProcessAllCardFiles(isUpdateOnly: true);
        }

        // ======================================================
        // 内部処理
        // ======================================================

        /// <summary>
        /// カードフォルダを走査し、SO作成・更新を行う共通処理
        /// </summary>
        /// <param name="isUpdateOnly">true: 画像のみ更新, false: 新規作成＋画像割り当て</param>
        private static void ProcessAllCardFiles(bool isUpdateOnly)
        {
            // クラスフォルダを取得
            string[] classFolders = Directory.GetDirectories(ImagesRootPath);

            foreach (string classFolder in classFolders)
            {
                // フォルダ名からクラス番号とクラス名を取得
                string folderName = Path.GetFileName(classFolder);
                string[] parts = folderName.Split('_');

                if (parts.Length < 2)
                {
                    Debug.LogWarning($"[CardDataGenerator] フォルダ名形式不正: {folderName}");
                    continue;
                }

                string className = parts[1];

                // CardClass 列挙型に変換
                if (!System.Enum.TryParse<CardData.CardClass>(className, out CardData.CardClass cardClass))
                {
                    Debug.LogWarning($"[CardDataGenerator] クラス変換失敗: {className}");
                    continue;
                }

                // クラスフォルダ内の PNG ファイルを取得
                string[] files = Directory.GetFiles(classFolder, "*.png");

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    // ファイル名が短すぎる場合はスキップ
                    if (fileName.Length < CardIdLength + 1)
                    {
                        Debug.LogWarning($"[CardDataGenerator] ファイル名短すぎ: {fileName}");
                        continue;
                    }

                    // カードIDを取得
                    string idStr = fileName.Substring(0, CardIdLength);
                    if (!int.TryParse(idStr, out int cardId))
                    {
                        Debug.LogWarning($"[CardDataGenerator] ID変換失敗: {fileName}");
                        continue;
                    }

                    // カード名を取得
                    int underscoreIndex = fileName.IndexOf('_');
                    if (underscoreIndex < 0 || underscoreIndex >= fileName.Length - 1)
                    {
                        Debug.LogWarning($"[CardDataGenerator] カード名取得失敗: {fileName}");
                        continue;
                    }
                    string cardName = fileName.Substring(underscoreIndex + 1);

                    // 保存先フォルダ作成
                    string outputFolder = Path.Combine(ResourcesRootPath, folderName);
                    if (!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);
                    }

                    // SO のフルパス
                    string assetPath = Path.Combine(outputFolder, $"{cardId:D5}.asset");

                    // 既存 SO をロード
                    CardData cardSO = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);

                    if (isUpdateOnly)
                    {
                        // 画像のみ更新モード
                        if (cardSO != null)
                        {
                            ApplyCardImage(cardSO, folderName, fileName, cardName, cardId);
                        }
                        else
                        {
                            Debug.LogWarning($"[CardDataGenerator] 更新対象なし: {cardName} ({cardId:D5})");
                        }
                        continue;
                    }

                    // 新規作成モード
                    if (cardSO != null)
                    {
                        // 既存 SO はスキップ
                        Debug.Log($"[CardDataGenerator] SKIP: すでに存在 {cardName} ({cardId:D5})");
                        continue;
                    }

                    // SO 新規作成
                    cardSO = ScriptableObject.CreateInstance<CardData>();

                    // ID設定（非公開フィールド）
                    cardSO.GetType().GetField("cardId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(cardSO, cardId);

                    // カード名設定（非公開フィールド）
                    cardSO.GetType().GetField("cardName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(cardSO, cardName);

                    // 最大枚数設定（非公開フィールド）
                    cardSO.GetType().GetField("maxCopies", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .SetValue(cardSO, DefaultMaxCopies);

                    // AssetDatabase に保存
                    AssetDatabase.CreateAsset(cardSO, assetPath);

                    // 画像割り当て
                    ApplyCardImage(cardSO, folderName, fileName, cardName, cardId);

                    // ログ出力
                    Debug.Log($"[CardDataGenerator] Created: {cardName} ({cardId:D5})");
                }
            }

            // AssetDatabase 更新
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[CardDataGenerator] 処理完了");
        }

        // ======================================================
        // 画像割り当て共通メソッド
        // ======================================================

        /// <summary>
        /// CardData に画像を Resources からロードして割り当て
        /// </summary>
        private static void ApplyCardImage(CardData cardSO, string folderName, string fileName, string cardName, int cardId)
        {
            string resourcePath = $"{ResourcesLoadRoot}{folderName}/{fileName}";
            Sprite sprite = Resources.Load<Sprite>(resourcePath);

            if (sprite != null)
            {
                cardSO.CardImage = sprite; // CardImage 大文字
                Debug.Log($"[CardDataGenerator] Image Applied: {cardName} ({cardId:D5})");
            }
            else
            {
                Debug.LogWarning($"[CardDataGenerator] Spriteロード失敗: {resourcePath}");
            }
        }
    }
}