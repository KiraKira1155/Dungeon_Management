using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Armour
{
    [CreateAssetMenu(fileName = "Armour", menuName = "GZGame / Armour")]
    public class ArmourAttributes : ScriptableObject
    {
        private const int MAX_LV = 3;
        [Header("‹­‰»’l")]
        [SerializeField] private ArmourManager.ArmourID _armourID;
        [SerializeField] private float[] _enhancedValue = new float[MAX_LV];

        public ArmourManager.ArmourID armourID {  get { return _armourID; } }
        public float[] enhancedValue { get { return _enhancedValue; } }
    }
}
