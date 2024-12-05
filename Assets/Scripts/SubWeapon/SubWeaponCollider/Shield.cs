using Sound;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SubWeapon
{
    public class Shield : BaseSubWeapon
    {
        private const float ROTATION_SPEED = 360.0f;
        private float attackIntervalCnt;
        private int attackCnt;
        private List<CircleMove> shieldList = new List<CircleMove>();

        private byte previousLv;
        private float speed;

        private class CircleMove
        {
            public Transform shield;
            public float angle;
        }

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.shield;
        }


        protected override void AttackStart()
        {
            previousLv = CurrentSubWeaponLv();
            switch (previousLv)
            {
                case 0:
                    speed = 360.0f * 2.5f / 5.0f;
                    break;

                case 1:
                    speed = 360.0f * 3.0f / 5.0f;
                    break;

                default:
                    speed = 360.0f * 3.5f / 5.0f;
                    break;
            }

            var shieldAmount = SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), previousLv);
            for (int i = 0; i < shieldAmount; i++)
            {
                var transform = SubWeaponManager.I.AttackObjGeneration(SubWeaponManager.I.baseStatus.GetAttackObj(GetWeaponID()), Player.PlayerManager.I.GetPlayerTransform().position).transform;
                var moveObj = new CircleMove()
                {
                    shield = transform,
                    angle = (360 / shieldAmount) - (360 / shieldAmount) * i,
                };
                shieldList.Add(moveObj);
            }
            Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.shield, true);
            attackIntervalCnt = 0.0f;
        }

        protected override void AttackProcess()
        {
            var time = Time.deltaTime;
            var center = (Vector2)Player.PlayerManager.I.GetPlayerTransform().position;

            var index = 0;
            var indexList = new List<int>();
            foreach (var moveObj in shieldList)
            {
                var radius = moveObj.angle * Mathf.PI / 180.0f;

                moveObj.shield.position = new Vector3(center.x + Mathf.Cos(radius) * SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()).x,
                                                    center.y + Mathf.Sin(radius) * SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()).y, 0.0f);

                shieldList[index].angle += speed * time;

                // Boomerang‚ð‰ñ“]‚³‚¹‚é
                moveObj.shield.Rotate(0, 0, ROTATION_SPEED * time);

                attackIntervalCnt += time;
                if (attackIntervalCnt >= SubWeaponManager.I.baseStatus.GetNextAttackInterval(GetWeaponID()))
                {
                    attackIntervalCnt = 0;
                    CheckHitEnemy(moveObj.shield.position);
                    CheckHitMidBoss(moveObj.shield.position);
                    CheckHitBoss(moveObj.shield.position);
                }

                switch (previousLv)
                {
                    case 0:
                        if (moveObj.angle > 360 * 2.5f)
                            indexList.Add(index);
                        break;

                    case 1:
                        if (moveObj.angle > 360 * 3.0f)
                            indexList.Add(index);
                        break;

                    case 2:
                        if (moveObj.angle > 360 * 3.5f)
                            indexList.Add(index);
                        break;
                }

                index++;
            }

            if (indexList.Count != 0)
            {
                foreach (var i in indexList)
                {
                    SubWeaponManager.I.DestroyObj(shieldList[i].shield.gameObject);
                    shieldList.RemoveAt(i);
                }
            }

            var destroy = false;
            if (previousLv != CurrentSubWeaponLv())
            {
                if (shieldList.Count != 0)
                {
                    for (int i = shieldList.Count - 1; i >= 0; i--)
                    {
                        SubWeaponManager.I.DestroyObj(shieldList[i].shield.gameObject);
                        shieldList.RemoveAt(i);
                    }
                }
                destroy = true;
            }

            if (shieldList.Count == 0 || destroy)
            {
                AttackEnd();
                shieldList.Clear();
                Sound.SoundManager.I.StopSubWeaponSE(SoundManager.subWeaponAudioID.shield);
            }
        }

        protected override void DebugDrawCollider()
        {
            if(shieldList.Count != 0)
            {
                foreach (var moveObj in shieldList)
                {
                    DebugDraw.DebugDrawLine.DrawCircle(moveObj.shield.position, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 30, Color.yellow);
                }
            }
        }


        protected override void UpdateProcess()
        {
        }

    }
}
