using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class EnemyBaseStatus
    {
        [SerializeField] private EnemyStatusAttributes[] enemyStatus;

        /// <summary>
        /// 敵キャラクターの名前の取得
        /// </summary>
        /// <param name="id">敵キャラクターの識別ID</param>
        /// <returns></returns>
        public string GetEnemyStatusName(EnemyManager.EnemyCharactersID id)
        {
            foreach (EnemyStatusAttributes status in enemyStatus)
            {
                if (id == status.enemyID)
                {
                    return status.name;
                }
            }
            return null;

        }

        /// <summary>
        /// 敵キャラクターの基礎体力の取得
        /// </summary>
        /// <param name="id">敵キャラクターの識別ID</param>
        /// <returns></returns>
        public int GetEnemyStatusHP(EnemyManager.EnemyCharactersID id)
        {
            foreach (EnemyStatusAttributes status in enemyStatus)
            {
                if (id == status.enemyID)
                {
                    return status.hp;
                }
            }
            return 0;

        }

        /// <summary>
        /// 敵キャラクターの攻撃力の取得
        /// </summary>
        /// <param name="id">敵キャラクターの識別ID</param>
        /// <returns></returns>
        public int GetEnemyStatusATK(EnemyManager.EnemyCharactersID id)
        {
            foreach (EnemyStatusAttributes status in enemyStatus)
            {
                if (id == status.enemyID)
                {
                    return status.atk;
                }
            }
            return 0;

        }

        /// <summary>
        /// 敵キャラクターの経験値の取得
        /// </summary>
        /// <param name="id">敵キャラクターの識別ID</param>
        /// <returns></returns>
        public int GetEnemyStatusEXP(EnemyManager.EnemyCharactersID id)
        {
            foreach (EnemyStatusAttributes status in enemyStatus)
            {
                if (id == status.enemyID)
                {
                    return status.dropExp;
                }
            }
            return 0;
        }

        /// <summary>
        /// 敵キャラクターのダメージ判定の取得
        /// </summary>
        /// <param name="id">敵キャラクターの識別ID</param>
        /// <returns>ダメージを与えて、次のダメージが入るまでの時間を返す</returns>
        public float GetEnemyStatusAttackInterval(EnemyManager.EnemyCharactersID id)
        {
            foreach (EnemyStatusAttributes status in enemyStatus)
            {
                if (id == status.enemyID)
                {
                    return status.attackInterval;
                }
            }
            return 0;
        }

        /// <summary>
        /// 敵キャラクターの移動スピードの取得
        /// </summary>
        /// <param name="id">敵キャラクターの識別ID</param>
        /// <returns></returns>
        public float GetEnemyStatusMoveSpeed(EnemyManager.EnemyCharactersID id)
        {
            foreach (EnemyStatusAttributes status in enemyStatus)
            {
                if (id == status.enemyID)
                {
                    return status.Movespeed;
                }
            }
            return 0;
        }

        /// <summary>
        /// プレイヤーの検出範囲の取得
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public float GetEnemyStatusDetectionRadius(EnemyManager.EnemyCharactersID id)
        {

            foreach (EnemyStatusAttributes status in enemyStatus)
            {
                if (id == status.enemyID)
                {
                    return status.detectionRadius; 
                }
            }
            return 0; 
        }

    }
}

