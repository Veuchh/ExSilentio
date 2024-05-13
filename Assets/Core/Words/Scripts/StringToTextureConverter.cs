using System.Collections.Generic;
using UnityEngine;

public class StringToTextureConverter : MonoBehaviour
{
    Texture2D sourceTexture;
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

    private void Start()
    {
        Instance = this;

        GenerateDefaultTexture();
    }

    private void GenerateDefaultTexture()
    {
        //Create default black texture
        sourceTexture = new Texture2D(1, 1, TextureFormat.RGBA32, -1, false, false);
        sourceTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
        sourceTexture.wrapMode = TextureWrapMode.Clamp;
        sourceTexture.Apply();
    }

    public Texture2D GetTextureFromInput(string input, int textureSize = 1024, int wordPadding = 0, bool isVertical = false)
    {
        GenerateDefaultTexture();

        Texture2D output = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, -1, false, false);

        Graphics.ConvertTexture(sourceTexture, output);
        output.ReadPixels(new Rect(0, 0, output.width, output.height), 0, 0);

        input = input.ToUpper();

        output.wrapMode = TextureWrapMode.Clamp;

        if (isVertical)
        {
            float characterHeight = (textureSize - wordPadding * 2) / input.Length;
            float characterWidth = characterHeight / 2;

            float letterWidthPos = textureSize / 2 - characterWidth / 2;

            for (int characterIndex = 0; characterIndex < input.Length; characterIndex++)
            {
                Texture2D characterColors = GetLetterTexture(input[characterIndex], Mathf.FloorToInt(characterWidth), Mathf.FloorToInt(characterHeight));

                Graphics.CopyTexture(characterColors, 0, 0, 0, 0, Mathf.FloorToInt(characterWidth), Mathf.FloorToInt(characterHeight), output, 0, 0,
                    dstX: Mathf.FloorToInt(letterWidthPos),
                   dstY: Mathf.FloorToInt(wordPadding + (input.Length - characterIndex - 1) * characterHeight));
            }
        }

        else
        {
            int characterWidth = (textureSize - wordPadding * 2) / input.Length;
            int characterHeight = characterWidth * 2;

            int letterHeightPos = textureSize / 2 - characterHeight / 2;

            for (int characterIndex = 0; characterIndex < input.Length; characterIndex++)
            {
                Texture2D characterColors = GetLetterTexture(input[characterIndex], characterWidth, characterHeight);

                Graphics.CopyTexture(characterColors, 0, 0, 0, 0, characterWidth, characterHeight, output, 0, 0, wordPadding + characterIndex * characterWidth, letterHeightPos);
            }
        }

        output.Apply();

        return output;
    }

    Texture2D GetLetterTexture(char chr, int outputWidth, int outputHeight)
    {
        if (!charList.Contains(chr))
            return null;

        Sprite characterSprite = spritesList[charList.IndexOf(chr)];
        Rect textureRect = characterSprite.textureRect;

        Texture2D output = new Texture2D(outputWidth, outputHeight, TextureFormat.RGBA32, -1, false, false);

        Texture2D letterBaseResolution = new Texture2D(Mathf.FloorToInt(characterSprite.textureRect.width), Mathf.FloorToInt(characterSprite.textureRect.height), TextureFormat.RGBA32, -1, false, false);

        Graphics.CopyTexture(
            src: characterSprite.texture,
            srcElement: 0,
            srcMip: 0,
            srcX: Mathf.FloorToInt(textureRect.x),
            srcY: Mathf.FloorToInt(textureRect.y),
            srcWidth: Mathf.FloorToInt(textureRect.width),
            srcHeight: Mathf.FloorToInt(textureRect.height),
            dst: letterBaseResolution,
            dstElement: 0,
            dstMip: 0,
            dstX: 0,
            dstY: 0);

        Graphics.ConvertTexture(letterBaseResolution, output);

        output.ReadPixels(new Rect(0, 0, output.width, output.height), 0, 0);

        return output;
    }
}
