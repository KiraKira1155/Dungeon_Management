using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("プレイヤーの基本設定")]
        [SerializeField] private SpriteRenderer playerSprite;
        [SerializeField] private GameObject playerObj;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CircleCollider2D playerCollider;
        [SerializeField] private Animator playerAnimator; // Animatorフィールドを追加
        [SerializeField] private PlayerAutoAttackCollider playerAutoAttackCollider;
        [SerializeField] private GameObject directionPlayerFacing;


        [Header("当たり判定処理")]
        [SerializeField] private LayerMask playerMask;
        [Tooltip("敵との衝突時に敵から受ける反発係数")]
        [SerializeField] private float repulsionCoefficient; //反発係数

        [Tooltip("敵との衝突時に敵に加える反発係数")]
        [SerializeField] private float repulsionCoefficientForEnemy;
        [SerializeField] private float detectionRadius;

        private Vector2 moveLimitsMin;
        private Vector2 moveLimitsMax;
        private bool isMoveLimitActive = false;


        /// <summary>
        /// キャラクターの基本ステータス取得用
        /// </summary>
        public PlayerCharactersBasicStatus basicStatus = new PlayerCharactersBasicStatus();

        /// <summary>
        /// 現在使用中のキャラクターのステータス取得用
        /// </summary>
        public PlayerCharacterCurrentStatus currentStatus = new PlayerCharacterCurrentStatus();

        /// <summary>
        /// プレイヤーの攻撃、移動制御用
        /// </summary>
        public PlayerController controller = new PlayerController();

        /// <summary>
        /// プレイヤーのバー表示されているUIの制御用
        /// </summary>
        public PlayerBarController barController = new PlayerBarController();


        private GameSystem.SceneController.GameModeForBattle currentBattleScene;

        /// <summary>
        /// キャラクターの種類、識別IDとして使用
        /// </summary>
        public enum PlayerCharactersID
        {
            test,
        }

        private void Awake()
        {
            Init();
        }

        public override void BattleSceneInit()
        {
            currentStatus.SetPlayerCharacter(PlayerCharactersID.test);
        }

        /// <summary>
        /// プレイヤーの移動範囲を設定する
        /// </summary>
        /// <param name="bossPosition">ボスの位置</param>
        public void SetMoveLimits(Vector3 bossPosition)
        {
            moveLimitsMin = new Vector2(bossPosition.x - 7f, bossPosition.y - 9.5f);
            moveLimitsMax = new Vector2(bossPosition.x + 7f, bossPosition.y + 9.5f);
            isMoveLimitActive = true;
        }

        /// <summary>
        /// プレイヤーの移動範囲を制限するロジック
        /// </summary>
        /// <param name="position">新しいプレイヤーの位置</param>
        /// <returns>制限されたプレイヤーの位置</returns>
        public Vector3 LimitPlayerPosition(Vector3 position)
        {
            if (isMoveLimitActive)
            {
                position.x = Mathf.Clamp(position.x, moveLimitsMin.x, moveLimitsMax.x);
                position.y = Mathf.Clamp(position.y, moveLimitsMin.y, moveLimitsMax.y);
            }
            return position;
        }

        private void Update()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
            {
                if (currentBattleScene != GameSystem.SceneController.GameModeForBattle.pause)
                    playerAnimator.enabled = true;

                

                controller.DoUpdate();

                currentBattleScene = GameSystem.SceneController.GameModeForBattle.battle;
            }
            else
            {
                if (currentBattleScene != GameSystem.SceneController.GameModeForBattle.pause)
                    playerAnimator.enabled = false;

                currentBattleScene = GameSystem.SceneController.GameModeForBattle.pause;
            }

            Vector2 inputDirection = JoyStickManager.I.GetInputDirection();
            if (inputDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg;
                directionPlayerFacing.transform.rotation = Quaternion.Euler(0, 0, angle);
                directionPlayerFacing.transform.localPosition = inputDirection * 2;
            }

        }


        private void FixedUpdate()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
                controller.DoFixedUpdate();
        }

        private void LateUpdate()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
                barController.DoLateUpdate();
        }

        /// <summary>
        /// プレイヤーのAnimator取得用
        /// </summary>
        /// <returns>プレイヤーのAnimatorコンポーネント</returns>
        public Animator GetPlayerAnimator()
        {
            return playerAnimator;
        }

        /// <summary>
        /// プレイヤーのオブジェクトの取得用
        /// </summary>
        /// <returns>プレイヤーのオブジェクト、当たり判定を持っているもの</returns>
        public GameObject GetPlayerObj()
        {
            return playerObj;
        }

        /// <summary>
        /// プレイヤーのトランスフォーム取得用
        /// </summary>
        /// <returns>自分の Transform コンポーネント</returns>
        public Transform GetPlayerTransform() 
        { 
            return playerTransform; 
        }

        /// <summary>
        /// プレイヤーの当たり判定
        /// </summary>
        /// <returns>CircleCollider2D が返る、使用したいときは RigidBody を使用していないため
        /// <para>
        /// RigidBody を使用しない別処理を実装してください
        /// </para>
        /// </returns>
        public CircleCollider2D GetPlayerCollider()
        {
            return playerCollider;
        }

        /// <summary>
        /// プレイヤーのスプライトレンダラー取得用
        /// </summary>
        /// <returns></returns>
        public SpriteRenderer GetPlayerSpriterenderer()
        {
            return playerSprite;
        }

        /// <summary>
        /// プレイヤーが敵から受ける反発係数取得
        /// </summary>
        /// <returns></returns>
        public float GetRepulsionCoefficient()
        {
            return repulsionCoefficient;
        }

        /// <summary>
        /// プレイヤーが敵に与える反発係数
        /// </summary>
        /// <returns></returns>
        public float GetRepulsionCoefficientForEnemy()
        {
            return repulsionCoefficientForEnemy;
        }

        /// <summary>
        /// プレイヤーの当たり判定範囲
        /// </summary>
        /// <returns></returns>
        public float GetDetectionRadius()
        {
            return detectionRadius;
        }

        /// <summary>
        /// プレイヤーの攻撃判定スクリプト取得用
        /// </summary>
        /// <returns></returns>
        public PlayerAutoAttackCollider GetPlayerAutoAttackCollider()
        {
            return playerAutoAttackCollider;
        }

        /// <summary>
        /// プレイヤーのレイヤーを取得用
        /// </summary>
        /// <returns></returns>
        public LayerMask GetPlayerLayer()
        {
            return playerMask;
        }
    }
}
 