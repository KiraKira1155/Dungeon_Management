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
        /// �T�u����̊�{�X�e�[�^�X�̎擾
        /// </summary>
        public SubWeaponsBaseStatus baseStatus = new SubWeaponsBaseStatus();

        // =========== �T�u����̓o�^ ==========

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
        // ========== �T�u����̓o�^�I�� ==========

        private float gravityStrength = 1.2f;//�d�͂̋���

        private List<ParticleSystem> particleList = new List<ParticleSystem>();

        private (IBaseSubWeapon subWeapon, byte currentLv)[] holdingSubWeapon = new (IBaseSubWeapon subWeapon, byte currentLv)[MAX_HOLDING_NUM];

        private byte currentHoldingCount;

        private bool[] hitEnemy = new bool[MAX_HOLDING_NUM];
        private List<Enemy.IBaseEnemy>[] hitEnemyList = new List<Enemy.IBaseEnemy>[MAX_HOLDING_NUM];

        private GameSystem.SceneController.GameModeForBattle currentBattleScene;

        private Vector2 gravityPos;

        /// <summary>
        /// �T�u����̎���ID�Ƃ��Ďg�p
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
        /// �������̃T�u����̑S�f�[�^�擾�p
        /// </summary>
        /// <returns>�T�u����̃X�N���v�g�ƁA���݃��x��</returns>
        public (IBaseSubWeapon weaponData, byte currentLv)[] GetSubWeaponAllData()
        {
            return holdingSubWeapon;
        }

        /// <summary>
        /// ���ݑ������̃T�u����̃��x���擾�p
        /// </summary>
        /// <param name="weaponID">����ID</param>
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
        /// �U���̃G�t�F�N�g����
        /// </summary>
        /// <param name="particlePrefab">�p�[�e�B�N���R���|�[�l���g�������v���n�u</param>
        /// <param name="generatePos">�����ʒu</param>
        /// <returns>���������p�[�e�B�N���� GameObject�R���|�[�l���g</returns>
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
        /// �U���{�̂̃I�u�W�F�N�g����
        /// <para>
        /// �����ꍇ�́A���Ȏ�����낵�����肢���܂�
        /// </para>
        /// </summary>
        /// <param name="attackObjPrefab">�U���{�̂̃I�u�W�F�N�g�AImage�R���|�[�l���g�ŕ\���������</param>
        /// <param name="generatePos">�����ʒu</param>
        /// <returns>���������v���n�u�� GameObject�R���|�[�l���g</returns>
        public GameObject AttackObjGeneration(GameObject attackObjPrefab, Vector2 generatePos)
        {
            var obj = Instantiate(attackObjPrefab);
            var pos = new Vector3(generatePos.x, generatePos.y, 0.0f);
            obj.transform.position = pos;

            return obj;
        }

        /// <summary>
        /// ���ݑ������̃T�u����̎���ID�擾�p
        /// </summary>
        /// <param name="holdingNum">�����ԍ�</param>
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
        /// ���ݏ������Ă��镐��̐�
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
