using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SceneReferences : ScriptableObject
{
    [SerializeField, Scene] List<int> additiveScenes;

    public List<int> AdditiveScenes { get => additiveScenes; }
}
