using DG.Tweening;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class BossController
    {
        [SerializeField] public BossBarController bossBarController = new BossBarController();

        [SerializeField] private float detectionRadiusForPlayer; // �v���C���[���o���a
        [SerializeField] private float repulsionCoefficient;   // �����W��
        [SerializeField] private LayerMask bossLayer;
        [SerializeField] private float repulsionBossTime;

        public Vector2 moveLimitsMin { get; private set; }
        public Vector2 moveLimitsMax { get; private set; }

        private Collider2D hitEntity;
        private Vector3 directionVector;
        private Vector3 repulsionVector;
        private UnityEngine.Transform bossTransform;


        public void Init()
        {
            bossTransform = EnemySpawnManager.I.GetBossEnemy().GetEnemyTransform();
        }

        /// <summary>
        /// �{�X�̍s���͈͂�ݒ肷��
        /// </summary>
        /// <param name="centerPosition">�͈͂̒��S�ʒu</param>
        public void SetMoveLimits(Vector3 centerPosition)
        {
            moveLimitsMin = new Vector2(centerPosition.x - 6.5f, centerPosition.y - 8.5f);
            moveLimitsMax = new Vector2(centerPosition.x + 6.5f, centerPosition.y + 8.5f);
        }

        /// <summary>
        /// �{�X�̈ړ��ƃv���C���[�Ƃ̓����蔻����s��
        /// </summary>
        public void BossMoveCalculation()
        {
            MoveTowardsPlayer();
            DetectPlayer();
        }

        private void DetectPlayer()
        {
            hitEntity = CheckOverlapCirclePlayer();
            if (hitEntity != null)
            {
                EnemySpawnManager.I.GetBossEnemy().HitPlayer();
                repulsionVector = (hitEntity.transform.position - bossTransform.position).normalized;

                if (EnemySpawnManager.I.GetBossEnemy().IsAttack())
                {
                    Vector3 targetPosition = hitEntity.transform.position + (repulsionVector * repulsionCoefficient);
                    hitEntity.transform.DOMove(targetPosition, repulsionBossTime).SetEase(Ease.OutQuad);
                }
                else
                {
                    hitEntity.transform.position += (repulsionVector * repulsionCoefficient / 5);
                }

            }
        }

        private Collider2D CheckOverlapCirclePlayer()
        {
            return Physics2D.OverlapCircle(bossTransform.position, detectionRadiusForPlayer, Player.PlayerManager.I.GetPlayerLayer());
        }

        

        /// <summary>
        /// �v���C���[�̕����Ɉړ�
        /// </summary>
        private void MoveTowardsPlayer()
        {
            directionVector = (Player.PlayerManager.I.GetPlayerObj().transform.position - bossTransform.position).normalized;
            Vector3 newPosition = bossTransform.position + directionVector * EnemyManager.I.basicStatus.GetEnemyStatusMoveSpeed(EnemySpawnManager.I.GetBossEnemy().GetEnemyID()) * 0.02f;

            newPosition = new Vector3(
                    Mathf.Clamp(newPosition.x, moveLimitsMin.x, moveLimitsMax.x),
                    Mathf.Clamp(newPosition.y, moveLimitsMin.y, moveLimitsMax.y),
                    newPosition.z
                );

            bossTransform.position = newPosition;
        }

        public LayerMask GetBossLayer()
        {
            return bossLayer;
        }
    }
}