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

    private void Awake()
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
        //Quick and dirty fix : adapt texture size to input length
        textureSize = 128 * input.Length * (isVertical ? 2 : 1);
        Texture2D output = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, -1, false, false);

        Graphics.ConvertTexture(sourceTexture, output);
        output.ReadPixels(new Rect(0, 0, output.width, output.height), 0, 0);

        input = input.ToUpper();

        output.wrapMode = TextureWrapMode.Clamp;

        if (isVertical)
        {
            int characterHeight = (textureSize - wordPadding * 2) / input.Length;
            int characterWidth = characterHeight / 2;

            int letterWidthPos = textureSize / 2 - characterWidth / 2;

            for (int characterIndex = 0; characterIndex < input.Length; characterIndex++)
            {
                Texture2D characterColors = GetCharacterTexture(input[characterIndex], characterWidth, characterHeight);

                Graphics.CopyTexture(characterColors,
                    0,
                    0,
                    0,
                    0,
                    characterWidth,
                    characterHeight,
                    output,
                    0,
                    0,
                    dstX: letterWidthPos,
                    dstY: wordPadding + (input.Length - characterIndex - 1) * characterHeight);
            }
        }

        else
        {
            int characterWidth = (textureSize - wordPadding * 2) / input.Length;
            int characterHeight = characterWidth * 2;

            int letterHeightPos = textureSize / 2 - characterHeight / 2;

            for (int characterIndex = 0; characterIndex < input.Length; characterIndex++)
            {
                Texture2D characterColors = GetCharacterTexture(input[characterIndex], characterWidth, characterHeight);

                Graphics.CopyTexture(characterColors, 0, 0, 0, 0, characterWidth, characterHeight, output, 0, 0, wordPadding + characterIndex * characterWidth, letterHeightPos);
            }
        }

        output.Apply();

        return output;
    }

    Texture2D GetCharacterTexture(char chr, int outputWidth, int outputHeight)
    {
        if (!charList.Contains(chr))
            return null;

        //retrieve the sprite of the character
        Sprite characterSprite = spritesList[charList.IndexOf(chr)];

        //retrieve the coordinates of the character texture within the atlas
        Rect textureRect = characterSprite.textureRect;

        //Create the output texture with the proper size
        Texture2D output = new Texture2D(outputWidth, outputHeight, TextureFormat.RGBA32, -1, false, false);

        //Create the texture of the letter that we are copying
        Texture2D letterBaseResolution = new Texture2D(Mathf.FloorToInt(characterSprite.textureRect.width), Mathf.FloorToInt(characterSprite.textureRect.height), TextureFormat.RGBA32, -1, false, false);

        //Create the texture of the letter into the texture we just created
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

        //Copy the texture of the letter to the proper size
        Graphics.ConvertTexture(letterBaseResolution, output);

        //Retrieves the pixels data from the GPU to the CPU
        output.ReadPixels(new Rect(0, 0, output.width, output.height), 0, 0);

        return output;
    }
}
