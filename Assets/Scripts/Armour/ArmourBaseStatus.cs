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
        /// �����l�̎擾
        /// </summary>
        /// <param name="id"></param>
        /// <param name="armourLv">���݃��x��</param>
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
