using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    public class LightningStrike : BaseSubWeapon
    {

        private Vector2 lightningStrikePos;
        private float intervalTime;
        private int attackCnt;

        protected override void AttackStart()
        {
            attackCnt = 0;
            Attack();

            if (attackCnt == SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()))
                AttackEnd();
        }

        protected override void AttackProcess()
        {
            intervalTime += Time.deltaTime;
            if(intervalTime >= SubWeaponManager.I.baseStatus.GetNextAttackInterval(GetWeaponID()))
            {
                Attack();
            }

            if (attackCnt == SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()))
            {
                AttackEnd();
                return;
            }
        }

        private void Attack()
        {
            intervalTime = 0;
            GetTargetPos();
            SubWeaponManager.I.ParticleGeneration(SubWeaponManager.I.baseStatus.GetAttackEffect(GetWeaponID()), lightningStrikePos + new Vector2(0, 8.5f));
            DebugDraw.DebugDrawLine.DrawCircle(lightningStrikePos, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 30, Color.yellow);
            attackCnt++;

            Sound.SoundManager.I.PlaySubWeaponSE(Sound.SoundManager.subWeaponAudioID.lightningStrike,false);
            CheckHitEnemy(lightningStrikePos);
            CheckHitMidBoss(lightningStrikePos);
            CheckHitBoss(lightningStrikePos);
        }

        private void GetTargetPos()
        {
            var target = CheckTargetPos();

            switch (target)
            {
                // ターゲットがいなかった場合、プレイヤーに落とす
                case null:
                    lightningStrikePos = Player.PlayerManager.I.GetPlayerTransform().position;
                    break;

                default:
                    lightningStrikePos = target.GetEnemyTransform().position;
                    break;
            }
        }

        protected override void DebugDrawCollider()
        {
            DebugDraw.DebugDrawLine.DrawBox(CameraManager.I.GetMainCameraPos().position, SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()), Color.red);
        }
        protected override void UpdateProcess()
        {
        }

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.lightningStrike;
        }

        /// <summary>
        /// 攻撃対象の選択用
        /// </summary>
        /// <returns>ターゲットが存在すれば敵スクリプトを、いなければ null を返す</returns>
        private Enemy.IBaseEnemy CheckTargetPos()
        {
            var target = new List<Enemy.IBaseEnemy>();

            var enemy = Physics2D.OverlapBoxAll
                (
                CameraManager.I.GetMainCameraPos().position,
                SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()),
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
                SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()),
                0.0f,
                Enemy.EnemyManager.I.midBossController.GetMidBossLayer()
                );


            if (Enemy.EnemySpawnManager.I.GetMidBossEnemy() != null)
            {
                if (Enemy.EnemySpawnManager.I.GetMidBossEnemy().GetEnemyCollider() == midBoss)
                {
                    target.Add(Enemy.EnemySpawnManager.I.GetMidBossEnemy());
                }
            }

            var boss = Physics2D.OverlapBox
                (
                CameraManager.I.GetMainCameraPos().position,
                SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()),
                0.0f,
                Enemy.EnemyManager.I.bossController.GetBossLayer()
                );

            if(Enemy.EnemySpawnManager.I.GetBossEnemy() != null)
            {
                if(Enemy.EnemySpawnManager.I.GetBossEnemy().GetEnemyCollider() == boss)
                {
                    target.Add(Enemy.EnemySpawnManager.I.GetBossEnemy());
                }
            }

            if (target.Count == 0)
                return null;

            return target[Random.Range(0, target.Count)];
        }
    }
}
