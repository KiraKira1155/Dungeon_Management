using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class GoblinEnemy : BaseEnemy
    {
        public override EnemyManager.EnemyCharactersID GetEnemyID()
        {
            return EnemyManager.EnemyCharactersID.goblin;
        }
    }
}


