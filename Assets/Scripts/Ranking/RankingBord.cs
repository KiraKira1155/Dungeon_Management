using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RankingBord : MonoBehaviour
{
    [Header("表示制御用")]
    [SerializeField] private Image rankingBordImage;
    [SerializeField] private GameObject moreInfoObj;

    [Header("ランキング情報表示用")]
    [SerializeField] private TextMeshProUGUI rankingNumber;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI score;
    [Header("詳細情報表示用")]
    [SerializeField] private Image weapon_0;
    [SerializeField] private Image weapon_1;
    [SerializeField] private Image weapon_2;
    [SerializeField] private Image armour_0;
    [SerializeField] private Image armour_1;
    [SerializeField] private Image armour_2;
    [SerializeField] private Image armour_3;
    [SerializeField] private TextMeshProUGUI damageCount;
    [SerializeField] private TextMeshProUGUI totalDefeats;
    [SerializeField] private TextMeshProUGUI clearTime;

    public void InitSetting(RankingHandler.RankingData rankingData)
    {
        rankingNumber.text = rankingData.ranking.ToString();
        playerName.text = rankingData.user_nm;
        score.text = rankingData.point.ToString();

        var weapon = EquipmentSelectManager.EquipmentType.subWeapon;
        weapon_0.sprite = EquipmentSelectManager.I.GetSprite(weapon, rankingData.use_weapon_1);
        weapon_1.sprite = EquipmentSelectManager.I.GetSprite(weapon, rankingData.use_weapon_2);
        weapon_2.sprite = EquipmentSelectManager.I.GetSprite(weapon, rankingData.use_weapon_3);

        var armour = EquipmentSelectManager.EquipmentType.armour;
        armour_0.sprite = EquipmentSelectManager.I.GetSprite(armour, rankingData.use_equipment_1);
        armour_1.sprite = EquipmentSelectManager.I.GetSprite(armour, rankingData.use_equipment_2);
        armour_2.sprite = EquipmentSelectManager.I.GetSprite(armour, rankingData.use_equipment_3);
        armour_3.sprite = EquipmentSelectManager.I.GetSprite(armour, rankingData.use_equipment_4);

        damageCount.text = rankingData.casualty_count.ToString();
        totalDefeats.text = rankingData.total_enemy_defeated.ToString();
        clearTime.text = rankingData.boss_defeated_time.ToString();
        moreInfoObj.SetActive(false);
    }

    public Image GetRankingBordImage()
    {
        return rankingBordImage;
    }

    public void DisplayMoreInfo()
    {
        moreInfoObj.SetActive(!moreInfoObj.activeInHierarchy);
    }
}
