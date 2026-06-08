using Network;
using Player;
using UI;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{   
    [SerializeField] private CinemachineCamera playerCamera;
    protected override void Configure(IContainerBuilder builder)
    {
        // builder.RegisterComponent(NetworkManager.Singleton);
        builder.RegisterComponentInHierarchy<NetworkObjectSpawner>();
        builder.RegisterComponentInHierarchy<GameMode>();
        builder.RegisterComponentInHierarchy<MapCreator>();
        builder.RegisterComponentInHierarchy<PlayerInit>();
        builder.RegisterComponentInHierarchy<MainUIController>();
    }
}