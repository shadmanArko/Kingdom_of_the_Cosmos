using System.Collections.Generic;
using System.Linq;
using DBMS.JsonToScriptable;
using UnityEngine;

namespace zzz_TestScripts.AnimationSystemTest
{
public class AnimationStateSetter
    {
        // private readonly Animator _animator;
        // private AnimatorController _baseController;
        // private AnimatorOverrideController _overrideController;
        //
        // public AnimationStateSetter(Animator animator)
        // {
        //     _animator = animator;
        //     _baseController = _animator.runtimeAnimatorController as AnimatorController;
        //     if (_baseController == null)
        //     {
        //         Debug.LogError("The Animator must have an AnimatorController assigned, not an AnimatorOverrideController.");
        //         return;
        //     }
        //     // _overrideController = new AnimatorOverrideController(_baseController);
        //     // _overrideController.name = "NewPlayerAnimatorOverrideController";
        //     // _animator.runtimeAnimatorController = _overrideController;
        //
        //     
        // }
        //
        // public void SetupAnimationStates(string jsonFilePath)
        // {
        //     var jsonData = JsonDataReader.LoadFromJson<AnimationDatabase>(jsonFilePath);
        //     var animationDataList = jsonData.AnimationDatas;
        //
        //     var states = _baseController.layers[0].stateMachine.states;
        //
        //     foreach (var animationData in animationDataList)
        //     {
        //         var clip = Resources.Load<AnimationClip>(animationData.clipPath);
        //         if (clip == null)
        //         {
        //             Debug.LogError($"Animation clip is null: {animationData.stateName}");
        //             continue;
        //         }
        //         
        //         var matchedState = states.FirstOrDefault(thisState => thisState.state.name == animationData.stateName);
        //         if(matchedState.state == null) continue;
        //         matchedState.state.motion = clip;
        //         matchedState.state.speed = animationData.speed;
        //     }
        // }
        //
        // // private void SetAnimationSpeed(string stateName, float speed)
        // // {
        // //     foreach (var layer in _baseController.layers)
        // //     {
        // //         var stateMachine = layer.stateMachine;
        // //         foreach (var childState in stateMachine.states)
        // //         {
        // //             if (childState.state.name == stateName)
        // //             {
        // //                 childState.state.speed = speed;
        // //                 return;
        // //             }
        // //         }
        // //     }
        // //     Debug.LogWarning($"Animation state '{stateName}' not found in the Animator Controller.");
        // // }
    }
}