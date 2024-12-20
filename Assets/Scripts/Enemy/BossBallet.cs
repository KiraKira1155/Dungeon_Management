using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BossBallet : MonoBehaviour
    {
        private float moveSpeed = 5.0f; // 弾の移動速度
        private Vector2 balletDirection;

        public void Initialize(Vector2 direction, float speed)
        {
            this.balletDirection = direction.normalized; 
            this.moveSpeed = speed; 
            StartCoroutine(DestroyAfterTime(2.0f));
        }

        private void Update()
        {
            if (GameManager.I.sceneController.GetGameModeForBattleScene() != GameSystem.SceneController.GameModeForBattle.battle)
                return;

            // 弾を指定した方向に移動
            transform.Translate(balletDirection * moveSpeed * Time.deltaTime);
        }

        private IEnumerator DestroyAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}

