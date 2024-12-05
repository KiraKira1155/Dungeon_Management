using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSelectManager : Singleton<EquipmentSelectManager>
{
    [SerializeField] private SlotInfo debugSetting;

    private const float CAN_SELECT_INTERVAL_TIME = 0.5f;
    private float canSelectIntervalTime;
    private bool canSelect;

    private int rerunItem;
    private bool isRerun;

    private SlotInfo[] slotInfo = new SlotInfo[SLOT_MAX_NUM];

    public const byte SLOT_MAX_NUM = 3;
    private readonly Color SUB_WEAPON_FONT_COLOR = new Color(0.1529412f, 0.2941177f, 0.4117647f, 1.0f);
    private readonly Color ARMOUR_FONT_COLOR = new Color(0.1529412f, 0.4117647f, 0.2352941f, 1.0f);
    private readonly Color ITEM_FONT_COLOR = new Color(0.4117647f, 0.3058824f, 0.1529412f, 1.0f);

    private const float RERUN_ANIMATION_TIME = 0.3f;
    private float[] panelMoveSpeed = new float[3];
    private const float PANEL_ANIMATION_START_POS = -500;
    private const float PANEL_INIT_Y_POS = -115;
    private float[] panelLocalPos = new float[3]
    {
        -235,
        0,
        235
    };

    private const float MAX_LEVEL_TEXTURE_CHENGE_TIME = 0.08f;
    private float maxLevelChengeTimeCnt;
    private int currentIconNum;


    [SerializeField] private GameObject equipmentSelectCanvas;
    [Header("進化条件設定\n" +
        "※index 0番目は空けておく\n" +
        "SubWeaponManager の SubWeaponID 順に登録すること")]
    [SerializeField] private Armour.ArmourManager.ArmourID[] ELSEArmourList;
    [SerializeField] private Image[] elseSlots;
    [Header("レベルの画像、0が1LV")]
    [SerializeField] private Sprite[] levelImage = new Sprite[2];

    [Header("メイン武器の登録")]
    [SerializeField] private SelectEquipmentSetting mainWeaponSetting;

    [Header("サブ武器の登録\n" +
        "※SubWeaponManager の SubWeaponID 順に登録すること")]
    [SerializeField] private SelectEquipmentSetting[] subWeaponSettings;

    [Header("装備の登録\n" +
        "※ArmourManager の ArmourID 順に登録すること")]
    [SerializeField] private SelectEquipmentSetting[] armourSettings;

    [Header("アイテムの登録")]
    [SerializeField] private SelecItemSetting[] itemSettings;

    [Header("スロット設定")]
    [SerializeField] private Image[] selectSlotPanel = new Image[SLOT_MAX_NUM];
    [SerializeField] private Sprite[] selectSlotImage;
    [SerializeField] private TextMeshProUGUI[] equipmentNameText = new TextMeshProUGUI[SLOT_MAX_NUM];
    [SerializeField] private Image[] equipmentImageSlots = new Image[SLOT_MAX_NUM];
    [SerializeField] private TextMeshProUGUI[] equipmentDescriptionText = new TextMeshProUGUI[SLOT_MAX_NUM];
    [SerializeField] private Image[] levelIcon = new Image[SLOT_MAX_NUM];
    [SerializeField] private Image[] maxLevelIcon;
    [SerializeField] private Sprite[] maxLevelTexture;

    [Header("所有中装備設定")]
    [SerializeField] private Image[] subWeaponSlot = new Image[4];
    [SerializeField] private Image[] armourSlot = new Image[4];

    [Header("再抽選UI設定")]
    [SerializeField] private Image rerunPanel;
    [SerializeField] private Sprite[] rerunImage;
    [SerializeField] private TextMeshProUGUI rerunAmountText;

    /// <summary>
    /// 画像取得用
    /// </summary>
    /// <param name="equipmentTyep"> 装備タイプ </param>
    /// <param name="id"></param>
    /// <returns></returns>
    public Sprite GetSprite(EquipmentType equipmentType, int id)
    {
        id -= 1;
        if(id < 0)
            return null;

        switch (equipmentType)
        {
            case EquipmentType.mainWeapon:
                return mainWeaponSetting.GetSprit();
            case EquipmentType.subWeapon:
                return subWeaponSettings[id].GetSprit();
            default:
                return armourSettings[id].GetSprit();
        }
    }

    public enum EquipmentType
    {
        mainWeapon,
        subWeapon,
        armour,
        item,

        TYPE_AMOUNT
    }

    public override void BattleSceneInit()
    {
        DontDestroyOnLoad(gameObject);
        rerunItem = 0;
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.select)
            return;

        if (!GameManager.I.sceneController.battleSceneLoadEnd)
            return;


        if (isRerun)
        {
            RerunAnimation();
            return;
        }

        //// マウスクリックが検出されたら
        if (Input.GetMouseButtonDown(0) && canSelect)
        {
            if (MouseClickManager.I.CheckCursorOnTheImage(rerunPanel))
            {
                RerunSlot();
                return;
            }
            SelectSlot();
        }

        if(canSelectIntervalTime <= CAN_SELECT_INTERVAL_TIME)
        {
            canSelectIntervalTime += Time.deltaTime;
        }else
        {
            canSelect = true;
        }

        MaxLevelAnimation();
    }

    private void RerunAnimation()
    {
        for (int i = selectSlotPanel.Length - 1; i >= 0; i--)
        {
            if (selectSlotPanel[i].transform.localPosition.x < panelLocalPos[i])
            {
                selectSlotPanel[i].transform.localPosition += new Vector3(panelMoveSpeed[i], PANEL_INIT_Y_POS) * Time.deltaTime;

                if (selectSlotPanel[i].transform.localPosition.x >= panelLocalPos[i])
                {
                    selectSlotPanel[i].transform.localPosition = new Vector2(panelLocalPos[i], PANEL_INIT_Y_POS);
                }
                return;
            }
        }

        if (selectSlotPanel[0].transform.localPosition.x >= panelLocalPos[0])
        {
            isRerun = false;
        }
    }

    private void MaxLevelAnimation()
    {
        maxLevelChengeTimeCnt += Time.deltaTime;
        if(maxLevelChengeTimeCnt < MAX_LEVEL_TEXTURE_CHENGE_TIME)
            return;

        maxLevelChengeTimeCnt = 0;

        currentIconNum++;
        if(currentIconNum == maxLevelTexture.Length)
            currentIconNum = 0;

        foreach(var icon in maxLevelIcon)
        {
            if (icon.gameObject.activeInHierarchy)
            {
                icon.sprite = maxLevelTexture[currentIconNum];
            }
        }
    }

    private void RerunSlot()
    {
        if (rerunItem > 0)
        {
            StartSelectEquipmentEvent();
            isRerun = true;
            rerunItem--;
            rerunAmountText.text = rerunItem + " / 1";

            foreach (var panel in selectSlotPanel)
            {
                panel.gameObject.transform.localPosition = new Vector2(PANEL_ANIMATION_START_POS, PANEL_INIT_Y_POS);
            }
            for (int i = 0; i < panelMoveSpeed.Length; i++)
            {
                panelMoveSpeed[i] = (panelLocalPos[i] - PANEL_ANIMATION_START_POS) / RERUN_ANIMATION_TIME;
            }

            if (rerunItem == 0)
            {
                rerunPanel.sprite = rerunImage[1];
                rerunPanel.color = new Color(1, 1, 1, 0.1f);
            }
        }
    }

    private void SelectSlot()
    {
        var slot = MouseClickManager.I.CheckCursorOnTheImage(selectSlotPanel);
        if (slot == null)
            return;

        var slotNum = 0;

        if (selectSlotPanel[0] == slot)
            slotNum = 0;
        else if (selectSlotPanel[1] == slot)
            slotNum = 1;
        else if(selectSlotPanel[2] == slot)
            slotNum = 2;

        switch (slotInfo[slotNum].type)
        {
            case EquipmentType.mainWeapon:
                Player.PlayerManager.I.currentStatus.WeaponLvUp();
                var angle = 0;
                switch (Player.PlayerManager.I.currentStatus.GetWeaponLv())
                {
                    case 1:
                        angle = 180;
                        break;

                    case 2:
                        angle = 260;
                        break;

                    default:
                        angle = 360;
                        break;
                }
                Player.PlayerManager.I.GetPlayerAutoAttackCollider().SetAttackAngle(angle);
                break;

            case EquipmentType.subWeapon:
                SubWeapon.SubWeaponManager.I.SetSubWeapon((SubWeapon.SubWeaponManager.SubWeaponID)(slotInfo[slotNum].equipmentID));
                break;

            case EquipmentType.armour:
                Armour.ArmourManager.I.SetArmour((Armour.ArmourManager.ArmourID)(slotInfo[slotNum].equipmentID));
                break;

            case EquipmentType.item:

                break;
        }

        equipmentSelectCanvas.SetActive(false);
        canSelect = false;
        Sound.SoundManager.I.ResumeSE();
        GameManager.I.sceneController.SetGameModeForBattleScene(GameSystem.SceneController.GameModeForBattle.battle);
    }

    public void StartSelectEquipmentEvent()
    {
        if(rerunItem > 0)
        {
            rerunPanel.sprite = rerunImage[0];
            rerunPanel.color = new Color(1, 1, 1, 1);
        }
        else
        {
            rerunPanel.sprite = rerunImage[1];
            rerunPanel.color = new Color(1, 1, 1, 0.1f);
        }

#if UNITY_EDITOR
        if (!GameManager.I.test)
#endif
            Sound.SoundManager.I.PauseStopSE();

        rerunAmountText.text = rerunItem + " / 1";
        canSelectIntervalTime = 0.0f;
        canSelect = false;

        equipmentSelectCanvas.SetActive(true);
        GameManager.I.sceneController.SetGameModeForBattleScene(GameSystem.SceneController.GameModeForBattle.select);

        StartCoroutine(RandomSelectSlot());
    }

    private IEnumerator RandomSelectSlot()
    {
        var selectTypeSlots = new List<int>();

        for (int i = 0; i < (int)EquipmentType.TYPE_AMOUNT - 1; i++)
        {
            selectTypeSlots.Add(i);
        }

        // 所持数の状況を確認
        // サブ武器
        var weaponHoldingMax = false;
        if (SubWeapon.SubWeaponManager.I.GetHoldingAmount() == SubWeapon.SubWeaponManager.MAX_HOLDING_NUM)
            weaponHoldingMax = true;
        // アーマー装備
        var armourHoldingMax = false;
        if (Armour.ArmourManager.I.GetHoldingAmount() == Armour.ArmourManager.MAX_HOLDING_NUM)
            armourHoldingMax = true;

        // 所持数が上限の場合、レベル状況を確認
        // メイン武器
        var mainWeaponLvMax = false;
        if(Player.PlayerManager.I.currentStatus.GetWeaponLv() == Player.PlayerCharacterCurrentStatus.MAINWEAPON_MAX_LV)
            mainWeaponLvMax = true;

        // サブ武器
        var weaponAllLvMax = false;
        if (weaponHoldingMax)
        {
            var weponCnt = SubWeapon.SubWeaponManager.I.GetSubWeaponAllData().Length;
            for (int i = 0; i < weponCnt; i++)
            {
                if (SubWeapon.SubWeaponManager.I.GetSubWeaponAllData()[i].currentLv < SubWeapon.SubWeaponManager.MAX_SUB_WEAPON_LEVEL)
                {
                    foreach (var armour in Armour.ArmourManager.I.GetArmourAllData())
                    {
                        if (armour.armour == ELSEArmourList[(int)SubWeapon.SubWeaponManager.I.GetSubWeaponAllData()[i].weaponData.GetWeaponID()])
                        {
                            break;
                        }
                    }
                    break;
                }
                if (SubWeapon.SubWeaponManager.I.GetSubWeaponAllData()[i].currentLv != SubWeapon.SubWeaponManager.MAX_SUB_WEAPON_LEVEL - 1)
                {
                    break;
                }
                if (weponCnt - 1 == i)
                {
                    weaponAllLvMax = true;
                }
            }
        }
        // アーマー
        var armourAllLvMax = false;
        if (armourHoldingMax)
        {
            var armourCnt = Armour.ArmourManager.I.GetArmourAllData().Length;
            for(int i = 0;i < armourCnt;i++)
            {
                if (Armour.ArmourManager.I.GetArmourAllData()[i].currentLv != Armour.ArmourManager.MAX_ARMOUR_LEVEL)
                {
                    break;
                }
                if(armourCnt - 1 == i)
                {
                    armourAllLvMax = true;
                }
            }
        }


        // ===== 強化タイプを選択 =====
        var select = 0;

        if (mainWeaponLvMax)
        {
            for (int removeIndex = 0; removeIndex < selectTypeSlots.Count; removeIndex++)
            {
                if ((int)EquipmentType.mainWeapon == selectTypeSlots[removeIndex])
                {
                    selectTypeSlots.RemoveAt(removeIndex);
                    break;
                }
            }
        }
        if (weaponAllLvMax)
        {
            for (int removeIndex = 0; removeIndex < selectTypeSlots.Count; removeIndex++)
            {
                if ((int)EquipmentType.subWeapon == selectTypeSlots[removeIndex])
                {
                    selectTypeSlots.RemoveAt(removeIndex);
                    break;
                }
            }
        }
        if (armourAllLvMax)
        {
            for (int removeIndex = 0; removeIndex < selectTypeSlots.Count; removeIndex++)
            {
                if ((int)EquipmentType.armour == selectTypeSlots[removeIndex])
                {
                    selectTypeSlots.RemoveAt(removeIndex);
                    break;
                }
            }
        }
        // ===== 強化タイプの選択終了 =====


        // ===== タイプから強化対象を選択 =====
        // サブ武器のランダム範囲設定
        var subWeaponList = new List<int>();
        if (!weaponHoldingMax)
        {
            for (int i = (int)SubWeapon.SubWeaponManager.SubWeaponID.NONE + 1; i < (int)SubWeapon.SubWeaponManager.SubWeaponID.MAX_ID_COUNT; i++)
            {
                var isHolding = false;
                foreach (var weapon in SubWeapon.SubWeaponManager.I.GetSubWeaponAllData())
                {
                    if (weapon.weaponData == null)
                        continue;

                    if (i == (int)weapon.weaponData.GetWeaponID())
                    {
                        isHolding = true;
                        if (weapon.currentLv < SubWeapon.SubWeaponManager.MAX_SUB_WEAPON_LEVEL)
                        {
                            var canELSE = false;
                            foreach (var armour in Armour.ArmourManager.I.GetArmourAllData())
                            {
                                if (armour.armour == ELSEArmourList[(int)weapon.weaponData.GetWeaponID()])
                                {
                                    canELSE = true;
                                    subWeaponList.Add((int)weapon.weaponData.GetWeaponID());
                                    break;
                                }
                            }
                            if (!canELSE)
                            {
                                if (weapon.currentLv < SubWeapon.SubWeaponManager.MAX_SUB_WEAPON_LEVEL - 1)
                                {
                                    subWeaponList.Add((int)weapon.weaponData.GetWeaponID());
                                }
                            }
                            break;
                        }
                    }
                }

                if (!isHolding)
                {
                    subWeaponList.Add(i);
                }
            }
        }
        else
        {
            if (!weaponAllLvMax)
            {
                foreach(var weapon in SubWeapon.SubWeaponManager.I.GetSubWeaponAllData())
                {
                    if(weapon.currentLv == SubWeapon.SubWeaponManager.MAX_SUB_WEAPON_LEVEL)
                        continue;

                    var canELSE = false;
                    foreach (var armour in Armour.ArmourManager.I.GetArmourAllData())
                    {
                        if (armour.armour == ELSEArmourList[(int)weapon.weaponData.GetWeaponID()])
                        {
                            canELSE = true;
                            subWeaponList.Add((int)weapon.weaponData.GetWeaponID());
                            break;
                        }
                    }
                    if (!canELSE)
                    {
                        if (weapon.currentLv < SubWeapon.SubWeaponManager.MAX_SUB_WEAPON_LEVEL - 1)
                        {
                            subWeaponList.Add((int)weapon.weaponData.GetWeaponID());
                        }
                    }
                }
            }
        }

        // アーマーのランダム範囲設定
        var armourList = new List<int>();
        if (!armourHoldingMax)
        {
            for (int i = (int)Armour.ArmourManager.ArmourID.NONE + 1; i < (int)Armour.ArmourManager.ArmourID.MAX_ID_COUNT; i++)
            {
                var isHolding = false;
                foreach (var armour in Armour.ArmourManager.I.GetArmourAllData())
                {
                    if (i == (int)armour.armour)
                    {
                        isHolding = true;
                        if (armour.currentLv < Armour.ArmourManager.MAX_ARMOUR_LEVEL)
                        {
                            armourList.Add((int)armour.armour);
                            break;
                        }
                    }
                }

                if (!isHolding)
                {
                    armourList.Add(i);
                }
            }
        }
        else
        {
            if (!armourAllLvMax)
            {
                foreach (var armour in Armour.ArmourManager.I.GetArmourAllData())
                {
                    if (armour.currentLv < Armour.ArmourManager.MAX_ARMOUR_LEVEL)
                    {
                        armourList.Add((int)armour.armour);
                    }
                }
            }
        }

        yield return null;

        // 強化する対象の決定
        var isSelected = false;
        for (int i = 0; i < SLOT_MAX_NUM; i++)
        {
            var index = Random.Range(0, selectTypeSlots.Count);
            // 強化先がない場合はアイテムに
            if (selectTypeSlots.Count == 0)
            {
                select = (int)EquipmentType.item;
            }
            else
            {
                index = Random.Range(0, selectTypeSlots.Count);
                select = selectTypeSlots[index];

                if (select == (int)EquipmentType.mainWeapon)
                    selectTypeSlots.RemoveAt(index);
            }

            if (GameManager.I.test && i == 0)
            {
                if(debugSetting.type == EquipmentType.subWeapon)
                {
                    var level = SubWeapon.SubWeaponManager.I.GetSubWeaponCurrentLv((SubWeapon.SubWeaponManager.SubWeaponID)debugSetting.equipmentID);
                    if (level == 255)
                        level = 0;
                    else
                        level += 1;


                    if (level != SubWeapon.SubWeaponManager.MAX_SUB_WEAPON_LEVEL + 1)
                    {
                        slotInfo[i] = new SlotInfo()
                        {
                            equipmentID = debugSetting.equipmentID,
                            type = EquipmentType.subWeapon,
                            nextLevel = level
                        };
                        continue;
                    }
                }
                else if (debugSetting.type == EquipmentType.armour)
                {
                    var level = Armour.ArmourManager.I.GetArmourCurrentLv((Armour.ArmourManager.ArmourID)debugSetting.equipmentID);
                    if (level == 255)
                        level = 0;
                    else
                        level += 1;


                    if (level != Armour.ArmourManager.MAX_ARMOUR_LEVEL + 1)
                    {
                        slotInfo[i] = new SlotInfo()
                        {
                            equipmentID = debugSetting.equipmentID,
                            type = EquipmentType.armour,
                            nextLevel = level
                        };
                        continue;
                    }
                }
            }

            while (!isSelected)
            {
                switch (select)
                {
                    case (int)EquipmentType.mainWeapon:
                        slotInfo[i] = new SlotInfo()
                        {
                            equipmentID = 0,
                            nextLevel = (byte)(Player.PlayerManager.I.currentStatus.GetWeaponLv() + 1),
                            type = EquipmentType.mainWeapon
                        };
                        isSelected = true;
                        break;

                    case (int)EquipmentType.subWeapon:
                        if(subWeaponList.Count == 0)
                        {
                            if(selectTypeSlots.Count == 0)
                            {
                                select = (int)EquipmentType.item;
                                break;
                            }

                            index = Random.Range(0, selectTypeSlots.Count);
                            select = selectTypeSlots[index];
                            for(int removeIndex = 0; removeIndex < selectTypeSlots.Count; removeIndex++)
                            {
                                if((int)EquipmentType.subWeapon == selectTypeSlots[removeIndex])
                                {
                                    selectTypeSlots.RemoveAt(removeIndex);
                                    break;
                                }
                            }
                            break;
                        }

                        index = Random.Range(0, subWeaponList.Count);
                        var subWeaponID = subWeaponList[index];
                        subWeaponList.RemoveAt(index);

                        var subWeaponLv = SubWeapon.SubWeaponManager.I.GetSubWeaponCurrentLv((SubWeapon.SubWeaponManager.SubWeaponID)subWeaponID);
                        if (subWeaponLv == 255)
                            subWeaponLv = 0;
                        else
                            subWeaponLv += 1;

                        slotInfo[i] = new SlotInfo()
                        {
                            equipmentID = subWeaponID,
                            nextLevel = subWeaponLv,
                            type = EquipmentType.subWeapon
                        };
                        isSelected = true;
                        break;

                    case (int)EquipmentType.armour:
                        if (armourList.Count == 0)
                        {
                            if (selectTypeSlots.Count == 0)
                            {
                                select = (int)EquipmentType.item;
                                break;
                            }

                            index = Random.Range(0, selectTypeSlots.Count);
                            select = selectTypeSlots[index];
                            for (int removeIndex = 0; removeIndex < selectTypeSlots.Count; removeIndex++)
                            {
                                if ((int)EquipmentType.armour == selectTypeSlots[removeIndex])
                                {
                                    selectTypeSlots.RemoveAt(removeIndex);
                                    break;
                                }
                            }
                            break;
                        }

                        index = Random.Range(0, armourList.Count);
                        var armourID = armourList[index];
                        armourList.RemoveAt(index);

                        var armourLv = Armour.ArmourManager.I.GetArmourCurrentLv((Armour.ArmourManager.ArmourID)armourID);
                        if (armourLv == 255)
                            armourLv = 0;
                        else
                            armourLv += 1;

                        slotInfo[i] = new SlotInfo()
                        {
                            equipmentID = armourID,
                            nextLevel = armourLv,
                            type = EquipmentType.armour
                        };
                        isSelected = true;
                        break;

                    case (int)EquipmentType.item:
                        var itemIndex = Random.Range(0, itemSettings.Length);

                        slotInfo[i] = new SlotInfo()
                        {
                            equipmentID = itemIndex,
                            nextLevel = 0,
                            type = EquipmentType.item
                        };
                        isSelected = true;
                        break;

                }
            }

            isSelected = false;
        }

        yield return null;
        // 強化する対象の終了
        // ===== 強化対象を選択終了 =====

        // ===== スロット描画 =====
        for (int i = 0; i < SLOT_MAX_NUM; i++)
        {
            var id = slotInfo[i].equipmentID - 1;
            var level = slotInfo[i].nextLevel;

            switch (slotInfo[i].type)
            {
                case EquipmentType.mainWeapon:
                    DisplayMainWeaponrSlot(i, level);
                    break;

                case EquipmentType.subWeapon:
                    DisplaySubWeaponSlot(i, id, level);
                    break;

                case EquipmentType.armour:
                    DisplayArmourSlot(i, id, level);
                    break;

                case EquipmentType.item:
                    DisplayItemSlot(i, id + 1);
                    break;
            }

            // レベル表示
            DisplaySlotNextLevel(i, level);
        }

        // 保有中装備表示
        DisplayHoldingEquipment();
        // ===== スロット描画終了 =====
    }

    private void DisplayHoldingEquipment()
    {
        // 武器スロット
        subWeaponSlot[0].sprite = mainWeaponSetting.GetSprit();

        var weaponList = SubWeapon.SubWeaponManager.I.GetSubWeaponAllData();
        for(int i = 1; i < weaponList.Length + 1; i++)
        {
            if(weaponList[i - 1].weaponData == null)
            {
                subWeaponSlot[i].enabled = false;
            }
            else
            {
                subWeaponSlot[i].enabled = true;
                subWeaponSlot[i].sprite = subWeaponSettings[(int)weaponList[i - 1].weaponData.GetWeaponID() - 1].GetSprit();
            }
        }

        // 装備スロット
        var armourList = Armour.ArmourManager.I.GetArmourAllData();
        for (int i = 0; i < armourList.Length; i++)
        {
            if(armourList[i].armour == 0)
            {
                armourSlot[i].enabled = false;
            }
            else
            {
                armourSlot[i].enabled = true;
                armourSlot[i].sprite = armourSettings[(int)armourList[i].armour - 1].GetSprit();
            }
        }
    }

    private void DisplayMainWeaponrSlot(int slotIndex , byte level)
    {
        level--;
        selectSlotPanel[slotIndex].sprite = selectSlotImage[0];
        equipmentNameText[slotIndex].text = mainWeaponSetting.GetName();
        equipmentNameText[slotIndex].color = SUB_WEAPON_FONT_COLOR;
        equipmentImageSlots[slotIndex].sprite = mainWeaponSetting.GetSprit();
        equipmentDescriptionText[slotIndex].text = mainWeaponSetting.GetDescription(level);
        equipmentDescriptionText[slotIndex].color = SUB_WEAPON_FONT_COLOR;
        elseSlots[slotIndex].enabled = false;
    }
    private void DisplaySubWeaponSlot(int slotIndex, int id, byte level)
    {
        selectSlotPanel[slotIndex].sprite = selectSlotImage[0];
        equipmentNameText[slotIndex].text = subWeaponSettings[id].GetName();
        equipmentNameText[slotIndex].color = SUB_WEAPON_FONT_COLOR;
        equipmentImageSlots[slotIndex].sprite = subWeaponSettings[id].GetSprit();
        equipmentDescriptionText[slotIndex].text = subWeaponSettings[id].GetDescription(level);
        equipmentDescriptionText[slotIndex].color = SUB_WEAPON_FONT_COLOR;

        elseSlots[slotIndex].enabled = true;
        elseSlots[slotIndex].sprite = armourSettings[(int)ELSEArmourList[id + 1] - 1].GetSprit();
    }
    private void DisplayArmourSlot(int slotIndex, int id, byte level)
    {
        selectSlotPanel[slotIndex].sprite = selectSlotImage[1];
        equipmentNameText[slotIndex].text = armourSettings[id].GetName();
        equipmentNameText[slotIndex].color = ARMOUR_FONT_COLOR;
        equipmentImageSlots[slotIndex].sprite = armourSettings[id].GetSprit();
        equipmentDescriptionText[slotIndex].text = armourSettings[id].GetDescription(level);
        equipmentDescriptionText[slotIndex].color = ARMOUR_FONT_COLOR;

        elseSlots[slotIndex].enabled = true;
        elseSlots[slotIndex].sprite = subWeaponSettings[id].GetSprit();
    }
    private void DisplayItemSlot(int slotIndex, int id)
    {
        selectSlotPanel[slotIndex].sprite = selectSlotImage[2];
        equipmentNameText[slotIndex].text = itemSettings[id].GetName();
        equipmentNameText[slotIndex].color = ITEM_FONT_COLOR;
        equipmentImageSlots[slotIndex].sprite = itemSettings[id].GetSprit();
        equipmentDescriptionText[slotIndex].text = itemSettings[id].GetDescription();
        equipmentDescriptionText[slotIndex].color = ITEM_FONT_COLOR;
        elseSlots[slotIndex].enabled = false;
    }

    private void DisplaySlotNextLevel(int slotIndex, byte level)
    {
        if (slotInfo[slotIndex].type == EquipmentType.item)
        {
            levelIcon[slotIndex].gameObject.SetActive(false);
            maxLevelIcon[slotIndex].gameObject.SetActive(false);
        }
        else
        {
            if (level == 3)
            {
                maxLevelChengeTimeCnt = 0.0f;
                currentIconNum = 0;
                levelIcon[slotIndex].gameObject.SetActive(false);
                maxLevelIcon[slotIndex].gameObject.SetActive(true);
                maxLevelIcon[slotIndex].sprite = maxLevelTexture[0];
                return;
            }
            else
            {
                levelIcon[slotIndex].gameObject.SetActive(true);
                levelIcon[slotIndex].sprite = levelImage[level];
                maxLevelIcon[slotIndex].gameObject.SetActive(false);
            }
        }
    }

    public void GetReRunItem()
    {
        rerunItem++;
    }

    [System.Serializable]
    private struct SlotInfo
    {
        public int equipmentID;
        [HideInInspector] public byte nextLevel;
        public EquipmentType type;
    }
}

[System.Serializable]
public class SelectEquipmentSetting
{
    [SerializeField] private string equipmentName;
    [SerializeField] private Sprite equipmentSprit;
    [Header("レベルごとの取得時の説明")]
    [SerializeField] private string[] equipmentDescription;

    public Sprite GetSprit()
    {
        return equipmentSprit;
    }

    public string GetName()
    {
        return equipmentName;
    }

    public string GetDescription(byte level)
    {
        return equipmentDescription[level];
    }
}

[System.Serializable]
public class SelecItemSetting
{
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemSprit;
    [Header("レベルごとの取得時の説明")]
    [SerializeField] private string itemDescription;
    [SerializeField] private Item.ItemManager.ItemID itemID;

    public Item.ItemManager.ItemID GetID()
    {
        return itemID;
    }

    public Sprite GetSprit()
    {
        return itemSprit;
    }

    public string GetName()
    {
        return itemName;
    }

    public string GetDescription()
    {
        return itemDescription;
    }
}


