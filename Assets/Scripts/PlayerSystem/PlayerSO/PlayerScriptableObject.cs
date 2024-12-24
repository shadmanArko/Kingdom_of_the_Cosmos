using PlayerSystem.Models;
using UnityEngine;

namespace PlayerSystem.PlayerSO
{
    [CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "ScriptableObjects/PlayerScriptableObject")]
    public class PlayerScriptableObject : ScriptableObject
    {
        public Player player;
        
        public bool canTakeDamage;
    }
}
