using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class CaveGenerator : MonoBehaviour
{
    [Header("Cave Dimensions")]
    public int width = 100, height = 100;

    [Header("Noise Settings")]
    public float noiseScale = 0.1f, biomeNoiseScale = 0.05f;
    public int seed;
    public bool useRandomSeed = true;

    [Header("Threshold")]
    [Range(0, 1)] public float threshold = 0.5f, biomeThreshold = 0.7f;

    [Header("Tile Settings")]
    public Tilemap tilemap;
    public TileBase defaultTile;
    public TileBase[] biomeTiles;
    public TileBase[] oreTiles;

    private int[,] biomeMap;
    private bool[,] caveMap;

    public void GenerateCave()
    {
        if (!Application.isPlaying)
        {
            caveMap = new bool[width, height];
            biomeMap = new int[width, height];
            seed = useRandomSeed ? Random.Range(int.MinValue, int.MaxValue) : seed;

            GenerateNoise();
            AssignBiomes();
            ApplyTiles();

#if UNITY_EDITOR
            EditorUtility.SetDirty(tilemap);
#endif
        }
        else
        {
            Debug.LogWarning("Cave generation is disabled during play mode to preserve tiles.");
        }
    }

    private void GenerateNoise()
    {
        var random = new System.Random(seed);
        float offsetX = random.Next(-100000, 100000), offsetY = random.Next(-100000, 100000);

        int densityRadius = 15;
        float densityStrength = 0.6f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = 0f;
                float[] noiseScales = new float[] { noiseScale, noiseScale * 2, noiseScale * 0.5f };
                float[] noiseWeights = new float[] { 0.5f, 0.3f, 0.2f };

                for (int i = 0; i < noiseScales.Length; i++)
                {
                    noiseValue += Mathf.PerlinNoise((x + offsetX) * noiseScales[i], (y + offsetY) * noiseScales[i]) * noiseWeights[i];
                }

                noiseValue += (float)random.NextDouble() * 0.1f - 0.05f;

                bool isInDenseSpot = false;
                if (random.NextDouble() < 0.1f)
                {
                    int centerX = random.Next(0, width);
                    int centerY = random.Next(0, height);

                    float distance = Mathf.Sqrt(Mathf.Pow(x - centerX, 2) + Mathf.Pow(y - centerY, 2));
                    if (distance < densityRadius)
                    {
                        isInDenseSpot = true;
                        noiseValue += densityStrength;
                    }
                }

                caveMap[x, y] = noiseValue > threshold || isInDenseSpot;
            }
        }
    }

    private void AssignBiomes()
    {
        var random = new System.Random(seed);
        float offsetX = random.Next(-100000, 100000), offsetY = random.Next(-100000, 100000);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (caveMap[x, y])
                {
                    float noiseValue = Mathf.PerlinNoise((x + offsetX) * biomeNoiseScale, (y + offsetY) * biomeNoiseScale);
                    biomeMap[x, y] = noiseValue > biomeThreshold ? random.Next(biomeTiles.Length) : -1;

                    if (random.NextDouble() < 0.05f)
                        biomeMap[x, y] = -2;
                }
                else
                {
                    biomeMap[x, y] = -1;
                }
            }
        }
    }

    private void ApplyTiles()
    {
        if (tilemap == null || defaultTile == null || biomeTiles.Length == 0)
        {
            Debug.LogError("Tilemap or tiles are not assigned!");
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
                    TileBase tile = biomeMap[x, y] == -2 ? oreTiles[Random.Range(0, oreTiles.Length)] : biomeMap[x, y] >= 0 ? biomeTiles[biomeMap[x, y]] : defaultTile;
                    tilemap.SetTile(position, tile);
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
        if (GUILayout.Button("Generate Cave"))
        {
            ((CaveGenerator)target).GenerateCave();
        }
    }
}
