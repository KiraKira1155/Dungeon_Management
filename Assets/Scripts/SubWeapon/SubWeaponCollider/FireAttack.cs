using Sound;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace SubWeapon
{
    public class FireAttack : BaseSubWeapon
    {
        private const float ROTATION_SPEED = 360f;
        private const float SUMMON_FIRE_ATTACK = 0.1f;
        private const float DESTROY_TYME = 5.0f;

        private float summonIntervalTime;  //攻撃間隔
        private int attackCnt;
        private List<GameObject> fireAttackObj = new List<GameObject>();
        private List<GameObject> moveFireObj = new List<GameObject>();
        private List<Vector2> moveFireTargetPos = new List<Vector2>();
        private List<float> fireAttackIntervalTime = new List<float>();
        private List<float> fireAttackDestroyTime = new List<float>();

        protected override void AttackStart()
        {
            attackCnt = 0;
            summonIntervalTime = 0;
            SummonFireAttack();

            if (attackCnt == SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()))
                AttackEnd();
        }

        protected override void AttackProcess()
        {

            summonIntervalTime += Time.deltaTime;
            if (summonIntervalTime >= SUMMON_FIRE_ATTACK)
            {
                SummonFireAttack();
            }

            if (attackCnt == SubWeaponManager.I.baseStatus.GetTargetAmount(GetWeaponID(), CurrentSubWeaponLv()))
            {
                AttackEnd();
                return;
            }

        }
        protected override void UpdateProcess()
        {

            // 各オブジェクトを移動
            for (int i = 0; i < moveFireObj.Count; i++)
            {
                Vector2 direction = (moveFireTargetPos[i] - (Vector2)moveFireObj[i].transform.position).normalized;

                // オブジェクトを移動
                moveFireObj[i].transform.position += (Vector3)direction * SubWeaponManager.I.baseStatus.GetBulletSpeed(GetWeaponID()) * Time.deltaTime;
                moveFireObj[i].transform.rotation = Quaternion.FromToRotation(Vector3.down, direction);

                // ターゲット位置に到達したら
                if (Vector2.Distance(moveFireObj[i].transform.position, moveFireTargetPos[i]) < 0.1f)
                {
                    Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.fastFire, false);
                    fireAttackObj.Add(SubWeaponManager.I.ParticleGeneration(SubWeaponManager.I.baseStatus.GetAttackEffect(GetWeaponID()), moveFireTargetPos[i] ));

                    fireAttackIntervalTime.Add(0);
                    fireAttackDestroyTime.Add(0);

                    var particleSystem = fireAttackObj[fireAttackObj.Count - 1].GetComponent<ParticleSystem>();
                 if (particleSystem != null)
                {
                    var mainModule = particleSystem.main;
                    mainModule.startSize = SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv());
                }

                    SubWeaponManager.I.DestroyObj(moveFireObj[i]);                   
                    moveFireObj.RemoveAt(i);
                    moveFireTargetPos.RemoveAt(i);
                    Sound.SoundManager.I.PlaySubWeaponSE(SoundManager.subWeaponAudioID.fireAttack, false);
                    i--; 
                }

            }

            var time = Time.deltaTime;


            for (int i = 0; i < fireAttackObj.Count; i++)
            {
              
                fireAttackIntervalTime[i] += time;
                fireAttackDestroyTime[i] += time;


                if (fireAttackIntervalTime[i] > SubWeaponManager.I.baseStatus.GetNextAttackInterval(GetWeaponID()))
                {
                    Attack(i);
                }


                if (fireAttackDestroyTime[i] > DESTROY_TYME)
                {
                    SubWeaponManager.I.DestroyObj(fireAttackObj[i]);
                    Sound.SoundManager.I.StopSubWeaponSE(SoundManager.subWeaponAudioID.fireAttack);
                    fireAttackObj[i] = null;
                }
            }


            for (int i = fireAttackObj.Count - 1; i >= 0; i--)
            {
                if (fireAttackObj[i] == null)
                {

                    fireAttackObj.RemoveAt(i);
                    fireAttackIntervalTime.RemoveAt(i);
                    fireAttackDestroyTime.RemoveAt(i);
                }
            }
        }

     

 
        private void Attack(int index)
        {
            fireAttackIntervalTime[index] = 0;           
            var pos = fireAttackObj[index].transform.position;
            CheckHitEnemy(pos);
            CheckHitMidBoss(pos);
            CheckHitBoss(pos);
        }

        private void SummonFireAttack()
        {
            summonIntervalTime = 0;

            ExecuteFireAttack();
            moveFireObj.Add(SubWeaponManager.I.AttackObjGeneration(SubWeaponManager.I.baseStatus.GetAttackObj(GetWeaponID()), Player.PlayerManager.I.GetPlayerTransform().position));

            attackCnt++;
        }
      
        protected override void DebugDrawCollider()
        {
            if (fireAttackObj.Count == 0)
                return;

            foreach(var fire in fireAttackObj)
            {
                DebugDraw.DebugDrawLine.DrawCircle(fire.transform.position, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), CurrentSubWeaponLv()), 15, Color.yellow);
            }
        }

       

        public override SubWeaponManager.SubWeaponID GetWeaponID()
        {
            return SubWeaponManager.SubWeaponID.fireAttack;
        }

     
        private void ExecuteFireAttack()
        {
            float angleOffset = 0f;

            // 各レベルに応じた設定
            switch (CurrentSubWeaponLv())
            {
                case 0:
                    angleOffset = 180.0f;
                    break;
                case 1:
                    angleOffset = 90.0f;
                    break;
                case 2:
                    angleOffset = 60.0f;
                    break;
                case 3:
                    angleOffset = 36.0f;
                    break;
                default:
                    return; 
            }

            // 角度計算
            float angle = angleOffset * attackCnt;
            float radian = angle * Mathf.Deg2Rad;

            // 攻撃位置を計算
            moveFireTargetPos.Add((Vector2)Player.PlayerManager.I.GetPlayerTransform().position + new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * SubWeaponManager.I.baseStatus.GetDetectionSize(GetWeaponID()));
        }
    }
}


