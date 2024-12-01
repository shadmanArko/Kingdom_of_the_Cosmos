using System;
using System.Collections.Generic;
using Champions.Models;
using Enemy.Models;
using zzz_TestScripts;

namespace DBMS.GameData
{
    [Serializable]
    public class GameData
    {
        public List<Champion> champions;
        public List<Weapon> weapons;
        public Enemies enemies;
    }
}
