using WeaponSystem.Models;

namespace PlayerSystem.Signals.BattleSceneSignals
{
    public class MeleeLightAttackSignal
    {
        public WeaponData weaponData;

        public MeleeLightAttackSignal(WeaponData data)
        {
            weaponData = data;
        }
    }
}
