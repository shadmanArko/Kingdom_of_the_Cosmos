using UnityEngine;
using WeaponSystem.Models;

namespace DBMS.WeaponsData
{
    [CreateAssetMenu(fileName = "WeaponDatabaseScriptable", menuName = "ScriptableObjects/WeaponDatabaseScriptable")]
    public class WeaponDatabaseScriptable : ScriptableObject
    {
        public WeaponDatabase weaponDatabase;
    }
}