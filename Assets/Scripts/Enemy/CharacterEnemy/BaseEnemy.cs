using GameSystem;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public abstract class BaseEnemy : MonoBehaviour, IBaseEnemy
    {
        private int battleID;
        private int currentHp;
        private bool isAttack;
        private bool isDeath;
        private bool canAttack;

        private bool notDrop;

        protected bool isSlowSpeed;
        protected bool isGravityEffect;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject thisObj;
        [SerializeField] private Transform thisTransform;
        [SerializeField] private CircleCollider2D thisCollider;
        
        [SerializeField] private  EnemySpriteController spriteController = new EnemySpriteController();

        private float attackIntervalCount;

        private GameSystem.SceneController.GameModeForBattle currentBattleScene;


        public void InitSetting(int battleID, Vector3 spawnPos)
        {
            isAttack = false;
            isDeath = false;
            canAttack = false;
            notDrop = false;
            isSlowSpeed = false;
            isGravityEffect = false;
            attackIntervalCount = 0;

            this.battleID = battleID;
            thisTransform.position = spawnPos;
            thisObj.SetActive(true);
            thisCollider.enabled = true;
            attackIntervalCount = 0;
            currentHp = EnemyManager.I.basicStatus.GetEnemyStatusHP(GetEnemyID());

            if(GetEnemyID() == EnemyManager.EnemyCharactersID.boss)
            {
                if (GameManager.I.stageSelectHandller.GetStage() == 0)
                    currentHp = currentHp / 2;

#if UNITY_EDITOR
                Debug.Log(currentHp);
#endif
            }
        }


        private void Update()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
            {
                if (currentBattleScene != GameSystem.SceneController.GameModeForBattle.battle)
                    spriteController.PlayAnimation();

                if (attackIntervalCount <= EnemyManager.I.basicStatus.GetEnemyStatusAttackInterval(GetEnemyID()))
                {
                    canAttack = false;
                    attackIntervalCount += Time.deltaTime;
                }
                else
                {
                    canAttack = true;
                }

                currentBattleScene = GameSystem.SceneController.GameModeForBattle.battle;
            }
            else
            {
                if (currentBattleScene != GameSystem.SceneController.GameModeForBattle.pause)
                    spriteController.StopAnimation();

                currentBattleScene = GameSystem.SceneController.GameModeForBattle.pause;

            }
        }

        public void EndDeathAnimation()
        {
            spriteController.EndDeathAnimation();
            DeathEvent();
        }

        public void NotDrop()
        {
            notDrop = true;
        }

        public void DropExp()
        {
            if(notDrop)
                return;

            Item.ItemManager.I.ItemDrop(thisTransform.position);
            DropExpManager.I.ExpDrop(thisTransform.position);
        }

        private void DeathEvent()
        {
            thisObj.SetActive(false);
        }

        public void Animation()
        {
            spriteController.Animation(this);
        }

        public int AttackPlayer()
        {
            isAttack = false;
            return EnemyManager.I.basicStatus.GetEnemyStatusATK(GetEnemyID());
        }

        public GameObject GetEnemyGameObject()
        {
            return thisObj;
        }

        public CircleCollider2D GetEnemyCollider()
        {
            return thisCollider;
        }

        public Transform GetEnemyTransform()
        {
            return thisTransform;
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            return spriteRenderer;
        }

        public int GetCurrentHP()
        {
            return currentHp;
        }

        public void BeAttacked(int damage)
        {
            currentHp -= damage;

            if (currentHp <= 0)
            {
                Death();

                if (GetEnemyID() == EnemyManager.EnemyCharactersID.boss)
                {
                    TimeController.canBattleTimeCount = false;
                    return;
                }
            }
        }

        public void ChangeScene()
        {
            GameManager.I.sceneController.SetGameModeForBattleScene(SceneController.GameModeForBattle.death);
            GameManager.I.ChengeNewScene(SceneController.GameScene.gameClear, true);
        }



        public void Death()
        {
            isDeath = true;
            thisCollider.enabled = false;
            spriteController.StartDeathAnimation();
            Sound.SoundManager.I.PlayDeathEnemySE(GetEnemyID());

            if (notDrop)
                return;
            BattleManager.I.EnemyDefeated();
        }

        public abstract EnemyManager.EnemyCharactersID GetEnemyID();

        public bool IsAttack()
        {
            return isAttack;
        }

        public bool IsEnemyDeath()
        {
            return isDeath;
        }

        public bool CanAttack()
        {
            return canAttack;
        }

        public void SetBattleID(int id)
        {
            battleID = id;
        }

        public int GetBattleID()
        {
            return battleID;
        }

        public void HitPlayer()
        {
            if(attackIntervalCount >= EnemyManager.I.basicStatus.GetEnemyStatusAttackInterval(GetEnemyID()))
            {
                attackIntervalCount = 0;
                isAttack = true;
            }
        }


        public void SetIsSlowSpeed(bool isSlowSpeed)
        {
            this.isSlowSpeed = isSlowSpeed;
        }

        public bool GetIsSlowSpeed()
        {
            return isSlowSpeed;
        }

        public void SetGravityEffect(bool gravityEffect)
        {
            isGravityEffect = gravityEffect;
        }

        public bool GetGravityEffect()
        {
            return isGravityEffect;
        }

    }

    public interface IBaseEnemy
    {
        /// <summary>
        /// ���S�A�j���[�V�����I���p
        /// </summary>
        abstract void EndDeathAnimation();

        /// <summary>
        /// �A�j���[�V�����p
        /// </summary>
        abstract void Animation();

        /// <summary>
        /// �����������ɌĂяo��
        /// </summary>
        abstract void InitSetting(int battleID, Vector3 spawnPos);

        /// <summary>
        /// �G�̃Q�[���I�u�W�F�N�g�{�́Atransform���g�p�������Ƃ��� GetEnemyTransform() ���g�p����B�L���b�V�������Ȃ����߁B
        /// </summary>
        /// <returns>������ GameObject �R���|�[�l���g</returns>
        abstract GameObject GetEnemyGameObject();

        /// <summary>
        /// �G�̃g�����X�t�H�[���f�[�^
        /// </summary>
        /// <returns>������ Transform �R���|�[�l���g</returns>
        abstract Transform GetEnemyTransform();

        /// <summary>
        /// �G�̓����蔻��
        /// </summary>
        /// <returns>CircleCollider2D ���Ԃ�A�g�p�������Ƃ��� RigidBody ���g�p���Ă��Ȃ�����
        /// <para>
        /// RigidBody ���g�p���Ȃ��ʏ������������Ă�������
        /// </para>
        /// </returns>
        abstract CircleCollider2D GetEnemyCollider();

        /// <summary>
        /// �G�̕`��R���|�[�l���g�擾
        /// </summary>
        /// <returns>SpriteRender</returns>
        abstract SpriteRenderer GetSpriteRenderer();
        /// <summary>
        /// �G�L�����N�^�[�̎���ID
        /// </summary>
        /// <returns>�G�L�����N�^�[��enum�^</returns>
        abstract EnemyManager.EnemyCharactersID GetEnemyID();

        /// <summary>
        /// �v���C���[�ɍU�����s������
        /// </summary>
        /// <returns>�U�����s������true</returns>
        abstract bool IsAttack();

        /// <summary>
        /// �G�L�����N�^�[���U���\���ǂ���
        /// </summary>
        /// <returns>�U���\�ł����true</returns>
        abstract bool CanAttack();

        /// <summary>
        /// ���݂�HP��
        /// </summary>
        /// <returns></returns>
        abstract int GetCurrentHP();

        /// <summary>
        /// �_���[�W��^����p
        /// </summary>
        /// <param name="damage">�^����_���[�W��</param>
        abstract void BeAttacked(int damage);

        /// <summary>
        /// �}�b�v��̎���ID�擾�p 
        /// </summary>
        /// <returns></returns>
        abstract int GetBattleID();

        /// <summary>
        /// �}�b�v��̎���ID�̕ۑ��p
        /// </summary>
        /// <param name="id">SpawnEnemyManager�ŊǗ�</param>
        abstract void SetBattleID(int id);

        /// <summary>
        /// ����ł��邩�̊m�F�p
        /// </summary>
        /// <returns>����ł���ꍇ true ��Ԃ�</returns>
        abstract bool IsEnemyDeath();

        /// <summary>
        /// �v���C���[�ɏՓ˂������ɌĂԁA�U������p
        /// </summary>
        abstract void HitPlayer();

        /// <summary>
        /// �v���C���[�ɍU�����s�����p�A�������͍̂s��Ȃ�
        /// </summary>
        /// <returns>�^����_���[�W���Ԃ�</returns>
        abstract int AttackPlayer();

        /// <summary>
        /// �o���l�h���b�v�p
        /// </summary>
        abstract void DropExp();

        /// <summary>
        /// �G�̈ړ����x�ύX
        /// </summary>
        /// <param name="isSlowSpeed">�������邩</param>
        abstract void SetIsSlowSpeed(bool isSlowSpeed);

        /// <summary>
        /// �G�̈ړ��X�s�[�h��x�����邩
        /// </summary>
        /// <returns></returns>
        abstract bool GetIsSlowSpeed();

        /// <summary>
        /// ���͂̉e�����󂯂邩
        /// </summary>
        /// <param name="isGravity"></param>
        abstract void SetGravityEffect(bool isGravity);

        /// <summary>
        /// �d�͂̉e�����󂯂Ă��邩
        /// </summary>
        /// <returns></returns>
        abstract bool GetGravityEffect();

        abstract void NotDrop();

        abstract void Death();
    }
}