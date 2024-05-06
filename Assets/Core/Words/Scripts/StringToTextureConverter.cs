using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Localization.SmartFormat.Core.Output;
using UnityEngine.UIElements;
using UnityEngine.Windows;

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

    bool d = false;

    Dictionary<char, Texture2D> _textures;

    private void Start()
    {
        Instance = this;
    }

    public Texture2D GetTextureFromInput(string input, int textureSize = 1024, int wordPadding = 0)
    {
        if (!d)
        {
            d = true;


            //Create default black texture
            sourceTexture = new Texture2D(2, 2, TextureFormat.RGBA32, -1, false, false);

            sourceTexture.alphaIsTransparency = true;
            sourceTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
            sourceTexture.SetPixel(0, 1, new Color(0, 0, 0, 0));
            sourceTexture.SetPixel(1, 0, new Color(0, 0, 0, 0));
            sourceTexture.SetPixel(1, 1, new Color(0, 0, 0, 0));

            sourceTexture.wrapMode = TextureWrapMode.Clamp;

            sourceTexture.Apply();
        }
        Texture2D output = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, -1, false, false);



        if (Graphics.ConvertTexture(sourceTexture, output))
        
        {

        }
        else { }
        foreach (var item in output.GetPixels())
        {
            if (item.r == 0)
            {
                //yeh
            }
        }
        input = input.ToUpper();

        output.wrapMode = TextureWrapMode.Clamp;
        int characterWidth = (textureSize - wordPadding * 2) / input.Length;
        int characterHeight = characterWidth * 2;

        int letterHeightPos = textureSize / 2 - characterHeight / 2;

        for (int characterIndex = 0; characterIndex < input.Length; characterIndex++)
        {
            Texture2D characterColors = GetColorArrayFromCharacter(input[characterIndex], characterWidth, characterHeight);

            foreach (var item in characterColors.GetPixels())
            {
                if (item.r == 0)
                {
                    //yeh
                }
            }

            Graphics.CopyTexture(characterColors, 0, 0, 0, 0, characterWidth, characterHeight, output, 0, 0, wordPadding + characterIndex * characterWidth, letterHeightPos);
        }

        output.Apply();

        return output;
    }

    Texture2D GetColorArrayFromCharacter(char chr, int outputWidth, int outputHeight)
    {
        Sprite characterSprite = spritesList[charList.IndexOf(chr)];

        Texture2D letterBaseTexture = characterSprite.texture;
        Texture2D output = new Texture2D(outputWidth, outputHeight, TextureFormat.RGBA32, -1, false, false);
        foreach (var item in output.GetPixels())
        {
            if (item.r == 0)
            {
                //yeh
            }
        }
        Graphics.ConvertTexture(letterBaseTexture, output);
        foreach (var item in output.GetPixels())
        {
            if (item.r == 0)
            {
                //yeh
            }
        }
        //utput.Resize(outputWidth, outputHeight);
        output.Apply();

        return output;
    }
}
