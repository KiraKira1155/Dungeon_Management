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
        private Transform itemParent;//�A�C�e�������p�̐e�I�u�W�F�N�g
        [SerializeField]
        private GameObject itemPrefab;//�h���b�v�A�C�e��obj
        [SerializeField]
        private int initialSize;//�ŏ��ɐ�������A�C�e���̐��v�[���̃T�C�Y

        private List<BaseItem> itemPool;//��A�N�e�B�u��Ԃ̃A�C�e���Ǘ�
        private List<BaseItem> getItems;

        [Header("ItemID�̏��Ԃɓo�^")]
        [SerializeField]
        private ItemSetting[] itemSettings;//�A�C�e�����

        [SerializeField]
        [Tooltip("0��100���A100��1���̊m���œG����A�C�e����������")]
        [Range(0, 100)]
        private int dropChance;

        private GameObject initItemPoolObj; // �A�C�e���̃v�[���I�u�W�F�N�g�����p�L���b�V�� 
        private BaseItem initItemdata; // �A�C�e���̃f�[�^�����ݒ�p�L���b�V�� 
        private int currentMapItemID;//���݂̃}�b�v��̃A�C�e���擾

        [Header("�A�C�e�����ʐݒ�")]
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
                //�A�C�e���ƃv���C���[�̋������v�Z
                disPos.x = itemPos.x - playerPos.x;  // x���W�̍�
                disPos.y = itemPos.y - playerPos.y;  // y���W�̍�

                distance = Mathf.Sqrt(disPos.x * disPos.x + disPos.y * disPos.y);  // �������v�Z
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
            //�w�肵���������A�C�e���𐶐����ăv�[���ɒǉ�
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
        /// �A�C�e���擾�����Ƃ��̎�ޕʂ̏���
        /// </summary>
        /// <param name="item">�擾�A�C�e��</param>
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
                    Debug.Log("�ݒ肳��Ă��Ȃ��A�C�e���ł�");
                    break;
            }

            //�A�C�e�����A�N�e�B�u�ɂ��ăv�[���ɖ߂�����
            item.GetItemGameObject().SetActive(false);
        }

        /// <summary>
        /// �A�C�e���̃h���b�v�p
        /// </summary>
        /// <param name="dropPos">�A�C�e���̗�����ꏊ</param>
        public void ItemDrop(Vector3 dropPos)
        {
            if(!Probability())
                return;

            var itemSetting = RandomDropItem(); // �d�ݕt�����l�������A�C�e��ID���擾
            //��A�N�e�B�u�̃A�C�e���������Ďw�肵���ʒu�ɃA�C�e�����A�N�e�B�u�ɂ���
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
            //�v�[�����ɗ��p�\�ȃA�C�e�����Ȃ��ꍇ�V�����A�C�e���𐶐�����
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
        /// �A�C�e���̐����Ɣz�u�p
        /// </summary>
        /// <param name="itemSetting">�A�C�e�����</param>
        /// <param name="dropPos">�A�C�e���h���b�v�ʒu</param>
        private void ItemInstantiate(ItemSetting itemSetting, Vector3 dropPos)
        {
            //�A�C�e��obj��e�I�u�W�F�N�g�ɐ������A�C�e���v�[���Ɋi�[
            initItemPoolObj = Instantiate(itemPrefab, itemParent);
            initItemdata = initItemPoolObj.GetComponent<BaseItem>();

            //�A�C�e���̐ݒ����������
            initItemdata.InitItemData(currentMapItemID, itemSetting);
            initItemdata.GetItemTransform().position = dropPos;
            //�v�[���ɃA�C�e����ǉ�
            itemPool.Add(initItemdata);
            getItems.Add(initItemdata);
            currentMapItemID++;

            dropItemSystem.AddItemList(initItemdata.GetSpriteRenderer());
        }

        /// <summary>
        /// �A�C�e���̒��I�m���v�Z
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
        private Sprite textures;//�A�C�e���̃e�N�X�`��

        [SerializeField]
        private float itemWeights; // �e�A�C�e���̏d�݂�ݒ肷��z��

        [SerializeField]
        private ItemManager.ItemID itemID;//�A�C�e���̎��

        /// <summary>
        /// �A�C�e���̃e�N�X�`���擾�p
        /// </summary>
        /// <returns></returns>
        public Sprite GetItemTexture()
        {
            return textures;
        }

        /// <summary>
        /// �A�C�e���̒��I�m���擾�p    
        /// </summary>
        /// <returns></returns>
        public float GetItemWeight()
        {
            return itemWeights;
        }

        /// <summary>
        /// �A�C�e����ID���擾
        /// </summary>
        /// <returns></returns>
        public ItemManager.ItemID GetItemID()
        {
            return itemID;
        }
    }
}


