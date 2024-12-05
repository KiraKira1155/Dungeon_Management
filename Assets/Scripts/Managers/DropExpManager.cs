using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class DropExpManager : Singleton<DropExpManager>
    {
        [SerializeField] private Transform dropExpParent;
        [SerializeField] private GameObject dropExpPrefab;

        [SerializeField] private int initialPoolSize;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float getDistance;
        private List<GameObject> dropExpPool = new List<GameObject>();
        private List<GameObject> mapExp = new List<GameObject>();
        private List<GameObject> destroyExp = new List<GameObject>();

        private const int EXP_AMOUNT = 1;

        // ƒLƒƒƒbƒVƒ…
        private GameObject initPoolObj;
        private Vector3 directionVector;
        private float distance;
        private int playerGetExp;
        private bool lvUp;

        public override void BattleSceneInit()
        {
            StartCoroutine(InitPool());
        }

        public override void BattleSceneClear()
        {
            dropExpPool.Clear();
            mapExp.Clear();
            destroyExp.Clear();
        }


        private void Awake()
        {
            Init();
        }

        private void FixedUpdate()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
                return;

            if (mapExp.Count == 0)
                return;

            foreach (var exp in mapExp)
            {
                directionVector = (Player.PlayerManager.I.GetPlayerTransform().position - exp.transform.position).normalized;
                exp.transform.position += directionVector * moveSpeed * 0.02f;

                distance = Vector3.Distance(Player.PlayerManager.I.GetPlayerTransform().position, exp.transform.position);
                if (distance < getDistance)
                {
                    lvUp = true;
                    playerGetExp += EXP_AMOUNT;
                    destroyExp.Add(exp);
                    exp.SetActive(false);
                }
            }

            if (lvUp)
            {
                Player.PlayerManager.I.currentStatus.ExpGain(playerGetExp);
                lvUp = false;
                playerGetExp = 0;
            }

            if (destroyExp.Count == 0)
                return;

            foreach(var exp in destroyExp)
            {
                mapExp.Remove(exp);
            }

            destroyExp.Clear();
        }

        private IEnumerator InitPool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                initPoolObj = Instantiate(dropExpPrefab, dropExpParent);
                dropExpPool.Add(initPoolObj);
                initPoolObj.SetActive(false);

                if (i % 100 == 0)
                    yield return null;
            }
        }

        public void ExpDrop(Vector3 dropPos)
        {
            foreach (var exp in dropExpPool)
            {
                if (!exp.activeSelf)
                {
                    exp.SetActive(true);
                    exp.transform.position = dropPos;
                    mapExp.Add(exp);

                    return;
                }
            }

            mapExp.Add(DropExpInstantiate(dropPos));
        }

        private GameObject DropExpInstantiate(Vector3 dropPos)
        {
            initPoolObj = Instantiate(dropExpPrefab, dropExpParent);
            initPoolObj.transform.position = dropPos;

            dropExpPool.Add(initPoolObj);
            return initPoolObj;
        }
    }
}
