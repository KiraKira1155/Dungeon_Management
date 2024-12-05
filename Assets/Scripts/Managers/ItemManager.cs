using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class ItemManager : Singleton<ItemManager>
    {
        [SerializeField] private float getItemDistance;

        [SerializeField] public DropItemSystem dropItemSystem = new DropItemSystem();

        [SerializeField]
        private Transform itemParent;//アイテム生成用の親オブジェクト
        [SerializeField]
        private GameObject itemPrefab;//ドロップアイテムobj
        [SerializeField]
        private int initialSize;//最初に生成するアイテムの数プールのサイズ

        private List<BaseItem> itemPool;//非アクティブ状態のアイテム管理
        private List<BaseItem> getItems;

        [Header("ItemIDの順番に登録")]
        [SerializeField]
        private ItemSetting[] itemSettings;//アイテム情報

        [SerializeField]
        [Tooltip("0で100％、100で1％の確立で敵からアイテムが落ちる")]
        [Range(0, 100)]
        private int dropChance;

        private GameObject initItemPoolObj; // アイテムのプールオブジェクト生成用キャッシュ 
        private BaseItem initItemdata; // アイテムのデータ初期設定用キャッシュ 
        private int currentMapItemID;//現在のマップ上のアイテム取得

        [Header("アイテム効果設定")]
        [SerializeField] private RecoveryPotion recoveryPotion = new RecoveryPotion();
        [SerializeField] private SmallSkillPotion smallSkillPotion = new SmallSkillPotion();
        [SerializeField] private NormalSkillPotion normalSkillPotion = new NormalSkillPotion();
        [SerializeField] private BigSkillPotion bigSkillPotion = new BigSkillPotion();
        [SerializeField] private UpgradeRerunCoin rerunCoin = new UpgradeRerunCoin();

        public enum ItemID
        {
            smallskillpoint,
            normalskillpoint,
            bigskillpoint,
            potion,
            coin,
            COUNT
        }

        private void Awake()
        {
            Init();
        }

        private void FixedUpdate()
        {
            dropItemSystem.DoFixedUpdate();
            GetItemPosCalculation();
        }

        private void GetItemPosCalculation()
        {
            var useList = new List<BaseItem>();
            var distance = 0.0f;
            var disPos = Vector2.zero;
            var playerPos = (Vector2)Player.PlayerManager.I.GetPlayerTransform().position;
            DebugDraw.DebugDrawLine.DrawCircle(playerPos, getItemDistance, 30, Color.black);
            foreach (var item in getItems)
            {
                if(!item.GetSpriteRenderer().enabled)
                    continue;

                var itemPos = (Vector2)item.GetItemTransform().position;
                //アイテムとプレイヤーの距離を計算
                disPos.x = itemPos.x - playerPos.x;  // x座標の差
                disPos.y = itemPos.y - playerPos.y;  // y座標の差

                distance = Mathf.Sqrt(disPos.x * disPos.x + disPos.y * disPos.y);  // 距離を計算
                if (distance <= getItemDistance)
                {
                    UseDropItem(item);
                    useList.Add(item);
                }
            }

            if(useList.Count != 0)
            {
                foreach (var useItem in useList)
                {
                    getItems.Remove(useItem);
                }

                useList.Clear();
            }
        }

        public override void BattleSceneInit()
        {
            StartCoroutine(InitPool());
        }

        private IEnumerator InitPool()
        {
            //指定した数だけアイテムを生成してプールに追加
            itemPool = new List<BaseItem>();
            getItems = new List<BaseItem>();

            for (int i = 0; i < initialSize; i++)
            {
                initItemPoolObj = Instantiate(itemPrefab, itemParent);
                itemPool.Add(initItemPoolObj.GetComponent<BaseItem>());

                initItemPoolObj.SetActive(false);
                if(i % 20 == 0)
                    yield return null;
            }

            yield break;
        }

        /// <summary>
        /// アイテム取得したときの種類別の処理
        /// </summary>
        /// <param name="item">取得アイテム</param>
        public void UseDropItem(BaseItem item)
        {

            switch (item.GetItemID())
            {

                case ItemManager.ItemID.smallskillpoint:
                    smallSkillPotion.UseAction();
                    break;
                case ItemManager.ItemID.normalskillpoint:
                    normalSkillPotion.UseAction();
                    break;
                case ItemManager.ItemID.bigskillpoint:
                    bigSkillPotion.UseAction();
                    break;
                case ItemManager.ItemID.potion:
                    recoveryPotion.UseAction();
                    break;
                case ItemManager.ItemID.coin:
                    rerunCoin.UseAction();
                    break;

                default:
                    Debug.Log("設定されていないアイテムです");
                    break;
            }

            //アイテムを非アクティブにしてプールに戻す処理
            item.GetItemGameObject().SetActive(false);
        }

        /// <summary>
        /// アイテムのドロップ用
        /// </summary>
        /// <param name="dropPos">アイテムの落ちる場所</param>
        public void ItemDrop(Vector3 dropPos)
        {
            if(!Probability())
                return;

            var itemSetting = RandomDropItem(); // 重み付けを考慮したアイテムIDを取得
            //非アクティブのアイテムを見つけて指定した位置にアイテムをアクティブにする
            foreach (var item in itemPool)
            {
                if (!item.GetItemGameObject().activeSelf)
                {
                    item.GetItemTransform().position = dropPos;
                    item.GetItemGameObject().SetActive(true);
                    item.InitItemData(currentMapItemID, itemSetting);

                    getItems.Add(item);
                    currentMapItemID++;
                    dropItemSystem.AddItemList(item.GetSpriteRenderer());

                    return;
                }
            }
            //プール内に利用可能なアイテムがない場合新しいアイテムを生成する
            ItemInstantiate(itemSetting, dropPos);
        }

        private bool Probability()
        {
            var rate = Random.value * 100.0f;
            if (dropChance == 100.0f && rate == dropChance)
            {
                return true;
            }
            else if (rate < dropChance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// アイテムの生成と配置用
        /// </summary>
        /// <param name="itemSetting">アイテム情報</param>
        /// <param name="dropPos">アイテムドロップ位置</param>
        private void ItemInstantiate(ItemSetting itemSetting, Vector3 dropPos)
        {
            //アイテムobjを親オブジェクトに生成しアイテムプールに格納
            initItemPoolObj = Instantiate(itemPrefab, itemParent);
            initItemdata = initItemPoolObj.GetComponent<BaseItem>();

            //アイテムの設定情報を初期化
            initItemdata.InitItemData(currentMapItemID, itemSetting);
            initItemdata.GetItemTransform().position = dropPos;
            //プールにアイテムを追加
            itemPool.Add(initItemdata);
            getItems.Add(initItemdata);
            currentMapItemID++;

            dropItemSystem.AddItemList(initItemdata.GetSpriteRenderer());
        }

        /// <summary>
        /// アイテムの抽選確立計算
        /// </summary>
        /// <returns></returns>
        public ItemSetting RandomDropItem()
        {
            float total = 0;
            foreach (var item in itemSettings)
            {
                total += item.GetItemWeight();
            }

            float value = Random.Range(0, total);
            foreach (var item in itemSettings)
            {
                value -= item.GetItemWeight();
                if (value <= 0)
                    return item;
            }

            return itemSettings[0];
        }
    }

    [System.Serializable]
    public class ItemSetting
    {
        [SerializeField]
        private Sprite textures;//アイテムのテクスチャ

        [SerializeField]
        private float itemWeights; // 各アイテムの重みを設定する配列

        [SerializeField]
        private ItemManager.ItemID itemID;//アイテムの種類

        /// <summary>
        /// アイテムのテクスチャ取得用
        /// </summary>
        /// <returns></returns>
        public Sprite GetItemTexture()
        {
            return textures;
        }

        /// <summary>
        /// アイテムの抽選確立取得用    
        /// </summary>
        /// <returns></returns>
        public float GetItemWeight()
        {
            return itemWeights;
        }

        /// <summary>
        /// アイテムのIDを取得
        /// </summary>
        /// <returns></returns>
        public ItemManager.ItemID GetItemID()
        {
            return itemID;
        }
    }
}


