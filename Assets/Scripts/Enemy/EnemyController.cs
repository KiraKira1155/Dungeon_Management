using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class EnemyController
    {
        [SerializeField] private float repulsionCoefficient; //反発係数
        [SerializeField] private LayerMask enemyLayer;

        private Vector3 directionVector;
        private Vector3 repulsionVector;

        /// <summary>
        /// 敵の移動用関数
        /// </summary>
        /// <param name="enemy">動かす対象の IBaseEnemy が引数</param>
        public void EnemyMoveCalculation(IBaseEnemy enemy, float deltaTime)
        {
            var slowSpeedMagnification = 1.0f;
            if (enemy.GetIsSlowSpeed())
                slowSpeedMagnification = 0.5f;

            // 反発移動
            foreach (var hitEnemy in CheckOverlapCircle(enemy))
            {
                if (hitEnemy != enemy.GetEnemyCollider())
                {
                    repulsionVector = (hitEnemy.gameObject.transform.position - enemy.GetEnemyTransform().position) * repulsionCoefficient * slowSpeedMagnification;

                    enemy.GetEnemyTransform().position -= repulsionVector;
                }
                else
                {
                    directionVector = (Player.PlayerManager.I.GetPlayerObj().transform.position - enemy.GetEnemyTransform().position).normalized;
                    enemy.GetEnemyTransform().position += directionVector * EnemyManager.I.basicStatus.GetEnemyStatusMoveSpeed(enemy.GetEnemyID()) * deltaTime * slowSpeedMagnification;
                }
            }

            // 重力の影響を受ける処理
            if (enemy.GetGravityEffect())
            {
                // 重力場の位置と敵の位置との距離を計算
                //float distance = Vector2.Distance(SubWeapon.SubWeaponManager.I.GetGravityPos(), enemy.GetEnemyTransform().position);

                // 引き寄せる力を計算（距離が近いほど強くなる）
                Vector2 pullDirection = (SubWeapon.SubWeaponManager.I.GetGravityPos() - (Vector2)enemy.GetEnemyTransform().position).normalized;
                //float pullStrength = gravityStrength / (distance * distance); // 距離に反比例する力

                // 力の強さを調整して引き寄せる
                float adjustedPullStrength = SubWeapon.SubWeaponManager.I.GetGravityStrength() * deltaTime; // 引き寄せる速度を調整
                enemy.GetEnemyTransform().position = Vector2.MoveTowards(enemy.GetEnemyTransform().position, SubWeapon.SubWeaponManager.I.GetGravityPos(), adjustedPullStrength);
            }
        }

        /// <summary>
        /// 当たり判定基本部分
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns>衝突している Collider2D が返る、自身の Collider2D も返るので注意</returns>
        private Collider2D[] CheckOverlapCircle(IBaseEnemy enemy)
        {
            var hit = Physics2D.OverlapCircleAll(enemy.GetEnemyTransform().position, EnemyManager.I.basicStatus.GetEnemyStatusDetectionRadius(enemy.GetEnemyID()), enemyLayer);
            return hit;
        }

        /// <summary>
        /// エネミーのレイヤー
        /// </summary>
        /// <returns></returns>
        public LayerMask GetEnemyLayer()
        {
            return enemyLayer;
        }
    }
}
