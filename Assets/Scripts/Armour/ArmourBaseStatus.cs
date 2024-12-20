using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Armour
{
    [System.Serializable]
    public class ArmourBaseStatus
    {
        [SerializeField] private ArmourAttributes[] armourAttributes;

        /// <summary>
        /// 強化値の取得
        /// </summary>
        /// <param name="id"></param>
        /// <param name="armourLv">現在レベル</param>
        /// <returns></returns>
        public float GetEnhancedValue(ArmourManager.ArmourID id, byte armourLv)
        {
            foreach (var status in armourAttributes)
            {
                if (id == status.armourID)
                {
                    return status.enhancedValue[armourLv];
                }
            }

            return 0;
        }

    }
}
