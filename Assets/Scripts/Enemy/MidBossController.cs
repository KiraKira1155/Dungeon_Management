using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class MidBossController
    {
        [SerializeField] private float detectionRadiusForPlayer; // プレイヤー検出半径
        [SerializeField] private float detectionRadiusForEnemy; // 敵検出半径
        [SerializeField] private float repulsionForceForPlayer; // プレイヤーに対する反発力
        [SerializeField] private float repulsionForceForEnemy; // 敵に対する反発力
        [SerializeField] private float repulsionTime;
        [SerializeField] private LayerMask midBossLayer;

        private Collider2D hitEntity;
        private Vector3 repulsionVector;
        private Vector3 directionVector;
        private Transform midBossTransform;

        public void Init()
        {
            midBossTransform = EnemySpawnManager.I.GetMidBossEnemy().GetEnemyTransform();
        }

        /// <summary>
        /// 中ボスの移動とプレイヤーと雑魚敵の当たり判定を行う。
        /// </summary>
        public void DetectAndRepel()
        {
            MidBossMoveCalculation();
            DetectEnemies();
            DetectPlayer();
        }

        /// <summary>
        /// プレイヤーの検出用
        /// </summary>
        private void DetectPlayer()
        {
            hitEntity = CheckOverlapCirclePlayer();
            if (hitEntity != null)
            {
                EnemySpawnManager.I.GetMidBossEnemy().HitPlayer();
                repulsionVector = (hitEntity.transform.position - midBossTransform.position).normalized;

                if (EnemySpawnManager.I.GetMidBossEnemy().IsAttack())
                {
                    Vector3 targetPosition = hitEntity.transform.position + (repulsionVector * repulsionForceForPlayer);
                    hitEntity.transform.DOMove(targetPosition, repulsionTime).SetEase(Ease.OutQuad);
                }
                else
                {
                    hitEntity.transform.position += (repulsionVector * repulsionForceForPlayer / 10);
                }
            }
        }

        /// <summary>
        /// 敵の検出用
        /// </summary>
        private void DetectEnemies()
        {
            hitEntity = CheckOverlapCircleEnemy();
            if (hitEntity != null)
            {
                repulsionVector = (hitEntity.transform.position - midBossTransform.position).normalized;
                hitEntity.transform.position += (repulsionVector * repulsionForceForEnemy);
            }
        }

        private Collider2D CheckOverlapCirclePlayer()
        {
            return Physics2D.OverlapCircle(midBossTransform.position, detectionRadiusForPlayer, Player.PlayerManager.I.GetPlayerLayer());
        }

        private Collider2D CheckOverlapCircleEnemy()
        {
            return Physics2D.OverlapCircle(midBossTransform.position, detectionRadiusForEnemy, EnemyManager.I.enemyController.GetEnemyLayer());
        }

        /// <summary>
        /// 中ボスの移動計算
        /// </summary>
        public void MidBossMoveCalculation()
        {
            switch (EnemySpawnManager.I.GetMidBossEnemy().CanAttack())
            {
                case false:
                    switch (CheckOverlapCirclePlayer())
                    {
                        case null:
                            directionVector = (PlayerManager.I.GetPlayerObj().transform.position - midBossTransform.position).normalized;
                            midBossTransform.position += directionVector * EnemyManager.I.basicStatus.GetEnemyStatusMoveSpeed(EnemySpawnManager.I.GetMidBossEnemy().GetEnemyID()) * 0.02f;
                            break;

                        default:

                            break;
                    }
                    break;

                default:
                    switch (CheckOverlapCirclePlayer())
                    {
                        case null:
                            directionVector = (PlayerManager.I.GetPlayerObj().transform.position - midBossTransform.position).normalized;
                            midBossTransform.position += directionVector * EnemyManager.I.basicStatus.GetEnemyStatusMoveSpeed(EnemySpawnManager.I.GetMidBossEnemy().GetEnemyID()) * 0.02f;
                            break;

                        default:

                            break;
                    }
                    break;
            }
        }

        public LayerMask GetMidBossLayer()
        {
            return midBossLayer;
        }
    }
}