using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    // Reference the player directly through inspector
    [SerializeField] private Transform playerTransform;

    public override void InstallBindings()
    {
        Container.Bind<Transform>()
            .FromInstance(playerTransform)
            .AsSingle();
    }
}