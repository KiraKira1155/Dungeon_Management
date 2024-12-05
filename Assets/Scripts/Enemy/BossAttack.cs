using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BossAttack : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private GameObject meteorPrefab;
        [SerializeField] private float detectionRadiusForPlayer;

        private float attackTimer = 0f;
        private bool isAttacking = false;
        private int lastAttackType = 0;
        private float currentDistance = 0f;
        private const float totalDistance = 10f;
        private Vector3 targetPosition;
        private Transform bossTransform;
        private Collider2D hitEntity;

        private float attackStartTime;

        // SpecialAttack2用の弾リスト
        private List<GameObject> activeBullets = new List<GameObject>();

        private void Awake()
        {
            if (bossTransform == null)
            {
                bossTransform = transform;
            }
        }

        private void Update()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
                return;

            if (!isAttacking)
            {
                attackTimer += Time.deltaTime;

                // 3秒ごとに攻撃を行う
                if (attackTimer >= 3f)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                {
                    AttackRoutine();
                    attackTimer = 0f;
                }
            }
            else
            {
                ExecuteCurrentAttack();
            }

            UpdateBullets(); // 弾の更新処理
        }

        private void AttackRoutine()
        {
            int attackType;

            do
            {
                attackType = Random.Range(1, 4);
            } while (attackType == lastAttackType);

            lastAttackType = attackType;
            isAttacking = true;
            currentDistance = 0f;

            switch (attackType)
            {
                case 1:
                    targetPosition = PlayerManager.I.GetPlayerTransform().position;
                    break;
                case 2:
                    attackStartTime = Time.time;
                    SpecialAttack2();
                    isAttacking = false;
                    break;
                case 3:
                    attackStartTime = Time.time;
                    SpecialAttack3();
                    isAttacking = false;
                    break;
            }
        }

        private void ExecuteCurrentAttack()
        {
            if (lastAttackType == 1)
            {
                UpdateSpecialAttack1();
            }

            // 突進が完了した条件
            if (currentDistance >= totalDistance)
            {
                isAttacking = false;
            }
        }

        private Collider2D CheckOverlapCirclePlayer(Vector2 pos)
        {
            Collider2D hit = Physics2D.OverlapCircle(pos, detectionRadiusForPlayer, Player.PlayerManager.I.GetPlayerLayer());
            return hit;
        }

        private void UpdateSpecialAttack1()
        {
            targetPosition = PlayerManager.I.GetPlayerTransform().position;
            Vector3 playerDirection = (targetPosition - transform.position).normalized;
            float distance = moveSpeed * Time.deltaTime;

            transform.position += playerDirection * distance;
            currentDistance += distance;

            if (transform.position.x < EnemyManager.I.bossController.moveLimitsMin.x || transform.position.x > EnemyManager.I.bossController.moveLimitsMax.x || transform.position.y < EnemyManager.I.bossController.moveLimitsMin.y || transform.position.y > EnemyManager.I.bossController.moveLimitsMax.y)
            {
                transform.position = new Vector3(
                    Mathf.Clamp(transform.position.x, EnemyManager.I.bossController.moveLimitsMin.x, EnemyManager.I.bossController.moveLimitsMax.x),
                    Mathf.Clamp(transform.position.y, EnemyManager.I.bossController.moveLimitsMin.y, EnemyManager.I.bossController.moveLimitsMax.y),
                    transform.position.z
                );
            }
        }

        private void SpecialAttack2()
        {
            float angleStep = 360f / 32;
            float angle = 0f;
            Vector2 bossPosition = transform.position;
            float moveSpeed = 7.0f;
            float lifeTime = 2.0f;

            // 弾の生成
            for (int i = 0; i < 32; i++)
            {
                GameObject bullet = Instantiate(bulletPrefab, bossPosition, Quaternion.identity);
                Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
                bullet.transform.up = direction;

                // 弾に必要な情報を保持してリストに追加
                activeBullets.Add(bullet);
                bullet.AddComponent<BulletData>().Initialize(direction, moveSpeed, lifeTime);

                angle += angleStep;
            }
        }

        // SpecialAttack2で生成した弾の更新処理
        private void UpdateBullets()
        {
            for (int i = activeBullets.Count - 1; i >= 0; i--)
            {
                GameObject bullet = activeBullets[i];
                BulletData bulletData = bullet.GetComponent<BulletData>();

                // 弾の移動
                bullet.transform.Translate(bulletData.Direction * bulletData.Speed * Time.deltaTime);

                // 当たり判定
                Collider2D hitEntity = Physics2D.OverlapCircle(bullet.transform.position, 0.2f, PlayerManager.I.GetPlayerLayer());
                if (hitEntity != null)
                {
                    PlayerManager.I.currentStatus.BeAttacked(EnemyManager.I.basicStatus.GetEnemyStatusATK(EnemyManager.EnemyCharactersID.boss));
                    Destroy(bullet);
                    activeBullets.RemoveAt(i); // リストから削除
                    continue;
                }

                // 寿命の確認
                bulletData.TimeElapsed += Time.deltaTime;
                if (bulletData.TimeElapsed >= bulletData.LifeTime)
                {
                    Destroy(bullet);
                    activeBullets.RemoveAt(i); // リストから削除
                }
            }
        }

        private void SpecialAttack3()
        {
            Vector2 playerPosition = PlayerManager.I.GetPlayerTransform().position;

            for (int i = 0; i < 7; i++)
            {
                Vector2 randomPosition;

                do
                {
                    randomPosition = playerPosition + Random.insideUnitCircle * 3;
                } while (randomPosition.x < EnemyManager.I.bossController.moveLimitsMin.x || randomPosition.x > EnemyManager.I.bossController.moveLimitsMax.x || randomPosition.y < EnemyManager.I.bossController.moveLimitsMin.y || randomPosition.y > EnemyManager.I.bossController.moveLimitsMax.y);

                GameObject meteorInstance = Instantiate(meteorPrefab, randomPosition, Quaternion.identity);

                Destroy(meteorInstance, 0.5f);

                hitEntity = CheckOverlapCirclePlayer(randomPosition);
                if (hitEntity != null)
                {
                    PlayerManager.I.currentStatus.BeAttacked(EnemyManager.I.basicStatus.GetEnemyStatusATK(EnemyManager.EnemyCharactersID.boss) / 2);
                }
            }
        }
    }

    // 弾の情報を保持するためのクラス
    public class BulletData : MonoBehaviour
    {
        public Vector2 Direction;
        public float Speed;
        public float LifeTime;
        public float TimeElapsed;

        public void Initialize(Vector2 direction, float speed, float lifeTime)
        {
            Direction = direction;
            Speed = speed;
            LifeTime = lifeTime;
            TimeElapsed = 0f;
        }
    }
}