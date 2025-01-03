using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class CaveGenerator : MonoBehaviour
{
    [Header("Cave Dimensions")]
    public int width = 100;
    public int height = 100;

    [Header("Noise Settings")]
    public float noiseScale = 0.1f;
    public int seed;
    public bool useRandomSeed = true;

    [Header("Threshold")]
    [Range(0, 1)]
    public float threshold = 0.5f;

    [Header("Biome Settings")]
    public float biomeNoiseScale = 0.05f;
    [Range(0, 1)]
    public float biomeThreshold = 0.7f;

    [Header("Tile Settings")]
    public Tilemap tilemap;  // Main tilemap for the cave
    public Tilemap outlineTilemap;  // Tilemap for the black outline
    public TileBase defaultTile;
    public TileBase[] biomeTiles;
    public TileBase blackTile;  // The black tile used for the outline
    public TileBase[] oreTiles; // Ore tiles to be placed in the cave

    private int[,] biomeMap;
    private bool[,] caveMap;

    public void GenerateCave()
    {
        caveMap = new bool[width, height];
        biomeMap = new int[width, height];

        if (useRandomSeed)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }

        GenerateNoise();
        AssignBiomes();
        ApplyTiles();
        ApplyOutline();
    }

    private void GenerateNoise()
    {
        System.Random random = new System.Random(seed);
        float offsetX = random.Next(-100000, 100000);
        float offsetY = random.Next(-100000, 100000);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float sampleX = (x + offsetX) * noiseScale;
                float sampleY = (y + offsetY) * noiseScale;
                float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
                caveMap[x, y] = noiseValue > threshold;
            }
        }
    }

    private void AssignBiomes()
    {
        System.Random random = new System.Random(seed);
        float offsetX = random.Next(-100000, 100000);
        float offsetY = random.Next(-100000, 100000);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveMap[x, y])
                {
                    float sampleX = (x + offsetX) * biomeNoiseScale;
                    float sampleY = (y + offsetY) * biomeNoiseScale;
                    float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

                    if (noiseValue > biomeThreshold)
                    {
                        biomeMap[x, y] = random.Next(biomeTiles.Length);
                    }
                    else
                    {
                        biomeMap[x, y] = -1; // Default stone tile
                    }

                    // Chance to place ore (use a random chance, modify as needed)
                    if (random.NextDouble() < 0.05f) // 5% chance to place ore
                    {
                        biomeMap[x, y] = -2; // Assign a special value for ore placement
                    }
                }
                else
                {
                    biomeMap[x, y] = -1; // No biome for empty space
                }
            }
        }
    }

    private void ApplyTiles()
    {
        if (tilemap == null || defaultTile == null || biomeTiles == null || biomeTiles.Length == 0)
        {
            Debug.LogError("Tilemap, default tile, or biome tiles are not assigned!");
            return;
        }

        tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                if (caveMap[x, y])
                {
                    int biomeIndex = biomeMap[x, y];
                    TileBase tile = (biomeIndex >= 0 && biomeIndex < biomeTiles.Length) ? biomeTiles[biomeIndex] : defaultTile;
                    if (biomeMap[x, y] == -2) // If it's ore
                    {
                        tile = oreTiles[Random.Range(0, oreTiles.Length)]; // Choose a random ore tile
                    }
                    tilemap.SetTile(position, tile);
                }
            }
        }
    }

    private void ApplyOutline()
    {
        if (outlineTilemap == null || blackTile == null)
        {
            Debug.LogError("Outline tilemap or black tile is not assigned!");
            return;
        }

        outlineTilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Check if the current tile is a cave tile and if it's on the edge
                if (caveMap[x, y])
                {
                    // Check if it's on the edge of the cave
                    bool isEdge = false;

                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (x + dx < 0 || x + dx >= width || y + dy < 0 || y + dy >= height || !caveMap[x + dx, y + dy])
                            {
                                isEdge = true;
                                break;
                            }
                        }

                        if (isEdge) break;
                    }

                    if (isEdge)
                    {
                        Vector3Int position = new Vector3Int(x, y, 0);
                        outlineTilemap.SetTile(position, blackTile);
                    }
                }
            }
        }
    }
}

[CustomEditor(typeof(CaveGenerator))]
public class CaveGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CaveGenerator caveGen = (CaveGenerator)target;

        if (GUILayout.Button("Generate Cave"))
        {
            caveGen.GenerateCave();
        }
    }
}
