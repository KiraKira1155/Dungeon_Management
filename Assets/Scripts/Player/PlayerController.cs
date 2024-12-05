using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerController
    {
        public PlayerMove move { get; private set; }
        public PlayerAutoAttack autoAttack { get; private set; }

        public PlayerSpriteController playerSpriteController = new PlayerSpriteController();


        public void InitPlayerSetting(PlayerManager.PlayerCharactersID id)
        {
            var player = PlayerManager.I.basicStatus;
            move = new PlayerMove(player.GetCharacterStatusMoveSpeed(id));
            autoAttack = new PlayerAutoAttack(player.GetCharacterStatusAttackInterval(id));
        }

        /// <summary>
        /// UpdateŠÖ”“à‚É‚Äˆ—‚ğs‚¤
        /// </summary>
        public void DoUpdate()
        {
            move.DoUpdate();
            autoAttack.DoUpdate();

        }

        public void DoFixedUpdate()
        {
            move.DoFixedUpdate();
            playerSpriteController.UpdateSpriteDirection();
        }
    }
}
