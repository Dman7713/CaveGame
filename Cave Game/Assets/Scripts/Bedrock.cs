using UnityEngine;
using UnityEngine.Tilemaps;

public class OutlineBoxGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public Tilemap tilemap; // Reference to the main Tilemap
    public RuleTile outlineTile; // The rule tile for the outline in the main Tilemap

    [Header("Box Dimensions")]
    public int width = 5; // Width of the box
    public int height = 5; // Height of the box

    [Header("Border Settings")]
    public int borderWidth = 1; // Width of the border
    [Range(0f, 1f)]
    public float noiseThresholdCenter = 0.2f; // Threshold for the center (dense area)
    [Range(0f, 1f)]
    public float noiseThresholdEdge = 0.8f; // Threshold for the edges (less dense)

    [Header("Noise Settings")]
    public int numberOfNoiseLayers = 3; // Number of noise layers to combine
    public float noiseLayerStrength = 0.5f; // Strength of the noise layers combined
    public float noiseScale = 0.1f; // Scaling for the noise, determines how "tight" or "spread out" the noise is

    [Header("Box Position")]
    public Vector3Int position = new Vector3Int(0, 0, 0); // Position to generate the box

    // Called to generate the box when button is pressed
    public void GenerateBox()
    {
        GenerateOutlineBox();
    }

    // Generate the outline box with the given width and height
    private void GenerateOutlineBox()
    {
        if (tilemap == null || outlineTile == null) return;

        tilemap.ClearAllTiles(); // Clear previous tiles in the main Tilemap
        Vector3Int bottomLeft = position;

        // Generate the box with noise applied only to the border area
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Check if the current tile is part of the border (within the border width)
                if (IsBorder(x, y))
                {
                    // Calculate the distance from the border
                    float distanceFromEdge = Mathf.Min(Mathf.Abs(x - 0), Mathf.Abs(x - width), Mathf.Abs(y - 0), Mathf.Abs(y - height));
                    float normalizedDistance = distanceFromEdge / borderWidth; // Normalize based on the border width

                    // Adjust noise threshold based on distance from the border's center
                    float noiseThreshold = Mathf.Lerp(noiseThresholdCenter, noiseThresholdEdge, normalizedDistance);

                    // Combine multiple layers of noise
                    float noiseValue = 0f;
                    for (int i = 0; i < numberOfNoiseLayers; i++)
                    {
                        float layerStrength = noiseLayerStrength * (i + 1);
                        float layerNoiseValue = Mathf.PerlinNoise((x + position.x + i * 1000) * noiseScale, (y + position.y + i * 1000) * noiseScale);
                        noiseValue += layerNoiseValue * layerStrength;
                    }

                    // Normalize combined noise to a range of 0-1
                    noiseValue = Mathf.Clamp01(noiseValue / numberOfNoiseLayers);

                    // If the noise value exceeds the threshold, set the tile on the main tilemap
                    if (noiseValue > noiseThreshold)
                    {
                        tilemap.SetTile(bottomLeft + new Vector3Int(x, y, 0), outlineTile);
                    }
                }
            }
        }
    }

    // Determines if a tile is part of the border
    private bool IsBorder(int x, int y)
    {
        return x < borderWidth || x >= width - borderWidth || y < borderWidth || y >= height - borderWidth;
    }
}
