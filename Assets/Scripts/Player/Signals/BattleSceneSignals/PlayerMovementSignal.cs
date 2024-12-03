using UnityEngine;

namespace Player.Signals.BattleSceneSignals
{
    public class PlayerMovementSignal
    {
        public Vector2 MovePos;
        
        public PlayerMovementSignal(Vector2 movePos)
        {
            MovePos = movePos;
        }
    }
}