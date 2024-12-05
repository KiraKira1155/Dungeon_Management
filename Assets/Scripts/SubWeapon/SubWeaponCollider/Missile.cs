using SubWeapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SubWeapon
{
    public class Missile : BaseSubWeapon
    {
        private const float EFFECT_SIZE = 2.0f;
        private float intervalTime;
        private int attackCnt;
        private List<GameObject> moveMissileObj = new List<GameObject>();
        private List<Vector2> moveMissileDirection= new List<Vector2>();

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
            if (intervalTime >= SubWeaponManager.I.baseStatus.GetNextAttackInterval(GetWeaponID()))
            {
                Attack();
            }
            else if (attackCnt == SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()))
            {
                AttackEnd();
            }
        }

        protected override void UpdateProcess()
        {
            // 各オブジェクトを移動
            for (int i = 0; i < moveMissileObj.Count; i++)
            {
                var pos = moveMissileObj[i].transform.position;

                moveMissileObj[i].transform.position += (Vector3)moveMissileDirection[i] * SubWeaponManager.I.baseStatus.GetBulletSpeed(GetWeaponID()) * Time.deltaTime;
                DebugDraw.DebugDrawLine.DrawCircle(moveMissileObj[i].transform.position, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 30, Color.yellow);

                if (HitEnemy(pos))
                {
                    Sound.SoundManager.I.StopSubWeaponSE(Sound.SoundManager.subWeaponAudioID.missile);
                    var effectObj = SubWeaponManager.I.ParticleGeneration(SubWeaponManager.I.baseStatus.GetAttackEffect(GetWeaponID()), pos);
                    if (effectObj != null)
                    {
                        // `GetAttackRange()`に基づいてスケールを設定
                        float attackRange = SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv())/ EFFECT_SIZE;
                        effectObj.transform.localScale = new Vector3(attackRange, attackRange, attackRange);
                    }
                    Sound.SoundManager.I.PlaySubWeaponSE(Sound.SoundManager.subWeaponAudioID.landmine, false);
                    CheckHitEnemy(pos);
                    CheckHitMidBoss(pos);
                    CheckHitBoss(pos);

                    SubWeaponManager.I.DestroyObj(moveMissileObj[i]);
                    moveMissileObj.RemoveAt(i);
                    moveMissileDirection.RemoveAt(i);
                    i--;

                    if (i == -1)
                        return;
                }

                if (DestroyMissile(i))
                {
                    SubWeaponManager.I.DestroyObj(moveMissileObj[i]);
                    moveMissileObj.RemoveAt(i);
                    moveMissileDirection.RemoveAt(i);
                    i--;

                    if (i == -1)
                        return;
                }
            }

        }

        private bool DestroyMissile(int index)
        {
            var destroy = false;

            if (CameraManager.I.GetMainCameraPos().position.x + SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()).x < moveMissileObj[index].transform.position.x)
                destroy = true;    
            if (CameraManager.I.GetMainCameraPos().position.x - SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()).x > moveMissileObj[index].transform.position.x)
                destroy = true;
            if (CameraManager.I.GetMainCameraPos().position.y + SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()).y < moveMissileObj[index].transform.position.y)
                destroy = true;
            if (CameraManager.I.GetMainCameraPos().position.y - SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()).y > moveMissileObj[index].transform.position.y)
                destroy = true;

            return destroy;
        }

        private bool HitEnemy(Vector2 missilePos)
        {
            // 敵との接触を確認
            var hitEnemyList = Physics2D.OverlapCircleAll(missilePos, 0.2f, Enemy.EnemyManager.I.enemyController.GetEnemyLayer());
            if (hitEnemyList.Length != 0)
            {
                return true;
            }

            // 中ボスとの接触を確認
            var hitEnemy = Physics2D.OverlapCircle(missilePos, 0.2f, Enemy.EnemyManager.I.midBossController.GetMidBossLayer());
            if (hitEnemy != null)
            {
                return true;
            }

            // ボスとの接触を確認
            hitEnemy = Physics2D.OverlapCircle(missilePos, 0.2f, Enemy.EnemyManager.I.bossController.GetBossLayer());
            if (hitEnemy != null)
            {
                return true;
            }

            return false;
        }

        private void Attack()
        {
            intervalTime = 0;

            var target = CheckTargetPos();
            var targetPos = Vector2.zero;

            switch (target)
            {
                // ターゲットがいなかった場合、上に発射する
                case null:
                    targetPos = Player.PlayerManager.I.GetPlayerTransform().position + Vector3.up;
                    break;

                default:
                    targetPos = target.GetEnemyTransform().position;
                    break;
            }
            var missileObj = SubWeaponManager.I.AttackObjGeneration(SubWeaponManager.I.baseStatus.GetAttackObj(GetWeaponID()), Player.PlayerManager.I.GetPlayerTransform().position);
            float attackRange = SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()) / EFFECT_SIZE;
;
            if (missileObj != null)
            {
                // ParticleSystem のスケールを変更
                missileObj.transform.localScale = new Vector3(attackRange, attackRange, attackRange);
            }
            Sound.SoundManager.I.PlaySubWeaponSE(Sound.SoundManager.subWeaponAudioID.missile, false);
            moveMissileObj.Add(missileObj);

            moveMissileDirection.Add((targetPos - (Vector2)missileObj.transform.position).normalized);

            var diff = ((Vector2)missileObj.transform.position - targetPos);
            missileObj.transform.rotation = Quaternion.FromToRotation(Vector3.down, diff);

            attackCnt++;
        }

        protected override void DebugDrawCollider()
        {
            DebugDraw.DebugDrawLine.DrawBox(CameraManager.I.GetMainCameraPos().position, SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()), Color.red);
        }

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.missile;
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
