using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerSpriteController
    {
        private const string isMoving = "isMoving";

        public void UpdateSpriteDirection()
        {
            if (JoyStickManager.I.GetInputDirection().x < 0)
            {
                // 左向き
                PlayerManager.I.GetPlayerSpriterenderer().flipX = true;
                PlayerManager.I.GetPlayerAnimator().SetBool(isMoving, true);
            }
            else if (JoyStickManager.I.GetInputDirection().x > 0)
            {
                // 右向き
                PlayerManager.I.GetPlayerSpriterenderer().flipX = false;
                PlayerManager.I.GetPlayerAnimator().SetBool(isMoving, true);
            }
            else if (JoyStickManager.I.GetInputDirection() == Vector2.zero)
            {
                // 待機状態
                PlayerManager.I.GetPlayerAnimator().SetBool(isMoving, false);
            }
        }
    }
}
