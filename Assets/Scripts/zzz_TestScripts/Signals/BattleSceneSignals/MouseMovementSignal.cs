using UnityEngine;

namespace Signals.BattleSceneSignals
{
    public class MouseMovementSignal
    {
        public Vector3 MousePos;
        public MouseMovementSignal(Vector2 mousePos)
        {
            MousePos = mousePos;
        }
    }
}
