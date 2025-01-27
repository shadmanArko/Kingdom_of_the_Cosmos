using Cysharp.Threading.Tasks;

namespace PlayerStats
{
    public interface IPlayerStatsController
    {
        void HandleStatPickup(StatPickup pickup);
        void SetTribe(TribeType tribe);
        void SetWeapon(WeaponType weapon);
        UniTask ApplyTemporaryStat(StatType statType, float value, float duration);
        void ModifyPermanentStat(StatType statType, float value);
    }
}