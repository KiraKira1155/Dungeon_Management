using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Enemy
{
    [CreateAssetMenu(fileName = "'enemyName'SpawnOder", menuName = "GZGame / Enemy Spawn Oder")]
    public class EnemySpawnOderAttributes : ScriptableObject
    {
        [Header("���̃X�|�[�����鎞��(�b)")]
        [SerializeField] private int[] _spawnTime;
        [Header("�o����")]
        [SerializeField] private int[] _spawnAmount;
        public int[] spawnTime { get { return _spawnTime; } }
        public int[] spawnAmount { get { return _spawnAmount; } }

    }
}
