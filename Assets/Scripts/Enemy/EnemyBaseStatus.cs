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
        /// �G�L�����N�^�[�̖��O�̎擾
        /// </summary>
        /// <param name="id">�G�L�����N�^�[�̎���ID</param>
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
        /// �G�L�����N�^�[�̊�b�̗͂̎擾
        /// </summary>
        /// <param name="id">�G�L�����N�^�[�̎���ID</param>
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
        /// �G�L�����N�^�[�̍U���͂̎擾
        /// </summary>
        /// <param name="id">�G�L�����N�^�[�̎���ID</param>
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
        /// �G�L�����N�^�[�̌o���l�̎擾
        /// </summary>
        /// <param name="id">�G�L�����N�^�[�̎���ID</param>
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
        /// �G�L�����N�^�[�̃_���[�W����̎擾
        /// </summary>
        /// <param name="id">�G�L�����N�^�[�̎���ID</param>
        /// <returns>�_���[�W��^���āA���̃_���[�W������܂ł̎��Ԃ�Ԃ�</returns>
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
        /// �G�L�����N�^�[�̈ړ��X�s�[�h�̎擾
        /// </summary>
        /// <param name="id">�G�L�����N�^�[�̎���ID</param>
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
        /// �v���C���[�̌��o�͈͂̎擾
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

