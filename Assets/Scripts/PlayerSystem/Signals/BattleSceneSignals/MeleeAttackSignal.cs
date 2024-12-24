using WeaponSystem.Models;

namespace PlayerSystem.Signals.BattleSceneSignals
{
    public class MeleeAttackSignal
    {
        public WeaponData weaponData;

        public MeleeAttackSignal(WeaponData data)
        {
            weaponData = data;
        }
    }
}
