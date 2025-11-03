// ======================================================
// CardFilterGroupController.cs
// 作成者     : 高橋一翔
// 作成日時   : 2025-11-03
// 更新日時   : 2025-11-03
// 概要       : フィルターグループ（クラス/パック/レアリティ/コスト等）を
//             一括管理・表示切替する制御クラス
// ======================================================

using UnityEngine;

namespace CardGame.UISystem.Controller
{
    /// <summary>
    /// フィルターグループ全体を統括してアクティブ制御を行うクラス  
    /// 各種UIグループの ON / OFF を一括トグルまたは明示的に設定可能
    /// </summary>
    public class CardFilterGroupController
    {
        // ======================================================
        // フィールド
        // ======================================================

        /// <summary>管理対象のフィルターグループ配列</summary>
        private readonly GameObject[] _filterGroups;

        // ======================================================
        // コンストラクタ
        // ======================================================

        /// <summary>
        /// フィルターグループ制御クラスのインスタンスを生成
        /// </summary>
        /// <param name="groups">対象となるGameObject配列</param>
        public CardFilterGroupController(GameObject[] groups)
        {
            _filterGroups = groups;
        }

        // ======================================================
        // パブリックメソッド
        // ======================================================

        /// <summary>
        /// フィルターグループを一括でトグル表示  
        /// （1つでもONなら全OFF、全OFFなら全ON）
        /// </summary>
        public void ToggleAllGroups()
        {
            if (_filterGroups == null || _filterGroups.Length == 0)
            {
                Debug.LogWarning("[CardFilterGroupController] 管理対象のグループが設定されていません");
                return;
            }

            // --------------------------------------------------
            // 現在どれかがアクティブかをチェック
            // --------------------------------------------------
            bool anyActive = false;
            foreach (var group in _filterGroups)
            {
                if (group != null && group.activeSelf)
                {
                    anyActive = true;
                    break;
                }
            }

            // --------------------------------------------------
            // 状態を反転して一括設定
            // --------------------------------------------------
            SetAllGroupsActive(!anyActive);
        }

        /// <summary>
        /// すべてのグループを指定状態に変更
        /// </summary>
        /// <param name="isActive">有効化するかどうか</param>
        public void SetAllGroupsActive(bool isActive)
        {
            if (_filterGroups == null) return;

            foreach (var group in _filterGroups)
            {
                if (group == null) continue;
                group.SetActive(isActive);
            }
        }
    }
}