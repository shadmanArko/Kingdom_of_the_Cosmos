namespace Player.Signals.BattleSceneSignals
{
    public class HeavyAttackChargeMeterSignal
    {
        public float attackAngleBase;
        public float attackAngleHeight;

        public HeavyAttackChargeMeterSignal(float angleBase, float angleHeight)
        {
            attackAngleBase = angleBase;
            attackAngleHeight = angleHeight;
        }
    }
}