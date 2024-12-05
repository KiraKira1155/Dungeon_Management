using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameSystem
{
    [System.Serializable]
    public static class TimeController
    {
        private static TextMeshProUGUI timeText;
        // �c�莞��
        private static float countdownTime;
        // �o�ߎ���
        private static float elapsedTime;

        public static bool canBattleTimeCount;

        /// <summary>
        /// �^�C�}�[�̏������p
        /// </summary>
        /// <param name="text">�\������e�L�X�g</param>
        /// <param name="minutes">�^�C�����~�b�g�F��</param>
        /// <param name="seconds">�^�C�����~�b�g�F�b</param>
        public static void InitTimer(TextMeshProUGUI text, int minutes, int seconds)
        {
            elapsedTime = 0;
            timeText = text;
            SetCountdown(minutes, seconds);
            DisplayCountdownTimer();
            canBattleTimeCount = true;
        }

        public static void DoUpdate()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != SceneController.GameModeForBattle.battle)
                return;

            CountdownTimer();
        }

        public static void DoFixedUpdate()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != SceneController.GameModeForBattle.battle)
                return;

            DisplayCountdownTimer();
        }

        /// <summary>
        /// �J�E���g�_�E���̐ݒ�p�A�ǂ񂾎��_�Ń^�C�}�[�������������
        /// </summary>
        /// <param name="minutes">�^�C�����~�b�g�F��</param>
        /// <param name="seconds">�^�C�����~�b�g�F�b</param>
        public static void SetCountdown(int minutes,  int seconds)
        {
            countdownTime = minutes * 60 + seconds;
        }

        /// <summary>
        /// �J�E���g�_�E���֐�
        /// </summary>
        public static void CountdownTimer()
        {
            // �퓬�I��
            if (countdownTime <= 0)
            {
                GameManager.I.sceneController.SetGameModeForBattleScene(SceneController.GameModeForBattle.death);
                GameManager.I.ChengeNewScene(SceneController.GameScene.gameOver, true);
                return;
            }

            countdownTime -= Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }

        /// <summary>
        /// �J�E���g�_�E���\���p
        /// </summary>
        private static void DisplayCountdownTimer()
        {
            var span = new System.TimeSpan(0, 0, (int)countdownTime);
            timeText.text = span.ToString(@"mm\:ss");
        }

        /// <summary>
        /// �c�莞�Ԃ̎擾�p
        /// </summary>
        /// <returns>�c�莞�Ԃ�b���ŕԂ�</returns>
        public static float GetTimeRemaining()
        {
            return countdownTime;
        }

        /// <summary>
        /// �o�ߎ��Ԃ̎擾�p
        /// </summary>
        /// <returns>�퓬�J�n����̌o�ߎ��Ԃ�b���ŕԂ�</returns>
        public static float GetTimeElpsed()
        {
            return elapsedTime;
        }
    }
}
