using System.Collections.Generic;
using UnityEngine;

public class StringToTextureConverter : MonoBehaviour
{
    const int CHARACTER_TEXTURE_WIDTH = 410;

    public static StringToTextureConverter Instance;

    [SerializeField] List<Sprite> spritesList;

    List<char> charList = new List<char>()
{
    ' ', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
    'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
    'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
};

    Dictionary<char, Texture2D> _textures;

    private void Awake()
    {
        Instance = this;
    }

    public Texture2D GetTextureFromInput(string input, int textureSize = 1024, int wordPadding = 0)
    {
        input = input.ToUpper();
        Texture2D output = new Texture2D(textureSize, textureSize);
        int characterWidth = (textureSize - wordPadding * 2) / input.Length;

        Color[] color = new Color[256];

        for (int i = 0; i < 256; i++)
        {
            color[i] = Color.black;
        }

        int setPixels = 0;

        for (int x = 0; x < textureSize / 16; x++)
        {
            for (int y = 0; y < textureSize / 16; y++)
            {
                output.SetPixels(16 * x, 16 * y, 16, 16, color);
                setPixels += 256;
            }
        }

        int letterHeightPos = textureSize / 2 - characterWidth / 2;

        for (int characterIndex = 0; characterIndex < input.Length; characterIndex++)
        {
            Color[] characterColors = GetColorArrayFromCharacter(input[characterIndex], characterWidth);

            output.SetPixels(wordPadding + characterIndex * characterWidth, letterHeightPos, characterWidth, characterWidth, characterColors);
        }

        output.Apply();

        return output;
    }

    Color[] GetColorArrayFromCharacter(char chr, int outputSize)
    {
        Color[] output = new Color[outputSize * outputSize];

        if (!charList.Contains(chr))
        {
            for (int i = 0; i < outputSize * outputSize; i++)
                output[i] = Color.black;

            return output;
        }

        Sprite characterSprite = spritesList[charList.IndexOf(chr)];

        Vector2 texturePosition = characterSprite.textureRect.position;
        float textureDimention = characterSprite.textureRect.width;

        for (int i = 0; i < outputSize * outputSize; i++)
        {
            int x = Mathf.FloorToInt(Mathf.Lerp(0, textureDimention, Mathf.InverseLerp(0, outputSize, i % outputSize)));
            int y = Mathf.FloorToInt(Mathf.Lerp(0, textureDimention, Mathf.InverseLerp(0, outputSize, i / outputSize)));
            output[i] = characterSprite.texture.GetPixel(Mathf.FloorToInt(texturePosition.x) + x, Mathf.FloorToInt(texturePosition.y) + y);
        }

        return output;
    }
}
