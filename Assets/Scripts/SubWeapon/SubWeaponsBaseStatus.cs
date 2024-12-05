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
        /// 攻撃ダメージ取得用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="weaponLv">現在レベル</param>
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
        /// 攻撃の当たり判定範囲取得用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="weaponLv">武器の現在レベル</param>
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
        /// 攻撃の間隔取得用
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
        /// 最大攻撃範囲の設定用、当たり判定のサイズではなく攻撃を行う範囲のサイズ
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
        /// 弾速取得用
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
        /// 攻撃開始後のターゲットを選ぶ回数取得用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="weaponLv">武器の現在レベル</param>
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
        /// 攻撃開始後の連続攻撃のインターバル取得用
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
        /// 攻撃エフェクトの取得用
        /// </summary>
        /// <param name="id"></param>
        /// <returns>エフェクトのオブジェクト</returns>
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
        /// 最終レベル時の攻撃エフェクトの取得用
        /// </summary>
        /// <param name="id"></param>
        /// <returns>エフェクトのオブジェクト</returns>
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
