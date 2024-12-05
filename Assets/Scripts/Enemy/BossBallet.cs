using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class BossBallet : MonoBehaviour
    {
        private float moveSpeed = 5.0f; // ’e‚ÌˆÚ“®‘¬“x
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

            // ’e‚ðŽw’è‚µ‚½•ûŒü‚ÉˆÚ“®
            transform.Translate(balletDirection * moveSpeed * Time.deltaTime);
        }

        private IEnumerator DestroyAfterTime(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}

