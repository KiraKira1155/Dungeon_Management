using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    public class Boomerang : BaseSubWeapon
    {
        private const float ROTATION_SPEED = 360.0f;
        private float rotationSpeed = 2.0f;

        private float attackIntervalCnt;

        private CircleMove circleMove;
        private struct CircleMove
        {
            public Transform boomerang;
            public Vector2 center;
            public float angle;
            public float length;
        }

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.boomerang;
        }

        protected override void AttackStart()
        {
           Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.boomerang, true);           
           var obj = SubWeaponManager.I.AttackObjGeneration(SubWeaponManager.I.baseStatus.GetAttackObj(GetWeaponID()), Player.PlayerManager.I.GetPlayerTransform().position);
           
            circleMove = new CircleMove()
            {
                boomerang = obj.transform,
                center = obj.transform.position,
                angle = 0.0f,
                length = 0.0f
            };
            attackIntervalCnt = 0.0f;
        }

        protected override void AttackProcess()
        {
            var time = Time.deltaTime;
            var radius = circleMove.angle * Mathf.PI / 180.0f;
            circleMove.boomerang.position = new Vector3(circleMove.center.x + Mathf.Cos(radius) * circleMove.length, circleMove.center.y + Mathf.Sin(radius) * circleMove.length, 0.0f);
            circleMove.angle += SubWeaponManager.I.baseStatus.GetBulletSpeed(GetWeaponID());
            // ™X‚É‰“‚­‚É‚¢‚­Ý’è
            circleMove.length += (float)SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()) / 1000.0f;

            circleMove.boomerang.Rotate(0, 0, ROTATION_SPEED * time * rotationSpeed);

            attackIntervalCnt += time;
            if(attackIntervalCnt >= SubWeaponManager.I.baseStatus.GetNextAttackInterval(GetWeaponID()))
            {
                attackIntervalCnt = 0.0f;
                CheckHitEnemy(circleMove.boomerang.position);
                CheckHitMidBoss(circleMove.boomerang.position);
                CheckHitBoss(circleMove.boomerang.position);
            }

            var destroy = false;
            switch (CurrentSubWeaponLv())
            {
                case 0:
                    if (circleMove.angle > 360 * 3)
                        destroy = true;

                    break;


                case 1:
                    if (circleMove.angle > 360 * 4)
                        destroy = true;
                    break;

                default:
                    if (circleMove.angle > 360 * 5)
                        destroy = true;
                    break;
            }


            if (destroy)
            {
                SubWeaponManager.I.DestroyObj(circleMove.boomerang.gameObject);
                circleMove.boomerang = null;
                AttackEnd();
                Sound.SoundManager.I.StopSubWeaponSE(SoundManager.subWeaponAudioID.boomerang);
            }

        }

        protected override void DebugDrawCollider()
        {
            if(circleMove.boomerang != null)
                DebugDraw.DebugDrawLine.DrawCircle(circleMove.boomerang.position, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 30, Color.yellow);
        }

        protected override void UpdateProcess()
        {
        }
    }
}
