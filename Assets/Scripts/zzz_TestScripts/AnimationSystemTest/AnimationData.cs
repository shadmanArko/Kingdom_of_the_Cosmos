using System;
using System.Collections.Generic;
using UnityEngine;

namespace zzz_TestScripts.AnimationSystemTest
{
    [Serializable]
    public class AnimationData
    {
        public string stateName;
        public string folderPath;
        public List<Sprite> animationSprites;
        public float speed;
    }
}