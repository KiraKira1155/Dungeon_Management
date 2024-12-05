using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAutoAttack
    {
        private float attackInterval;
        private float intervalCount;
        private bool isAttack;

        /// <summary>
        /// キャラクターのオート攻撃の初期設定
        /// </summary>
        /// <param name="attackInterval">基本の攻撃間隔</param>
        public PlayerAutoAttack(float attackInterval)
        {
            this.attackInterval = attackInterval;
        }

        public void DoUpdate()
        {
            AutoAttackIntervalCount();                                                                                                                                                                                                                                                                                                                                                                 
        }

        /// <summary>
        /// 攻撃を行っているかの確認用
        /// </summary>
        /// <returns>攻撃を行っていれば true が返る</returns>
        public bool GetIsAttack()
        {
            return isAttack;
        }

        /// <summary>
        /// 敵に攻撃が当たった時に呼び出だす
        /// </summary>
        public void HitAttack()
        {
            isAttack = true;
        }

        /// <summary>
        /// オート攻撃処理終了呼び出し、
        /// </summary>
        public void EndAutoAttackProcess()
        {
            PlayerManager.I.GetPlayerAutoAttackCollider().HitEnemyInfoClear();
            isAttack = false;
        }

        /// <summary>
        /// プレイヤーのオート攻撃の間隔を取得
        /// </summary>
        /// <returns></returns>
        public float GetAttackInterval()
        {
            return attackInterval;
        }

        /// <summary>
        /// プレイヤーのオート攻撃してからの経過時間を取得
        /// </summary>
        /// <returns></returns>
        public float GetIntervalCount()
        {
            return intervalCount;
        }

        /// <summary>
        /// インターバルの時間を計算
        /// </summary>
        private void AutoAttackIntervalCount()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
                intervalCount += Time.deltaTime;

            if(attackInterval <= intervalCount)
            {
                PlayerManager.I.GetPlayerAutoAttackCollider().AutoAttackTargetCalculation();
                intervalCount = 0;

                if(PlayerManager.I.GetPlayerAutoAttackCollider().GetHitEnemyAmount() != 0)
                    HitAttack();
            }
        }
    }
}
