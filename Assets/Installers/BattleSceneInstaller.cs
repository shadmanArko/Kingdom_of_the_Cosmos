using Models;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "BattleSceneInstaller", menuName = "Installers/BattleSceneInstaller")]
public class BattleSceneInstaller : ScriptableObjectInstaller<BattleSceneInstaller>
{
    public Champion Champion;
    public string Name;
    public override void InstallBindings()
    {
    }
}