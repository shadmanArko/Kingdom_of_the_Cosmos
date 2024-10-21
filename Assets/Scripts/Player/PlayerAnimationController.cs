using System;
using UnityEngine;
using zzz_TestScripts.AnimationSystemTest;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRend;
        [SerializeField] private Animator animator;

        private void Start()
        {
            
        }

        [ContextMenu("Set New Animations")]
        public void SetNewAnimations()
        {
            // var animStateSetter = new AnimationStateSetter(animator);
            // var animJsonFilePath = "Assets/Scripts/zzz_TestScripts/AnimationSystemTest/AnimationDatas.json";
            // animStateSetter.SetupAnimationStates(animJsonFilePath);
        }

        public void SetSprite()
        {
            
        }

        public void PlayAnimation(string state)
        {
            Debug.Log($"Playing {state} animation");
            animator.Play(state);
        }
    }
}
