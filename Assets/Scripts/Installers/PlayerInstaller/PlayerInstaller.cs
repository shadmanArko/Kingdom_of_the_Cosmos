﻿using System;
using Experience;
using PlayerStats;
using PlayerStats.Manager;
using PlayerSystem.Controllers;
using PlayerSystem.Models;
using PlayerSystem.PlayerSO;
using PlayerSystem.Services.HealthService;
using PlayerSystem.Signals.BattleSceneSignals;
using PlayerSystem.Views;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Installers.PlayerInstaller
{
    [CreateAssetMenu(fileName = "PlayerInstaller", menuName = "Installers/PlayerInstaller")]
    public class PlayerInstaller : ScriptableObjectInstaller<PlayerInstaller>
    {
        [SerializeField] private PlayerScriptableObject playerScriptableObject;
        
        [SerializeField] private GameObject playerView;
        [SerializeField] private PlayerHealthView playerHealthView;
        [SerializeField] private ExpView expView;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle().NonLazy();
            Container.BindInterfacesTo<PlayerModManager>().AsSingle().NonLazy();
            
            //Scriptables
            Container.Bind<PlayerScriptableObject>().FromScriptableObject(playerScriptableObject).AsSingle().NonLazy();
            
            //Views
            Container.Bind<PlayerView>().FromComponentInNewPrefab(playerView).AsSingle();
            Container.Bind<PlayerHealthView>().FromComponentInNewPrefab(playerHealthView).AsSingle();
            
            
            // Container.Bind<ExpView>().FromComponentInNewPrefab(expView).AsSingle();
            Container.Bind<CompositeDisposable>().AsSingle();
            
            //Controllers
            Container.Bind<ExpController>().AsSingle();
            
            //Services
            Container.Bind<PlayerHealthService>().AsSingle();

            //Models
            Container.Bind<ExpModel>().AsSingle();
            Container.Bind<PlayerMod>().AsSingle();
            
            
            //Signals
            Container.DeclareSignal<PlayerMovementSignal>();
            Container.DeclareSignal<CancelHeavyAttackSignal>();
        }
    }
}