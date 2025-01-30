using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PlayerSystem.Models;
using Zenject;

namespace PlayerStats.Manager
{
    public class PlayerStatModificationManager
    {
        [Inject] private Player _player;
        [Inject] private PlayerStatsController _statsController;

        public void ApplyStatModification(StatModificationConfig config)
        {
            foreach (var modification in config.StatModifications)
            {
                StatType statType = ConvertStringToStatType(modification.Key);
                float value = modification.Value;

                if (config.IsPermanent)
                {
                    _statsController.ModifyPermanentStat(statType, value);
                }
                else
                {
                    _statsController.ApplyTemporaryStat(statType, value, config.Duration).Forget();
                }
            }
        }

        private StatType ConvertStringToStatType(string statName)
        {
            return Enum.TryParse(statName, out StatType statType) 
                ? statType 
                : throw new ArgumentException($"Unknown stat type: {statName}");
        }

        public List<StatModificationConfig> LoadConfigurationsFromJson(string jsonContent)
        {
            return JsonConvert.DeserializeObject<List<StatModificationConfig>>(jsonContent);
        }
    }
}