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
        /// �L�����N�^�[�̃I�[�g�U���̏����ݒ�
        /// </summary>
        /// <param name="attackInterval">��{�̍U���Ԋu</param>
        public PlayerAutoAttack(float attackInterval)
        {
            this.attackInterval = attackInterval;
        }

        public void DoUpdate()
        {
            AutoAttackIntervalCount();                                                                                                                                                                                                                                                                                                                                                                 
        }

        /// <summary>
        /// �U�����s���Ă��邩�̊m�F�p
        /// </summary>
        /// <returns>�U�����s���Ă���� true ���Ԃ�</returns>
        public bool GetIsAttack()
        {
            return isAttack;
        }

        /// <summary>
        /// �G�ɍU���������������ɌĂяo����
        /// </summary>
        public void HitAttack()
        {
            isAttack = true;
        }

        /// <summary>
        /// �I�[�g�U�������I���Ăяo���A
        /// </summary>
        public void EndAutoAttackProcess()
        {
            PlayerManager.I.GetPlayerAutoAttackCollider().HitEnemyInfoClear();
            isAttack = false;
        }

        /// <summary>
        /// �v���C���[�̃I�[�g�U���̊Ԋu���擾
        /// </summary>
        /// <returns></returns>
        public float GetAttackInterval()
        {
            return attackInterval;
        }

        /// <summary>
        /// �v���C���[�̃I�[�g�U�����Ă���̌o�ߎ��Ԃ��擾
        /// </summary>
        /// <returns></returns>
        public float GetIntervalCount()
        {
            return intervalCount;
        }

        /// <summary>
        /// �C���^�[�o���̎��Ԃ��v�Z
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
