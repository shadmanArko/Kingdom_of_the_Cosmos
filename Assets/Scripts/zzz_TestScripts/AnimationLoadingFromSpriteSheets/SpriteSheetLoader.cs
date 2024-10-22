using System.IO;
using Models;
using UnityEditor;
using UnityEngine;

namespace zzz_TestScripts.AnimationLoadingFromSpriteSheets
{
    public class SpriteSheetLoader
    {
        public SpriteSheetInfo LoadFromTexture(Texture2D spriteSheet, int spriteWidth, int spriteHeight)
        {
            Sprite[] sprites = new Sprite[192];
            //     spriteSheet.width / spriteWidth * (spriteSheet.height / spriteHeight)
            // ];

            for (int y = 0; y < spriteSheet.height; y += spriteHeight)
            {
                for (int x = 0; x < spriteSheet.width; x += spriteWidth)
                {
                    Sprite sprite = Sprite.Create(
                        spriteSheet,
                        new Rect(x, y, spriteWidth, spriteHeight),
                        new Vector2(0.5f, 0.5f),
                        100.0f
                    );
                    int index = (y / spriteHeight) * (spriteSheet.width / spriteWidth) + (x / spriteWidth);
                    sprites[index] = sprite;
                }
            }

            var spriteSheetInfo = new SpriteSheetInfo
            {
                name = spriteSheet.name,
                sprites = sprites
            };

            return spriteSheetInfo;
        }

        // Method 4: Load from file at runtime
        public SpriteSheetInfo LoadFromFile(string filePath)
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            SetPixelsPerUnit(texture,16);
            texture.LoadImage(fileData);
            texture.filterMode = FilterMode.Point;

            // Assuming sprites are square and arranged in a grid
            var spriteSize = 32; // Adjust this to match your sprite size
            return LoadFromTexture(texture, spriteSize, spriteSize);
        }
        
        void SetPixelsPerUnit(Texture2D textureToModify, int pixelsPerUnit)
        {
            if (textureToModify == null)
            {
                Debug.LogError("No texture assigned!");
                return;
            }

            // var assetPath = AssetDatabase.GetAssetPath(textureToModify);
            // var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            //
            // if (importer != null)
            // {
            //     importer.spritePixelsPerUnit = pixelsPerUnit;
            //     importer.SaveAndReimport();
            //     Debug.Log($"Set Pixels Per Unit to {pixelsPerUnit} for {textureToModify.name}");
            // }
            // else
            // {
            //     Debug.LogError($"Failed to get TextureImporter for {textureToModify.name}");
            // }
        }
    }
}
