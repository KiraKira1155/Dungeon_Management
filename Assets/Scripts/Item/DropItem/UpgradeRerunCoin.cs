using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class UpgradeRerunCoin : BaseItemEffect
    {
        protected override void Effect()
        {
            EquipmentSelectManager.I.GetReRunItem();
        }

        protected override void SE()
        {
            Sound.SoundManager.I.PlaySE(Sound.SoundManager.SEID.coin);
        }
    }

}
