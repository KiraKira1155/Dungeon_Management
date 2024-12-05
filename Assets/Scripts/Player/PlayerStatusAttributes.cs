using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "SamplePlayerStatus", menuName = "GZGame / PlayerStatus Attribute")]
    public class PlayerStatusAttributes : ScriptableObject
    {
        [SerializeField] private string _charaName;
        [SerializeField] private PlayerManager.PlayerCharactersID _characterID;
        [SerializeField] private int _hp;
        [SerializeField] private int _attackDamage;
        [SerializeField] private float _attackInterval;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private int _needSkillPoint;

        public string charaName { get { return _charaName; } }
        public PlayerManager.PlayerCharactersID characterID { get { return _characterID; } }
        public int hp { get { return _hp; } }
        public int attackDamage { get { return _attackDamage; } }
        public float attackInterval { get { return _attackInterval; } }
        public float moveSpeed { get { return _moveSpeed; } }
        public int needSkiollPoint { get { return _needSkillPoint; } }
    }
}
