using System;
using System.Collections.Generic;
using UnityEngine;
using zzz_TestScripts.AnimationLoadingFromSpriteSheets;
using zzz_TestScripts.AnimationSystemTest;

namespace Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRend;
        [SerializeField] public Animator animator;
        
        [SerializeField] private AnimationDataScriptable animationDataScriptable;
        [SerializeField] private List<Sprite> sprites;
        
        #region Current Sprite Index

        [SerializeField] private int currentSpriteIndex;
        public int CurrentSpriteIndex
        {
            get => currentSpriteIndex;
            set
            {
                if (sprites.Count > 0)
                {
                    currentSpriteIndex = value % sprites.Count;
                    if (currentSpriteIndex < 0)
                        currentSpriteIndex += sprites.Count;
                }
                else
                {
                    currentSpriteIndex = 0;
                }
            }
        }

        #endregion


        private void Start()
        {
            
        }
        
        private void LateUpdate()
        {
            if (sprites.Count > 0)
            {
                spriteRend.sprite = sprites[currentSpriteIndex];
            }
            
            
        }

        [ContextMenu("Set New Animations")]
        public void SetNewAnimations()
        {
            var animStateSetter = new AnimationStateSetter(animator);
            var animJsonFilePath = "Assets/Scripts/zzz_TestScripts/AnimationSystemTest/AnimationDatas.json";
            animStateSetter.SetupAnimationStates(animJsonFilePath);
        }

        public void SetSprite()
        {
            
        }

        public void PlayAnimation(string state)
        {
            Debug.Log($"Playing {state} animation");
            LoadSpriteBasedOnCurrentAnimation(state);
            animator.Play(state);
        }

        private void LoadSpriteBasedOnCurrentAnimation(string currentStateName)
        {
            if (animationDataScriptable.animationDatabases.Count <= 0)
            {
                Debug.LogError("Fatal Error: Scriptable Object has no data in it");
                return;
            }

            foreach (var animationData in animationDataScriptable.animationDatabases[0].animationDatas)
            {
                if(animationData.stateName != currentStateName) continue;
                sprites = animationData.animationSprites;
                CurrentSpriteIndex = 0;
                return;
            }
            
            Debug.LogError($"Fata Error: Animation state {currentStateName} could not be found in animation scriptable");
        }
    }
}
