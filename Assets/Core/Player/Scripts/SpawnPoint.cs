using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class SpawnPoint : MonoBehaviour
{
    [Flags]enum ComponentsToAddOnSpawn
    {
        ReflectionHandler = 2,
        DistanceToWater = 3,
    }

    [SerializeField] GameObject playerPrefab;
    [SerializeField] List<MeshRenderer> waterPlanes;
    [SerializeField] AK.Wwise.Event events;
    [SerializeField] ComponentsToAddOnSpawn componentsToAddToPlayer;

    private void Start()
    {
        GameObject player = Instantiate(playerPrefab, transform.position, transform.rotation);

        foreach (ComponentsToAddOnSpawn componentType in Enum.GetValues(typeof(ComponentsToAddOnSpawn)))
        {
            switch (componentType)
            {
                case ComponentsToAddOnSpawn.ReflectionHandler:
                    player.AddComponent<ReflectionHandler>().Init(waterPlanes);
                    break;
                case ComponentsToAddOnSpawn.DistanceToWater:
                    player.AddComponent<WwiseDistanceToWater>().Init(0, 35);
                    break;
            }
        }

        Destroy(gameObject);
    }
}
