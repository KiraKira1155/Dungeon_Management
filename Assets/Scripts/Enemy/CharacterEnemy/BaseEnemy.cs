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
        /// 死亡アニメーション終了用
        /// </summary>
        abstract void EndDeathAnimation();

        /// <summary>
        /// アニメーション用
        /// </summary>
        abstract void Animation();

        /// <summary>
        /// 初期生成時に呼び出す
        /// </summary>
        abstract void InitSetting(int battleID, Vector3 spawnPos);

        /// <summary>
        /// 敵のゲームオブジェクト本体、transformを使用したいときは GetEnemyTransform() を使用する。キャッシュが少ないため。
        /// </summary>
        /// <returns>自分の GameObject コンポーネント</returns>
        abstract GameObject GetEnemyGameObject();

        /// <summary>
        /// 敵のトランスフォームデータ
        /// </summary>
        /// <returns>自分の Transform コンポーネント</returns>
        abstract Transform GetEnemyTransform();

        /// <summary>
        /// 敵の当たり判定
        /// </summary>
        /// <returns>CircleCollider2D が返る、使用したいときは RigidBody を使用していないため
        /// <para>
        /// RigidBody を使用しない別処理を実装してください
        /// </para>
        /// </returns>
        abstract CircleCollider2D GetEnemyCollider();

        /// <summary>
        /// 敵の描画コンポーネント取得
        /// </summary>
        /// <returns>SpriteRender</returns>
        abstract SpriteRenderer GetSpriteRenderer();
        /// <summary>
        /// 敵キャラクターの識別ID
        /// </summary>
        /// <returns>敵キャラクターのenum型</returns>
        abstract EnemyManager.EnemyCharactersID GetEnemyID();

        /// <summary>
        /// プレイヤーに攻撃を行ったか
        /// </summary>
        /// <returns>攻撃を行ったらtrue</returns>
        abstract bool IsAttack();

        /// <summary>
        /// 敵キャラクターが攻撃可能かどうか
        /// </summary>
        /// <returns>攻撃可能であればtrue</returns>
        abstract bool CanAttack();

        /// <summary>
        /// 現在のHP量
        /// </summary>
        /// <returns></returns>
        abstract int GetCurrentHP();

        /// <summary>
        /// ダメージを与える用
        /// </summary>
        /// <param name="damage">与えるダメージ量</param>
        abstract void BeAttacked(int damage);

        /// <summary>
        /// マップ上の識別ID取得用 
        /// </summary>
        /// <returns></returns>
        abstract int GetBattleID();

        /// <summary>
        /// マップ上の識別IDの保存用
        /// </summary>
        /// <param name="id">SpawnEnemyManagerで管理</param>
        abstract void SetBattleID(int id);

        /// <summary>
        /// 死んでいるかの確認用
        /// </summary>
        /// <returns>死んでいる場合 true を返す</returns>
        abstract bool IsEnemyDeath();

        /// <summary>
        /// プレイヤーに衝突した時に呼ぶ、攻撃判定用
        /// </summary>
        abstract void HitPlayer();

        /// <summary>
        /// プレイヤーに攻撃を行う時用、処理自体は行わない
        /// </summary>
        /// <returns>与えるダメージが返る</returns>
        abstract int AttackPlayer();

        /// <summary>
        /// 経験値ドロップ用
        /// </summary>
        abstract void DropExp();

        /// <summary>
        /// 敵の移動速度変更
        /// </summary>
        /// <param name="isSlowSpeed">減速するか</param>
        abstract void SetIsSlowSpeed(bool isSlowSpeed);

        /// <summary>
        /// 敵の移動スピードを遅くするか
        /// </summary>
        /// <returns></returns>
        abstract bool GetIsSlowSpeed();

        /// <summary>
        /// 引力の影響を受けるか
        /// </summary>
        /// <param name="isGravity"></param>
        abstract void SetGravityEffect(bool isGravity);

        /// <summary>
        /// 重力の影響を受けているか
        /// </summary>
        /// <returns></returns>
        abstract bool GetGravityEffect();

        abstract void NotDrop();

        abstract void Death();
    }
}