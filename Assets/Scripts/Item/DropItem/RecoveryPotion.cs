using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class RecoveryPotion : BaseItemEffect
    {
        protected override void Effect()
        {
            Player.PlayerManager.I.currentStatus.BeAttacked(-GetEffectAmount());
        }

        protected override void SE()
        {
            Sound.SoundManager.I.PlaySE(Sound.SoundManager.SEID.healItem);
        }
    }

}





