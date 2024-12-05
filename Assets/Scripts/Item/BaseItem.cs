using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class BaseItem : MonoBehaviour
    {
        [SerializeField] private int mapItemID;
        private ItemManager.ItemID itemID;
        [SerializeField] private SpriteRenderer thisSpriteRenderer;
        [SerializeField] private GameObject thisObj;
        [SerializeField] private Transform thisTransform;

        public SpriteRenderer GetSpriteRenderer()
        {
            return thisSpriteRenderer;
        }
     
        public void InitItemData(int mapItemID, ItemSetting itemSetting)
        {
            this.mapItemID = mapItemID;
            itemID = itemSetting.GetItemID();
            thisSpriteRenderer.sprite = itemSetting.GetItemTexture();
        }

        public GameObject GetItemGameObject()
        {
            return thisObj;
        }

        public ItemManager.ItemID GetItemID()
        {
            return itemID;
        }

        public Transform GetItemTransform()
        {
            return thisTransform;
        }

        public int GetMapItemID()
        {
            return mapItemID;
        }
        
    }
}