using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeltonEnemy : BaseEnemy
{
    public override EnemyManager.EnemyCharactersID GetEnemyID()
    {
        return EnemyManager.EnemyCharactersID.skelton;
    }
}
