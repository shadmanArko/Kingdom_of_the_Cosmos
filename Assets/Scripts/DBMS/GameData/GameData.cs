using System;
using System.Collections.Generic;
using Models;

namespace GameData
{
    [Serializable]
    public class GameData
    {
        public List<Champion> champions;
        public List<Weapon> weapons;
        public Enemies enemies;
    }
}
