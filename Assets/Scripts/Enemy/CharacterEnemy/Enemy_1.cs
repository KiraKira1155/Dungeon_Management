using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Enemy_1 : BaseEnemy
    {
        public override EnemyManager.EnemyCharactersID GetEnemyID()
        {
            return EnemyManager.EnemyCharactersID.Enemy_1;
        }

    }
}


