using Armour;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    [System.Serializable]
    public class SubWeaponsBaseStatus
    {
        [SerializeField] private SubWeaponsStatusAttributes[] subWeaponsStatus;

        public string GetName(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.name;
                }
            }
            return null;
        }

        /// <summary>
        /// �U���_���[�W�擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <param name="weaponLv">���݃��x��</param>
        /// <returns></returns>
        public int GetAttackPower(SubWeaponManager.SubWeaponID id, byte weaponLv)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    int attack = (int)((float)status.attack * (float)status.attackMagnification[weaponLv] * (float)ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.attackUP));
                    return attack;
                }
            }
            return 0;
        }

        /// <summary>
        /// �U���̓����蔻��͈͎擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <param name="weaponLv">����̌��݃��x��</param>
        /// <returns></returns>
        public float GetAttackRange(SubWeaponManager.SubWeaponID id, byte weaponLv)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    float range = status.attackRange * status.attackRangeMagnification[weaponLv];
                    return range;
                }
            }
            return 0;
        }

        /// <summary>
        /// �U���̊Ԋu�擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public float GetAttackInterval(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return ((float)status.attackInterval * (float)ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.shortenAttackInterval));
                }
            }
            return 0;
        }

        /// <summary>
        /// �ő�U���͈͂̐ݒ�p�A�����蔻��̃T�C�Y�ł͂Ȃ��U�����s���͈͂̃T�C�Y
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Vector2 GetDetectionSize(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.detectionSize;
                }
            }
            return Vector2.zero;
        }

        /// <summary>
        /// �e���擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public float GetBulletSpeed(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return ((float)status.bulletSpeed * (float)ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.bulletSpeedUP));
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// �U���J�n��̃^�[�Q�b�g��I�ԉ񐔎擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <param name="weaponLv">����̌��݃��x��</param>
        /// <returns></returns>
        public int GetTargetAmount(SubWeaponManager.SubWeaponID id, byte weaponLv)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.targetAmount[weaponLv];
                }
            }
            return 0;
        }

        /// <summary>
        /// �U���J�n��̘A���U���̃C���^�[�o���擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public float GetNextAttackInterval(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return ((float)status.nextAttackInterval * (float)ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.shortenAttackInterval));
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// �U���G�t�F�N�g�̎擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <returns>�G�t�F�N�g�̃I�u�W�F�N�g</returns>
        public GameObject GetAttackEffect(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.attackEffect;
                }
            }
            return null;
        }

        public Animator GetAttackAnimator(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.attackAnimator;
                }
            }
            return null;
        }

        /// <summary>
        /// �ŏI���x�����̍U���G�t�F�N�g�̎擾�p
        /// </summary>
        /// <param name="id"></param>
        /// <returns>�G�t�F�N�g�̃I�u�W�F�N�g</returns>
        public GameObject GetAttackEffectMaxLevel(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.attackEffectMaxLevel;
                }
            }
            return null;
        }

        public GameObject GetAttackObj(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.attackObj;
                }
            }
            return null;
        }
        public GameObject GetAttackObjMaxLevel(SubWeaponManager.SubWeaponID id)
        {
            foreach (var status in subWeaponsStatus)
            {
                if (id == status.SubWeaponID)
                {
                    return status.attackObjMaxLevel;
                }
            }
            return null;
        }
    }
}
