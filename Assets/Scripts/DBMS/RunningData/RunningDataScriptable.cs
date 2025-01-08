using System.Collections.Generic;
using PlayerSystem.Controllers;
using PlayerSystem.Services;
using UnityEngine;
using UnityEngine.Serialization;

namespace DBMS.RunningData
{
    [CreateAssetMenu(fileName = "RunningDataScriptable", menuName = "ScriptableObjects/RunningDataScriptable", order = 0)]
    public class RunningDataScriptable : ScriptableObject
    {
        public PlayerController playerController;
        public Vector2 moveDirection;
        public Vector2 attackDirection;
        public List<Vector2> attackAnglePoints;
        public float attackAngle;
        public Vector3 playerAttackAnglePosition;
        
        
        public Vector3 closestEnemyToPlayer;
        
        //For test! Will be removed later
        public WeaponThrowService weaponThrowService;
        public Vector2 playerVelocity;
        public float playerVelocityMagnitude;
    }
}
