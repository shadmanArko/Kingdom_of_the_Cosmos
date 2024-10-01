namespace WeaponSystem.WeaponModels
{
    [System.Serializable]
    public class WeaponData
    {
        public string name;
        public int damage;
        public float cooldown;
        public string type; // Controlled or Automatic
        public string triggerCondition; // Only for automatic weapons
    
    }
}