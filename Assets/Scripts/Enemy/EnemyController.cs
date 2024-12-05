using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class EnemyController
    {
        [SerializeField] private float repulsionCoefficient; //�����W��
        [SerializeField] private LayerMask enemyLayer;

        private Vector3 directionVector;
        private Vector3 repulsionVector;

        /// <summary>
        /// �G�̈ړ��p�֐�
        /// </summary>
        /// <param name="enemy">�������Ώۂ� IBaseEnemy ������</param>
        public void EnemyMoveCalculation(IBaseEnemy enemy, float deltaTime)
        {
            var slowSpeedMagnification = 1.0f;
            if (enemy.GetIsSlowSpeed())
                slowSpeedMagnification = 0.5f;

            // �����ړ�
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

            // �d�͂̉e�����󂯂鏈��
            if (enemy.GetGravityEffect())
            {
                // �d�͏�̈ʒu�ƓG�̈ʒu�Ƃ̋������v�Z
                //float distance = Vector2.Distance(SubWeapon.SubWeaponManager.I.GetGravityPos(), enemy.GetEnemyTransform().position);

                // �����񂹂�͂��v�Z�i�������߂��قǋ����Ȃ�j
                Vector2 pullDirection = (SubWeapon.SubWeaponManager.I.GetGravityPos() - (Vector2)enemy.GetEnemyTransform().position).normalized;
                //float pullStrength = gravityStrength / (distance * distance); // �����ɔ���Ⴗ���

                // �͂̋����𒲐����Ĉ����񂹂�
                float adjustedPullStrength = SubWeapon.SubWeaponManager.I.GetGravityStrength() * deltaTime; // �����񂹂鑬�x�𒲐�
                enemy.GetEnemyTransform().position = Vector2.MoveTowards(enemy.GetEnemyTransform().position, SubWeapon.SubWeaponManager.I.GetGravityPos(), adjustedPullStrength);
            }
        }

        /// <summary>
        /// �����蔻���{����
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns>�Փ˂��Ă��� Collider2D ���Ԃ�A���g�� Collider2D ���Ԃ�̂Œ���</returns>
        private Collider2D[] CheckOverlapCircle(IBaseEnemy enemy)
        {
            var hit = Physics2D.OverlapCircleAll(enemy.GetEnemyTransform().position, EnemyManager.I.basicStatus.GetEnemyStatusDetectionRadius(enemy.GetEnemyID()), enemyLayer);
            return hit;
        }

        /// <summary>
        /// �G�l�~�[�̃��C���[
        /// </summary>
        /// <returns></returns>
        public LayerMask GetEnemyLayer()
        {
            return enemyLayer;
        }
    }
}
