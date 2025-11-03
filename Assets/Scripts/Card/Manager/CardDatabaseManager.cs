// ======================================================
// CardDatabaseManager.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : カードデータベースをシーン間で共有するシングルトン
// ======================================================

using UnityEngine;
using CardGame.CardSystem.Data;
using CardGame.CardSystem.Utility;

namespace CardGame.CardSystem.Manager
{
    /// <summary>
    /// シングルトンとして動作するカードデータベース管理クラス  
    /// ゲーム全体でカードデータを共有する
    /// </summary>
    public class CardDatabaseManager : MonoBehaviour
    {
        // ======================================================
        // 定数
        // ======================================================

        /// <summary>シングルトンの名前</summary>
        private const string SINGLETON_NAME = "CardDatabaseManager";

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>インスタンス参照</summary>
        public static CardDatabaseManager Instance { get; private set; }

        /// <summary>カードデータベース</summary>
        private CardDatabase _cardDatabase;

        /// <summary>カードデータロード用ヘルパー</summary>
        private CardDataLoader _loader;

        // ======================================================
        // Unityイベント
        // ======================================================

        /// <summary>
        /// Awake処理でシングルトン初期化とデータロード
        /// </summary>
        private void Awake()
        {
            // すでに存在する場合は自分を破棄
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // シングルトン設定
            Instance = this;
            gameObject.name = SINGLETON_NAME;

            // シーンを跨いで保持
            DontDestroyOnLoad(gameObject);

            // カードデータを初期化
            InitializeDatabase();
        }

        // ======================================================
        // プライベートメソッド
        // ======================================================

        /// <summary>
        /// CardDataLoaderを使ってカードデータをロードし、CardDatabaseを生成
        /// </summary>
        private void InitializeDatabase()
        {
            _loader = new CardDataLoader();
            _loader.LoadAllCardData();
            _cardDatabase = new CardDatabase(_loader.AllCardData);
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// CardDatabaseを取得する
        /// </summary>
        public CardDatabase GetCardDatabase()
        {
            return _cardDatabase;
        }

        /// <summary>
        /// CardDataLoaderを取得する（必要に応じて）
        /// </summary>
        public CardDataLoader GetCardDataLoader()
        {
            return _loader;
        }
    }
}