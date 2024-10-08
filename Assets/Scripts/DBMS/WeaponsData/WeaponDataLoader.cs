using DBMS.JsonToScriptable;
using WeaponSystem.Models;

namespace DBMS.WeaponsData
{
    public class WeaponDataLoader
    {
        private WeaponDatabaseScriptable _weaponDatabaseScriptable;

        public WeaponDataLoader(WeaponDatabaseScriptable weaponDatabaseScriptable)
        {
            _weaponDatabaseScriptable = weaponDatabaseScriptable;
        }

        public void LoadWeaponData()
        {
            var weaponDatabaseFilePath = "Assets/Scripts/DBMS/WeaponsData/WeaponDatabase.json";
            var weaponDatabase = JsonToScriptableReader.LoadFromJson<WeaponDatabase>(weaponDatabaseFilePath);
        
            if (weaponDatabase != null)
            {
                _weaponDatabaseScriptable.weaponDatabase = weaponDatabase;
            }
        }
        
    }
}