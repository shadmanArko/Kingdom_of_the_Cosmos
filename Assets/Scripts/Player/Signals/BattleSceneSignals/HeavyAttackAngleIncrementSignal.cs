namespace Player.Signals.BattleSceneSignals
{
    public class HeavyAttackAngleIncrementSignal
    {
        public float attackAngleBase;
        public float attackAngleHeight;

        public HeavyAttackAngleIncrementSignal(float angleBase, float angleHeight)
        {
            attackAngleBase = angleBase;
            attackAngleHeight = angleHeight;
        }
    }
}