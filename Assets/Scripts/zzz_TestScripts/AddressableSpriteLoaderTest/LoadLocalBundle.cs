using System.Collections;
using System.IO;
using UnityEngine;

namespace zzz_TestScripts.AddressableSpriteLoaderTest
{
    public class LoadLocalBundle : MonoBehaviour
    {
        public string bundlePath; // Full path to the .bundle file
        public Transform imageParent; // Parent to hold loaded images in the scene

        void Start()
        {
            // Example: Provide the absolute path to the .bundle file
            bundlePath = Path.Combine("/Users/shadman.arko/Desktop", "addressabletestspritepackagebundle_assets_all_0e14ff9c68da7a08f31dd875827f5d81.bundle");
        
            if (!File.Exists(bundlePath))
            {
                Debug.LogError("AssetBundle file not found at: " + bundlePath);
                return;
            }

            StartCoroutine(LoadBundle(bundlePath));
        }

        IEnumerator LoadBundle(string path)
        {
            // Load the AssetBundle
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            if (bundle == null)
            {
                Debug.LogError("Failed to load AssetBundle!");
                yield break;
            }

            // Load all Texture2D assets
            AssetBundleRequest request = bundle.LoadAllAssetsAsync<Texture2D>();
            yield return request;

            Texture2D[] textures = request.allAssets as Texture2D[];
            if (textures == null || textures.Length == 0)
            {
                Debug.LogError("No textures found in the AssetBundle.");
                bundle.Unload(false);
                yield break;
            }

            Debug.Log($"Loaded {textures.Length} textures from bundle.");

            // Display textures in the scene
            foreach (var texture in textures)
            {
                GameObject imageObject = new GameObject(texture.name);
                imageObject.transform.SetParent(imageParent, false);

                var renderer = imageObject.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }

            // Unload the bundle (assets remain in memory)
            bundle.Unload(false);
        }
    }
}