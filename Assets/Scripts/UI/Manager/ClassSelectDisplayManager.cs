// ======================================================
// ClassSelectDisplayManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : クラス選択キャンバス管理クラス
//             クラスボタン押下で確認パネルを表示し、選択されたクラスをPickSceneに渡す
// ======================================================

using CardGame.CardSystem.Data;
using CardGame.CardSystem.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.GameSystem.Manager
{
    /// <summary>
    /// クラス選択キャンバスの管理クラス
    /// クラスボタン押下に応じて確認パネルを表示
    /// </summary>
    public class ClassSelectDisplayManager : MonoBehaviour
    {
        // ======================================================
        // 構造体
        // ======================================================

        /// <summary>
        /// クラスボタンとクラス種別の対応
        /// </summary>
        [Serializable]
        private struct ClassButtonData
        {
            public Button Button;             
            public CardData.CardClass Class;
        }

        // ======================================================
        // インスペクタ設定
        // ======================================================

        [Header("クラスボタン")]
        [SerializeField]
        /// <summary>8クラスのボタン配列</summary>
        private ClassButtonData[] classButtonDataArray;

        [Header("確認パネル")]
        [SerializeField]
        /// <summary>選択確認用パネル</summary>
        private ConfirmPanelController confirmPanel;

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>カードデータベース（最大枚数管理用）</summary>
        private CardDatabase _cardDatabase;

        /// <summary>カード生成と表示を管理するコントローラ</summary>
        private ClassCardDisplayController _cardDisplayController = new ClassCardDisplayController();

        /// <summary>選択中のクラスID</summary>
        private int _selectedClassIndex = -1;

        // ======================================================
        // 辞書
        // ======================================================

        /// <summary>クラス列挙値からカタカナ名への変換辞書</summary>
        private readonly Dictionary<CardData.CardClass, string> ClassNameMap = new Dictionary<CardData.CardClass, string>()
        {
            { CardData.CardClass.Neutral, "ニュートラル" },
            { CardData.CardClass.Elf, "エルフ" },
            { CardData.CardClass.Royal, "ロイヤル" },
            { CardData.CardClass.Witch, "ウィッチ" },
            { CardData.CardClass.Dragon, "ドラゴン" },
            { CardData.CardClass.Nightmare, "ナイトメア" },
            { CardData.CardClass.Bishop, "ビショップ" },
            { CardData.CardClass.Nemesis, "ネメシス" }
        };
        
        // ======================================================
        // Unityイベント
        // ======================================================

        private void Start()
        {
            CardDatabaseManager manager = CardDatabaseManager.Instance;
            if (manager == null)
            {
                Debug.LogError("CardDatabaseManagerが存在しません。");
                return;
            }

            _cardDatabase = manager.GetCardDatabase();

            // ここで初回抽選
            UpdateClassCards();

            // ボタン押下イベント登録
            for (int i = 0; i < classButtonDataArray.Length; i++)
            {
                int index = i;
                classButtonDataArray[i].Button.onClick.AddListener(() => OnClassButtonClicked(index));
            }

            confirmPanel.Initialize(OnConfirmOk, OnConfirmBack);
        }

        private void OnEnable()
        {
            if (_cardDatabase != null)
            {
                UpdateClassCards();
            }
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// 各クラスボタン上のカード画像を更新
        /// </summary>
        private void UpdateClassCards()
        {
            foreach (ClassButtonData btnData in classButtonDataArray)
            {
                if (btnData.Button != null)
                {
                    _cardDisplayController.GenerateCards(btnData.Button, btnData.Class, _cardDatabase);
                }
            }
        }

        /// <summary>
        /// クラスボタン押下時処理
        /// </summary>
        /// <param name="classIndex">選択されたクラスのインデックス</param>
        private void OnClassButtonClicked(int classIndex)
        {
            _selectedClassIndex = classIndex;

            if (classIndex < 0 || classIndex >= classButtonDataArray.Length)
            {
                Debug.LogError($"不正なクラスインデックス: {classIndex}");
                return;
            }
            
            string className = ClassNameMap.ContainsKey(classButtonDataArray[classIndex].Class)
                ? ClassNameMap[classButtonDataArray[classIndex].Class]
                : "不明";

            confirmPanel.Show($"{className} を選択しますか？");
        }

        /// <summary>
        /// 確認パネルのOKボタン押下時
        /// PickSceneへ遷移
        /// </summary>
        private void OnConfirmOk()
        {
            confirmPanel.Hide();
        }

        /// <summary>
        /// 確認パネルのBACKボタン押下時
        /// パネルを閉じる
        /// </summary>
        private void OnConfirmBack()
        {
            confirmPanel.Hide();
        }
    }
}
