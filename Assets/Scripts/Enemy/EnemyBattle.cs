using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyBattle : MonoBehaviour
    {
        // �_���[�W��^�����G�L�����N�^�[��ID���i�[���郊�X�g
        private List<int> damageEnemyIDs = new List<int>();

        /// <summary>
        /// �v���C���[�Ƀ_���[�W��^�����G��ID���擾
        /// </summary>
        /// <param name="enemyID">�_���[�W��^�����G��ID</param>
        public void RecordDamage(int enemyID)
        {
            if (!damageEnemyIDs.Contains(enemyID))
            {
                damageEnemyIDs.Add(enemyID);
                Debug.Log($"Enemy ID {enemyID} dealt damage.");
            }
        }

        /// <summary>
        /// �S�Ẵ_���[�W��^�����GID�̃��X�g���擾���܂�
        /// </summary>
        /// <returns>�_���[�W��^�����GID�̃��X�g</returns>
        public List<int> GetDamageEnemyIDs()
        {
            return damageEnemyIDs;
        }

        /// <summary>
        /// �w�肵���GID���v���C���[�Ƀ_���[�W��^�������m�F
        /// </summary>
        /// <param name="enemyID">�m�F�������GID</param>
        /// <returns>�_���[�W��^�����ꍇ��true�A�����łȂ��ꍇ��false</returns>
        public bool DidEnemyDealDamage(int enemyID)
        {
            return damageEnemyIDs.Contains(enemyID);
        }

        /// <summary>
        /// �_���[�W�����N���A
        /// </summary>
        public void ClearDamageInfo()//�_���[�W��񂪈ꎞ�I�ɕK�v�Ȃ�g���@�^�����_���[�W�̓GID�̏�񂪂����ƕK�v�ł���̂ł���Ώ�����

        {
            damageEnemyIDs.Clear();�@
        }
        
    }
}

