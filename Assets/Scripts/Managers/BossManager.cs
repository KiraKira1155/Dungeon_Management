using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BossManager : Singleton<BossManager>
    {
        private Vector2 moveLimitsMin;
        private Vector2 moveLimitsMax;
        private bool isMoveLimitActive = false;
        private BossController bossController;

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// ボスの行動範囲を設定する
        /// </summary>
        /// <param name="centerPosition">範囲の中心位置</param>
        public void SetMoveLimits(Vector3 centerPosition)
        {
            moveLimitsMin = new Vector2(centerPosition.x - 7.5f, centerPosition.y - 10.0f);
            moveLimitsMax = new Vector2(centerPosition.x + 7.5f, centerPosition.y + 10.0f);
            isMoveLimitActive = true;
        }

        /// <summary>
        /// ボスの行動範囲制限を無効化する
        /// </summary>
        public void DisableMoveLimits()
        {
            isMoveLimitActive = false;
        }
        

        /// <summary>
        /// ボスの位置を制限する
        /// </summary>
        /// <param name="position">新しいボスの位置</param>
        /// <returns>制限されたボスの位置</returns>
        public Vector3 LimitBossPosition(Vector3 position)
        {
            if (isMoveLimitActive)
            {
                position.x = Mathf.Clamp(position.x, moveLimitsMin.x, moveLimitsMax.x);
                position.y = Mathf.Clamp(position.y, moveLimitsMin.y, moveLimitsMax.y);
            }
            return position;
        }

        // BossControllerを初期化
        public void InitBossController(BossController controller)
        {
            bossController = controller;
        }

        // BossControllerを取得するメソッド
        public BossController GetBossController()
        {
            return bossController;
        }
    }
}
