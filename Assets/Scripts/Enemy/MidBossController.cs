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
        [SerializeField] private float detectionRadiusForPlayer; // �v���C���[���o���a
        [SerializeField] private float detectionRadiusForEnemy; // �G���o���a
        [SerializeField] private float repulsionForceForPlayer; // �v���C���[�ɑ΂��锽����
        [SerializeField] private float repulsionForceForEnemy; // �G�ɑ΂��锽����
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
        /// ���{�X�̈ړ��ƃv���C���[�ƎG���G�̓����蔻����s���B
        /// </summary>
        public void DetectAndRepel()
        {
            MidBossMoveCalculation();
            DetectEnemies();
            DetectPlayer();
        }

        /// <summary>
        /// �v���C���[�̌��o�p
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
        /// �G�̌��o�p
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
        /// ���{�X�̈ړ��v�Z
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