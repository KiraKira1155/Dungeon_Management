using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SubWeapon.SubWeaponManager;

namespace SubWeapon
{
    public class BallAttack : BaseSubWeapon
    {
        private const float ROTATION_SPEED = 360f;
        // 移動方向ベクトル
        private float intervalTime;
        private List<Transform> ballPos = new List<Transform>();
        private List<Vector2> moveDir = new List<Vector2>();
        private float bulletSpeedUp;

        protected override void AttackStart()
        {
            Attack();
        }

        protected override void AttackProcess()
        {
           if(SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()) > ballPos.Count)
            {
                Attack();
            }
        }

        protected override void UpdateProcess()
        {
            // ボールの位置を更新
            if (ballPos.Count != 0)
            {
                // カメラの境界を取得
                Vector2 cameraPos = CameraManager.I.GetMainCameraPos().position;
                Vector2 detectionSize = SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID());

                // 上下左右の境界を計算
                var topLeft = cameraPos + new Vector2(-detectionSize.x / 2, detectionSize.y / 2);
                var bottomRight = cameraPos + new Vector2(detectionSize.x / 2, -detectionSize.y / 2);

                int ballNum = 0;
                foreach (var ball in ballPos)
                {
                    ball.position += (Vector3)(moveDir[ballNum] * Time.deltaTime);

                    ball.Rotate(0, 0, ROTATION_SPEED * Time.deltaTime);

                    Vector2 hitPos = ball.position;

                    if (topLeft.x >= hitPos.x - 0.5f)
                    {
                        Collision(ball ,hitPos + new Vector2(-0.5f, 0), ballNum);
                    }
                    if (topLeft.y <= hitPos.y + 0.5f)
                    {
                        Collision(ball, hitPos + new Vector2(0, 0.5f), ballNum);
                    }
                    if (bottomRight.y >= hitPos.y - 0.5f)
                    {
                        Collision(ball, hitPos + new Vector2(0, -0.5f), ballNum);
                    }
                    if (bottomRight.x <= hitPos.x + 0.5f)
                    {
                        Collision(ball, hitPos + new Vector2(0.5f, 0), ballNum);
                    }

                    var attackRange = SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv());

                    // 敵との接触を確認
                    var hitEnemieList = Physics2D.OverlapCircleAll(ball.position, attackRange, Enemy.EnemyManager.I.enemyController.GetEnemyLayer());
                    if (hitEnemieList.Length != 0)
                    {
                        Collision(ball, hitEnemieList[0].transform.position, ballNum);
                    }

                    // 中ボスとの接触を確認
                    var hitEnemie = Physics2D.OverlapCircle(ball.position, attackRange, Enemy.EnemyManager.I.midBossController.GetMidBossLayer());
                    if (hitEnemie != null)
                    {
                        Collision(ball, hitEnemie.transform.position, ballNum);
                    }

                    // ボスとの接触を確認
                    hitEnemie = Physics2D.OverlapCircle(ball.position, attackRange, Enemy.EnemyManager.I.bossController.GetBossLayer());
                    if (hitEnemie != null)
                    {
                        Collision(ball, hitEnemie.transform.position, ballNum);
                    }


                    CheckHitEnemy(ball.position);
                    CheckHitMidBoss(ball.position);
                    CheckHitBoss(ball.position);
                    

                    ballNum++;
                }

            }
        }

        private void Attack()
        {
            if(ballPos.Count != 0)
            {
                foreach(var ball in ballPos)
                {
                    SubWeaponManager.I.DestroyObj(ball.gameObject);
                }

                moveDir.Clear();
                ballPos.Clear();
            }


            switch (CurrentSubWeaponLv())
            {
                case 0:
                    bulletSpeedUp = 1 * SubWeaponManager.I.baseStatus.GetBulletSpeed(GetWeaponID());
                    break;

                case 1:
                    bulletSpeedUp = 1.2f * SubWeaponManager.I.baseStatus.GetBulletSpeed(GetWeaponID());
                    break;

                case 2:
                    bulletSpeedUp = 1.5f * SubWeaponManager.I.baseStatus.GetBulletSpeed(GetWeaponID());
                    break;

                case 3:
                    bulletSpeedUp = 1.8f * SubWeaponManager.I.baseStatus.GetBulletSpeed(GetWeaponID());
                    break;
            }

            for (int i = 0; i < SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()); i++)
            {
                var ball = SubWeaponManager.I.AttackObjGeneration(SubWeaponManager.I.baseStatus.GetAttackObj(GetWeaponID()), Player.PlayerManager.I.GetPlayerTransform().position);
                moveDir.Add(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized * bulletSpeedUp);
                ballPos.Add(ball.transform);
            }

        }

        private void Collision(Transform ballPos, Vector2 enemyPos, int ballNum)
        {
            // 敵との衝突時の反射ベクトルを計算
            var normal = ((Vector2)ballPos.position - enemyPos).normalized;
            float volume = Mathf.Abs(Vector3.Dot(moveDir[ballNum], normal));
            moveDir[ballNum] = (moveDir[ballNum] + 2 * volume * normal).normalized * bulletSpeedUp;
            Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.ball, false);
        }



        protected override void DebugDrawCollider()
        {
        }


        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.ball;
        }
    }
}