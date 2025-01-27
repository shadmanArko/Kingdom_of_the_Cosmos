using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerStats
{
    [Serializable]
    public class StatModificationConfig
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public Dictionary<string, float> StatModifications { get; set; }
        public bool IsPermanent { get; set; }
        public float Duration { get; set; }
        public Sprite Icon { get; set; }
    }
}