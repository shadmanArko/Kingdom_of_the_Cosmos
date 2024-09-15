using UnityEngine;

namespace PlayerScripts
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRend;
        [SerializeField] private Animator animator;


        public void SetSprite()
        {
            
        }

        public void PlayAnimation(string state)
        {
            animator.Play(state);
        }
    }
}
