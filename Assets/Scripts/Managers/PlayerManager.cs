using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        [Header("�v���C���[�̊�{�ݒ�")]
        [SerializeField] private SpriteRenderer playerSprite;
        [SerializeField] private GameObject playerObj;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CircleCollider2D playerCollider;
        [SerializeField] private Animator playerAnimator; // Animator�t�B�[���h��ǉ�
        [SerializeField] private PlayerAutoAttackCollider playerAutoAttackCollider;
        [SerializeField] private GameObject directionPlayerFacing;


        [Header("�����蔻�菈��")]
        [SerializeField] private LayerMask playerMask;
        [Tooltip("�G�Ƃ̏Փˎ��ɓG����󂯂锽���W��")]
        [SerializeField] private float repulsionCoefficient; //�����W��

        [Tooltip("�G�Ƃ̏Փˎ��ɓG�ɉ����锽���W��")]
        [SerializeField] private float repulsionCoefficientForEnemy;
        [SerializeField] private float detectionRadius;

        private Vector2 moveLimitsMin;
        private Vector2 moveLimitsMax;
        private bool isMoveLimitActive = false;


        /// <summary>
        /// �L�����N�^�[�̊�{�X�e�[�^�X�擾�p
        /// </summary>
        public PlayerCharactersBasicStatus basicStatus = new PlayerCharactersBasicStatus();

        /// <summary>
        /// ���ݎg�p���̃L�����N�^�[�̃X�e�[�^�X�擾�p
        /// </summary>
        public PlayerCharacterCurrentStatus currentStatus = new PlayerCharacterCurrentStatus();

        /// <summary>
        /// �v���C���[�̍U���A�ړ�����p
        /// </summary>
        public PlayerController controller = new PlayerController();

        /// <summary>
        /// �v���C���[�̃o�[�\������Ă���UI�̐���p
        /// </summary>
        public PlayerBarController barController = new PlayerBarController();


        private GameSystem.SceneController.GameModeForBattle currentBattleScene;

        /// <summary>
        /// �L�����N�^�[�̎�ށA����ID�Ƃ��Ďg�p
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
        /// �v���C���[�̈ړ��͈͂�ݒ肷��
        /// </summary>
        /// <param name="bossPosition">�{�X�̈ʒu</param>
        public void SetMoveLimits(Vector3 bossPosition)
        {
            moveLimitsMin = new Vector2(bossPosition.x - 7f, bossPosition.y - 9.5f);
            moveLimitsMax = new Vector2(bossPosition.x + 7f, bossPosition.y + 9.5f);
            isMoveLimitActive = true;
        }

        /// <summary>
        /// �v���C���[�̈ړ��͈͂𐧌����郍�W�b�N
        /// </summary>
        /// <param name="position">�V�����v���C���[�̈ʒu</param>
        /// <returns>�������ꂽ�v���C���[�̈ʒu</returns>
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
        /// �v���C���[��Animator�擾�p
        /// </summary>
        /// <returns>�v���C���[��Animator�R���|�[�l���g</returns>
        public Animator GetPlayerAnimator()
        {
            return playerAnimator;
        }

        /// <summary>
        /// �v���C���[�̃I�u�W�F�N�g�̎擾�p
        /// </summary>
        /// <returns>�v���C���[�̃I�u�W�F�N�g�A�����蔻��������Ă������</returns>
        public GameObject GetPlayerObj()
        {
            return playerObj;
        }

        /// <summary>
        /// �v���C���[�̃g�����X�t�H�[���擾�p
        /// </summary>
        /// <returns>������ Transform �R���|�[�l���g</returns>
        public Transform GetPlayerTransform() 
        { 
            return playerTransform; 
        }

        /// <summary>
        /// �v���C���[�̓����蔻��
        /// </summary>
        /// <returns>CircleCollider2D ���Ԃ�A�g�p�������Ƃ��� RigidBody ���g�p���Ă��Ȃ�����
        /// <para>
        /// RigidBody ���g�p���Ȃ��ʏ������������Ă�������
        /// </para>
        /// </returns>
        public CircleCollider2D GetPlayerCollider()
        {
            return playerCollider;
        }

        /// <summary>
        /// �v���C���[�̃X�v���C�g�����_���[�擾�p
        /// </summary>
        /// <returns></returns>
        public SpriteRenderer GetPlayerSpriterenderer()
        {
            return playerSprite;
        }

        /// <summary>
        /// �v���C���[���G����󂯂锽���W���擾
        /// </summary>
        /// <returns></returns>
        public float GetRepulsionCoefficient()
        {
            return repulsionCoefficient;
        }

        /// <summary>
        /// �v���C���[���G�ɗ^���锽���W��
        /// </summary>
        /// <returns></returns>
        public float GetRepulsionCoefficientForEnemy()
        {
            return repulsionCoefficientForEnemy;
        }

        /// <summary>
        /// �v���C���[�̓����蔻��͈�
        /// </summary>
        /// <returns></returns>
        public float GetDetectionRadius()
        {
            return detectionRadius;
        }

        /// <summary>
        /// �v���C���[�̍U������X�N���v�g�擾�p
        /// </summary>
        /// <returns></returns>
        public PlayerAutoAttackCollider GetPlayerAutoAttackCollider()
        {
            return playerAutoAttackCollider;
        }

        /// <summary>
        /// �v���C���[�̃��C���[���擾�p
        /// </summary>
        /// <returns></returns>
        public LayerMask GetPlayerLayer()
        {
            return playerMask;
        }
    }
}
 