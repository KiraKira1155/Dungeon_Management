using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    public class LandMine : BaseSubWeapon
    {
        private const float DETNATION_TIME = 1.0f;
        private const float DESTROY_TIME = 10.0f;
        private readonly Vector2 EXPROSION_POS = new Vector2(0.0f, 1.2f);

        private Vector2 landminePos;
        private float intervalTime;
        private float explosionRadius;
        private List<(GameObject obj, float detonationTime, bool detonation, float destroyTime)> landmines = new List<(GameObject, float, bool, float)>();

        protected override void AttackStart()
        {
            landmines.Clear();
            Attack();

        }

        protected override void AttackProcess()
        {

            intervalTime += Time.deltaTime;
            if (intervalTime >= SubWeaponManager.I.baseStatus.GetAttackInterval(GetWeaponID()))
            {
                Attack();
            }
        }

        private void Attack()
        {
            intervalTime = 0;

            landminePos = Player.PlayerManager.I.GetPlayerTransform().position;

            landmines.Add((SubWeaponManager.I.AttackObjGeneration(SubWeaponManager.I.baseStatus.GetAttackObj(GetWeaponID()), landminePos), 0.0f, false, 0.0f));

        }

       
      
        protected override void UpdateProcess()
        {
            landmines.RemoveAll(item => item.obj == null);

            // 地雷がアクティブな場合、敵をチェック
            if (landmines.Count != 0)
            {

                for (int i = 0; i < landmines.Count; i++)
                {
                    landmines[i] = (landmines[i].obj, landmines[i].detonationTime, landmines[i].detonation, landmines[i].destroyTime + Time.deltaTime);

                    if (landmines[i].destroyTime > DESTROY_TIME)
                    {
                        Detonation(i, landmines[i].obj.transform.position);
                    }
                }

                for (int i = 0; i < landmines.Count; i++)
                {
                    if (landmines[i].detonation)
                    {
                        landmines[i] =(landmines[i].obj, landmines[i].detonationTime + Time.deltaTime, true, landmines[i].destroyTime);
                    }
                }

                for (int i = 0; i < landmines.Count; i++)
                {
                    var pos = landmines[i].obj.transform.position;
                    if (EnemyCollider(pos) != null)
                    {
                        Detonation(i, pos);
                        continue;
                    }
                    else if (MidBossCollider(pos) != null)
                    {
                        Detonation(i, pos);
                        continue;
                    }
                    else if (BossCollider(pos) != null)
                    {
                        Detonation(i, pos);
                        continue;
                    }
                }
            }
        }

        private void Detonation(int landmineNum, Vector2 pos)
        {
            if (!landmines[landmineNum].detonation)
            {
                landmines[landmineNum] = (landmines[landmineNum].obj, 0, true, landmines[landmineNum].destroyTime);
            }
            else if (landmines[landmineNum].detonationTime > DETNATION_TIME)
            {
                CheckHitEnemy(pos);
                CheckHitMidBoss(pos);
                CheckHitBoss(pos);

                SubWeaponManager.Destroy(landmines[landmineNum].obj);

                SubWeaponManager.I.ParticleGeneration(SubWeaponManager.I.baseStatus.GetAttackEffect(GetWeaponID()), (Vector2)landmines[landmineNum].obj.transform.position + EXPROSION_POS);
                Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.landmine, false);
            }
        }

        private Collider2D[] EnemyCollider(Vector2 pos)
        {
            var enemy = Physics2D.OverlapCircleAll
                (pos, 1, Enemy.EnemyManager.I.enemyController.GetEnemyLayer());

            if (enemy.Length == 0)
                return null;

            return enemy;
        }

        private Collider2D MidBossCollider(Vector2 pos)
        {
            return Physics2D.OverlapCircle
                (pos, 1, Enemy.EnemyManager.I.midBossController.GetMidBossLayer());
        }
        private Collider2D BossCollider(Vector2 pos)
        {
            return Physics2D.OverlapCircle
                (pos, 1, Enemy.EnemyManager.I.bossController.GetBossLayer());
        }


        protected override void DebugDrawCollider()
        {
            foreach (var landmine in landmines)
            {
                Vector2 landminePos = landmine.obj.transform.position;
                DebugDraw.DebugDrawLine.DrawCircle(landminePos, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 30, Color.yellow);
            }
        }

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.landmine;
        }

      
    }


}
