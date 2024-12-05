using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class DropItemSystem
    {
        [SerializeField]
        [Header("アイテム表示距離")]
        private float activeDis = 13;
        private List<SpriteRenderer> items = new List<SpriteRenderer>();
       
        public void DoFixedUpdate()
        {
            for (int i = 0; i < items.Count; i++)
            {
                // アイテムとプレイヤーの距離を計算
                float distance = ItemDis(Player.PlayerManager.I.GetPlayerTransform().position, items[i].transform.position);

                // プレイヤーの範囲内にいる場合、アイテムをアクティブ化
                if (distance <= activeDis)
                {
                    // アクティブ化
                    items[i].enabled = true;
                }
                else
                {
                    //非アクティブ化
                    items[i].enabled = false;
                }
            }
        }

        public void AddItemList(SpriteRenderer itemSprite)
        {
            items.RemoveAll(x => !x.gameObject.activeInHierarchy);
            items.Add(itemSprite);
        }

        private float ItemDis(Vector3 playerPos, Vector3 itemPos)
        {
            //アイテムとプレイヤーの距離を計算
            float dx = itemPos.x - playerPos.x;  // x座標の差
            float dy = itemPos.y - playerPos.y;  // y座標の差

            return Mathf.Sqrt(dx * dx + dy * dy);  // 距離を計算
        }

    }
}
