using GameSystem;
using Player;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace Enemy
{
    public class EnemySpawnManager : Singleton<EnemySpawnManager>
    {
        [SerializeField] private Transform enemyParentTransform;
        [Header("ステージごとの敵出現設定")]
        [SerializeField] private EnemyStageSetting[] enemyStageSettings;
        [Header("ボス設定")]
        [SerializeField] private GameObject wallObj;

        private int enemyKinds;
        private IBaseEnemy[][] enemyPool;  // 敵のオブジェクトプール
        private GameObject initEnemyPoolObj; // 敵のプールオブジェクト生成用キャッシュ

        private List<IBaseEnemy>[] mapEnemisList;
        private List<IBaseEnemy> allMapEnemyList = new List<IBaseEnemy>();

        private bool isMidBossSpawned = false;
        private bool isBossSpawned = false;

        private int currentBattleID = 0;

        // 敵召喚計算用変数
        private Vector2 spawnRadius;
        private Vector2 absRadius;
        private float addAbs;
        private bool isSpawn;

        private int stageNum;

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            BattleSceneInit();
        }

        public override void BattleSceneInit()
        {
            GameManager.I.BattleSceneInit();

            currentBattleID = 0;
            isMidBossSpawned = false;
            isBossSpawned = false;


            enemyPool = null;
            mapEnemisList = null;
            allMapEnemyList.Clear();

            stageNum = GameManager.I.stageSelectHandller.GetStage();

            foreach (var oder in enemyStageSettings[stageNum].spawnOder)
            {
                oder.InitSpawnOder();
            }
            StartCoroutine(InitPool());

        }

        private IEnumerator InitPool()
        {
            EquipmentSelectManager.I.StartSelectEquipmentEvent();
            yield return null;

            allMapEnemyList = new List<IBaseEnemy>();

            enemyKinds = enemyStageSettings[stageNum].enemyPoolSettings.Length;
            // プールの初期化：敵の種類に合わせて行う
            enemyPool = new IBaseEnemy[enemyKinds][];
            mapEnemisList = new List<IBaseEnemy>[enemyKinds];

            // プールの初期化：敵のプールサイズに合わせて行う
            for (int i = 0; i < enemyKinds; i++)
            {
                enemyPool[i] = new IBaseEnemy[enemyStageSettings[stageNum].enemyPoolSettings[i].poolSize];
                yield return null;
            }

            // プール用オブジェクト生成
            for (int i = 0; i < enemyKinds; i++)
            {
                var task = StartCoroutine(InitEnemyPool(enemyStageSettings[stageNum].enemyPoolSettings[i], enemyPool[i]));
                yield return task;
            }
            yield return null;

            if (!GameManager.I.test)
            {
                StartCoroutine(GameManager.I.UnLoadPreviousScene(true));
            }

            GameManager.I.sceneController.EndBattleSceneLoad();
            yield break;
        }

        private void Update()
        {
            if (!GameManager.I.sceneController.battleSceneLoadEnd)
                return;

            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
            {
                if (!isBossSpawned)
                {
                    SpawnEnemies();
                }
            }
        }

        /// <summary>
        /// 敵をマップ上に召喚させるよう
        /// </summary>
        private void SpawnEnemies()
        {
            var time = Time.deltaTime;
            foreach (var oder in enemyStageSettings[stageNum].spawnOder)
            {
                var id = (int)oder.enemyID;
                switch (id)
                {
                    case (int)EnemyManager.EnemyCharactersID.boss:
                        if (oder.GetSpawnOder(time))
                        {
                            isBossSpawned = true;
                            SpawnBoss();
                        }
                        break;

                    case (int)EnemyManager.EnemyCharactersID.midBoss:
                        if (GetMidBossEnemy() != null)
                            break;

                        if (oder.GetSpawnOder(time))
                        {

                            isMidBossSpawned = true;
                            SpawnMidBoss(enemyPool[id], oder);
                        }
                        break;

                    case (int)EnemyManager.EnemyCharactersID.midBoss_1:
                        if (GetMidBossEnemy() != null)
                            break;

                        if (oder.GetSpawnOder(time))
                        {

                            isMidBossSpawned = true;
                            SpawnMidBoss(enemyPool[id], oder);
                        }
                        break;

                    default:
                        if (oder.GetSpawnOder(time))
                        {
                            SpawnEnemyPoolSystem(enemyPool[id], oder);
                        }
                        break;
                }
            }


            UpdateMapEnemyData();
        }

        private void SpawnMidBoss(IBaseEnemy[] enemyPool, EnemySpawnOder oder)
        {
            enemyPool[0].InitSetting(currentBattleID, GetSpawnPosition(oder.GetSpawnRadius(), oder.GetMaxSpawnRange()));
            currentBattleID++;

            if (currentBattleID == enemyStageSettings[stageNum].enemyMaxLength)
                currentBattleID = 0;

            EnemyManager.I.midBossController.Init();
        }

        private void SpawnBoss()
        {
            EnemyManager.I.bossController.Init();

            var kind = (int)EnemyManager.EnemyCharactersID.boss;
            enemyPool[kind][0].InitSetting(currentBattleID, GetBossSpawnPosition());

            currentBattleID++;

            if (currentBattleID == enemyStageSettings[stageNum].enemyMaxLength)
                currentBattleID = 0;



            Vector3 bossPosition = GetBossEnemy().GetEnemyTransform().position;

            PlayerManager.I.SetMoveLimits(bossPosition);
            EnemyManager.I.bossController.SetMoveLimits(bossPosition);

            var wall = Instantiate(wallObj);
            wall.transform.position = bossPosition;


            EnemyManager.I.bossController.bossBarController.ShowHPBar();


            foreach (var enemy in GetAllMapEnemy())
            {
                enemy.NotDrop();
                enemy.Death();
            }

        }

        /// <summary>
        /// 敵プールの初期化
        /// </summary>
        /// <param name="poolSetting"></param>
        /// <param name="enemyPool"></param>
        /// <returns></returns>
        private IEnumerator InitEnemyPool(EnemyPoolSetting poolSetting, IBaseEnemy[] enemyPool)
        {
            for (int i = 0; i < poolSetting.poolSize; i++)
            {
                initEnemyPoolObj = Instantiate(poolSetting.enemyPrefab, enemyParentTransform);
                enemyPool[i] = initEnemyPoolObj.transform.GetChild(0).GetComponent<IBaseEnemy>();
                initEnemyPoolObj.SetActive(false);

                if (i % 20 == 0)
                    yield return null;
            }

            yield break;
        }

        /// <summary>
        /// マップ上の敵管理用
        /// </summary>
        private void UpdateMapEnemyData()
        {
            allMapEnemyList.Clear();
            for (int i = 0; i < enemyKinds; i++)
            {
                mapEnemisList[i] = new List<IBaseEnemy>();

                if (i != (int)EnemyManager.EnemyCharactersID.boss && i != (int)EnemyManager.EnemyCharactersID.midBoss && i != (int)EnemyManager.EnemyCharactersID.midBoss_1)
                {
                    foreach (var enemy in enemyPool[i])
                    {
                        if (enemy.GetEnemyGameObject().activeInHierarchy)
                        {
                            mapEnemisList[i].Add(enemy);
                        }
                    }
                }
            }

            foreach (var mapEnemis in mapEnemisList)
            {
                if (mapEnemis?.Count > 0)
                    allMapEnemyList.AddRange(mapEnemis);
            }
        }

        /// <summary>
        /// 敵プールから敵をマップ上に召喚するよう
        /// </summary>
        /// <param name="enemyPool">敵プールオブジェクト</param>
        /// <param name="oder">スポーン対象</param>
        private void SpawnEnemyPoolSystem(IBaseEnemy[] enemyPool, EnemySpawnOder oder)
        {
            var cnt = 0;
            if (oder.GetSpawnAmount() <= 0)
                return;

            foreach (var enemy in enemyPool)
            {
                if (!enemy.GetEnemyGameObject().activeInHierarchy)
                {
                    if (currentBattleID == enemyStageSettings[stageNum].enemyMaxLength)
                        currentBattleID = 0;

                    enemy.InitSetting(currentBattleID, GetSpawnPosition(oder.GetSpawnRadius(), oder.GetMaxSpawnRange()));

                    currentBattleID++;
                    cnt++;

                    if (oder.GetSpawnAmount() == cnt)
                        return;
                }
            }
        }

        /// <summary>
        /// 敵マップ上のスポーン位置計算用
        /// </summary>
        /// <returns></returns>
        /// <param name="spawnRadius"></param>
        /// <param name="maxSpawnRange"></param>
        /// <returns></returns>
        private Vector3 GetSpawnPosition((float min, float max) radius, float maxSpawnRange)
        {
            isSpawn = false;
            while (!isSpawn)
            {
                spawnRadius.x = Random.Range(-maxSpawnRange, maxSpawnRange);
                spawnRadius.y = Random.Range(-maxSpawnRange, maxSpawnRange);

                absRadius = spawnRadius * spawnRadius;
                addAbs = absRadius.x + absRadius.y;
                if (radius.max > addAbs && addAbs > radius.min)
                {
                    isSpawn = true;
                }
            }

            return spawnRadius + (Vector2)PlayerManager.I.GetPlayerTransform().position;
        }

        private Vector3 GetBossSpawnPosition()
        {
            Vector3 playerPosition = Player.PlayerManager.I.GetPlayerObj().transform.position;

            return playerPosition + new Vector3(0f, 5f, 0f);
        }

        /// <summary>
        /// マップ上にいる敵の検索用、中ボス・ボスは別関数で取得
        /// </summary>
        /// <param name="gameObject">検索したい敵のオブジェクト</param>
        /// <returns></returns>
        public IBaseEnemy GetMapEnemy(GameObject gameObject)
        {
            foreach (var enemy in allMapEnemyList)
            {
                if (enemy.GetEnemyGameObject() == gameObject)
                    return enemy;
            }

            return null;
        }

        /// <summary>
        /// マップ上にいる全敵オブジェクト確認用、中ボス・ボスは別関数で取得
        /// </summary>
        /// <returns>マップ上にいる敵スクリプトの配列を返す</returns>
        public IBaseEnemy[] GetAllMapEnemy()
        {
            return allMapEnemyList.ToArray();
        }

        /// <summary>
        /// 中ボスの取得用、マップ上にいない場合は null が返る
        /// </summary>
        /// <returns></returns>
        public IBaseEnemy GetMidBossEnemy()
        {
            if (!IsMidBossSpawned())
                return null;

            if (enemyPool[(int)EnemyManager.EnemyCharactersID.midBoss][0].GetEnemyGameObject().activeInHierarchy)
            {
                return enemyPool[(int)EnemyManager.EnemyCharactersID.midBoss][0];
            }
            else if (enemyPool[(int)EnemyManager.EnemyCharactersID.midBoss_1][0].GetEnemyGameObject().activeInHierarchy)
            {
                return enemyPool[(int)EnemyManager.EnemyCharactersID.midBoss_1][0];
            }

            return null;
        }

        /// <summary>
        /// 中ボスが出現しているかの確認用
        /// </summary>
        /// <returns></returns>
        public bool IsMidBossSpawned()
        {
            return isMidBossSpawned;
        }

        public IBaseEnemy GetBossEnemy()
        {
            if (!IsBossSpawned() || enemyPool[(int)EnemyManager.EnemyCharactersID.boss][0] == null)
                return null;
            return enemyPool[(int)EnemyManager.EnemyCharactersID.boss][0];
        }

        public bool IsBossSpawned()
        {
            return isBossSpawned;
        }
    }

    [System.Serializable]
    public class EnemyStageSetting
    {
        [SerializeField] private string stageNasme;
        [SerializeField] private int _enemyMaxLength;
        [Header("基本出現設定")]
        [SerializeField] private EnemySpawnOder[] _spawnOder;

        [Header("敵各種のプールデータ設定")]
        [SerializeField] private EnemyPoolSetting[] _enemyPoolSettings;

        public int enemyMaxLength { get { return _enemyMaxLength; } }
        public EnemySpawnOder[] spawnOder {  get { return _spawnOder; } }
        public EnemyPoolSetting[] enemyPoolSettings {  get { return _enemyPoolSettings; } }
    }

    [System.Serializable]
    public class EnemyPoolSetting
    {
        [SerializeField] private GameObject _enemyPrefab;  // 敵のプレハブ
        [SerializeField] private int _poolSize;  // マップ上の最大敵数

        public GameObject enemyPrefab { get { return _enemyPrefab; } } // 敵のプレハブ
        public int poolSize { get { return _poolSize; } }
    }

    [System.Serializable]
    public class EnemySpawnOder
    {
        [Header("出現する敵の範囲設定")]
        [SerializeField] private float MinSpawnRange;
        [SerializeField] private float MaxSpawnRange;

        [Header("出現する敵の種類")]
        [SerializeField] private EnemyManager.EnemyCharactersID _enemyID;

        [Header("出現のオーダーアセット")]
        [SerializeField] private EnemySpawnOderAttributes oder;
        public EnemyManager.EnemyCharactersID enemyID { get { return _enemyID; } }

        public bool canSpawn { get; private set; }
        public int oderAmount { get; private set; }
        public int currentOderNum { get; private set; }

        private (float min, float max) spawnRadius;

        private float currentTime;
        private float elpsedTime;

        public void InitSpawnOder()
        {
            canSpawn = true;
            oderAmount = oder.spawnTime.Length < oder.spawnAmount.Length ? oder.spawnTime.Length : oder.spawnAmount.Length;
            elpsedTime += oder.spawnTime[0];
            currentOderNum = 0;
            spawnRadius.min = MinSpawnRange * MinSpawnRange;
            spawnRadius.max = MaxSpawnRange * MaxSpawnRange;
        }

        public float GetMaxSpawnRange()
        {
            return MaxSpawnRange;
        }

        public (float min, float max) GetSpawnRadius()
        {
            return spawnRadius;
        }

        public bool GetCanSpawn()
        {
            return canSpawn;
        }

        /// <summary>
        /// スポーン管理用
        /// </summary>
        /// <param name="time">現在の戦闘経過時間</param>
        /// <returns>スポーンする量</returns>
        public bool GetSpawnOder(float time)
        {
            if (!canSpawn)
                return false;

            currentTime += time;
            if (currentTime >= elpsedTime)
            {
                currentOderNum++;
                if (oderAmount == currentOderNum)
                {
                    canSpawn = false;
                }
                else
                {
                    elpsedTime = oder.spawnTime[currentOderNum];
                }
                return true;
            }

            return false;
        }

        public int GetSpawnAmount()
        {
            return oder.spawnAmount[currentOderNum - 1];
        }
    }
}

