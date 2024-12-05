using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    public class Funnel : BaseSubWeapon
    {
        private List<FunnelStatus> funnelList = new List<FunnelStatus>();
        private byte previousLv;
        private bool init = false;

        private struct FunnelStatus
        {
            public GameObject funnelObj;
            public float attackInterval;
            public float moveSpeed;
        }

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.funnel;
        }

        protected override void AttackStart()
        {
        }

        protected override void AttackProcess()
        {

        }
        protected override void UpdateProcess()
        {
            if (!init)
            {
                previousLv = 255;
                init = true;
            }

            if(previousLv != CurrentSubWeaponLv())
            {
                if(previousLv != 255)
                {
                    for(int i = funnelList.Count - 1; i >= 0; i--)
                    {
                        SubWeaponManager.I.DestroyObj(funnelList[i].funnelObj);
                        funnelList.RemoveAt(i);
                    }
                }

                for(int i = 0; i < SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()); i++)
                {
                    var funnel = new FunnelStatus
                    {
                        funnelObj =SubWeaponManager.I.AttackObjGeneration(SubWeaponManager.I.baseStatus.GetAttackObj(GetWeaponID()), Player.PlayerManager.I.GetPlayerTransform().position),
                        attackInterval = 0.0f,
                        moveSpeed = 1.0f
                    };
                    funnelList.Add(funnel);
                }
            }



            previousLv = CurrentSubWeaponLv();
        }

        protected override void DebugDrawCollider()
        {

        }



        private Vector2 GetTargetPos()
        {
            var target = CheckTargetPos();

            switch (target)
            {
                // ターゲットがいなかった場合、ランダムな場所に発射
                case null:
                    var detectuionSize = SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID());
                    var direction = new Vector2(detectuionSize.x / 2, detectuionSize.y / 2);
                    Vector2 cameraPos = CameraManager.I.GetMainCameraPos().position;
                    var targetPos = Vector2.zero;
                    targetPos.x = Random.Range(-direction.x + cameraPos.x, direction.x + cameraPos.x);
                    targetPos.y = Random.Range(-direction.y + cameraPos.y, direction.x + cameraPos.y);

                    return targetPos;

                default:
                    return target.GetEnemyTransform().position;
            }
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

            if (Enemy.EnemySpawnManager.I.GetBossEnemy() != null)
            {
                if (Enemy.EnemySpawnManager.I.GetBossEnemy().GetEnemyCollider() == boss)
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
