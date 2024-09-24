using UnityEngine;

namespace DBMS.RunningData
{
    [CreateAssetMenu(fileName = "RunningDataScriptable", menuName = "ScriptableObjects/RunningDataScriptable", order = 0)]
    public class RunningDataScriptable : ScriptableObject
    {
        public Vector2 attackDirection;
    }
}
