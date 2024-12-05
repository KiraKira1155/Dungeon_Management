using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemy
{
    [System.Serializable]
    public class EnemySpriteController
    {
        private float diff;
        private const string isDeath = "isDeath";
        [SerializeField] private Animator thisAnimator;

        /// <summary>
        /// 死亡アニメーションを再生する
        /// </summary>
        public void StartDeathAnimation()
        {
            if(thisAnimator != null)
                thisAnimator.SetBool(isDeath, true);
        }

        public void EndDeathAnimation()
        {
            thisAnimator.SetBool(isDeath, false);
        }


        public void Animation(IBaseEnemy enemy)
        {
            UpdateSpriteDirection(enemy);

            if (GameManager.I.sceneController.GetGameModeForBattleScene() == GameSystem.SceneController.GameModeForBattle.battle)
                thisAnimator.enabled = true;
            else
                thisAnimator.enabled = false;
        }

        public void UpdateSpriteDirection(IBaseEnemy enemy)
        {
            diff = Player.PlayerManager.I.GetPlayerTransform().position.x - enemy.GetEnemyTransform().position.x;

            //　プレイヤーが右側にいる
            if (diff > 0)
                enemy.GetSpriteRenderer().flipX = false;
            else if(diff < 0)
                enemy.GetSpriteRenderer().flipX = true;
        }

        public void StopAnimation()
        {
            thisAnimator.enabled = false;
        }

        public void PlayAnimation()
        {
            thisAnimator.enabled = true;
        }
    }
}

