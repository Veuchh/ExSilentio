using LW.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealableObjectHandler : MonoBehaviour
{
    public static RevealableObjectHandler Instance { get; private set; }

    List<RevealableObjectBundle> bundles = new List<RevealableObjectBundle>();

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void RegisterBundle(RevealableObjectBundle newBundle)
    {
        if (!bundles.Contains(newBundle))
            bundles.Add(newBundle);
        else
            Debug.LogWarning("A bundle attempted to register twice. Register attempt rejected.");
    }

    public void AttemptWordDiscovery(DatabaseQueryResult queryResult)
    {

    }
}
