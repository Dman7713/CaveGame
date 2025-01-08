using UnityEngine;
using UnityEngine.Tilemaps;

public class TilePlacer : MonoBehaviour
{
    public Tilemap targetTilemap; // The Tilemap to work with
    public Tilemap backgroundTilemap; // The Tilemap to place the background tiles on
    public RuleTile tileToPlaceBehind; // The RuleTile you want to place behind

    public TileBase[] targetTilesToCheck; // Tiles to check for on the targetTilemap

    private void Start()
    {
        if (targetTilemap != null && backgroundTilemap != null && tileToPlaceBehind != null)
        {
            PlaceTileBehind();
        }
    }

    void PlaceTileBehind()
    {
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
                    }
                }
            }
        }
    }
}
