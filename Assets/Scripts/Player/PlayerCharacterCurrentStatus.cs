using Armour;
using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerCharacterCurrentStatus
    {
        private PlayerManager.PlayerCharactersID characterID;
        private int baseMaxHP;
        private int maxHP;
        private int attackDamage;
        private int currentHP;
        private int currentExp;
        private int currentLv;

        private byte mainWeaponCurrentLv;
        public const byte MAINWEAPON_MAX_LV = 3;

        public PlayerNextLvCalculation nextLvCalculation = new PlayerNextLvCalculation();

        
        /// <summary>
        /// バトルに使用するキャラクターのステータスの設定用
        /// </summary>
        /// <param name="id">使用するキャラクターの識別ID</param>
        public void SetPlayerCharacter(PlayerManager.PlayerCharactersID id)
        {
            characterID = id;
            PlayerManager.I.controller.InitPlayerSetting(id);

            var status = PlayerManager.I.basicStatus;
            attackDamage = status.GetCharacterStatusAttackDamage(characterID);
            maxHP = status.GetCharacterStatusHP(characterID);
            currentHP = maxHP;
            baseMaxHP = maxHP;

            currentLv = 1;
            nextLvCalculation.Init();
            PlayerManager.I.barController.ExpBarController();
            PlayerManager.I.barController.PlayerLvDisplayInOder();
            mainWeaponCurrentLv = 0;

            PlayerManager.I.GetPlayerAutoAttackCollider().SetAttackAngle(100);
        }

        /// <summary>
        /// プレイヤーの攻撃力取得用
        /// </summary>
        /// <returns></returns>
        public int GetAttackDamage()
        {
            return attackDamage;
        }

        /// <summary>
        /// プレイヤーの現在HP取得用
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCurrentHP()
        {
            return currentHP;
        }

        /// <summary>
        /// プレイヤーの最大HP取得用
        /// </summary>
        /// <returns></returns>
        public int GetMaxHP()
        {
            return maxHP;
        }

        public int GetBaseMaxHP()
        {
            return baseMaxHP;
        }

        /// <summary>
        /// プレイヤーのダメージ処理用
        /// </summary>
        /// <param name="damage">与えるダメージ量</param>
        public void BeAttacked(int damage)
        {
            if(damage > 0)
            {
                var nomarizedDamage = (float)damage * (float)ArmourManager.I.GetArmourEnhancedValue(ArmourManager.ArmourID.defensUP);
                if(nomarizedDamage < 1)
                {
                    nomarizedDamage = 1;
                }
                damage = (int)(nomarizedDamage);
            }

            currentHP -= damage;

            if(currentHP > maxHP)
                currentHP = maxHP;

            if (currentHP <= 0 && GameManager.I.sceneController.GetGameModeForBattleScene() != SceneController.GameModeForBattle.death)
            {
                GameManager.I.sceneController.SetGameModeForBattleScene(SceneController.GameModeForBattle.death);
                GameManager.I.ChengeNewScene(SceneController.GameScene.gameOver, true);
            }
        }

        /// <summary>
        /// 操作中のキャラクターID取得
        /// </summary>
        /// <returns></returns>
        public PlayerManager.PlayerCharactersID GetCharacterID()
        {
            return characterID;
        }

        /// <summary>
        /// プレイヤーの現在レベル取得用
        /// </summary>
        /// <returns></returns>
        public int GetCurrentLv()
        {
            return currentLv;
        }

        /// <summary>
        /// レベルアップ処理
        /// </summary>
        public void LvUp()
        {
            currentLv++;
        }

        /// <summary>
        /// 現在獲得した経験値量
        /// </summary>
        /// <returns></returns>
        public int GetCurrentExp()
        {
            return currentExp;
        }

        public void SetRemainingExp(int remainingExp)
        {
            currentExp = remainingExp;
        }

        /// <summary>
        /// プレイヤーが経験値を得る処理用
        /// </summary>
        /// <param name="expAmount"></param>
        public void ExpGain(int expAmount)
        {
            currentExp += expAmount;

            PlayerManager.I.currentStatus.nextLvCalculation.CalculationCurrentLv();
            PlayerManager.I.barController.ExpBarController();
        }

        public void WeaponLvUp()
        {
            mainWeaponCurrentLv++;
        }

        public byte GetWeaponLv()
        {
            return mainWeaponCurrentLv;
        }

        public void SetMaxHP(int maxHpAmount)
        {
            maxHP = maxHpAmount;
        }
    }
}
