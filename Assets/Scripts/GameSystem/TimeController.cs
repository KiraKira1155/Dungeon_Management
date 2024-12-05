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
        // 残り時間
        private static float countdownTime;
        // 経過時間
        private static float elapsedTime;

        public static bool canBattleTimeCount;

        /// <summary>
        /// タイマーの初期化用
        /// </summary>
        /// <param name="text">表示するテキスト</param>
        /// <param name="minutes">タイムリミット：分</param>
        /// <param name="seconds">タイムリミット：秒</param>
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
        /// カウントダウンの設定用、読んだ時点でタイマーが初期化される
        /// </summary>
        /// <param name="minutes">タイムリミット：分</param>
        /// <param name="seconds">タイムリミット：秒</param>
        public static void SetCountdown(int minutes,  int seconds)
        {
            countdownTime = minutes * 60 + seconds;
        }

        /// <summary>
        /// カウントダウン関数
        /// </summary>
        public static void CountdownTimer()
        {
            // 戦闘終了
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
        /// カウントダウン表示用
        /// </summary>
        private static void DisplayCountdownTimer()
        {
            var span = new System.TimeSpan(0, 0, (int)countdownTime);
            timeText.text = span.ToString(@"mm\:ss");
        }

        /// <summary>
        /// 残り時間の取得用
        /// </summary>
        /// <returns>残り時間を秒数で返す</returns>
        public static float GetTimeRemaining()
        {
            return countdownTime;
        }

        /// <summary>
        /// 経過時間の取得用
        /// </summary>
        /// <returns>戦闘開始からの経過時間を秒数で返す</returns>
        public static float GetTimeElpsed()
        {
            return elapsedTime;
        }
    }
}
