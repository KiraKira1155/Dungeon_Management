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
        /// キャラクターの名前取得用
        /// </summary>
        /// <param name="id">キャラクターの識別ID</param>
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
        /// キャラクターの基本HP取得用
        /// </summary>
        /// <param name="id">キャラクターの識別ID</param>
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
        /// キャラクターの基本攻撃力取得用
        /// </summary>
        /// <param name="id">キャラクターの識別ID</param>
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
        /// キャラクターの基本攻撃間隔取得用
        /// </summary>
        /// <param name="id">キャラクターの識別ID</param>
        /// <returns>攻撃が発動してから次の攻撃が発動するまでのインターバルタイムが返る</returns>
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
        /// キャラクターの基本移動スピード取得用
        /// </summary>
        /// <param name="id">キャラクターの識別ID</param>
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
        /// キャラクターの基本ステータスの全データ取得用
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