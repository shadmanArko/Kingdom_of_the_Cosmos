using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using zzz_TestScripts.AnimationSystemTest;

namespace zzz_TestScripts.AnimationLoadingFromSpriteSheets
{
    [CreateAssetMenu(fileName = "AnimationDataScriptable", menuName = "ScriptableObjects/AnimationDataScriptable")]
    public class AnimationDataScriptable : ScriptableObject
    {
        public List<AnimationDatabase> animationDatabases;
    }
}
