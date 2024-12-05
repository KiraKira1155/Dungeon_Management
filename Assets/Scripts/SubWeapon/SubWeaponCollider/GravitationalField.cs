using GameSystem;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    public class GravitationalField : BaseSubWeapon
    {
        private const float GRAVITY_MODIFICATION = 2.0f; // ìGÇà¯Ç´äÒÇπÇÈîÕàÕÇÃï‚ê≥íl
        private const float DESTROY_TIME = 5.0f;

        private GameObject gravityAttackObj;
        private Vector2 gravityPos;

        private float attackInterval;
        private float gravityDestroyTime;




        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.gravitationalField;
        }


        protected override void AttackStart()
        {
            // ÉJÉÅÉâÇÃã´äEÇéÊìæ
            Vector2 cameraPos = CameraManager.I.GetMainCameraPos().position;
            Vector2 detectionSize = SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID());

            // è„â∫ç∂âEÇÃã´äEÇåvéZ
            var topLeft = cameraPos + new Vector2(-detectionSize.x / 2, detectionSize.y / 2);
            var bottomRight = cameraPos + new Vector2(detectionSize.x / 2, -detectionSize.y / 2);

            float randomX = Random.Range(topLeft.x, bottomRight.x);
            float randomY = Random.Range(bottomRight.y, topLeft.y);

            gravityAttackObj = SubWeaponManager.I.ParticleGeneration(SubWeaponManager.I.baseStatus.GetAttackEffect(GetWeaponID()), new Vector2(randomX, randomY));
            float attackRange = SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv())/ GRAVITY_MODIFICATION;
            if (gravityAttackObj != null)
            {
                // ParticleSystem ÇÃÉXÉPÅ[ÉãÇïœçX
                gravityAttackObj.transform.localScale = new Vector3(attackRange, attackRange, attackRange);
            }

            Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.gravitationalField,true);
            gravityPos = gravityAttackObj.transform.position;
            SubWeaponManager.I.SetGravityPos(gravityPos);

            gravityDestroyTime = 0.0f;
            attackInterval = 0.0f;
        }

        protected override void AttackProcess()
        {
            var deltaTime = Time.deltaTime;
            
            attackInterval += deltaTime;
            if (attackInterval >= SubWeaponManager.I.baseStatus.GetNextAttackInterval(GetWeaponID()))
            {
                Attack();
            }

            gravityDestroyTime += deltaTime;
            if(gravityDestroyTime >= DESTROY_TIME)
            {
                SubWeaponManager.I.DestroyObj(gravityAttackObj);
                gravityAttackObj = null;
                AttackEnd();
                Sound.SoundManager.I.StopSubWeaponSE(SoundManager.subWeaponAudioID.gravitationalField);
            }

        }

        protected override void DebugDrawCollider()
        {
            DebugDraw.DebugDrawLine.DrawBox(CameraManager.I.GetMainCameraPos().position, SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()), Color.red);
            DebugDraw.DebugDrawLine.DrawCircle(gravityPos, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()) * GRAVITY_MODIFICATION, 30, Color.yellow);
            DebugDraw.DebugDrawLine.DrawCircle(gravityPos, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 30, Color.blue);
        }

        protected override void UpdateProcess()
        {
            if (gravityAttackObj != null)
            {
                // ìGÇà¯Ç´äÒÇπÇÈèàóù
                GravityField();
            }
         }
    

        private void Attack()
        {
            CheckHitEnemy(gravityPos);
            CheckHitMidBoss(gravityPos);
            CheckHitBoss(gravityPos);


            attackInterval = 0.0f;
        }

        private void GravityField()
        {
            float detectionRadius = SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()) * GRAVITY_MODIFICATION;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(gravityPos, detectionRadius, Enemy.EnemyManager.I.enemyController.GetEnemyLayer());

            if(hitEnemies != null)
            {
                foreach (var enemy in Enemy.EnemySpawnManager.I.GetAllMapEnemy())
                {
                    foreach (var enemyCollider in hitEnemies)
                    {
                        if (enemy.GetEnemyCollider() == enemyCollider)
                        {
                            enemy.SetGravityEffect(true);
                            continue;
                        }
                    }
                }
            }
        }
        

    }
}

