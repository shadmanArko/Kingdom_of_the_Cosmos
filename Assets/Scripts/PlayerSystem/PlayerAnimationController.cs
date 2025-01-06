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

        private void LateUpdate()
        {
            if (currentAnimationData.animationSprites.Count > 0 && CurrentSpriteIndex < currentAnimationData.animationSprites.Count)
            {
                spriteRend.sprite = currentAnimationData.animationSprites[currentSpriteIndex];
            }
        }

        public void PlayAnimation(string state)
        {
            if (animator == null || !animator.isActiveAndEnabled) return;
            if (currentAnimationData.stateName == state && animator.GetCurrentAnimatorStateInfo(0).length != 0) return;
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
            Debug.Log("Current loaded animation: " + currentAnimationData.stateName);
            
            if (animationDataScriptable.animationDatabases.Count <= 0)
            {
                Debug.LogError("Fatal Error: Scriptable Object has no data in it");
                return;
            }

            foreach (var data in animationDataScriptable.animationDatabases[0].animationDatas)
            {
                if(data.stateName != currentStateName) continue;
                CurrentSpriteIndex = 0;
                spriteRend.flipX = data.flipX;
                spriteRend.flipY = data.flipY;
                currentAnimationData = data;
                return;
            }
            
            Debug.LogError($"Fata Error: Animation state {currentStateName} could not be found in animation scriptable");
        }
    }
}
