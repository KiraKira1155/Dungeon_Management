using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy 
{
    [System.Serializable]
    public class BossBarController
    {
        [SerializeField] private GameObject hpBarParent;
        [SerializeField] private Image hpBar;

        public void Init()
        {
            hpBarParent.SetActive(false);
        }

        public void ShowHPBar()
        {
            hpBarParent.SetActive(true);
        }

        public void DoLateUpdate()
        {
            HPBarController();
        }

        private void HPBarController()
        {
            float maxHp = (float)EnemyManager.I.basicStatus.GetEnemyStatusHP(EnemyManager.EnemyCharactersID.boss);
            if (GameManager.I.stageSelectHandller.GetStage() == 0)
                maxHp = maxHp / 2;

            if (EnemySpawnManager.I != null && EnemySpawnManager.I.IsBossSpawned())
            {
                var bossEnemy = EnemySpawnManager.I.GetBossEnemy();
                if (bossEnemy != null)
                {
                    hpBar.fillAmount = (float)bossEnemy.GetCurrentHP() / maxHp;
                }
            }
        }

        public void HideHpBar()
        {
            hpBarParent.SetActive(true);
        }
       
    }
}

