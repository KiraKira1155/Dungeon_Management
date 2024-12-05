using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class JoyStickManager : Singleton<JoyStickManager>
    {
        [SerializeField] private GameObject joyStickObj;
        [SerializeField] private GameObject joyStickCenterObj;
        [SerializeField] private Image joyStickImage;
        private Vector2 initJoyStickPos;
        [SerializeField] private float frameRadius;
        private Vector2 inputJoyStickForce;
        private Vector2 initJoyStickScreenPos;

        private float joyStickAngle;
        private bool isInput;

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
            {
                inputJoyStickForce.x = 0;
                inputJoyStickForce.y = 0;
                joyStickObj.SetActive(false);
                return;
            }
            if(GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
            {
                joyStickObj.SetActive(true);
            }

            if (Input.GetMouseButtonDown(0) && MouseClickManager.I.CheckCursorOnTheImage(joyStickImage))
            {
                isInput = true;
                initJoyStickPos = joyStickCenterObj.transform.position;
                initJoyStickScreenPos = CameraManager.I.GetMainCamera().WorldToScreenPoint(initJoyStickPos);
            }
            if (Input.GetMouseButton(0) && isInput)
            {
                // ���݂̃}�E�X�̃X�N���[�����W����W���C�X�e�B�b�N�̏����ʒu���������݂̃W���C�X�e�B�b�N�̍��W�����߂�
                Vector2 inputPos = Input.mousePosition;
                inputPos = CameraManager.I.GetMainCamera().WorldToScreenPoint(inputPos);
                var joyStickPos = inputPos - initJoyStickScreenPos;

                // ���S���猩��joyStickPos�̈ʒu��joyStickDirection �ɑ��
                var joyStickDirection = joyStickPos - initJoyStickPos;
                joyStickCenterObj.transform.position = initJoyStickPos + joyStickDirection / Vector2.Distance(initJoyStickPos, joyStickPos) * frameRadius;

                // �W���C�X�e�B�b�N�̓��͕���������͂����p�x���擾
                var angle = Mathf.Atan2(joyStickDirection.y, joyStickDirection.x) * Mathf.Rad2Deg;
                // �p�x�����W�A���ɕϊ�
                var radian = angle * (Mathf.PI / 180);
                // ���W�A������P�ʃx�N�g�����擾�A�x�N�g���̒����͏��1�ƂȂ邽�ߐi�ދ����͈��
                inputJoyStickForce = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

                // ���͕���
                PlayerManager.I.GetPlayerAutoAttackCollider().SetAttackDirection(angle);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isInput = false;
                joyStickCenterObj.transform.localPosition = Vector2.zero;      //���W�� initJoyStickPos �ɖ߂�
                inputJoyStickForce.x = 0;
                inputJoyStickForce.y = 0;
            }
        }

        /// <summary>
        /// �W���C�X�e�B�b�N�̓��͂������K�����������x�N�g�����擾
        /// <para>
        /// �v���C���[�̈ړ��p
        /// </para>
        /// </summary>
        /// <returns>���͂��ꂽVecter2�̒l</returns>
        public Vector2 GetInputDirection()
        {
            return inputJoyStickForce;
        }
    }
}
