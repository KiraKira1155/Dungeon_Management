using Enemy;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : Singleton<TitleManager>
{
    [SerializeField] private GameObject startText;
    [SerializeField] private TextMeshProUGUI userIdText;
    [SerializeField] private GameObject titleCanvas;
    [Header("ステージセレクト設定")]
    [SerializeField] private int MaxStageNum;
    [SerializeField] private GameObject stageSelectCanvas;
    [SerializeField] private Image rightArrow;
    [SerializeField] private Image leftArrow;
    [SerializeField] private Image[] stageImages = new Image[2];
    [SerializeField] private TextMeshProUGUI[] stageTexts = new TextMeshProUGUI[2];
    [Tooltip("0が左側アニメーションスタートの位置\n" +
        "1が表示位置\n" +
        "2が左側アニメーションスタートの位置")]
    [SerializeField] private Vector2[] stageImageLocalPos = new Vector2[3];
    [Tooltip("アニメーションの時間")]
    [SerializeField] private float stageAnimationSpeed;
    private float stageMoveSpeed;

    private int previousStageNum;
    private int stageNum;

    private float interval = 1.0f;
    private float time;

    private bool isTitle;
    private bool isLoad;
    private bool canNextScene;
    private bool isAnimation;

    private void Awake()
    {
        Init();

        time = 0;
        StartCoroutine(LoadID());
        isLoad = false;
        titleCanvas.SetActive(true);
        stageSelectCanvas.SetActive(false);
        stageNum = 0;
        isTitle = true;

        stageImages[0].transform.localPosition = stageImageLocalPos[2];
        stageImages[1].transform.localPosition = stageImageLocalPos[1];
        stageTexts[1].text = GetStageText(stageNum);
    }

    private IEnumerator LoadID()
    {
        do
        {
            yield return null;
        } while (!SaveDataManager.SaveFileCheck());

        userIdText.text = "ID : " + SaveDataManager.Load().id;
        StartCoroutine(GameManager.I.UnLoadPreviousScene(true));
        isLoad = true;
    }

    private void Update()
    {
        if (!isLoad)
            return;

        if (isTitle)
        {

            time += Time.deltaTime;

            if (time > interval)
            {
                time = 0;
                if (startText.activeSelf)
                {
                    startText.SetActive(false);
                }
                else
                {
                    startText.SetActive(true);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                isTitle = false;
                Sound.SoundManager.I.PlayButtonSE(Sound.SoundManager.buttonAudioID.startButton);
                StartCoroutine(FadeOut());
            }
        }
        else
        {
            StageChangeAnimation();

            if (Input.GetMouseButtonDown(0) && canNextScene)
            {
                StageChenge();

                if (MouseClickManager.I.CheckCursorOnTheImage(stageImages) && !isAnimation)
                {
                    Sound.SoundManager.I.PlayButtonSE(Sound.SoundManager.buttonAudioID.selectButton);
                    GameManager.I.stageSelectHandller.SetStage(stageNum);
                    GameManager.I.ChengeNewScene(GameSystem.SceneController.GameScene.battle, true);
                    canNextScene = false;
                }
            }
        }
    }

    private void StageChenge()
    {
        previousStageNum = stageNum;

        stageMoveSpeed = (stageImageLocalPos[1].x - stageImageLocalPos[0].x) / stageAnimationSpeed;

        if (MouseClickManager.I.CheckCursorOnTheImage(rightArrow))
        {
            if (stageNum < MaxStageNum)
            {
                isAnimation = true;
                stageNum++;

                Sound.SoundManager.I.PlayButtonSE(Sound.SoundManager.buttonAudioID.pauseButton);
                stageImages[0].transform.localPosition = stageImageLocalPos[1];
                stageTexts[0].text = GetStageText(previousStageNum);
                stageImages[1].transform.localPosition = stageImageLocalPos[2];
                stageTexts[1].text = GetStageText(stageNum);
            }
        }
        else if (MouseClickManager.I.CheckCursorOnTheImage(leftArrow))
        {
            if (stageNum > 0)
            {              
                isAnimation = true;
                stageNum--;

                Sound.SoundManager.I.PlayButtonSE(Sound.SoundManager.buttonAudioID.pauseButton);
                stageImages[0].transform.localPosition = stageImageLocalPos[1];
                stageTexts[0].text = GetStageText(previousStageNum);
                stageImages[1].transform.localPosition = stageImageLocalPos[0];
                stageTexts[1].text = GetStageText(stageNum);
            }
        }
    }

    private void StageChangeAnimation()
    {
        if (!isAnimation)
            return;

        // 右側の矢印をクリック
        // 左側にアニメーション
        if(previousStageNum < stageNum)
        {
            stageImages[0].transform.localPosition -= new Vector3(stageMoveSpeed, stageImageLocalPos[0].y, 0.0f) * Time.deltaTime;
            stageImages[1].transform.localPosition -= new Vector3(stageMoveSpeed, stageImageLocalPos[0].y, 0.0f) * Time.deltaTime;

            if(stageImages[0].transform.localPosition.x < stageImageLocalPos[0].x)
            {
                stageImages[1].transform.localPosition = stageImageLocalPos[1];
                stageImages[0].transform.localPosition = stageImageLocalPos[0];
                isAnimation = false;
            }
        }
        // 左側の矢印をクリック
        // 右側にアニメーション
        else
        {
            stageImages[0].transform.localPosition += new Vector3(stageMoveSpeed, stageImageLocalPos[0].y, 0.0f) * Time.deltaTime;
            stageImages[1].transform.localPosition += new Vector3(stageMoveSpeed, stageImageLocalPos[0].y, 0.0f) * Time.deltaTime;

            if (stageImages[0].transform.localPosition.x > stageImageLocalPos[2].x)
            {
                stageImages[1].transform.localPosition = stageImageLocalPos[1];
                stageImages[0].transform.localPosition = stageImageLocalPos[2];
                isAnimation = false;
            }
        }
    }

    private string GetStageText(int stageNum)
    {
        if (stageNum == 0)
            return "訓練場";
        else
            return "α-1";
    }

    private IEnumerator FadeOut()
    {
        FadeManager.I.fadeOut();

        yield return new WaitForSeconds(FadeManager.I.GetFadeTime());

        titleCanvas.SetActive(false);
        stageSelectCanvas.SetActive(true);

        StartCoroutine(FadeIn());
    }


    private IEnumerator FadeIn()
    {
        FadeManager.I.fadeIn();
        yield return new WaitForSeconds(FadeManager.I.GetFadeTime());
        canNextScene = true;
    }


}
