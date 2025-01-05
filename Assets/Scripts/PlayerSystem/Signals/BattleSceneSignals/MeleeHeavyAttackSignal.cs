using WeaponSystem.Models;

namespace PlayerSystem.Signals.BattleSceneSignals
{
    public class MeleeHeavyAttackSignal
    {
        public WeaponData weaponData;

        public MeleeHeavyAttackSignal(WeaponData data)
        {
            weaponData = data;
        }
    }
}