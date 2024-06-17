using LW.Data;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LW.Player
{
    [SelectionBase]
    public class SpawnPoint : MonoBehaviour
    {
        [Flags]
        enum ComponentsToAddOnSpawn
        {
            ReflectionHandler = 2,
            DistanceToWater = 3,
        }

        [SerializeField] GameObject playerPrefab;
        [SerializeField] List<GameObject> instantiateAsPlayerChild;
        [SerializeField] List<GameObject> setAsPlayerChild;
        [SerializeField] List<MeshRenderer> waterPlanes;
        [SerializeField] ComponentsToAddOnSpawn componentsToAddToPlayer;
        [SerializeField] bool changePlayer;
        [SerializeField, ShowIf(nameof(changePlayer))] float playerSpeed = 8;
        [SerializeField, ShowIf(nameof(changePlayer))] float playerHeight = 2;
        [SerializeField, ShowIf(nameof(changePlayer))] float playerWidth = .5f;

        private void Start()
        {
            GameObject player = Instantiate(playerPrefab, transform.position, transform.rotation);

            StaticData.PlayerTransform = player.transform;

            foreach (ComponentsToAddOnSpawn componentType in Enum.GetValues(typeof(ComponentsToAddOnSpawn)))
            {
                if (componentsToAddToPlayer.HasFlag(componentType))
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

            foreach (GameObject prefab in instantiateAsPlayerChild)
            {
                Instantiate(prefab, player.transform.position, player.transform.rotation, player.transform);
            }

            foreach (GameObject go in setAsPlayerChild)
            {
                go.transform.parent = player.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }

            foreach (var item in FindObjectsOfType<AkTriggerEnter>())
            {
                item.triggerObject = player;
            }

            foreach (var item in FindObjectsOfType<AkTriggerExit>())
            {
                item.triggerObject = player;
            }

            if (changePlayer)
            {
                CharacterController controller = player.GetComponent<CharacterController>();
                controller.height = playerHeight;
                controller.radius= playerWidth;

                player.GetComponent<PlayerMovement>().SetNewSpeed(playerSpeed);
            }

            Destroy(gameObject);
        }
    }
}