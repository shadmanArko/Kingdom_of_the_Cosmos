using System.IO;
using UnityEngine;

public class InspectAssetBundle : MonoBehaviour
{
    public string bundlePath; // Path to the AssetBundle file

    void Start()
    {
        // Provide the path to your AssetBundle
        bundlePath = Path.Combine("/Users/shadman.arko/Desktop", "addressabletestspritepackagebundle_assets_all_0e14ff9c68da7a08f31dd875827f5d81.bundle");
        
        if (!File.Exists(bundlePath))
        {
            Debug.LogError("AssetBundle file not found at: " + bundlePath);
            return;
        }

        // Load the AssetBundle
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return;
        }

        // Get all asset names
        string[] assetNames = bundle.GetAllAssetNames();
        Debug.Log($"Found {assetNames.Length} assets in the bundle:");

        foreach (string assetName in assetNames)
        {
            Debug.Log(assetName);
        }

        bundle.Unload(false); // Unload the bundle after inspection
    }
}