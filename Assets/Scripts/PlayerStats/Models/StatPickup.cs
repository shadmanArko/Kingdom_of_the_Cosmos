namespace PlayerStats
{
    public class StatPickup
    {
        public StatType StatType { get; set; }
        public float Value { get; set; }
        public bool IsPermanent { get; set; }
        public float Duration { get; set; } // Only used for temporary stats
    }
}