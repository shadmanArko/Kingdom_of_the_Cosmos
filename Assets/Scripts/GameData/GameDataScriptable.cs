using System;
using System.IO;
using UnityEngine;

namespace GameData
{
    [CreateAssetMenu(fileName = "GameDataScriptable", menuName = "ScriptableObjects/GameDataScriptable", order = 0)]
    public class GameDataScriptable : ScriptableObject
    {
        public GameData gameData;
    }
}