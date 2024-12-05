using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [System.Serializable]
    public class MapController
    {
        [SerializeField] private GameObject TileMapParent;
        private Transform[] mapObj = new Transform[49];
        private float cellSize; // �e�O���b�h�Z���̃T�C�Y
        private Vector2Int playerGridPos; // �v���C���[�̌��݂̃O���b�h�ʒu

        public void DoStart()
        {
            for(int i = 0; i < mapObj.Length; i++)
            {
                mapObj[i] = TileMapParent.transform.GetChild(i);
            }
        }

        public void InitBattle(Sprite mapTexture, float cellSize)
        {
            for (int i = 0; i < mapObj.Length; i++)
            {
                mapObj[i].GetComponent<SpriteRenderer>().sprite = mapTexture;
            }

            this.cellSize = cellSize;

            UpdateGridPositions();
        }

        public void DoUpdate()
        {
            Vector2Int newPlayerGridPos = new Vector2Int(
                Mathf.FloorToInt(Player.PlayerManager.I.GetPlayerTransform().position.x / cellSize),
                Mathf.FloorToInt(Player.PlayerManager.I.GetPlayerTransform().position.y / cellSize)
            );

            // �v���C���[�̃O���b�h�ʒu���ς�����ꍇ�̂ݍĔz�u
            if (newPlayerGridPos != playerGridPos)
            {
                playerGridPos = newPlayerGridPos;
                UpdateGridPositions();
            }
        }

        void UpdateGridPositions()
        {
            int index = 0;

            // 5*5�̃I�u�W�F�N�g��z�u
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    mapObj[index].localPosition = new Vector3((playerGridPos.x + x) * cellSize, (playerGridPos.y + y) * cellSize, 0.0f);
                    index++;
                }
            }
        }
    }
}

