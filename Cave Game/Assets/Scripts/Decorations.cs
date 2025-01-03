using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class SpawnTileAboveBelowMultiple : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public Tilemap sourceTilemap; // The tilemap with the specific tiles
    public Tilemap targetTilemap; // The tilemap where the new tiles will be placed

    [Header("Tile Settings")]
    public TileBase[] specificTiles; // Array of specific tiles to check for
    public TileBase[] targetTiles;   // Array of target tiles to place

    [Header("Spawn Settings")]
    public int numberOfTilesToSpawn = 10; // Maximum number of tiles to spawn

    [ContextMenu("Spawn Above/Below Tiles")]
    public void SpawnAboveBelowTiles()
    {
        if (sourceTilemap == null || targetTilemap == null || specificTiles.Length != targetTiles.Length)
        {
            Debug.LogError("Please assign all required references and ensure arrays match in length.");
            return;
        }

        int spawnedCount = 0;

        BoundsInt bounds = sourceTilemap.cellBounds;

        // Iterate through the bounds of the source tilemap
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int currentPosition = new Vector3Int(x, y, 0);

                // Check if the tile at the current position matches any specific tile
                for (int i = 0; i < specificTiles.Length; i++)
                {
                    if (sourceTilemap.GetTile(currentPosition) == specificTiles[i])
                    {
                        // Define positions above and below
                        Vector3Int abovePosition = currentPosition + new Vector3Int(0, 1, 0);
                        Vector3Int belowPosition = currentPosition + new Vector3Int(0, -1, 0);

                        // Spawn above if possible
                        if (!targetTilemap.HasTile(abovePosition) && spawnedCount < numberOfTilesToSpawn)
                        {
                            targetTilemap.SetTile(abovePosition, targetTiles[i]);
                            spawnedCount++;

                            if (spawnedCount >= numberOfTilesToSpawn)
                                break;
                        }

                        // Spawn below if possible
                        if (!targetTilemap.HasTile(belowPosition) && spawnedCount < numberOfTilesToSpawn)
                        {
                            targetTilemap.SetTile(belowPosition, targetTiles[i]);
                            spawnedCount++;

                            if (spawnedCount >= numberOfTilesToSpawn)
                                break;
                        }
                    }
                }

                if (spawnedCount >= numberOfTilesToSpawn)
                    break;
            }

            if (spawnedCount >= numberOfTilesToSpawn)
                break;
        }

        Debug.Log($"Spawned {spawnedCount} tiles above or below the specific tiles.");
    }

    [ContextMenu("Remove Target Tiles")]
    public void RemoveTargetTiles()
    {
        if (targetTilemap == null || targetTiles.Length == 0)
        {
            Debug.LogError("Please assign the target tilemap and target tiles.");
            return;
        }

        BoundsInt bounds = targetTilemap.cellBounds;

        // Iterate through the bounds of the target tilemap
        for (int x = bounds.xMin; x <= bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y <= bounds.yMax; y++)
            {
                Vector3Int currentPosition = new Vector3Int(x, y, 0);

                // Check if the tile at the current position is one of the target tiles
                foreach (TileBase targetTile in targetTiles)
                {
                    if (targetTilemap.GetTile(currentPosition) == targetTile)
                    {
                        targetTilemap.SetTile(currentPosition, null);
                    }
                }
            }
        }

        Debug.Log("Removed all target tiles from the target tilemap.");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpawnTileAboveBelowMultiple))]
public class SpawnTileAboveBelowMultipleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpawnTileAboveBelowMultiple script = (SpawnTileAboveBelowMultiple)target;
        if (GUILayout.Button("Spawn Above/Below Tiles"))
        {
            script.SpawnAboveBelowTiles();
        }

        if (GUILayout.Button("Remove Target Tiles"))
        {
            script.RemoveTargetTiles();
        }
    }
}
#endif
