using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapManager : Singleton<MapManager>
    {
        [SerializeField] public MapController controller = new MapController();
        [Header("各ステージに対応したものを入れる")]
        [SerializeField] public TimeCountDown[] countDowns;
        [SerializeField] public float[] cellSize;
        [SerializeField] public Sprite[] stageTextures;

        private void Awake()
        {
            Init();
            controller.DoStart();
        }

        public override void BattleSceneInit()
        {
            var stageNum = GameManager.I.stageSelectHandller.GetStage();
            controller.InitBattle(stageTextures[stageNum], cellSize[stageNum]);
        }

        private void Update()
        {
            controller.DoUpdate();
        }
    }

    [System.Serializable]
    public class TimeCountDown
    {
        [SerializeField] private int _countdownMinutes;
        [SerializeField] private int _countdownSeconds;
        public int countdownMinutes { get { return _countdownMinutes; } }
        public int countdownSeconds { get { return _countdownSeconds; } }
    }
}