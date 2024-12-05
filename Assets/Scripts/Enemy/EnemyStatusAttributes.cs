using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyStatus", menuName = "GZGame / Enemy Status")]
    public class EnemyStatusAttributes : ScriptableObject
    {
        [Header("ステータス")]

        [SerializeField] private EnemyManager.EnemyCharactersID _enemyID;
        [SerializeField] private string _enemyName = "";
        [SerializeField] private int _hp;
        [SerializeField] private int _atk;
        [SerializeField] private int _dropExp;
        [SerializeField] private float _attackInterval;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _detectionRadius;

        public EnemyManager.EnemyCharactersID enemyID { get { return _enemyID; } }
        public string enemyName { get { return _enemyName; } }
        public int hp { get { return _hp; } }
        public int atk { get { return _atk; } }
        public int dropExp { get { return _dropExp; } }
        public float attackInterval { get { return _attackInterval; } }
        public float Movespeed { get { return _moveSpeed; } }
        public float detectionRadius { get { return _detectionRadius; } }
    }
}
