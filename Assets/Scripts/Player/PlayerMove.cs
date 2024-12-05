using Armour;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class PlayerMove
    {
        private float moveSpeed; 
        private const float MOVE_SPEED_ADJUSTMENT = 10.0f;

        private Vector3 repulsionVector;
        private Vector3 directionVector;
        private float time;

        /// <summary>
        /// �L�����N�^�[�̈ړ��̏����ݒ�
        /// </summary>
        /// <param name="moveSpeed">��{�̈ړ����x</param>
        public PlayerMove(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }

        /// <summary>
        /// Update�֐����ŌĂ�
        /// </summary>
        public void DoUpdate()
        {
        }

        /// <summary>
        /// FixedUpdate�֐����ŌĂ�
        /// </summary>
        public void DoFixedUpdate()
        {
            PlayerMoveCalculation();
        }

        /// <summary>
        /// �v���C���[�̈ړ��p
        /// </summary>
        public void PlayerMoveCalculation()
        {
            // �v���C���[�̈ړ�����
            Vector3 newPosition = PlayerManager.I.GetPlayerTransform().position + new Vector3(
                JoyStickManager.I.GetInputDirection().x * moveSpeed / MOVE_SPEED_ADJUSTMENT * ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.moveSpeedUP),
                JoyStickManager.I.GetInputDirection().y * moveSpeed / MOVE_SPEED_ADJUSTMENT * ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.moveSpeedUP),
                0.0f
            );

            // �ړ��͈͐�����K�p
            newPosition = PlayerManager.I.LimitPlayerPosition(newPosition);

            var previusPos = PlayerManager.I.GetPlayerTransform().position;

            // �V�����ʒu��K�p
            PlayerManager.I.GetPlayerTransform().position = newPosition;

            foreach (var hitEnemy in CheckOverlapCircleEnemy())
            {
                //�G�𔽔�
                repulsionVector = (PlayerManager.I.GetPlayerTransform().position - hitEnemy.transform.position) * PlayerManager.I.GetRepulsionCoefficientForEnemy();
                hitEnemy.transform.position -= repulsionVector;

                repulsionVector = (hitEnemy.transform.position - PlayerManager.I.GetPlayerTransform().position) * PlayerManager.I.GetRepulsionCoefficient();
                Enemy.EnemySpawnManager.I.GetMapEnemy(hitEnemy.gameObject).HitPlayer();
                PlayerManager.I.GetPlayerTransform().position -= repulsionVector;
            }
        }

        /// <summary>
        /// �����蔻���{����
        /// </summary>
        /// <returns></returns>
        private Collider2D[] CheckOverlapCircleEnemy()
        {
            var hit = Physics2D.OverlapCircleAll(PlayerManager.I.GetPlayerTransform().position, PlayerManager.I.GetDetectionRadius(), Enemy.EnemyManager.I.enemyController.GetEnemyLayer());
            return hit;
        }
    }
}
