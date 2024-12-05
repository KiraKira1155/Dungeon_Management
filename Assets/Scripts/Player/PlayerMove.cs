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
        /// キャラクターの移動の初期設定
        /// </summary>
        /// <param name="moveSpeed">基本の移動速度</param>
        public PlayerMove(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }

        /// <summary>
        /// Update関数内で呼ぶ
        /// </summary>
        public void DoUpdate()
        {
        }

        /// <summary>
        /// FixedUpdate関数内で呼ぶ
        /// </summary>
        public void DoFixedUpdate()
        {
            PlayerMoveCalculation();
        }

        /// <summary>
        /// プレイヤーの移動用
        /// </summary>
        public void PlayerMoveCalculation()
        {
            // プレイヤーの移動処理
            Vector3 newPosition = PlayerManager.I.GetPlayerTransform().position + new Vector3(
                JoyStickManager.I.GetInputDirection().x * moveSpeed / MOVE_SPEED_ADJUSTMENT * ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.moveSpeedUP),
                JoyStickManager.I.GetInputDirection().y * moveSpeed / MOVE_SPEED_ADJUSTMENT * ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.moveSpeedUP),
                0.0f
            );

            // 移動範囲制限を適用
            newPosition = PlayerManager.I.LimitPlayerPosition(newPosition);

            var previusPos = PlayerManager.I.GetPlayerTransform().position;

            // 新しい位置を適用
            PlayerManager.I.GetPlayerTransform().position = newPosition;

            foreach (var hitEnemy in CheckOverlapCircleEnemy())
            {
                //敵を反発
                repulsionVector = (PlayerManager.I.GetPlayerTransform().position - hitEnemy.transform.position) * PlayerManager.I.GetRepulsionCoefficientForEnemy();
                hitEnemy.transform.position -= repulsionVector;

                repulsionVector = (hitEnemy.transform.position - PlayerManager.I.GetPlayerTransform().position) * PlayerManager.I.GetRepulsionCoefficient();
                Enemy.EnemySpawnManager.I.GetMapEnemy(hitEnemy.gameObject).HitPlayer();
                PlayerManager.I.GetPlayerTransform().position -= repulsionVector;
            }
        }

        /// <summary>
        /// 当たり判定基本部分
        /// </summary>
        /// <returns></returns>
        private Collider2D[] CheckOverlapCircleEnemy()
        {
            var hit = Physics2D.OverlapCircleAll(PlayerManager.I.GetPlayerTransform().position, PlayerManager.I.GetDetectionRadius(), Enemy.EnemyManager.I.enemyController.GetEnemyLayer());
            return hit;
        }
    }
}
