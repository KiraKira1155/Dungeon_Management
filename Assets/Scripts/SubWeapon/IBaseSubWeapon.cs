using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubWeapon
{
    public abstract class BaseSubWeapon : IBaseSubWeapon
    {
        private bool attackStart;
        private bool isAttack;
        private float attackInterval;
        private float intervalCount;
        private bool hitAttack;

        private List<Enemy.IBaseEnemy> hitEnemy = new List<Enemy.IBaseEnemy>(); // 攻撃が当たった敵の保存用
        private Enemy.IBaseEnemy hitMidBoss;
        private Enemy.IBaseEnemy hitBoss;


        public abstract SubWeaponManager.SubWeaponID GetWeaponID();

        /// <summary>
        /// 当たり判定の表示用
        /// </summary>
        protected abstract void DebugDrawCollider();

        protected abstract void AttackStart();

        protected abstract void AttackProcess();

        protected byte CurrentSubWeaponLv()
        {
            return SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID());
        }

        /// <summary>
        /// Update()内で一番始めに処理される
        /// </summary>
        protected abstract void UpdateProcess();

        public List<Enemy.IBaseEnemy> GetHitEnemyList()
        {
            var enemyList = new List<Enemy.IBaseEnemy>();

            if(hitEnemy.Count != 0)
            {
                enemyList.AddRange(hitEnemy);
            }

            if(hitMidBoss != null)
            {
                enemyList.Add(hitMidBoss);
            }

            if(hitBoss != null)
            {
                enemyList.Add(hitBoss);
            }

            hitEnemy.Clear();
            hitMidBoss = null;
            hitBoss = null;

            return enemyList;
        }

        /// <summary>
        /// 攻撃処理終了用、AttackProcess()の呼び出しが終了する
        /// <para>
        /// AttackStart()内で呼ぶことでAttackProcessの処理を実行せずに攻撃処理を終わらせられる
        /// </para>
        /// </summary>
        protected void AttackEnd()
        {
            attackStart = false;
        }

        public void DoUpdate()
        {
            UpdateProcess();

            AutoAttackIntervalCount();

            if (attackStart)
            {
                AttackProcess();
            }

            if (isAttack)
            {
                AttackStart();
                isAttack = false;
                attackStart = true;
            }

#if UNITY_EDITOR
            DebugDrawCollider();
#endif
        }

        /// <summary>
        /// インターバルの時間を計算
        /// </summary>
        private void AutoAttackIntervalCount()
        {
            if (attackStart)
                return;
            
            attackInterval += Time.deltaTime;

            if (attackInterval >= SubWeaponManager.I.baseStatus.GetAttackInterval(GetWeaponID()))
            {
                isAttack = true;
                attackInterval = 0;
            }
        }

        public void HitEnemyProcessEnd()
        {
            hitAttack = false;
        }

        public bool GetIsHitEnemy()
        {
            return hitAttack;
        }

        /// <summary>
        /// 攻撃の最大距離半径内にいる敵をすべて取得する
        /// </summary>
        /// <param name="pos">攻撃を行う中心座標</param>
        /// <returns>範囲内全敵のCollider2D配列</returns>
        protected Collider2D[] GetHitEnemyCollider(Vector2 pos)
        {
            var enemy = Physics2D.OverlapCircleAll
                (pos, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID())), Enemy.EnemyManager.I.enemyController.GetEnemyLayer());
            
            if(enemy.Length == 0)
                return null;
            
            return enemy;
        }

        protected Collider2D GetHitMidBossCollider(Vector2 pos)
        {
            return Physics2D.OverlapCircle
                (pos, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID())), Enemy.EnemyManager.I.midBossController.GetMidBossLayer());
        }
        protected Collider2D GetHitBossCollider(Vector2 pos)
        {
            return Physics2D.OverlapCircle
                (pos, SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID())), Enemy.EnemyManager.I.bossController.GetBossLayer());
        }

        /// <summary>
        /// 攻撃が当たっているかの処理
        /// <para>
        /// 当たっていた場合自動で当たった敵は保存される
        /// </para>
        /// </summary>
        /// <param name="pos">攻撃を行う中心座標</param>
        protected void CheckHitEnemy(Vector2 pos)
        {
            var hit = Physics2D.OverlapCircleAll
                (
                pos,
                SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID())),
                Enemy.EnemyManager.I.enemyController.GetEnemyLayer()
                );

            if (hit.Length != 0)
                HitEnemy(hit);
        }


        /// <summary>
        /// 攻撃が当たった敵の保存用、中ボスはHitMidBoss()・ボスはHitBoss()に入れる
        /// </summary>
        /// <param name="hitEnemyCollider">当たった敵のコライダー</param>
        private void HitEnemy(Collider2D[] hitEnemyCollider)
        {
            hitAttack = true;

            foreach (var enemy in Enemy.EnemySpawnManager.I.GetAllMapEnemy())
            {
                foreach (var target in hitEnemyCollider)
                {
                    if (enemy.GetEnemyCollider() == target)
                    {
                        hitEnemy.Add(enemy);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 攻撃が当たっているかの処理
        /// <para>
        /// 当たっていた場合自動で当たった敵は保存される
        /// </para>
        /// </summary>
        /// <param name="pos">攻撃を行う中心座標</param>
        protected void CheckHitMidBoss(Vector2 pos)
        {
            var hit = Physics2D.OverlapCircle
                (
                pos,
                SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID())),
                Enemy.EnemyManager.I.midBossController.GetMidBossLayer()
                );

            if (hit != null)
                HitMidBoss(hit);
        }

        /// <summary>
        /// 中ボスに攻撃が当たった時の保存用
        /// </summary>
        /// <param name="hitMidBossCollider">当たった中ボスのコライダー</param>
        private void HitMidBoss(Collider2D hitMidBossCollider)
        {
            hitAttack = true;

            var enemy = Enemy.EnemySpawnManager.I.GetMidBossEnemy();
            if (enemy.GetEnemyCollider() == hitMidBossCollider)
            {
                hitMidBoss = enemy;
            }
        }

        /// <summary>
        /// 攻撃が当たっているかの処理
        /// <para>
        /// 当たっていた場合自動で当たった敵は保存される
        /// </para>
        /// </summary>
        /// <param name="pos">攻撃を行う中心座標</param>
        protected void CheckHitBoss(Vector2 pos)
        {
            var hit = Physics2D.OverlapCircle
                (
                pos,
                SubWeaponManager.I.baseStatus.GetAttackRange(GetWeaponID(), SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID())),
                Enemy.EnemyManager.I.bossController.GetBossLayer()
                );

            if (hit != null)
                HitBoss(hit);
        }

        /// <summary>
        /// ボスに攻撃が当たった時の保存用
        /// </summary>
        /// <param name="hitBossCollider">当たったボスのコライダー</param>
        private void HitBoss(Collider2D hitBossCollider)
        {
            hitAttack = true;

            var enemy = Enemy.EnemySpawnManager.I.GetBossEnemy();
            if (enemy.GetEnemyCollider() == hitBossCollider)
            {
                hitBoss = enemy;
            }
        }
    }

    public interface IBaseSubWeapon
    {
        /// <summary>
        /// Update()内で呼び出すよう
        /// </summary>
        abstract void DoUpdate();

        /// <summary>
        /// 識別IDの取得用
        /// </summary>
        /// <returns></returns>
        abstract SubWeaponManager.SubWeaponID GetWeaponID();

        /// <summary>
        /// 敵への攻撃処理終了
        /// </summary>
        abstract void HitEnemyProcessEnd();

        /// <summary>
        /// 攻撃が当たったかの確認用
        /// </summary>
        /// <returns>当たっていれば true が返る</returns>
        abstract bool GetIsHitEnemy();

        /// <summary>
        /// 攻撃が当たった敵の取得用
        /// </summary>
        /// <returns>当たった敵のスクリプト</returns>
        abstract List<Enemy.IBaseEnemy> GetHitEnemyList();

    }
}
