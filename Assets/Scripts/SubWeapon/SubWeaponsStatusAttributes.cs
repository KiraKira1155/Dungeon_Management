using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    [CreateAssetMenu(fileName = "SubWeaponsStatus",menuName = "GZGame / SubWeapons Status")]
    public class SubWeaponsStatusAttributes : ScriptableObject
    {
        private const int MAX_LV = 4;
        [Header("武器ステータス")]
        [SerializeField] private SubWeaponManager.SubWeaponID subWeaponID;
        [Header("攻撃力設定")]
        [SerializeField] private int _attack;
        [SerializeField] private float[] _attackMagnification = new float[MAX_LV];

        [Header("攻撃範囲設定")]
        [SerializeField] private Vector2 _detectionSize;
        [SerializeField] private float _attackRange;
        [SerializeField] private float[] _attackRangeMagnification = new float[MAX_LV];

        [Header("攻撃間隔設定")]
        [SerializeField] private float _attackInterval;

        [Header("攻撃開始後の設定")]
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private int[] _targetAmount = new int[MAX_LV];
        [SerializeField] float _nextAttackInterval;
        [SerializeField] GameObject _attackEffect;
        [SerializeField] Animator _attackAnimator;
        [SerializeField] GameObject _attackEffectMaxLevel;

        [Header("攻撃本体のオブジェクト")]
        [SerializeField] private GameObject _attackObj;
        [SerializeField] private GameObject _attackObjMaxLevel;

        public SubWeaponManager.SubWeaponID SubWeaponID { get { return subWeaponID; } }
        public int attack { get { return _attack; } }
        public float[] attackMagnification { get { return _attackMagnification; } }
        public float attackRange { get { return _attackRange; } }
        public float[] attackRangeMagnification { get { return _attackRangeMagnification; } }
        public float attackInterval { get { return _attackInterval; } }
        public Vector2 detectionSize { get { return _detectionSize; } }
        public int[] targetAmount { get { return _targetAmount; } }
        public float nextAttackInterval { get { return _nextAttackInterval; } }
        public GameObject attackEffect { get { return _attackEffect; } }
        public Animator attackAnimator { get { return _attackAnimator; } } 
        public GameObject attackEffectMaxLevel { get { return _attackEffectMaxLevel; } }
        public float bulletSpeed { get { return _bulletSpeed; } } 
        public GameObject attackObj { get { return _attackObj; } }
        public GameObject attackObjMaxLevel { get { return _attackObjMaxLevel; } }
    }
}

