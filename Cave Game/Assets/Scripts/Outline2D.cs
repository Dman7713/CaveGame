using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(LineRenderer))]
public class SpecificTileOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    public Color outlineColor = Color.black; // Outline color
    public float outlineThickness = 0.05f;  // Outline thickness

    private LineRenderer lineRenderer;
    private Tilemap tilemap;

    void OnValidate()
    {
        tilemap = GetComponent<Tilemap>();
        lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Configure the LineRenderer
        lineRenderer.startWidth = outlineThickness;
        lineRenderer.endWidth = outlineThickness;
        lineRenderer.loop = false; // Outline isn't a single loop
        lineRenderer.useWorldSpace = true; // Tilemap works in world space
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = outlineColor;
        lineRenderer.endColor = outlineColor;

        GenerateTileOutline();
    }

    void GenerateTileOutline()
    {
        if (tilemap == null) return;

        var positions = new System.Collections.Generic.List<Vector3>();

        // Iterate through active tiles in the Tilemap
        foreach (var cellPos in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(cellPos))
            {
                AddTileEdgesToOutline(cellPos, positions);
            }
        }

        // Assign all outline positions to the LineRenderer
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }

    void AddTileEdgesToOutline(Vector3Int cellPos, System.Collections.Generic.List<Vector3> positions)
    {
        Vector3 cellWorldPos = tilemap.CellToWorld(cellPos);
        Vector3 tileSize = tilemap.cellSize;

        // World-space coordinates for the edges of the tile
        Vector3 bottomLeft = cellWorldPos;
        Vector3 bottomRight = cellWorldPos + new Vector3(tileSize.x, 0, 0);
        Vector3 topLeft = cellWorldPos + new Vector3(0, tileSize.y, 0);
        Vector3 topRight = cellWorldPos + new Vector3(tileSize.x, tileSize.y, 0);

        // Check neighboring tiles and add only exposed edges
        if (!tilemap.HasTile(cellPos + Vector3Int.down)) // Bottom edge
        {
            positions.Add(bottomLeft);
            positions.Add(bottomRight);
        }
        if (!tilemap.HasTile(cellPos + Vector3Int.up)) // Top edge
        {
            positions.Add(topLeft);
            positions.Add(topRight);
        }
        if (!tilemap.HasTile(cellPos + Vector3Int.left)) // Left edge
        {
            positions.Add(bottomLeft);
            positions.Add(topLeft);
        }
        if (!tilemap.HasTile(cellPos + Vector3Int.right)) // Right edge
        {
            positions.Add(bottomRight);
            positions.Add(topRight);
        }
    }
}
