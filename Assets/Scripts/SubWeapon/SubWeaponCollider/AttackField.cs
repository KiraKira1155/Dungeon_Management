using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    public class AttackField : BaseSubWeapon
    {
        private Transform fieldParticlePos;
        private float intervalTime;


        protected override void AttackStart()
        {
            fieldParticlePos = SubWeaponManager.I.ParticleGeneration(SubWeaponManager.I.baseStatus.GetAttackEffect(GetWeaponID()), Player.PlayerManager.I.GetPlayerTransform().position).transform;
            Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.attackField,true);
        }

        protected override void AttackProcess()
        {
            // プレイヤーを中心に攻撃位置を設定
            fieldParticlePos.position = Player.PlayerManager.I.GetPlayerTransform().position;
            UpdateParticleScale();
            var enemyList = GetHitEnemyCollider(fieldParticlePos.position);
            if (enemyList != null)
            {
                if (CurrentSubWeaponLv() == 3)
                {
                    foreach (var enemy in Enemy.EnemySpawnManager.I.GetAllMapEnemy())
                    {
                        foreach (var enemyCollider in enemyList)
                        {
                            if (enemy.GetEnemyCollider() == enemyCollider)
                            {
                                enemy.SetIsSlowSpeed(true);
                                continue;
                            }
                        }
                    }
                }
            }

            intervalTime += Time.deltaTime;
            if (intervalTime >= SubWeaponManager.I.baseStatus.GetNextAttackInterval(GetWeaponID()))
            {
                Attack();
            }
        }

        private void Attack()
        {
            intervalTime = 0;

            CheckHitEnemy(fieldParticlePos.position);
            CheckHitMidBoss(fieldParticlePos.position);
            CheckHitBoss(fieldParticlePos.position);
        }

        protected override void UpdateProcess()
        {
        }


        protected override void DebugDrawCollider()
        {
            DebugDraw.DebugDrawLine.DrawCircle(Player.PlayerManager.I.GetPlayerTransform().position, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 30, Color.yellow);
        }

        private void UpdateParticleScale()
        {
            float attackRange = SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv());
            if (fieldParticlePos != null)
            {
                fieldParticlePos.localScale = new Vector3(attackRange, attackRange, attackRange);
            }
        }

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.attackField;
        }
    }
}

   
