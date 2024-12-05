using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {


        /// <summary>
        /// 敵キャラクターの基本ステータスを取得
        /// </summary>
        public EnemyBaseStatus basicStatus = new EnemyBaseStatus();

        /// <summary>
        /// 敵キャラクターの移動制御用
        /// </summary>
        public EnemyController enemyController = new EnemyController();

        /// <summary>
        /// 中ボスの移動制御用
        /// </summary>
        public MidBossController midBossController = new MidBossController();

        /// <summary>
        /// ボスの移動制御用
        /// </summary>
        public BossController bossController = new BossController();

        /// <summary>
        /// 敵キャラクターの種類、識別IDとして使用
        /// </summary>
        public enum EnemyCharactersID
        {
            goblin,
            skelton,
            Enemy_1,
            Enemy_2,
            Enemy_3,
            Enemy_4,
            Enemy_5,
            Enemy_6,
            midBoss,
            midBoss_1,
            boss,
        }

        private void Awake()
        {
            Init();
        }

        private void FixedUpdate()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
                return;

            foreach (var enemy in EnemySpawnManager.I.GetAllMapEnemy())
            {
                if (!enemy.IsEnemyDeath())
                {
                    enemyController.EnemyMoveCalculation(enemy, Time.deltaTime);
                    enemy.Animation();
                }

                enemy.SetIsSlowSpeed(false);
                enemy.SetGravityEffect(false);
            }

            if (EnemySpawnManager.I.GetMidBossEnemy() != null && !EnemySpawnManager.I.GetMidBossEnemy().IsEnemyDeath())
                midBossController.DetectAndRepel();


            if (EnemySpawnManager.I.GetBossEnemy() != null && !EnemySpawnManager.I.GetBossEnemy().IsEnemyDeath())
                bossController.BossMoveCalculation();
        }

        private void LateUpdate()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
                return;

            if (EnemySpawnManager.I.GetBossEnemy() != null)
                bossController.bossBarController.DoLateUpdate();
        }
    }
}
