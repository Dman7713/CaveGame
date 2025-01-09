using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode] // Allows script to run in the Editor without entering Play mode
public class TilePlacer : MonoBehaviour
{
    [Header("Tilemap References")]
    public Tilemap targetTilemap; // The Tilemap to work with
    public Tilemap backgroundTilemap; // The Tilemap to place the background tiles on

    [Header("Tile Placement Settings")]
    public RuleTile tileToPlaceBehind; // The RuleTile to place behind
    public TileBase[] targetTilesToCheck; // Tiles to check for on the target Tilemap

    [ContextMenu("Place Tiles Behind")]
    public void PlaceTileBehind()
    {
        if (targetTilemap == null || backgroundTilemap == null || tileToPlaceBehind == null)
        {
            Debug.LogWarning("Make sure all references are assigned before using this function.");
            return;
        }

        // Clear all tiles from the background Tilemap before placing new ones
        ClearBackgroundTiles();

        // Iterate over every position in the target Tilemap
        BoundsInt bounds = targetTilemap.cellBounds;
        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            // Check if the current position has a tile
            if (targetTilemap.HasTile(position))
            {
                TileBase currentTile = targetTilemap.GetTile(position);

                // Check if the current tile is in the targetTilesToCheck array
                foreach (TileBase tile in targetTilesToCheck)
                {
                    if (currentTile == tile)
                    {
                        // Place the designated RuleTile behind the current tile on the background Tilemap
                        backgroundTilemap.SetTile(position, tileToPlaceBehind);
                        break; // Exit the loop once a match is found
                    }
                }
            }
        }

        Debug.Log("Tiles placed behind successfully!");
    }

    private void ClearBackgroundTiles()
    {
        // Clear all tiles from the background Tilemap
        backgroundTilemap.ClearAllTiles();
        Debug.Log("Background tiles cleared!");
    }
}
