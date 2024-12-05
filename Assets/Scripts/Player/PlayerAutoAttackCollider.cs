using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerAutoAttackCollider : MonoBehaviour
    {
        [SerializeField] private float _autoAttackAngle;
        [SerializeField] private float _autoAttackDistance;
        [SerializeField] private Transform _attackTransform;
        [SerializeField] private Quaternion _initAngle;
        [SerializeField] private Vector3 _attackPos;
        [SerializeField] private float angleModification;
        [Header("エフェクト設定")]
        [SerializeField] private GameObject autoAttackEffectPrefab;
        [SerializeField] private float distanceModification;

        public float autoAttackAngle { get { return _autoAttackAngle; } }
        public float autoAttackDistance { get { return _autoAttackDistance; } }
        public Transform attackTransform { get { return _attackTransform; } }
        public Quaternion initAngle { get { return _initAngle; } }
        public Vector3 attackPos { get { return _attackPos; } }

        private List<GameObject> hitEnemy = new List<GameObject>();

        private List<Enemy.IBaseEnemy> hitEnemyScripts = new List<Enemy.IBaseEnemy>();
        private Collider2D hitMidBossCollider = new Collider2D();
        private Collider2D hitBossCollider = new Collider2D();

        private int hitAmount;

        // 攻撃判定用変数
        private Vector2 diff;
        private float enemyPosRadian;
        private float enemyPosAngle; // プレイヤーから見た敵キャラのいる方向

        private float currentPlayerAngle;
        private float targetAngle; // 攻撃範囲（℃）の半分、最小と最大の計算用
        private float minDetectionRange; // 最小検知範囲
        private float maxDetectionRange; // 最大検知範囲
        private float enemyAngleModification;
        private bool doAngleCorrection;

        private List<Transform> autoAttackEffects = new List<Transform>();
        private List<float> autoAttackEffectsMoveDistance = new List<float>();
        private List<Vector2> autoAttackEffectsDirections = new List<Vector2>();

        private float effectMoveSpeed;
        private float angle;
        private Vector2 inputDirection;

        private void Awake()
        {
            targetAngle = autoAttackAngle / 2;
            angle = 90;
            inputDirection = new Vector2(0, 1);
        }

        private void Update()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
                return;

            var input = JoyStickManager.I.GetInputDirection();
            if (input != Vector2.zero)
            {
                inputDirection = input;
            }

            if (autoAttackEffects.Count != 0)
            {
                var time = Time.deltaTime;
                var indexList = new List<int>();
                for (int i = 0; i < autoAttackEffects.Count; i++)
                {
                    var moveSpeed = autoAttackEffectsDirections[i] * time * 8;
                    autoAttackEffects[i].position += (Vector3)moveSpeed;
                    autoAttackEffectsMoveDistance[i] += moveSpeed.magnitude;
                    if (autoAttackEffectsMoveDistance[i] > autoAttackDistance - distanceModification)
                    {
                        indexList.Add(i);
                        Destroy(autoAttackEffects[i].gameObject);
                    }
                }

                for(int i = indexList.Count - 1;i >= 0; i--)
                {
                    autoAttackEffects.RemoveAt(indexList[i]);
                    autoAttackEffectsMoveDistance.RemoveAt(indexList[i]);
                    autoAttackEffectsDirections.RemoveAt(indexList[i]);
                }
            }
        }

        /// <summary>
        /// 攻撃方向の設定用
        /// </summary>
        /// <param name="angle"></param>
        public void SetAttackDirection(float angle)
        {
            _attackTransform.rotation = Quaternion.Euler(0, 0, angle - angleModification);
        }

        /// <summary>
        /// 攻撃範囲の設定用
        /// </summary>
        /// <param name="angle"></param>
        public void SetAttackAngle(float angle)
        {
            _autoAttackAngle = angle;
            targetAngle = angle / 2;
        }

        /// <summary>
        /// 攻撃距離の設定用
        /// </summary>
        /// <param name="distance"></param>
        public void SetAttackDistance(float distance)
        {
            _autoAttackDistance += distance;
        }

        private void AttackStart()
        {
            angle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;
            var radian = 0.0f;
            var ang = 0.0f;
            switch (PlayerManager.I.currentStatus.GetWeaponLv())
            {
                case 0:
                    GenerateEffect(angle, inputDirection);
                    break;

                case 1:
                    ang = angle + 45;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle - 45;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));
                    break;

                case 2:
                    ang = angle + 30;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle - 30;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle + 65;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle - 65;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));
                    break;

                case 3:
                    ang = angle;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle + 45;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle + 90;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle + 135;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);

                    ang = angle + 180;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle + 225;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle + 270;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    GenerateEffect(ang, new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));

                    ang = angle + 315;
                    radian = Mathf.Repeat(ang, 360) * (Mathf.PI / 180);
                    break;
            }


        }

        private void GenerateEffect(float angle, Vector2 direction)
        {
            direction = direction.normalized;
            var effect = Instantiate(autoAttackEffectPrefab);
            effect.transform.position = PlayerManager.I.GetPlayerTransform().position;
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);

            autoAttackEffects.Add(effect.transform);
            autoAttackEffectsMoveDistance.Add(0);
            autoAttackEffectsDirections.Add(direction);
        }

        /// <summary>
        /// オート攻撃が当たった敵の取得用
        /// </summary>
        /// <returns>当たった敵のスクリプトが返る</returns>
        public Enemy.IBaseEnemy[] GetHitEnemy()
        {
            hitEnemyScripts.Clear();

            if (Enemy.EnemySpawnManager.I.GetAllMapEnemy().Length != 0)
            {
                foreach (var enemy in Enemy.EnemySpawnManager.I.GetAllMapEnemy())
                {
                    foreach (var hit in hitEnemy)
                    {
                        if (enemy.GetEnemyGameObject() == hit)
                        {
                            hitEnemyScripts.Add(enemy);
                            break;
                        }
                    }
                }
            }


            if (Enemy.EnemySpawnManager.I.GetMidBossEnemy() != null)
            {
                if (Enemy.EnemySpawnManager.I.GetMidBossEnemy().GetEnemyCollider() == hitMidBossCollider)
                {
                    hitEnemyScripts.Add(Enemy.EnemySpawnManager.I.GetMidBossEnemy());
                }
            }

            return hitEnemyScripts.ToArray();
        }

        /// <summary>
        /// オート攻撃が当たった敵の数取得用
        /// </summary>
        /// <returns></returns>
        public int GetHitEnemyAmount()
        {
            return hitAmount;
        }

        /// <summary>
        /// 攻撃処理後に呼び出す
        /// </summary>
        public void HitEnemyInfoClear()
        {
            hitEnemy.Clear();
        }

        /// <summary>
        /// オート攻撃の対象範囲にいる敵の判定用
        /// </summary>
        public void AutoAttackTargetCalculation()
        {
            AttackStart();

            // 全方位の場合
            if (autoAttackAngle == 360)
            {
                hitMidBossCollider = CheckHitMidBoss();
                if (hitMidBossCollider != null)
                    hitEnemy.Add(hitMidBossCollider.gameObject);

                foreach (var enemy in CheckHitEnemy())
                    hitEnemy.Add(enemy.gameObject);

                hitAmount += hitEnemy.Count;
                return;
            }

            currentPlayerAngle = attackTransform.localEulerAngles.z;
            minDetectionRange = Mathf.Repeat((currentPlayerAngle - targetAngle), 360);
            maxDetectionRange = Mathf.Repeat((currentPlayerAngle + targetAngle), 360);

            if (maxDetectionRange - minDetectionRange > 0)
                doAngleCorrection = false;
            else if (maxDetectionRange - minDetectionRange < 0)
                doAngleCorrection = true;
            else 
                doAngleCorrection = false;

            foreach (var enemy in CheckHitEnemy())
            {
                diff = enemy.transform.position - attackTransform.position;
                enemyPosRadian = Mathf.Atan2(diff.y, diff.x);
                enemyPosAngle = Mathf.Repeat(enemyPosRadian * Mathf.Rad2Deg - angleModification, 360);

                if (doAngleCorrection)
                {
                    if(enemyPosAngle > 180)
                    {
                        if(360 > enemyPosAngle && minDetectionRange < enemyPosAngle)
                            hitEnemy.Add(enemy.gameObject);
                    }
                    else
                    {
                        if(0 < enemyPosAngle && maxDetectionRange > enemyPosAngle)
                            hitEnemy.Add(enemy.gameObject);
                    }
                }
                else
                {
                    if (minDetectionRange < enemyPosAngle && maxDetectionRange > enemyPosAngle)
                        hitEnemy.Add(enemy.gameObject);
                }
            }


            hitMidBossCollider = CheckHitMidBoss();
            if (hitMidBossCollider != null)
            {
                diff = hitMidBossCollider.transform.position - attackTransform.position;
                enemyPosRadian = Mathf.Atan2(diff.y, diff.x);
                enemyPosAngle = Mathf.Repeat(enemyPosRadian * Mathf.Rad2Deg - angleModification, 360);

                if (doAngleCorrection)
                {
                    if (enemyPosAngle > 180)
                    {
                        if (360 > enemyPosAngle && minDetectionRange < enemyPosAngle)
                            hitEnemy.Add(hitMidBossCollider.gameObject);
                    }
                    else
                    {
                        if (0 < enemyPosAngle && maxDetectionRange > enemyPosAngle)
                            hitEnemy.Add(hitMidBossCollider.gameObject);
                    }
                }
                else
                {
                    if (minDetectionRange < enemyPosAngle && maxDetectionRange > enemyPosAngle)
                        hitEnemy.Add(hitMidBossCollider.gameObject);
                }
            }

            hitBossCollider = CheckHitBoss();
            if(hitBossCollider != null)
            {
                diff = hitBossCollider.transform.position - attackTransform.position;
                enemyPosRadian = Mathf.Atan2(diff.y, diff.x);
                enemyPosAngle = Mathf.Repeat(enemyPosRadian * Mathf.Rad2Deg - angleModification, 360);

                if (doAngleCorrection)
                {
                    if (enemyPosAngle > 180)
                    {
                        if (360 > enemyPosAngle && minDetectionRange < enemyPosAngle)
                            hitEnemy.Add(hitBossCollider.gameObject);
                    }
                    else
                    {
                        if (0 < enemyPosAngle && maxDetectionRange > enemyPosAngle)
                            hitEnemy.Add(hitBossCollider.gameObject);
                    }
                }
                else
                {
                    if (minDetectionRange < enemyPosAngle && maxDetectionRange > enemyPosAngle)
                        hitEnemy.Add(hitBossCollider.gameObject);
                }
            }
           

            hitAmount = hitEnemy.Count;
        }

        /// <summary>
        /// オート攻撃の最大距離半径内にいる敵をすべて取得する
        /// </summary>
        /// <returns>範囲内全敵のCollider2D配列</returns>
        private Collider2D[] CheckHitEnemy()
        {
            var hit = Physics2D.OverlapCircleAll(attackTransform.position, autoAttackDistance, Enemy.EnemyManager.I.enemyController.GetEnemyLayer());
            return hit;
        }

        /// <summary>
        /// オート攻撃の最大距離半径内にいる中ボスを取得する
        /// </summary>
        /// <returns>範囲内全敵のCollider2D配列</returns>
        private Collider2D CheckHitMidBoss()
        {
            var hit = Physics2D.OverlapCircle(attackTransform.position, autoAttackDistance, Enemy.EnemyManager.I.midBossController.GetMidBossLayer());
            return hit;
        }

        private Collider2D CheckHitBoss()
        {
            var hit = Physics2D.OverlapCircle(attackTransform.position, autoAttackDistance, Enemy.EnemyManager.I.bossController.GetBossLayer());
            return hit;
        }
    }
}
