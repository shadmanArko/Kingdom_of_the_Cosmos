using DBMS.JsonToScriptable;
using UnityEditor.Animations;
using UnityEngine;

namespace zzz_TestScripts.AnimationSystemTest
{
    public class AnimationStateSetter
    {
        private readonly Animator _animator;

        public AnimationStateSetter(Animator animator)
        {
            _animator = animator;
        }

        private void SetupAnimationStates(string jsonFilePath)
        {
            var jsonData = JsonDataReader.LoadFromJson<AnimationDatabase>(jsonFilePath);

            var animationDataList = jsonData.AnimationDatas;

            foreach (var data in animationDataList)
            {
                var clip = Resources.Load<AnimationClip>(data.clipPath);
                if (clip != null)
                {
                    var overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController)
                        {
                            [data.stateName] = clip
                        };
                    _animator.runtimeAnimatorController = overrideController;

                    SetAnimationSpeed(data.stateName, data.speed);
                }
                else
                {
                    Debug.LogError($"Animation clip not found at path: {data.clipPath}");
                }
            }
        }

        private void SetAnimationSpeed(string stateName, float speed)
        {
            var controller = _animator.runtimeAnimatorController as AnimatorController;
            if (controller != null)
            {
                foreach (var layer in controller.layers)
                {
                    var stateMachine = layer.stateMachine;
                    foreach (var childState in stateMachine.states)
                    {
                        if (childState.state.name == stateName)
                        {
                            childState.state.speed = speed;
                            return;
                        }
                    }
                }
                Debug.LogWarning($"Animation state '{stateName}' not found in the Animator Controller.");
            }
            else
            {
                Debug.LogError("Unable to access AnimatorController. Make sure you're not using an AnimatorOverrideController.");
            }
        }
    }
}