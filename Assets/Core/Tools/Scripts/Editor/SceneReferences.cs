using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SceneReferences : ScriptableObject
{
    [SerializeField, Scene] List<int> additiveScenes;

    public List<int> AdditiveScenes { get => additiveScenes; }
}
