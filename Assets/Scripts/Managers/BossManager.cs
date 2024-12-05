using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BossManager : Singleton<BossManager>
    {
        private Vector2 moveLimitsMin;
        private Vector2 moveLimitsMax;
        private bool isMoveLimitActive = false;
        private BossController bossController;

        private void Awake()
        {
            Init();
        }

        /// <summary>
        /// �{�X�̍s���͈͂�ݒ肷��
        /// </summary>
        /// <param name="centerPosition">�͈͂̒��S�ʒu</param>
        public void SetMoveLimits(Vector3 centerPosition)
        {
            moveLimitsMin = new Vector2(centerPosition.x - 7.5f, centerPosition.y - 10.0f);
            moveLimitsMax = new Vector2(centerPosition.x + 7.5f, centerPosition.y + 10.0f);
            isMoveLimitActive = true;
        }

        /// <summary>
        /// �{�X�̍s���͈͐����𖳌�������
        /// </summary>
        public void DisableMoveLimits()
        {
            isMoveLimitActive = false;
        }
        

        /// <summary>
        /// �{�X�̈ʒu�𐧌�����
        /// </summary>
        /// <param name="position">�V�����{�X�̈ʒu</param>
        /// <returns>�������ꂽ�{�X�̈ʒu</returns>
        public Vector3 LimitBossPosition(Vector3 position)
        {
            if (isMoveLimitActive)
            {
                position.x = Mathf.Clamp(position.x, moveLimitsMin.x, moveLimitsMax.x);
                position.y = Mathf.Clamp(position.y, moveLimitsMin.y, moveLimitsMax.y);
            }
            return position;
        }

        // BossController��������
        public void InitBossController(BossController controller)
        {
            bossController = controller;
        }

        // BossController���擾���郁�\�b�h
        public BossController GetBossController()
        {
            return bossController;
        }
    }
}
