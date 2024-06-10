using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RespawnCommandCallbackHandler : MonoBehaviour
{
    public static RespawnCommandCallbackHandler Instance;

    [SerializeField] UnityEvent respawnCommandCallbacks;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void OnRespawnCommand()
    {
        respawnCommandCallbacks?.Invoke();
    }
}
