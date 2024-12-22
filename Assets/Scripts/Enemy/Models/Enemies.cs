using System;
using System.Collections.Generic;

namespace Enemy.Models
{
    [Serializable]
    public class Enemies
    {
        public List<MeleeEnemyData> MeleeEnemyDatas;
        public List<MeleeShieldedEnemyData> MeleeShieldedEnemyDatas;
        public List<RangedEnemyData> RangedEnemyDatas;
        public List<ShamanEnemyData> ShamanEnemyDatas;
    }
}