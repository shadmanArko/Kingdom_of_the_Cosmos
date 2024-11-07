using System.Threading.Tasks;
using UnityEngine;

namespace zzz_TestScripts.AddressableSpriteLoaderTest
{
    public class SpriteManager : MonoBehaviour
    {
        [SerializeField] private SpriteContainer spriteContainer;

        public async Task<Sprite> LoadSprite(string spriteName)
        {
            return await spriteContainer.GetSprite(spriteName);
        }

        public void UnloadSprite(string spriteName)
        {
            spriteContainer.UnloadSprite(spriteName);
        }

        private void OnDisable()
        {
            spriteContainer.UnloadAllSprites();
        }
    }
}