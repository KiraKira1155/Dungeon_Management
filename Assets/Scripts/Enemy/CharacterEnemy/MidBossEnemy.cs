using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class MidBossEnemy : BaseEnemy
    {
        public override EnemyManager.EnemyCharactersID GetEnemyID()
        {
            return EnemyManager.EnemyCharactersID.midBoss;
        }

    }
}

