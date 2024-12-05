using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    [System.Serializable]
    public class DropItemSystem
    {
        [SerializeField]
        [Header("�A�C�e���\������")]
        private float activeDis = 13;
        private List<SpriteRenderer> items = new List<SpriteRenderer>();
       
        public void DoFixedUpdate()
        {
            for (int i = 0; i < items.Count; i++)
            {
                // �A�C�e���ƃv���C���[�̋������v�Z
                float distance = ItemDis(Player.PlayerManager.I.GetPlayerTransform().position, items[i].transform.position);

                // �v���C���[�͈͓̔��ɂ���ꍇ�A�A�C�e�����A�N�e�B�u��
                if (distance <= activeDis)
                {
                    // �A�N�e�B�u��
                    items[i].enabled = true;
                }
                else
                {
                    //��A�N�e�B�u��
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
            //�A�C�e���ƃv���C���[�̋������v�Z
            float dx = itemPos.x - playerPos.x;  // x���W�̍�
            float dy = itemPos.y - playerPos.y;  // y���W�̍�

            return Mathf.Sqrt(dx * dx + dy * dy);  // �������v�Z
        }

    }
}
