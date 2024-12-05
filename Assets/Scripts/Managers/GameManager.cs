using GameSystem;
using JetBrains.Annotations;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private bool _test;
    public bool test { get { return _test; } }

    public GameClearHandler.PlayerScoreData scoreData { get; private set; }

    /// <summary>
    /// シーンの取得とシーンの切り替え用、戦闘中のモード取得切り替えもこれ
    /// </summary>
    public GameSystem.SceneController sceneController;

    public SignupHandler signupHandler { get; private set; }
    public GameClearHandler clearHandler { get; private set; }
    public RankingHandler rankingHandler { get; private set; }

    public StageSelectHandller stageSelectHandller { get; private set; }

    private void Awake()
    {
        Init();

        sceneController = new GameSystem.SceneController();
        clearHandler = new GameClearHandler();
        rankingHandler = new RankingHandler();
        signupHandler = new SignupHandler();
        stageSelectHandller = new StageSelectHandller();
    }

    private void Start()
    {
        if (!test)
        {
            DontDestroyOnLoad(gameObject);
            SaveDataManager.DoAwake();
            if (SaveDataManager.initGame)
            {
                ChengeNewScene(GameSystem.SceneController.GameScene.newGame, false);
            }
            else
            {
                ChengeNewScene(GameSystem.SceneController.GameScene.title, false);
            }
        }
        else
        {
            sceneController.SetGameSceneEnd();
            sceneController.SetGameScene(GameSystem.SceneController.GameScene.battle);
        }
    }

    private void Update()
    {
        GameSystem.TimeController.DoUpdate();
    }

    private void FixedUpdate()
    {
        GameSystem.TimeController.DoFixedUpdate();
    }

    public override void BattleSceneInit()
    {
        MenuManager.I.BattleSceneInit();
        Map.MapManager.I.BattleSceneInit();
        Enemy.EnemyManager.I.bossController.bossBarController.Init();
        Player.PlayerManager.I.BattleSceneInit();
        Armour.ArmourManager.I.BattleSceneInit();
        Enemy.DropExpManager.I.BattleSceneInit();
        Item.ItemManager.I.BattleSceneInit();
        Player.PlayerSkillManager.I.BattleSceneInit();
        BattleManager.I.BattleSceneInit();
        EquipmentSelectManager.I.BattleSceneInit();
    }

    public override void BattleSceneClear()
    {
        sceneController.BattleSceneEnd();


        SubWeapon.SubWeaponManager.I.BattleSceneClear();
        Enemy.DropExpManager.I.BattleSceneClear();
    }

    public void SetScore()
    {
        SetScoreData();
        PostRanking();
    }

    public void SetScoreData()
    {
        scoreData = new GameClearHandler.PlayerScoreData();

        scoreData.user_id = SaveDataManager.Load().id;

        scoreData.casualty_count = BattleManager.I.casualtyCount;
        scoreData.total_enemy_defeated = BattleManager.I.totalEnemyDefeated;


        if (GameManager.I.sceneController.GetGameScene() == SceneController.GameScene.gameClear)
        {
            var time = 0.0f;
            switch (stageSelectHandller.GetStage())
            {
                case 0:
                    time = 60 - TimeController.GetTimeRemaining();
                    break;

                case 1:
                    time = 90 - TimeController.GetTimeRemaining();
                    break;

            }

            scoreData.boss_defeated_time = (int)time;

            var calculationScore = (scoreData.total_enemy_defeated) - (scoreData.casualty_count * 2);

            var timeScore = (100 - (int)time) * 100;

            scoreData.point = 2000 + calculationScore + timeScore;
        }
        else
        {
            scoreData.boss_defeated_time = 0;
            scoreData.point = 0;
        }

        scoreData.weapon_data = new GameClearHandler.WeaponData();
        scoreData.weapon_data.weapon_1 = (int)SubWeapon.SubWeaponManager.I.GetHoldingSubWeaponID(0);
        scoreData.weapon_data.weapon_2 = (int)SubWeapon.SubWeaponManager.I.GetHoldingSubWeaponID(1);
        scoreData.weapon_data.weapon_3 = (int)SubWeapon.SubWeaponManager.I.GetHoldingSubWeaponID(2);

        scoreData.equipment_data = new GameClearHandler.EquipmentData();
        scoreData.equipment_data.equipment_1 = (int)Armour.ArmourManager.I.GetArmourAllData()[0].armour;
        scoreData.equipment_data.equipment_2 = (int)Armour.ArmourManager.I.GetArmourAllData()[1].armour;
        scoreData.equipment_data.equipment_3 = (int)Armour.ArmourManager.I.GetArmourAllData()[2].armour;
        scoreData.equipment_data.equipment_4 = (int)Armour.ArmourManager.I.GetArmourAllData()[3].armour;
    }


    private void PostRanking()
    {
        clearHandler.SetData(scoreData);
        StartCoroutine(clearHandler.PostScoreCoroutine());
    }

    /// <summary>
    /// ゲームシーン変更用
    /// </summary>
    /// <param name="scene">変更したいゲームシーン</param>
    /// <param name="changeInvokeTime">変更するのにかける秒数</param>
    public void ChengeNewScene(SceneController.GameScene scene, bool fade)
    {
        if (scene == SceneController.GameScene.gameClear || scene == SceneController.GameScene.gameOver)
        {
            GameManager.I.BattleSceneClear();
            SoundManager.I.DestroyButtleSE();

        }

        sceneController.SetPreviousScene();
        StartCoroutine(ChangeNewScene(scene, fade));
    }

    private IEnumerator ChangeNewScene(SceneController.GameScene scene, bool fade)
    {
        sceneController.SetGameScene(scene);

        if (fade)
        {
            FadeManager.I.fadeOut();
            yield return new WaitForSeconds(FadeManager.I.GetFadeTime() * 2);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneController.GetSceneName(scene), LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        sceneController.SetGameSceneEnd();
        yield break;
    }

    public IEnumerator UnLoadPreviousScene(bool fade, float changeInvokeTime = 0)
    {
        if (fade)
        {
            FadeManager.I.fadeIn();
        }
        else
        {
            FadeManager.I.fadeNone();
        }

        yield return new WaitForSeconds(changeInvokeTime);
        SceneManager.UnloadSceneAsync(sceneController.GetPreviousScene());
    }
}
