using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class StringToTextureConverter : MonoBehaviour
{
    const int CHARACTER_TEXTURE_WIDTH = 410;

    public static StringToTextureConverter Instance;

    private void Awake()
    {
        Instance = this;
    }

    public Texture2D GetTextureFromInput(string input, int textureSize = 1024, int wordPadding = 20)
    {
        Texture2D output = new Texture2D(textureSize, textureSize);

        int characterWidth = (textureSize - wordPadding * 2) / input.Count();

        int interpolationValue = CHARACTER_TEXTURE_WIDTH / characterWidth;

        float timeBefore = Time.realtimeSinceStartup;

        Color[] color = new Color[256];

        int setPixels = 0;

        for (int x = 0; x < textureSize/16; x++)
        {
            for (int y = 0; y < textureSize / 16; y++)
            {
                output.SetPixels(16 * x, 16 * y, 16, 16, color);
                setPixels += 256;
            }
        }

        Debug.Log(Time.realtimeSinceStartup - timeBefore);
        Debug.Log(setPixels + " " + 1024*1024);

        foreach (char chr in input)
        {

        }

        return null;
    }
}
