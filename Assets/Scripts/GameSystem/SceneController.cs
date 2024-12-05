using System.Collections;
using System.Collections.Generic;

namespace GameSystem
{
    public class SceneController
    {
        private GameScene gameScene = GameScene.init;
        private GameModeForBattle gameModeForBattle;

        private GameScene previusScene;

        public bool loadEnd {  get; private set; }
        public bool battleSceneLoadEnd {  get; private set; }

        public SceneController()
        {
            gameScene = GameScene.init;
            previusScene = GameScene.init;
        }

        // ゲームのシーン管理用
        public enum GameScene
        {
            init,
            newGame,
            title,
            stageSelect,
            battle,
            gameOver,
            gameClear
        }

        //バトル中のゲーム進行管理用
        public enum GameModeForBattle
        {
            none,
            battle,
            pause,
            select,
            useSkill,
            death
        }

        /// <summary>
        /// 現在のゲームシーンの取得用
        /// </summary>
        /// <returns>ゲームシーン用のenum型</returns>
        public GameScene GetGameScene()
        {
            return gameScene;
        }

        public string GetSceneName(GameScene scene)
        {
            switch (scene)
            {
                case GameScene.init:
                    return "InitializationScene";

                case GameScene.title:
                    return "TitleScene";

                case GameScene.newGame:
                    return "NewGameScene";

                case GameScene.stageSelect:
                    return "StageSlectScene";

                case GameScene.battle:
                    return "BatltleScene";

                case GameScene.gameClear:
                    return "ResultScene";

                case GameScene.gameOver:
                    return "ResultScene";

                default:
                    return null;
            }
        }

        /// <summary>
        /// ゲームモードがbattleのさい、ゲームモードが現在何なのかの取得用
        /// </summary>
        /// <returns>ゲームシーンがbattle出ない場合はnoneが、battleの場合は現在の戦闘状況(スキル発動中やレベルアップ選択中など)のenum型</returns>
        public GameModeForBattle GetGameModeForBattleScene()
        {
            if (gameScene != GameScene.battle)
                return GameModeForBattle.none;
            return gameModeForBattle;
        }

        public void SetGameModeForBattleScene(GameModeForBattle mode)
        {
            gameModeForBattle = mode;
        }

        public string GetPreviousScene()
        {
            return GetSceneName(previusScene);
        }

        public void SetPreviousScene()
        {
            loadEnd = false;
            previusScene = gameScene;
        }

        public void SetGameSceneEnd()
        {
            loadEnd = true;
        }

        public void SetGameScene(GameScene scene)
        {
            gameScene = scene;
        }

        public void EndBattleSceneLoad()
        {
            battleSceneLoadEnd = true;
        }

        public void BattleSceneEnd()
        {
            battleSceneLoadEnd = false;
        }
    }
}
