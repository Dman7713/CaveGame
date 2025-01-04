using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor; // Required for the Button attribute
using System.Collections.Generic;

public class PlaceTopAndBottomTiles : MonoBehaviour
{
    public Tilemap tilemap;          // Reference to the Tilemap
    public TileAtlas[] tileAtlases;  // An array of TileAtlases for each biome

    [System.Serializable]
    public class TileAtlas
    {
        public string biomeName;      // Name of the biome
        public TileBase targetTile;   // The specific tile to check for
        public TileBase bottomTile;      // The tile to place above the targetTile
        public TileBase topTile;   // The tile to place below the targetTile
    }

    // This method will be triggered by a button in the Inspector
    [ContextMenu("Place Top and Bottom Tiles")]
    public void PlaceTilesAboveAndBelow()
    {
        // Get the bounds of the tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Iterate through all tiles in the bounds
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);
                TileBase currentTile = tilemap.GetTile(position);

                // Iterate through each TileAtlas to find matching tiles for the current biome
                foreach (var atlas in tileAtlases)
                {
                    // Check if the current tile matches the targetTile for the current biome
                    if (currentTile == atlas.targetTile)
                    {
                        // Place the top tile above the targetTile
                        PlaceTileIfEmpty(new Vector3Int(x, y + 1, 0), atlas.bottomTile); // Above
                        // Place the bottom tile below the targetTile
                        PlaceTileIfEmpty(new Vector3Int(x, y - 1, 0), atlas.topTile); // Below
                    }
                }
            }
        }
    }

    void PlaceTileIfEmpty(Vector3Int position, TileBase tile)
    {
        // Only place the tile if the position is empty
        if (tilemap.GetTile(position) == null)
        {
            tilemap.SetTile(position, tile);
        }
    }
}
