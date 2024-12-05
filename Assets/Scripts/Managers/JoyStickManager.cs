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
                // 現在のマウスのスクリーン座標からジョイスティックの初期位置を引き現在のジョイスティックの座標を求める
                Vector2 inputPos = Input.mousePosition;
                inputPos = CameraManager.I.GetMainCamera().WorldToScreenPoint(inputPos);
                var joyStickPos = inputPos - initJoyStickScreenPos;

                // 中心から見たjoyStickPosの位置をjoyStickDirection に代入
                var joyStickDirection = joyStickPos - initJoyStickPos;
                joyStickCenterObj.transform.position = initJoyStickPos + joyStickDirection / Vector2.Distance(initJoyStickPos, joyStickPos) * frameRadius;

                // ジョイスティックの入力方向から入力した角度を取得
                var angle = Mathf.Atan2(joyStickDirection.y, joyStickDirection.x) * Mathf.Rad2Deg;
                // 角度をラジアンに変換
                var radian = angle * (Mathf.PI / 180);
                // ラジアンから単位ベクトルを取得、ベクトルの長さは常に1となるため進む距離は一定
                inputJoyStickForce = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized;

                // 入力方向
                PlayerManager.I.GetPlayerAutoAttackCollider().SetAttackDirection(angle);
            }
            if (Input.GetMouseButtonUp(0))
            {
                isInput = false;
                joyStickCenterObj.transform.localPosition = Vector2.zero;      //座標を initJoyStickPos に戻す
                inputJoyStickForce.x = 0;
                inputJoyStickForce.y = 0;
            }
        }

        /// <summary>
        /// ジョイスティックの入力した正規化した方向ベクトルを取得
        /// <para>
        /// プレイヤーの移動用
        /// </para>
        /// </summary>
        /// <returns>入力されたVecter2の値</returns>
        public Vector2 GetInputDirection()
        {
            return inputJoyStickForce;
        }
    }
}
