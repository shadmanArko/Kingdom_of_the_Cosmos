using System.Collections.Generic;
using UnityEngine;

namespace DBMS.RunningData
{
    [CreateAssetMenu(fileName = "RunningDataScriptable", menuName = "ScriptableObjects/RunningDataScriptable", order = 0)]
    public class RunningDataScriptable : ScriptableObject
    {
        public Vector2 attackDirection;
        public List<Vector2> attackAngle;
        public bool isAutoAttacking;
        public Transform closestEnemyToPlayer;
    }
}
