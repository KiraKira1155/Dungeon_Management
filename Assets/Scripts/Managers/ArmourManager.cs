using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Armour
{
    public class ArmourManager : Singleton<ArmourManager>
    {
        public const byte MAX_HOLDING_NUM = 4;
        public const byte MAX_ARMOUR_LEVEL = 2;

        private const float RECOVERY_INTERVAL = 5.0f;
        private float recoveryTime;


        [SerializeField] private ArmourBaseStatus baseStatus = new ArmourBaseStatus();

        private (ArmourID armour, byte currentLv)[] holdingArmour = new (ArmourID armour, byte currentLv)[MAX_HOLDING_NUM];
        private byte currentHoldingCount;

        public enum ArmourID
        {
            NONE = 0,

            attackUP,
            bulletSpeedUP,
            shortenAttackInterval,
            continuousHPRecovery,
            defensUP,
            extendedEffectDuration,
            moveSpeedUP,
            maxHPUP,
            spGainUP,
            mainWeaponExpandedAttackRange,

            MAX_ID_COUNT
        }

        public override void BattleSceneInit()
        {
            holdingArmour = new (ArmourID armour, byte currentLv)[MAX_HOLDING_NUM];
            currentHoldingCount = 0;
            recoveryTime = 0;
        }

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            foreach (var armour in holdingArmour)
            {
                if(armour.armour == ArmourID.continuousHPRecovery)
                {
                    recoveryTime += Time.deltaTime;
                    if (RECOVERY_INTERVAL <= recoveryTime)
                    {
                        recoveryTime = 0;

                        var playerHP = Player.PlayerManager.I.currentStatus;
                        var recoveryValue = (float)playerHP.GetMaxHP() * GetArmourEnhancedValue(ArmourID.continuousHPRecovery);

                        playerHP.BeAttacked(-(int)recoveryValue);
                    }
                }
            }
        }

        public void SetArmour(ArmourID armourID)
        {

            for (int i = 0; i < holdingArmour.Length; i++)
            {
                if (holdingArmour[i].armour == armourID)
                {
                    holdingArmour[i].currentLv++;

                    if (armourID == ArmourID.maxHPUP)
                    {
                        PlayerManager.I.currentStatus.SetMaxHP((int)((float)PlayerManager.I.currentStatus.GetBaseMaxHP() * (float)GetArmourEnhancedValue(ArmourID.maxHPUP)));
                    }
                    else if (armourID == ArmourID.mainWeaponExpandedAttackRange)
                    {
                        PlayerManager.I.GetPlayerAutoAttackCollider().SetAttackDistance((float)GetArmourEnhancedValue(ArmourID.mainWeaponExpandedAttackRange));
                    }
                    return;
                }
            }

            holdingArmour[currentHoldingCount] = (armourID, 0);
            currentHoldingCount++;

            if(armourID == ArmourID.maxHPUP)
            {
                PlayerManager.I.currentStatus.SetMaxHP((int)((float)PlayerManager.I.currentStatus.GetBaseMaxHP() * (float)GetArmourEnhancedValue(ArmourID.maxHPUP)));
            }
            else if (armourID == ArmourID.mainWeaponExpandedAttackRange)
            {
                PlayerManager.I.GetPlayerAutoAttackCollider().SetAttackDistance((float)GetArmourEnhancedValue(ArmourID.mainWeaponExpandedAttackRange));
            }
        }

        /// <summary>
        /// Œ»İ‘•”õ’†‚Ì–h‹ï‚ÌƒŒƒxƒ‹æ“¾—p
        /// </summary>
        /// <param name="armourID">¯•ÊID</param>
        /// <returns></returns>
        public byte GetArmourCurrentLv(ArmourID armourID)
        {
            if (currentHoldingCount != 0)
            {
                foreach (var armour in holdingArmour)
                {
                    if (armour.armour == armourID)
                        return armour.currentLv;
                }

                return 255;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// –h‹ï‚É‚æ‚é‹­‰»’læ“¾
        /// </summary>
        /// <param name="armourID"></param>
        /// <returns>‹­‰»’l ã¸‚·‚é“—Ê•ª‚Ì float (—á 120% ¨ 1.2f)
        /// <para>
        /// –h‹ï‚ª‚È‚¢ê‡‚Í"1"‚ª•Ô‚é
        /// </para>
        /// </returns>
        public float GetArmourEnhancedValue(ArmourID armourID)
        {
            if(currentHoldingCount != 0)
            {
                foreach (var armour in holdingArmour)
                {
                    if(armour.armour == armourID)
                        return baseStatus.GetEnhancedValue(armourID, armour.currentLv);
                }

                return 1;
            }
            else
            {
                return 1;
            }
        }

        public (ArmourID armour, byte currentLv)[] GetArmourAllData()
        {
            return holdingArmour;
        }

        /// <summary>
        /// Œ»İŠ‚µ‚Ä‚¢‚é•Ší‚Ì”
        /// </summary>
        /// <returns></returns>
        public byte GetHoldingAmount()
        {
            return currentHoldingCount;
        }
    }
}
