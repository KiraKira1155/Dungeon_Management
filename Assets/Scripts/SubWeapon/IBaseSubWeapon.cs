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

        private List<Enemy.IBaseEnemy> hitEnemy = new List<Enemy.IBaseEnemy>(); // �U�������������G�̕ۑ��p
        private Enemy.IBaseEnemy hitMidBoss;
        private Enemy.IBaseEnemy hitBoss;


        public abstract SubWeaponManager.SubWeaponID GetWeaponID();

        /// <summary>
        /// �����蔻��̕\���p
        /// </summary>
        protected abstract void DebugDrawCollider();

        protected abstract void AttackStart();

        protected abstract void AttackProcess();

        protected byte CurrentSubWeaponLv()
        {
            return SubWeaponManager.I.GetSubWeaponCurrentLv(GetWeaponID());
        }

        /// <summary>
        /// Update()���ň�Ԏn�߂ɏ��������
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
        /// �U�������I���p�AAttackProcess()�̌Ăяo�����I������
        /// <para>
        /// AttackStart()���ŌĂԂ��Ƃ�AttackProcess�̏��������s�����ɍU���������I��点����
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
        /// �C���^�[�o���̎��Ԃ��v�Z
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
        /// �U���̍ő勗�����a���ɂ���G�����ׂĎ擾����
        /// </summary>
        /// <param name="pos">�U�����s�����S���W</param>
        /// <returns>�͈͓��S�G��Collider2D�z��</returns>
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
        /// �U�����������Ă��邩�̏���
        /// <para>
        /// �������Ă����ꍇ�����œ��������G�͕ۑ������
        /// </para>
        /// </summary>
        /// <param name="pos">�U�����s�����S���W</param>
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
        /// �U�������������G�̕ۑ��p�A���{�X��HitMidBoss()�E�{�X��HitBoss()�ɓ����
        /// </summary>
        /// <param name="hitEnemyCollider">���������G�̃R���C�_�[</param>
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
        /// �U�����������Ă��邩�̏���
        /// <para>
        /// �������Ă����ꍇ�����œ��������G�͕ۑ������
        /// </para>
        /// </summary>
        /// <param name="pos">�U�����s�����S���W</param>
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
        /// ���{�X�ɍU���������������̕ۑ��p
        /// </summary>
        /// <param name="hitMidBossCollider">�����������{�X�̃R���C�_�[</param>
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
        /// �U�����������Ă��邩�̏���
        /// <para>
        /// �������Ă����ꍇ�����œ��������G�͕ۑ������
        /// </para>
        /// </summary>
        /// <param name="pos">�U�����s�����S���W</param>
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
        /// �{�X�ɍU���������������̕ۑ��p
        /// </summary>
        /// <param name="hitBossCollider">���������{�X�̃R���C�_�[</param>
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
        /// Update()���ŌĂяo���悤
        /// </summary>
        abstract void DoUpdate();

        /// <summary>
        /// ����ID�̎擾�p
        /// </summary>
        /// <returns></returns>
        abstract SubWeaponManager.SubWeaponID GetWeaponID();

        /// <summary>
        /// �G�ւ̍U�������I��
        /// </summary>
        abstract void HitEnemyProcessEnd();

        /// <summary>
        /// �U���������������̊m�F�p
        /// </summary>
        /// <returns>�������Ă���� true ���Ԃ�</returns>
        abstract bool GetIsHitEnemy();

        /// <summary>
        /// �U�������������G�̎擾�p
        /// </summary>
        /// <returns>���������G�̃X�N���v�g</returns>
        abstract List<Enemy.IBaseEnemy> GetHitEnemyList();

    }
}
