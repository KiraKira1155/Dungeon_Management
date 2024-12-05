using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player.PlayerManager;

namespace Player
{
    [System.Serializable]
    public class PlayerCharactersBasicStatus
    {
        [SerializeField] private PlayerStatusAttributes[] playerStatus;

        /// <summary>
        /// �L�����N�^�[�̖��O�擾�p
        /// </summary>
        /// <param name="id">�L�����N�^�[�̎���ID</param>
        /// <returns></returns>
        public string GetCharacterStatusName(PlayerCharactersID id)
        {
            foreach (PlayerStatusAttributes status in playerStatus)
            {
                if (id == status.characterID)
                {
                    return status.name;
                }
            }

            return null;
        }

        /// <summary>
        /// �L�����N�^�[�̊�{HP�擾�p
        /// </summary>
        /// <param name="id">�L�����N�^�[�̎���ID</param>
        /// <returns></returns>
        public int GetCharacterStatusHP(PlayerCharactersID id)
        {
            foreach (PlayerStatusAttributes status in playerStatus)
            {
                if (id == status.characterID)
                {
                    return status.hp;
                }
            }

            return 0;
        }

        /// <summary>
        /// �L�����N�^�[�̊�{�U���͎擾�p
        /// </summary>
        /// <param name="id">�L�����N�^�[�̎���ID</param>
        /// <returns></returns>
        public int GetCharacterStatusAttackDamage(PlayerCharactersID id)
        {
            foreach (PlayerStatusAttributes status in playerStatus)
            {
                if (id == status.characterID)
                {
                    return status.attackDamage;
                }
            }

            return 0;
        }

        /// <summary>
        /// �L�����N�^�[�̊�{�U���Ԋu�擾�p
        /// </summary>
        /// <param name="id">�L�����N�^�[�̎���ID</param>
        /// <returns>�U�����������Ă��玟�̍U������������܂ł̃C���^�[�o���^�C�����Ԃ�</returns>
        public float GetCharacterStatusAttackInterval(PlayerCharactersID id)
        {
            foreach (PlayerStatusAttributes status in playerStatus)
            {
                if (id == status.characterID)
                {
                    return status.attackInterval;
                }
            }

            return 0.0f;
        }

        /// <summary>
        /// �L�����N�^�[�̊�{�ړ��X�s�[�h�擾�p
        /// </summary>
        /// <param name="id">�L�����N�^�[�̎���ID</param>
        /// <returns></returns>
        public float GetCharacterStatusMoveSpeed(PlayerCharactersID id)
        {
            foreach (PlayerStatusAttributes status in playerStatus)
            {
                if (id == status.characterID)
                {
                    return status.moveSpeed;
                }
            }

            return 0.0f;
        }

        /// <summary>
        /// �L�����N�^�[�̊�{�X�e�[�^�X�̑S�f�[�^�擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PlayerStatusAttributes GetCharacterStatus(PlayerCharactersID id)
        {
            foreach (PlayerStatusAttributes status in playerStatus)
            {
                if (id == status.characterID)
                {
                    return status;
                }
            }

            return null;
        }

        public int GetNeedSkillPoint(PlayerCharactersID id)
        {
            foreach (PlayerStatusAttributes status in playerStatus)
            {
                if (id == status.characterID)
                {
                    return status.needSkiollPoint;
                }
            }

            return 0;
        }
    }

}