using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int width = 256; // Width of the grid
    public int height = 256; // Height of the grid
    public Texture2D gridTexture; // The texture to pass into the compute shader

    void Start()
    {
        gridTexture = new Texture2D(width, height, TextureFormat.R8, false); // R8 format for 1-channel grayscale

        // Fill the texture with walkable (white) and non-walkable (black) areas
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Example: Create random obstacles
                float value = Random.value > 0.9f ? 0f : 1f; // Randomly place obstacles
                gridTexture.SetPixel(x, y, new Color(value, value, value));
            }
        }

        gridTexture.Apply(); // Apply changes to the texture

        // Save the texture to a file (for testing/debugging)
        System.IO.File.WriteAllBytes(Application.dataPath + "/gridTexture.png", gridTexture.EncodeToPNG());
    }
}