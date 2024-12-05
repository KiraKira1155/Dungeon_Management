using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class BigSkillPotion : BaseItemEffect
    {
        protected override void Effect()
        {
            Player.PlayerSkillManager.I.IncreaseSkillPoints(GetEffectAmount());
        }

        protected override void SE()
        {
            Sound.SoundManager.I.PlaySE(Sound.SoundManager.SEID.skillItem);
        }
    }
}