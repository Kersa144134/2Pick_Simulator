// ======================================================
// CardData.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-10-30
// 更新日時   : 2025-10-30
// 概要       : カードの基本情報を保持する ScriptableObject クラス
//             カードIDからクラス・パック・レアリティを自動判別
// ======================================================

using UnityEngine;

namespace CardGame.CardSystem.Data
{
    /// <summary>
    /// カード1枚分の基本情報を定義する ScriptableObject クラス
    /// ID構造に基づいてクラス・パック・レアリティを分離可能
    /// </summary>
    [CreateAssetMenu(fileName = "CardData", menuName = "CardGame/Card Data")]
    public class CardData : ScriptableObject
    {
        // ======================================================
        // 列挙型
        // ======================================================

        /// <summary>
        /// カードの所属クラス
        /// 0:ニュートラル, 1:エルフ, 2:ロイヤル, 3:ウィッチ,
        /// 4:ドラゴン, 5:ナイトメア, 6:ビショップ, 7:ネメシス
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
        /// カードのレアリティ
        /// 0:ブロンズ, 1:シルバー, 2:ゴールド, 3:レジェンド
        /// </summary>
        public enum CardRarity
        {
            Bronze = 0,
            Silver = 1,
            Gold = 2,
            Legend = 3
        }

        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>
        /// カード画像（立ち絵やアートなど）  
        /// UI表示に直接使用可能なSpriteを保持する
        /// </summary>
        public Sprite CardImage = null;

        /// <summary>
        /// カードを一意に識別する5桁整数ID
        /// 例：21342 → ロイヤル / パック13 / ゴールド / 番号2
        /// </summary>
        [SerializeField]
        private int cardId = 0;

        /// <summary>
        /// カードのコスト
        /// </summary>
        [SerializeField, Min(0)]
        private int cardCost = 0;

        /// <summary>
        /// カードの表示名
        /// </summary>
        [SerializeField]
        private string cardName = string.Empty;

        /// <summary>
        /// カードの効果テキストまたは説明文
        /// </summary>
        [SerializeField, TextArea(2, 5)]
        private string cardText = string.Empty;

        /// <summary>
        /// デッキに編成できる最大枚数
        /// </summary>
        [SerializeField, Range(0, 3)]
        private int maxCopies = 3;

        // ======================================================
        // プロパティ
        // ======================================================

        /// <summary>カードIDを取得</summary>
        public int CardId
        {
            get { return cardId; }
        }

        /// <summary>カードコストを取得</summary>
        public int CardCost
        {
            get { return cardCost; }
        }

        /// <summary>カード名を取得</summary>
        public string CardName
        {
            get { return cardName; }
        }

        /// <summary>カードテキストを取得</summary>
        public string CardText
        {
            get { return cardText; }
        }

        /// <summary>最大枚数を取得</summary>
        public int MaxCopies
        {
            get { return maxCopies; }
        }

        /// <summary>カードの所属クラスをIDから算出して取得</summary>
        public CardClass ClassType
        {
            get { return (CardClass)(cardId / 10000); }
        }

        /// <summary>カードの属するパック番号（2桁）をIDから取得</summary>
        public int PackNumber
        {
            get { return (cardId / 100) % 100; }
        }

        /// <summary>カードのレアリティをIDから算出して取得</summary>
        public CardRarity Rarity
        {
            get { return (CardRarity)((cardId / 10) % 10); }
        }

        /// <summary>カードIDの末尾番号（重複防止用）を取得</summary>
        public int SubId
        {
            get { return cardId % 10; }
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// カード情報を整形してコンソール出力（デバッグ用）
        /// </summary>
        public void PrintCardInfo()
        {
            // 解析した各情報を出力してデータ確認を容易にする
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