
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [System.Serializable]
    public class PlayerNextLvCalculation
    {
        [SerializeField] private int[] needExp;
        private int nextNeedExp;
        private bool lvUP;

        public void Init()
        {
            nextNeedExp = needExp[0];
        }

        public void CalculationCurrentLv()
        {
            var exp = PlayerManager.I.currentStatus.GetCurrentExp();
            while (exp >= nextNeedExp)
            {
                lvUP = true;
                exp -= nextNeedExp;
                if(needExp.Length <= PlayerManager.I.currentStatus.GetCurrentLv())
                {
                    nextNeedExp = 1000;
                }
                else
                {
                    nextNeedExp = needExp[PlayerManager.I.currentStatus.GetCurrentLv()];
                }
                PlayerManager.I.currentStatus.LvUp();
                PlayLvUpSE();
            }

            if (lvUP)
            {
                PlayerManager.I.currentStatus.SetRemainingExp(exp);
                PlayerManager.I.barController.PlayerLvDisplayInOder();
                lvUP = false;
                EquipmentSelectManager.I.StartSelectEquipmentEvent();
            }
        }

        /// <summary>
        /// 次のレベルアップに必要な経験治療の取得用
        /// </summary>
        /// <returns></returns>
        public int GetNextNeedExp()
        {
            return nextNeedExp;
        }
        /// <summary>
        /// プレイヤーのレベルアップ時のSE
        /// </summary>
        private void PlayLvUpSE()
        {
            Sound.SoundManager.I.PlaySE(0);
        }
    }
}
