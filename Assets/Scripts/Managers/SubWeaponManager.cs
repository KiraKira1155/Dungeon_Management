using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace SubWeapon
{
    [System.Serializable]
    public class SubWeaponManager : Singleton<SubWeaponManager>
    {
        public const byte MAX_HOLDING_NUM = 3;
        public const byte MAX_SUB_WEAPON_LEVEL = 3;
        /// <summary>
        /// サブ武器の基本ステータスの取得
        /// </summary>
        public SubWeaponsBaseStatus baseStatus = new SubWeaponsBaseStatus();

        // =========== サブ武器の登録 ==========

        private static LightningStrike lightningStrike;
        private static AttackField attackField;
        private static FireAttack fireAttack;
        private static Missile missile;
        private static BallAttack ball;
        private static Boomerang boomerang;
        private static Funnel funnel;
        private static LandMine landMine;
        private static Shield shield;
        private static GravitationalField gravitationalField;

        private IBaseSubWeapon[] subWeaponList;
        // ========== サブ武器の登録終了 ==========

        private float gravityStrength = 1.2f;//重力の強さ

        private List<ParticleSystem> particleList = new List<ParticleSystem>();

        private (IBaseSubWeapon subWeapon, byte currentLv)[] holdingSubWeapon = new (IBaseSubWeapon subWeapon, byte currentLv)[MAX_HOLDING_NUM];

        private byte currentHoldingCount;

        private bool[] hitEnemy = new bool[MAX_HOLDING_NUM];
        private List<Enemy.IBaseEnemy>[] hitEnemyList = new List<Enemy.IBaseEnemy>[MAX_HOLDING_NUM];

        private GameSystem.SceneController.GameModeForBattle currentBattleScene;

        private Vector2 gravityPos;

        /// <summary>
        /// サブ武器の識別IDとして使用
        /// </summary>
        public enum SubWeaponID
        {
            NONE = 0,

            attackField,
            lightningStrike,
            fireAttack,
            missile,
            ball,
            boomerang,
            funnel,
            landmine,
            shield,
            gravitationalField,

            MAX_ID_COUNT
        }

        public override void BattleSceneClear()
        {
            particleList.RemoveAll(item => item == null);

            holdingSubWeapon = new (IBaseSubWeapon subWeapon, byte currentLv)[MAX_HOLDING_NUM];
            currentHoldingCount = 0;
            hitEnemy = new bool[MAX_HOLDING_NUM];
            hitEnemyList = new List<Enemy.IBaseEnemy>[MAX_HOLDING_NUM];
        }

        private void Awake()
        {
            Init();

            lightningStrike = new LightningStrike();
            attackField = new AttackField();
            fireAttack = new FireAttack();
            missile = new Missile();
            ball = new BallAttack();
            boomerang = new Boomerang();
            funnel = new Funnel();
            landMine = new LandMine();
            shield = new Shield();
            gravitationalField = new GravitationalField();

            subWeaponList = new IBaseSubWeapon[]
            {
                attackField,
                lightningStrike,
                fireAttack,
                missile,
                ball,
                boomerang,
                funnel,
                landMine,
                shield,
                gravitationalField
            };
        }

        public void SetSubWeapon(SubWeaponID weaponID)
        {
            for(int i = 0; i < holdingSubWeapon.Length; i++)
            {
                if (holdingSubWeapon[i].subWeapon != null)
                {
                    if (holdingSubWeapon[i].subWeapon.GetWeaponID() == weaponID)
                    {
                        holdingSubWeapon[i] = (holdingSubWeapon[i].subWeapon, (byte)(holdingSubWeapon[i].currentLv + 1));

                        if(holdingSubWeapon[i].subWeapon.GetWeaponID() == SubWeaponID.gravitationalField && holdingSubWeapon[i].currentLv == MAX_SUB_WEAPON_LEVEL)
                        {
                            gravityStrength *= 3;
                        }
                        return;
                    }
                }
            }

            foreach(var weapon in subWeaponList)
            {
                if(weapon.GetWeaponID() == weaponID)
                {
                    holdingSubWeapon[currentHoldingCount].subWeapon = weapon;
                    break;
                }
            }

            holdingSubWeapon[currentHoldingCount].currentLv = 0;

            currentHoldingCount++;
        }

        private void Update()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
            {
                if (currentBattleScene != GameSystem.SceneController.GameModeForBattle.battle)
                {
                    particleList.RemoveAll(item => item == null);
                    foreach (var particle in particleList)
                    {
                        particle.Play();
                    }
                }

                    
                for (int i = 0; i < currentHoldingCount; i++)
                {

                    hitEnemy[i] = false;
                    holdingSubWeapon[i].subWeapon.DoUpdate();

                    if (holdingSubWeapon[i].subWeapon.GetIsHitEnemy())
                    {
                        hitEnemyList[i] = new List<Enemy.IBaseEnemy>();
                        hitEnemy[i] = true;
                        hitEnemyList[i].AddRange(holdingSubWeapon[i].subWeapon.GetHitEnemyList());
                    }
                }

                currentBattleScene = GameSystem.SceneController.GameModeForBattle.battle;
            }
            else
            {
                if (currentBattleScene != GameSystem.SceneController.GameModeForBattle.pause)
                {
                    particleList.RemoveAll(item => item == null);
                    if (particleList.Count != 0)
                    {
                        foreach (var particle in particleList)
                        {
                            particle.Pause();
                        }
                    }
                }


                currentBattleScene = GameSystem.SceneController.GameModeForBattle.pause;
            }
        }

        public bool[] GetHitEnemy()
        {
            return hitEnemy;
        }

        public (List<Enemy.IBaseEnemy> enemyList, int damage) GetHitEnemyList(int holdingNum)
        {
            hitEnemy[holdingNum] = false;
            return (hitEnemyList[holdingNum], baseStatus.GetAttackPower(holdingSubWeapon[holdingNum].subWeapon.GetWeaponID(), holdingSubWeapon[holdingNum].currentLv));
        }

        /// <summary>
        /// 装備中のサブ武器の全データ取得用
        /// </summary>
        /// <returns>サブ武器のスクリプトと、現在レベル</returns>
        public (IBaseSubWeapon weaponData, byte currentLv)[] GetSubWeaponAllData()
        {
            return holdingSubWeapon;
        }

        /// <summary>
        /// 現在装備中のサブ武器のレベル取得用
        /// </summary>
        /// <param name="weaponID">識別ID</param>
        /// <returns></returns>
        public byte GetSubWeaponCurrentLv(SubWeaponID weaponID)
        {
            if (currentHoldingCount != 0)
            {
                foreach (var weapon in holdingSubWeapon)
                {
                    if(weapon.subWeapon != null)
                    {
                        if (weapon.subWeapon.GetWeaponID() == weaponID)
                            return weapon.currentLv;
                    }
                }

                return 255;
            }
            else
            {
                return 255;
            }
        }

        /// <summary>
        /// 攻撃のエフェクト生成
        /// </summary>
        /// <param name="particlePrefab">パーティクルコンポーネントがついたプレハブ</param>
        /// <param name="generatePos">召喚位置</param>
        /// <returns>召喚したパーティクルの GameObjectコンポーネント</returns>
        public GameObject ParticleGeneration(GameObject particlePrefab, Vector2 generatePos)
        {
            particleList.RemoveAll(item => item == null);

            var obj = Instantiate(particlePrefab);
            var pos =  new Vector3(generatePos.x, generatePos.y, 0.0f);
            obj.transform.position = pos;

            particleList.Add(obj.GetComponent<ParticleSystem>());

            return obj;
        }

        /// <summary>
        /// 攻撃本体のオブジェクト生成
        /// <para>
        /// 消す場合は、自己実装よろしくお願いします
        /// </para>
        /// </summary>
        /// <param name="attackObjPrefab">攻撃本体のオブジェクト、Imageコンポーネントで表示するもの</param>
        /// <param name="generatePos">召喚位置</param>
        /// <returns>召喚したプレハブの GameObjectコンポーネント</returns>
        public GameObject AttackObjGeneration(GameObject attackObjPrefab, Vector2 generatePos)
        {
            var obj = Instantiate(attackObjPrefab);
            var pos = new Vector3(generatePos.x, generatePos.y, 0.0f);
            obj.transform.position = pos;

            return obj;
        }

        /// <summary>
        /// 現在装備中のサブ武器の識別ID取得用
        /// </summary>
        /// <param name="holdingNum">装備番号</param>
        /// <returns></returns>
        public SubWeaponID GetHoldingSubWeaponID(byte holdingNum)
        {
            if(holdingSubWeapon[holdingNum].subWeapon == null)
                return SubWeaponID.NONE;

            return holdingSubWeapon[holdingNum].subWeapon.GetWeaponID();
        }


        public void DestroyObj(GameObject destroyObj)
        {
            Destroy(destroyObj);
        }

        /// <summary>
        /// 現在所持している武器の数
        /// </summary>
        /// <returns></returns>
        public byte GetHoldingAmount()
        {
            return currentHoldingCount;
        }

        public void SetGravityPos(Vector2 setPos)
        {
            gravityPos = setPos;
        }

        public Vector2 GetGravityPos()
        {
            return gravityPos;
        }

        public float GetGravityStrength()
        {
            return gravityStrength;
        }
    }
}
