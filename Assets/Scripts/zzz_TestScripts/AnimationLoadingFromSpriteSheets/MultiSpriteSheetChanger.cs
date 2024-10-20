using System.Collections.Generic;
using Models;
using UnityEngine;

namespace zzz_TestScripts.AnimationLoadingFromSpriteSheets
{
    public class MultiSpriteSheetChanger : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public List<SpriteSheetInfo> spriteSheets;

        #region Current Sprite Sheet Index

        [SerializeField] private int currentSpriteSheetIndex;
        public int CurrentSpriteSheetIndex
        {
            get => currentSpriteSheetIndex;
            set
            {
                currentSpriteSheetIndex = Mathf.Clamp(value, 0, spriteSheets.Count - 1);
                CurrentSpriteIndex = currentSpriteIndex; // Ensure sprite index is valid for new spriteSheet
            }
        }

        #endregion
        
        #region Current Sprite Index

        [SerializeField] private int currentSpriteIndex;
        public int CurrentSpriteIndex
        {
            get => currentSpriteIndex;
            set
            {
                if (spriteSheets.Count > 0 && spriteSheets[currentSpriteSheetIndex].sprites.Length > 0)
                {
                    currentSpriteIndex = value % spriteSheets[currentSpriteSheetIndex].sprites.Length;
                    if (currentSpriteIndex < 0)
                        currentSpriteIndex += spriteSheets[currentSpriteSheetIndex].sprites.Length;
                }
                else
                {
                    currentSpriteIndex = 0;
                }
            }
        }

        #endregion

        private void Start()
        {
            // spriteSheets = new List<SpriteSheetInfo>();
        }

        private void LateUpdate()
        {
            if (spriteSheets.Count > 0 && spriteSheets[currentSpriteSheetIndex].sprites.Length > 0)
            {
                spriteRenderer.sprite = spriteSheets[currentSpriteSheetIndex].sprites[currentSpriteIndex];
            }
        }

        public void AddSpriteSheet(SpriteSheetInfo spriteSheetInfo)
        {
            spriteSheets.Add(new SpriteSheetInfo { name = spriteSheetInfo.name, sprites = spriteSheetInfo.sprites });
        }

        public void ChangeSpriteSheet(string spriteSheetName)
        {
            int index = spriteSheets.FindIndex(sheet => sheet.name == spriteSheetName);
            if (index != -1)
            {
                CurrentSpriteSheetIndex = index;
            }
            else
            {
                Debug.LogWarning($"SpriteSheet '{spriteSheetName}' not found.");
            }
        }

        public void ChangeSpriteSheet(int index)
        {
            CurrentSpriteSheetIndex = index;
        }

        #region Test

        [ContextMenu("ToggleSpriteSheet")]
        public void ToggleSpriteSheet()
        {
            if(spriteSheets.Count <= 1) return;
            currentSpriteSheetIndex = (currentSpriteSheetIndex + 1) % spriteSheets.Count;
        }

        // [ContextMenu("LoadFromFile")]
        // private void LoadSpriteSheetFromFile()
        // {
        //     var filePath = "Assets/Sprites/Characters/Puny-Characters/Orc-Peon-Cyan.png";
        //     var spriteSheetLoader = new SpriteSheetLoader();
        //     var spriteSheetInfo = spriteSheetLoader.LoadFromFile(filePath);
        //     if (spriteSheetInfo.sprites.Length <= 0)
        //     {
        //         Debug.LogError($"Sprite sheet has no sprites {spriteSheetInfo.name}");
        //         return;
        //     }
        //     
        //     AddSpriteSheet(spriteSheetInfo);
        //     ChangeSpriteSheet(spriteSheets.Count - 1);
        // }

        #endregion
    }
}