using UnityEngine;
using zzz_TestScripts.AnimationLoadingFromSpriteSheets;
using zzz_TestScripts.AnimationSystemTest;

namespace PlayerSystem
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRend;
        [SerializeField] public Animator animator;
        
        [SerializeField] private AnimationDataScriptable animationDataScriptable;
        
        [SerializeField] private string currentAnimationName;
        [SerializeField] private AnimationData currentAnimationData;
        
        #region Current Sprite Index

        [SerializeField] private int currentSpriteIndex;
        public int CurrentSpriteIndex
        {
            get => currentSpriteIndex;
            set
            {
                if (currentAnimationData.animationSprites.Count > 0)
                {
                    currentSpriteIndex = value % currentAnimationData.animationSprites.Count;
                    if (currentSpriteIndex < 0)
                        currentSpriteIndex += currentAnimationData.animationSprites.Count;
                }
                else
                {
                    currentSpriteIndex = 0;
                }
            }
        }

        #endregion

        private void Update()
        {
            if(currentAnimationData.stateName != currentAnimationName)
                LoadSpriteBasedOnCurrentAnimation(currentAnimationName);
        }

        private void LateUpdate()
        {
            if (currentAnimationData.animationSprites.Count > 0)
            {
                spriteRend.sprite = currentAnimationData.animationSprites[currentSpriteIndex];
            }
        }

        public void PlayAnimation(string state)
        {
            if (animator == null || !animator.isActiveAndEnabled) return;
            currentAnimationName = state;
            LoadSpriteBasedOnCurrentAnimation(state);
            if (currentAnimationData.triggerName != "")
            {
                animator.SetTrigger(currentAnimationData.triggerName);
            }
            else
            {
                animator.Play(currentAnimationData.stateName);
            }
        }

        private void LoadSpriteBasedOnCurrentAnimation(string currentStateName)
        {
            if (currentAnimationData.stateName == currentStateName) return;
            
            if (animationDataScriptable.animationDatabases.Count <= 0)
            {
                Debug.LogError("Fatal Error: Scriptable Object has no data in it");
                return;
            }

            foreach (var data in animationDataScriptable.animationDatabases[0].animationDatas)
            {
                if(data.stateName != currentStateName) continue;
                currentAnimationData = data;
                CurrentSpriteIndex = 0;
                return;
            }
            
            Debug.LogError($"Fata Error: Animation state {currentStateName} could not be found in animation scriptable");
        }
    }
}
