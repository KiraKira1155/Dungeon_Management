using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class ResultManager : Singleton<ResultManager>
{
    private const int RANKING_PANEL_AMOUNT = 100;

    [SerializeField] private Scrollbar scrollbar;
    [Header("表示パネル")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject rankingPanel;
    [SerializeField] private GameObject buttonPanel;

    [Header("ランキング表示用オブジェクト設定")]
    [SerializeField] private Transform rankingContntObj;
    [SerializeField] private GameObject rankingBordPrefab;
    [SerializeField] private GameObject infoBordPrefab;

    [Header("ランキング表示用アイコン")]
    [SerializeField] private Image rankingIcons;
    [SerializeField] private Image hideRankingIcons;
    [Header("ボタン設定")]
    [SerializeField] private Image restartIcon;
    [SerializeField] private Image homeIcon;
    [Header("詳細情報表示用")]
    [SerializeField] private string[] locationName;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI bossDefeatedTimeText;
    [SerializeField] private TextMeshProUGUI totalEnemyDefeatedTimeText;
    [SerializeField] private TextMeshProUGUI casualtyCountText;

    private GameObject[] infoBordList = new GameObject[RANKING_PANEL_AMOUNT];
    private RankingBord[] rankingBordList = new RankingBord[RANKING_PANEL_AMOUNT];
    private Image[] rankingBordImageList = new Image[RANKING_PANEL_AMOUNT];

    public bool isClear;

    private const float GET_RANKING_INTERVAL_TIME = 10.0f;
    private float getRankingIntervalCnt;

    private bool lodingRankingData;

    private bool canNextScene;

#if UNITY_EDITOR
    [SerializeField] private bool test;
#endif
    private void Awake()
    {
        Init();

        getRankingIntervalCnt = 10.0f;
        lodingRankingData = true;

        InitGenerateRankingPanel();
        canNextScene = true;
    }

    void Start()
    {
        GameManager.I.SetScore();

        if (GameManager.I.sceneController.GetGameScene() == GameSystem.SceneController.GameScene.gameClear)
        {
            clearPanel.SetActive(true);
            gameOverPanel.SetActive(false);
        }
        else
        {
            gameOverPanel.SetActive(true);
            clearPanel.SetActive(false);
        }

        DateTime today = DateTime.Now;

        string formattedDate = $"20XX.{today.ToString("MM.dd")}";

        dayText.text = formattedDate;

        DisplayFinalScore();

        StartCoroutine(GameManager.I.UnLoadPreviousScene(true));
    }

    private void InitGenerateRankingPanel()
    {
        rankingPanel.SetActive(false);

        for (int i = 0; i < RANKING_PANEL_AMOUNT; i++)
        {
            var obj = Instantiate(rankingBordPrefab, rankingContntObj);
            rankingBordList[i] = obj.GetComponent<RankingBord>();
            rankingBordImageList[i] = rankingBordList[i].GetRankingBordImage(); 

            infoBordList[i] = Instantiate(infoBordPrefab, rankingContntObj);
        }
    }

    private void Update()
    {
        if (!lodingRankingData)
            return;

        DisplayMoreInfo();
        ClickRankingImage();
        SceneChenge();
    }

    private void DisplayMoreInfo()
    {
        if (Input.GetMouseButtonDown(0))
        {   
            int i = 0;
            foreach (var bord in rankingBordList)
            {
                if (bord.GetRankingBordImage() == MouseClickManager.I.ResultImageClick(rankingBordImageList[i]))
                {
                    bord.DisplayMoreInfo();
                    infoBordList[i].SetActive(!infoBordList[i].activeInHierarchy);
                    return;
                }
                i++;
            }
        }
    }

    private void ClickRankingImage()
    {
        if (MouseClickManager.I.ResultImageClick(hideRankingIcons))
        {
            rankingPanel.SetActive(false);
            buttonPanel.SetActive(true);
        } 

        if (getRankingIntervalCnt < GET_RANKING_INTERVAL_TIME)
        {
            var imageList = 
            getRankingIntervalCnt += Time.deltaTime;
            if (MouseClickManager.I.ResultImageClick(rankingIcons))
            {
                rankingPanel.SetActive(true);
                buttonPanel.SetActive(false);

            }
        }
        else if(MouseClickManager.I.ResultImageClick(rankingIcons))
        {
           // getRankingIntervalCnt = 0.0f;
           buttonPanel.SetActive(false);
            //StartCoroutine(GetRanking());          
        }
    }

    private IEnumerator GetRanking()
    {
        lodingRankingData = false;
        var task = GameManager.I.rankingHandler.GetRankingCoroutine();

        yield return task;

        rankingPanel.SetActive(true);
        lodingRankingData = true;

#if UNITY_EDITOR
        Debug.Log("データ取得完了");
#endif

        StartCoroutine(DisplayRankingData());

        yield break;
    }


    private IEnumerator DisplayRankingData()
    {
        int indexNum = GameManager.I.rankingHandler.rankingDataList.ranking_data.Length;

        int i = 0;
        foreach (var bord in rankingBordList)
        {
            if(i < indexNum)
            {
                bord.InitSetting(GameManager.I.rankingHandler.rankingDataList.ranking_data[i]);
                i++;

                if (i % 10 == 0)
                    yield return null;
            }
            else
            {
                bord.gameObject.SetActive(false);
            }
        }

        yield break;
    }

    private void SceneChenge()
    {
        if (!canNextScene)
            return;

        if (MouseClickManager.I.ResultImageClick(restartIcon))
        {
            SceneManager.MoveGameObjectToScene(EquipmentSelectManager.I.gameObject, SceneManager.GetActiveScene());
            GameManager.I.ChengeNewScene(GameSystem.SceneController.GameScene.battle, true);

            canNextScene = false;
        }
        if (MouseClickManager.I.ResultImageClick(homeIcon))
        {
            SceneManager.MoveGameObjectToScene(EquipmentSelectManager.I.gameObject, SceneManager.GetActiveScene());
            GameManager.I.ChengeNewScene(GameSystem.SceneController.GameScene.title, true);
            canNextScene = false;
        }
    }

    private void DisplayFinalScore()
    {
        locationText.text = locationName[GameManager.I.stageSelectHandller.GetStage()];

        scoreText.text = GameManager.I.scoreData.point.ToString();

        var span = new System.TimeSpan(0, 0, GameManager.I.scoreData.boss_defeated_time);
        bossDefeatedTimeText.text = span.ToString(@"mm\:ss");

        totalEnemyDefeatedTimeText.text = GameManager.I.scoreData.total_enemy_defeated.ToString();
        casualtyCountText.text = GameManager.I.scoreData.casualty_count.ToString();
        
    }

}

