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

        // �Q�[���̃V�[���Ǘ��p
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

        //�o�g�����̃Q�[���i�s�Ǘ��p
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
        /// ���݂̃Q�[���V�[���̎擾�p
        /// </summary>
        /// <returns>�Q�[���V�[���p��enum�^</returns>
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
        /// �Q�[�����[�h��battle�̂����A�Q�[�����[�h�����݉��Ȃ̂��̎擾�p
        /// </summary>
        /// <returns>�Q�[���V�[����battle�o�Ȃ��ꍇ��none���Abattle�̏ꍇ�͌��݂̐퓬��(�X�L���������⃌�x���A�b�v�I�𒆂Ȃ�)��enum�^</returns>
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
