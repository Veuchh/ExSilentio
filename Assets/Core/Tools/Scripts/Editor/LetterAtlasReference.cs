using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LetterAtlasReference : ScriptableObject
{
    [SerializeField] List<Sprite> spritesList;

    public List<Sprite> SpritesList => spritesList;

}
