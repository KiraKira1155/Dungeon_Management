using Enemy;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerSkillManager : Singleton<PlayerSkillManager>
    {
        [SerializeField] private Transform skillCanvas;

        [SerializeField] private GameObject[] skillObj;
        [SerializeField] private GameObject darkenScreen;
        [SerializeField] private Animator cutInAnimator;
        [SerializeField] private SpriteRenderer cutInSprite;

        [SerializeField] private Image notSkillImage;
        [SerializeField] private Image skillPossibleImage;
        [SerializeField] private Image skillImpossibleImage;

        private const float POINT_INCREASE_INTERVAL_TIME = 1.0f;
        private const int INCREASE_TIME_POINT = 100;
        private int currentSkillPoints = 0;
        private float skillPointTimer = 0f;

        private int skillNumToActivate = 0;

        private void Awake()
        {
            Init();
        }

        public override void BattleSceneInit()
        {
            darkenScreen.SetActive(false);
            cutInAnimator.enabled = false;
            cutInSprite.enabled = false;

            skillPossibleImage.gameObject.SetActive(false);
            skillImpossibleImage.gameObject.SetActive(true);

            currentSkillPoints = 0;
            skillPointTimer = 0.0f;

            UpdateSkillUI();
        }

        private void Update()
        {
            skillCanvas.position = CameraManager.I.GetMainCameraPos().position;


            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
            {
                skillPointTimer += Time.deltaTime;
                if (skillPointTimer >= POINT_INCREASE_INTERVAL_TIME)
                {
                    skillPointTimer = 0f;
                    IncreaseSkillPoints(INCREASE_TIME_POINT);
                }

                if (currentSkillPoints >= PlayerManager.I.basicStatus.GetNeedSkillPoint(PlayerManager.I.currentStatus.GetCharacterID()) && Input.GetMouseButton(1))
                {
                    Sound.SoundManager.I.PauseStopSE();
                    ActivateSkillWithCutIn(0);
                    HideUI();
                    GameManager.I.sceneController.SetGameModeForBattleScene(GameSystem.SceneController.GameModeForBattle.useSkill);                   
                }
            }
        }

        private void HideUI()
        {
            MenuManager.I.HideUI();
            PlayerManager.I.barController.HideUI();
            notSkillImage.enabled = false;
        }

        public void DisplayUI()
        {
            notSkillImage.enabled = true;
        }

        public void IncreaseSkillPoints(int amount)
        {
            currentSkillPoints = Mathf.Min(currentSkillPoints + amount, PlayerManager.I.basicStatus.GetNeedSkillPoint(PlayerManager.I.currentStatus.GetCharacterID()));
            UpdateSkillUI();
        }

        private void UpdateSkillUI()
        {
            float fillAmount = (float)currentSkillPoints / PlayerManager.I.basicStatus.GetNeedSkillPoint(PlayerManager.I.currentStatus.GetCharacterID());
            skillImpossibleImage.fillAmount = fillAmount;

            if (currentSkillPoints >= PlayerManager.I.basicStatus.GetNeedSkillPoint(PlayerManager.I.currentStatus.GetCharacterID()))
            {
                skillImpossibleImage.gameObject.SetActive(false);
                skillPossibleImage.gameObject.SetActive(true);
            }
            else
            {
                skillImpossibleImage.gameObject.SetActive(true);
                skillPossibleImage.gameObject.SetActive(false);

            }
        }

        private void ActivateSkillWithCutIn(int skillNum)
        {
            darkenScreen.SetActive(true);
            cutInAnimator.enabled = true;
            cutInAnimator.Rebind();
            cutInAnimator.Play(cutInAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0.0f);
            cutInSprite.enabled = true;

            skillNumToActivate = skillNum;

            currentSkillPoints = 0;
            skillPointTimer = 0f;

            Sound.SoundManager.I.PlayVoiceSE(Sound.SoundManager.VoideID.aka);
            UpdateSkillUI();
        }

        private void ActivateSkill(int skillNum)
        {
            GetSkillObject(skillNum).SetActive(true);
        }

        public GameObject GetSkillObject(int skillNum)
        {
            return skillObj[skillNum];
        }

        public void SkillReset()
        {
            darkenScreen.SetActive(false);

            Sound.SoundManager.I.ResumeSE();
        }

        public void AnimationEnd()
        {
            cutInAnimator.enabled = false;
            cutInSprite.enabled = false;

            ActivateSkill(skillNumToActivate);
        }

        public void AnimationEffectSound()
        {
            SoundManager.I.PlaySkillSE(SoundManager.skillAudioID.skillEnd);
        }


    }
}
