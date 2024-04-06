using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StringToTextureConverter : MonoBehaviour
{
    const int CHARACTER_TEXTURE_WIDTH = 410;

    public static StringToTextureConverter Instance;

    [SerializeField] Texture2D defaultTexture;

    private void Awake()
    {
        Instance = this;
    }

    public Texture2D GetTextureFromInput(string input, int textureSize = 1024, int wordPadding = 20)
    {
        Texture2D output = defaultTexture;

        int characterWidth = (textureSize - wordPadding * 2) / input.Count();

        int interpolationValue = CHARACTER_TEXTURE_WIDTH / characterWidth;

        float timeBefore = Time.realtimeSinceStartup;

        for (int x = 0; x < textureSize; x++)
        {
            for (int y = 0; y < textureSize; y++)
            {
                output.SetPixel(x, y, Color.black);
            }
        }

        Debug.Log(Time.realtimeSinceStartup - timeBefore);

        foreach (char chr in input)
        {

        }

        return null;
    }
}
