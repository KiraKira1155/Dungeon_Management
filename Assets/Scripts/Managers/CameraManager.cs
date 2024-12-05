using GameSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform mainCameraPos;
    [SerializeField] private float followDistance;
    [SerializeField] private float smoothSpeed;

    private float distance;
    private bool isAdulation;

    private float deltaTime;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        SetCamera();

        if (!GameManager.I.sceneController.battleSceneLoadEnd)
            return;

        deltaTime = Time.deltaTime;
    }

    public void SetCamera()
    {
        if (GameManager.I.sceneController.GetGameScene() != SceneController.GameScene.battle)
        {
            GetMainCameraPos().position = new Vector3(0, 0, -10.0f);
        }
    }

    private void LateUpdate()
    {
        if (!GameManager.I.sceneController.battleSceneLoadEnd)
            return;

        if (GameManager.I.sceneController.GetGameModeForBattleScene() != SceneController.GameModeForBattle.battle)
            return;


        distance = Vector2.Distance
            (
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(Player.PlayerManager.I.GetPlayerTransform().position.x, Player.PlayerManager.I.GetPlayerTransform().position.y)
            );

        if (distance > followDistance)
        {
            isAdulation = true;
        }

        if (isAdulation)
        {
            transform.position = Vector3.Lerp(transform.position, Player.PlayerManager.I.GetPlayerTransform().position, smoothSpeed * deltaTime);

            if (distance < 0.1f)
            {
                isAdulation = false;
            }
        }
    }

    /// <summary>
    /// メインカメラ取得用
    /// </summary>
    /// <returns></returns>
    public Camera GetMainCamera()
    {
        return mainCamera;
    }

    /// <summary>
    /// メインカメラのトランスフォーム取得
    /// </summary>
    /// <returns></returns>
    public Transform GetMainCameraPos()
    {
        return mainCameraPos;
    }
}
