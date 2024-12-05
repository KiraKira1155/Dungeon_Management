using Enemy;
using Player;
using SubWeapon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NextSkillAction : MonoBehaviour
{
    [SerializeField] private int nextSkillNum;

    private void Update()
    {
        DebugDraw.DebugDrawLine.DrawBox(CameraManager.I.GetMainCameraPos().position,
                new Vector2(10, 18), Color.red);
    }

    public void StartNextSkill()
    {
        PlayerSkillManager.I.GetSkillObject(nextSkillNum).SetActive(true);

        if(nextSkillNum != 15)
            Sound.SoundManager.I.PlaySkillSE(Sound.SoundManager.skillAudioID.skill);
        else
            Sound.SoundManager.I.PlaySkillSE(Sound.SoundManager.skillAudioID.skillEnd);
    }

    public void EndThisSkill()
    {
        PlayerSkillManager.I.GetSkillObject(nextSkillNum - 1).SetActive(false);

        if (nextSkillNum == 15)
        {

            PlayerSkillManager.I.GetSkillObject(15).SetActive(false);
            PlayerSkillManager.I.SkillReset();
            DisplayUI();
            GameManager.I.sceneController.SetGameModeForBattleScene(GameSystem.SceneController.GameModeForBattle.battle);

            CheckTarget();
        }
    }

    private void CheckTarget()
    {
        var target = new List<Enemy.IBaseEnemy>();

        var enemy = Physics2D.OverlapBoxAll
                (
                CameraManager.I.GetMainCameraPos().position,
                new Vector2(10, 18),
                0.0f,
                Enemy.EnemyManager.I.enemyController.GetEnemyLayer()
                );

        foreach (var chooseEnemy in Enemy.EnemySpawnManager.I.GetAllMapEnemy())
        {
            if (chooseEnemy.IsEnemyDeath())
                continue;

            foreach (var hit in enemy)
            {
                if (chooseEnemy.GetEnemyCollider() == hit)
                {
                    target.Add(chooseEnemy);
                    break;
                }
            }
        }

        var midBoss = Physics2D.OverlapBox
                (
                CameraManager.I.GetMainCameraPos().position,
                new Vector2(10, 18),
                0.0f,
                Enemy.EnemyManager.I.midBossController.GetMidBossLayer()
                );


        if (Enemy.EnemySpawnManager.I.GetMidBossEnemy() != null)
        {
            if (Enemy.EnemySpawnManager.I.GetMidBossEnemy().GetEnemyCollider() == midBoss)
            {
                Enemy.EnemySpawnManager.I.GetMidBossEnemy().BeAttacked(1000);
            }
        }

        var boss = Physics2D.OverlapBox
            (
            CameraManager.I.GetMainCameraPos().position,
                new Vector2(10, 18),
            0.0f,
            Enemy.EnemyManager.I.bossController.GetBossLayer()
            );

        if (Enemy.EnemySpawnManager.I.GetBossEnemy() != null)
        {
            if (Enemy.EnemySpawnManager.I.GetBossEnemy().GetEnemyCollider() == boss)
            {
                Enemy.EnemySpawnManager.I.GetBossEnemy().BeAttacked(2000);
            }
        }

        if (target.Count == 0)
            return;

        foreach (var mapEnemy in Enemy.EnemySpawnManager.I.GetAllMapEnemy())
        {
            foreach(var targetEnemy in target)
            {
                if(mapEnemy.GetEnemyGameObject() == targetEnemy.GetEnemyGameObject())
                {
                    targetEnemy.Death();
                    continue;
                }
            }
        }
    }

    private void DisplayUI()
    {
        MenuManager.I.DisplayUI();
        PlayerManager.I.barController.DisplayUI();
        PlayerSkillManager.I.DisplayUI();
    }
}
