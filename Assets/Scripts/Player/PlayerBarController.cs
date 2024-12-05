using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    [System.Serializable]
    public class PlayerBarController
    {
        [SerializeField] private GameObject battleUICanvas;
        [Header("ÉvÉåÉCÉÑÅ[UIê›íË")]
        [SerializeField] private Image hpBar;
        [SerializeField] private Image autoAttackIntervalBar;
        [SerializeField] private Image expBar;
        [SerializeField] private TextMeshProUGUI playerLvText;
        private const string lvText = "Lv.";
        StringBuilder allLvText = new StringBuilder(8);

        public void DoLateUpdate()
        {
            HPBarController();
            AutoAttackIntervalBarController();
        }

        private void HPBarController()
        {
            var player = PlayerManager.I.currentStatus;
            hpBar.fillAmount = (float)player.GetCurrentHP() / (float)player.GetMaxHP();
        }

        private void AutoAttackIntervalBarController()
        {
            var player = PlayerManager.I.controller.autoAttack;
            autoAttackIntervalBar.fillAmount = player.GetIntervalCount() / player.GetAttackInterval();
        }

        public void ExpBarController()
        {
            var player = PlayerManager.I.currentStatus;
            expBar.fillAmount = (float)player.GetCurrentExp() / (float)player.nextLvCalculation.GetNextNeedExp();
        }

        public void PlayerLvDisplayInOder()
        {
            allLvText.Clear();
            allLvText.Append(lvText);
            allLvText.Append(PlayerManager.I.currentStatus.GetCurrentLv());

            playerLvText.text = allLvText.ToString();
        }

        public void HideUI()
        {
            battleUICanvas.SetActive(false);
        }

        public void DisplayUI()
        {
            battleUICanvas.SetActive(true);
        }
    }
}
