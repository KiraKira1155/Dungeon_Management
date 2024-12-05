using Enemy;
using GameSystem;
using Map;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] private Image[] menuImages;
    [SerializeField] private GameObject battleMune;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TextMeshProUGUI timeText;
    [Header("討伐数表示用")]
    [SerializeField] private GameObject killCountObj;
    [SerializeField] private TextMeshProUGUI killCOuntText;
    [Header("イベント表示用")]
    [SerializeField] private GameObject EnemyIventParentObj;
    [SerializeField] private GameObject BossHpCanvas;
    [Header("警告文設定")]
    [SerializeField] private warningMessageSetting warningMessageSetting;
    [Header("装備表示用")]
    [SerializeField] private Image[] subWeapon = new Image[3];
    [SerializeField] private Image[] armour = new Image[4];


    private bool init;

    private enum MenuButton
    {
        pause,
        play,
        battleQuit
    }

    private void Awake()
    {
        Init();
        init = false;
    }

    public override void BattleSceneInit()
    {
        killCOuntText.text = "0";

        battleMune.SetActive(true);
        pauseMenu.SetActive(false);
        killCountObj.SetActive(true);
        EnemyIventParentObj.SetActive(true);
        warningMessageSetting.InitWarningMessage(GameManager.I.stageSelectHandller.GetStage());

        var count = MapManager.I.countDowns[GameManager.I.stageSelectHandller.GetStage()];
        TimeController.InitTimer(timeText, count.countdownMinutes, count.countdownSeconds);

        init = true;
    }

    private void Update()
    {
        if(!init)
            return;

        warningMessageSetting.UpdateWarningMessage();

        if (GameManager.I.sceneController.GetGameModeForBattleScene() == SceneController.GameModeForBattle.battle || GameManager.I.sceneController.GetGameModeForBattleScene() == SceneController.GameModeForBattle.pause)
        {
            // 停止
            if (MouseClickManager.I.PauseImageClick(menuImages[(int)MenuButton.pause]))
            {
                GameManager.I.sceneController.SetGameModeForBattleScene(GameSystem.SceneController.GameModeForBattle.pause);

                battleMune.SetActive(false);
                pauseMenu.SetActive(true);
                DisplayEquipment();
                Sound.SoundManager.I.PauseStopSE();
            }
            // 再生
            else if (MouseClickManager.I.PauseImageClick(menuImages[(int)MenuButton.play]))
            {
                GameManager.I.sceneController.SetGameModeForBattleScene(GameSystem.SceneController.GameModeForBattle.battle);

                battleMune.SetActive(true);
                pauseMenu.SetActive(false);
                Sound.SoundManager.I.ResumeSE();
            }
            //else if (MouseClickManager.I.CheckCursorOnTheImage(menuImages) == menuImages[(int)MenuButton.battleQuit])
            //{
            //    GameManager.I.sceneController.SetGameScene(GameSystem.SceneController.GameScene.stageSelect);

            //}
        }
    }

    public void DisplayEquipment()
    {
        byte index = 0;
        foreach (var weapon in subWeapon)
        {
            weapon.sprite = EquipmentSelectManager.I.GetSprite(EquipmentSelectManager.EquipmentType.subWeapon, (int)SubWeapon.SubWeaponManager.I.GetHoldingSubWeaponID(index));

            if(weapon.sprite == null)
                weapon.enabled = false;
            else
                weapon.enabled = true;

            index++;
        }

        index = 0;
        foreach(var armour in armour)
        {
            armour.sprite = EquipmentSelectManager.I.GetSprite(EquipmentSelectManager.EquipmentType.armour, (int)Armour.ArmourManager.I.GetArmourAllData()[index].armour);

            if(armour.sprite == null)
                armour.enabled = false;
            else
                armour.enabled = true;

            index++;
        }
    }

    public void HideUI()
    {
        menuImages[(int)MenuButton.pause].enabled = false;
        killCountObj.SetActive(false);
        EnemyIventParentObj.SetActive(false);
        timeText.enabled = false;
        if (EnemySpawnManager.I.GetBossEnemy() != null)
        {
            BossHpCanvas.SetActive(false);
        }
    }

    public void DisplayUI()
    {
        menuImages[(int)MenuButton.pause].enabled = true;
        killCountObj.SetActive(true);
        EnemyIventParentObj.SetActive(true);
        timeText.enabled = true;

        if (EnemySpawnManager.I.GetBossEnemy() != null)
        {
            BossHpCanvas.SetActive(true);
        }
    }


    public void SetBGMVolume(float volume)
    {
        Sound.SoundManager.I.SetBGMVolume(volume);
    }

    public void SetSEVolume(float volume)
    {
        Sound.SoundManager.I.SetSEVolume(volume);
    }

    public void SetVoiceVolume(float volume)
    {
        {
            Sound.SoundManager.I.SetVoiceVolume(volume); ;
        }
    }

    public void KillCountText(int killNum)
    {
        killCOuntText.text = killNum.ToString();
    }
}

[System.Serializable]
public class warningMessageSetting
{
    [SerializeField] private float displayTime;
    [SerializeField] private float flashingIntervalTime;
    [SerializeField] private Vector3 maxSize;
    [SerializeField] private Vector3 minSize;
    [SerializeField] private int flashingNum;
    [Header("ステージごとの設定")]
    [SerializeField] private StageSetting[] stageSettings;
    [Header("表示内容設定")]
    [SerializeField] private GameObject background; 
    [SerializeField] private Image enemy;
    [SerializeField] private Image midBoss;
    [SerializeField] private Image boss;

    private int maxWarningMessage;
    private Vector3 scaleChengeSpeed;
    private int currentWarningMessageNum;
    private StageSetting currentStageSetting;
    private bool isDisplayWarnimgMessage;
    private bool isExpantion;
    private Image targetMessage;
    private float displayCnt;
    private float flashingTimeCnt;
    private int flashingCnt;
    private bool canFlashing;

    public enum WarningMessageID
    {
        enemy,
        midBoss,
        boss
    }

    public void UpdateWarningMessage()
    {
        if (GameManager.I.sceneController.GetGameModeForBattleScene() != SceneController.GameModeForBattle.battle)
            return;

        if (isDisplayWarnimgMessage)
        {
            displayCnt += Time.deltaTime;
            if (displayCnt >= displayTime)
            {
                isDisplayWarnimgMessage = false;
                background.SetActive(false);
                return;
            }



            if (!canFlashing)
                return;

            flashingTimeCnt += Time.deltaTime;

            if (isExpantion)
            {
                if (flashingCnt == flashingNum)
                {
                    targetMessage.transform.localScale += scaleChengeSpeed * Time.deltaTime / 2;
                    if (flashingTimeCnt >= flashingIntervalTime * 2.2f)
                    {
                        canFlashing = false;
                        return;
                    }
                }
                else
                {
                    targetMessage.transform.localScale += scaleChengeSpeed * Time.deltaTime;

                    if (flashingTimeCnt >= flashingIntervalTime)
                    {
                        flashingTimeCnt = 0;
                        targetMessage.transform.localScale = maxSize;
                        isExpantion = false;
                        flashingCnt++;
                    }
                }
            }
            else
            {
                targetMessage.transform.localScale -= scaleChengeSpeed * Time.deltaTime;
                if (flashingTimeCnt >= flashingIntervalTime)
                {

                    flashingTimeCnt = 0;
                    targetMessage.transform.localScale = minSize;
                    isExpantion = true;
                }
            }
        }

        if (maxWarningMessage == currentWarningMessageNum)
            return;

        if (currentStageSetting.GetDisplayStartTime(currentWarningMessageNum) <= TimeController.GetTimeElpsed())
        {
            Debug.Log(currentStageSetting.GetDisplayStartTime(currentWarningMessageNum));
            HideMessage();
            switch (currentStageSetting.GetMessageID(currentWarningMessageNum))
            {
                case WarningMessageID.enemy:
                    targetMessage = enemy;
                    break;

                case WarningMessageID.midBoss:
                    targetMessage = midBoss;
                    break;

                case WarningMessageID.boss:
                    targetMessage = boss;
                    break;
            }
            background.SetActive(true);
            displayCnt = 0;
            flashingTimeCnt = 0;
            flashingCnt = 0;
            canFlashing = true;
            isExpantion = false;
            targetMessage.enabled = true;
            targetMessage.transform.localScale = maxSize;
            currentWarningMessageNum++;
            isDisplayWarnimgMessage = true;
        }
    }

    public void InitWarningMessage(int stageNum)
    {
        HideMessage();
        currentWarningMessageNum = 0;
        background.SetActive(false);

        scaleChengeSpeed = new Vector3((maxSize.x - minSize.x) / flashingIntervalTime, (maxSize.y - minSize.y) / flashingIntervalTime, 0);
        currentStageSetting = stageSettings[stageNum];
        maxWarningMessage = currentStageSetting.GetMaxMassageCount();
    }

    private void HideMessage()
    {
        enemy.gameObject.SetActive(true);
        midBoss.gameObject.SetActive(true);
        boss.gameObject.SetActive(true);

        enemy.enabled = false;
        midBoss.enabled = false;
        boss.enabled = false;
    }

    [System.Serializable]
    private class StageSetting
    {
        [SerializeField] private string stageName;
        [SerializeField] private int[] displayStartTime;
        [SerializeField] private WarningMessageID[] messageID;

        public int GetDisplayStartTime(int index)
        {
            return displayStartTime[index];
        }

        public WarningMessageID GetMessageID(int index)
        {
            return messageID[index];
        }

        public int GetMaxMassageCount()
        {
            return displayStartTime.Length;
        }
    }
}
