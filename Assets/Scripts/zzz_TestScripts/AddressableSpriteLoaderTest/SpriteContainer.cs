using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[CreateAssetMenu(fileName = "SpriteContainer", menuName = "Game/Sprite Container")]
public class SpriteContainer : ScriptableObject
{
    // Reference to the main sprite sheet texture
    [SerializeField] private AssetReference spriteSheetReference;

    // Store the names of individual sprites you want to load
    [SerializeField] private List<string> spriteNames = new List<string>();

    private Dictionary<string, Sprite> _loadedSprites = new Dictionary<string, Sprite>();
    private bool _isSpriteSheetLoaded = false;
    private IList<Sprite> _spriteSheet;

    public async Task<Sprite> GetSprite(string spriteName)
    {
        if (_loadedSprites.ContainsKey(spriteName))
            return _loadedSprites[spriteName];

        try
        {
            // Load the entire sprite sheet if not already loaded
            if (!_isSpriteSheetLoaded)
            {
                _spriteSheet = await spriteSheetReference.LoadAssetAsync<IList<Sprite>>().Task;
                _isSpriteSheetLoaded = true;
            }
        
            // Find the specific sprite by name
            var sprite = _spriteSheet.FirstOrDefault(s => s.name == spriteName);
            if (sprite != null)
            {
                _loadedSprites[spriteName] = sprite;
                return sprite;
            }
        
            Debug.LogError($"Sprite {spriteName} not found in sprite sheet");
            return null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load sprite: {e.Message}");
            return null;
        }
    }

    // Unload a specific sprite
    public void UnloadSprite(string spriteName)
    {
        if (_loadedSprites.ContainsKey(spriteName))
        {
            _loadedSprites.Remove(spriteName);
        }
    }

    // Unload all sprites
    public void UnloadAllSprites()
    {
        _loadedSprites.Clear();
        
        if (_isSpriteSheetLoaded)
        {
            spriteSheetReference.ReleaseAsset();
            _isSpriteSheetLoaded = false;
            _spriteSheet = null;
        }
    }

    private void OnDisable()
    {
        UnloadAllSprites();
    }
}