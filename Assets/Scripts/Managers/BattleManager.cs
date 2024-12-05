using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{
    public int casualtyCount {  get; private set; }
    public int totalEnemyDefeated {  get; private set; }

    private void Awake()
    {
        Init();
    }

    public override void BattleSceneInit()
    {
        casualtyCount = 0;
    }

    private void LateUpdate()
    {
        if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
            return;

        PlayerAttackTheEnemy();
        AttackThePlayer();
    }

    /// <summary>
    /// プレイヤーからエネミーにオート攻撃
    /// </summary>
    private void PlayerAttackTheEnemy()
    {

        AutoAttackTheEnemy();
        SubWeaponAttackTheEnemy();


        Player.PlayerManager.I.controller.autoAttack.EndAutoAttackProcess();
    }

    /// <summary>
    /// オート攻撃処理
    /// </summary>
    private void AutoAttackTheEnemy()
    {
        // 攻撃を行っていなければ処理なし。
        if (!Player.PlayerManager.I.controller.autoAttack.GetIsAttack())
            return;

        foreach (var enemy in Player.PlayerManager.I.GetPlayerAutoAttackCollider().GetHitEnemy())
        {
            enemy.BeAttacked(Player.PlayerManager.I.currentStatus.GetAttackDamage());
        }
    }

    /// <summary>
    /// サブ武器攻撃処理
    /// </summary>
    private void SubWeaponAttackTheEnemy()
    {
        for (int i = 0; i < SubWeapon.SubWeaponManager.I.GetHitEnemy().Length; i++)
        {
            if (!SubWeapon.SubWeaponManager.I.GetHitEnemy()[i])
                continue;

            var attackDamage = SubWeapon.SubWeaponManager.I.GetHitEnemyList(i).damage;
            foreach (var enemy in SubWeapon.SubWeaponManager.I.GetHitEnemyList(i).enemyList)
            {
                enemy.BeAttacked(attackDamage);
            }
        }
    }

    private void AttackThePlayer()
    {
        int damage = 0;
        foreach(var enemy in Enemy.EnemySpawnManager.I.GetAllMapEnemy())
        {
            if (enemy.IsAttack())
            {
                damage += enemy.AttackPlayer();
                casualtyCount++;
            }
        }

        var midBoss = Enemy.EnemySpawnManager.I.GetMidBossEnemy();
        if (midBoss != null)
            if (midBoss.IsAttack())
            {
                damage += midBoss.AttackPlayer();
                casualtyCount++;
            }

        var boss = Enemy.EnemySpawnManager.I.GetBossEnemy();
        if(boss != null)
            if (boss.IsAttack())
            {
                damage += boss.AttackPlayer();
                casualtyCount++;
            }

        Player.PlayerManager.I.currentStatus.BeAttacked(damage);
    }

    public void EnemyDefeated()
    {
        totalEnemyDefeated++;
        MenuManager.I.KillCountText(totalEnemyDefeated);
    }
}
