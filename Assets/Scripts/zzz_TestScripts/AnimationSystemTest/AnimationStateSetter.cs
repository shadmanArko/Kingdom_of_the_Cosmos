using System.Collections.Generic;
using System.Linq;
using DBMS.JsonToScriptable;
using UnityEditor.Animations;
using UnityEngine;

namespace zzz_TestScripts.AnimationSystemTest
{
public class AnimationStateSetter
    {
        private readonly Animator _animator;
        private AnimatorController _baseController;
        private AnimatorOverrideController _overrideController;

        public AnimationStateSetter(Animator animator)
        {
            _animator = animator;
            _baseController = _animator.runtimeAnimatorController as AnimatorController;
            if (_baseController == null)
            {
                Debug.LogError("The Animator must have an AnimatorController assigned, not an AnimatorOverrideController.");
                return;
            }
            _overrideController = new AnimatorOverrideController(_baseController);
            _overrideController.name = "NewPlayerAnimatorOverrideController";
            _animator.runtimeAnimatorController = _overrideController;
        }

        public void SetupAnimationStates(string jsonFilePath)
        {
            var jsonData = JsonDataReader.LoadFromJson<AnimationDatabase>(jsonFilePath);
            var animationDataList = jsonData.AnimationDatas;

            foreach (var animationData in animationDataList)
            {
                Debug.Log($"anim data found: {animationData.stateName}");
            }

            var clipOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            _overrideController.GetOverrides(clipOverrides);

            foreach (var data in animationDataList)
            {
                var clip = Resources.Load<AnimationClip>(data.clipPath);
                if (clip != null)
                {
                    var originalClip = _overrideController[data.stateName];
                    Debug.Log($"Original clip is null {originalClip == null}");
                    Debug.Log($"found original clip {originalClip.name}");
                    
                    var index = clipOverrides.FindIndex(kvp => kvp.Key == originalClip);
                    var modifiedClipOverride = new KeyValuePair<AnimationClip, AnimationClip>(originalClip, clip);
                    clipOverrides[index] = modifiedClipOverride;
                    
                    // clipOverrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(originalClip, clip));
                    SetAnimationSpeed(data.stateName, data.speed);
                }
                else
                {
                    Debug.LogError($"Animation clip not found at path: {data.clipPath}");
                }
            }

            _overrideController.ApplyOverrides(clipOverrides);
        }

        private void SetAnimationSpeed(string stateName, float speed)
        {
            foreach (var layer in _baseController.layers)
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
    }
}