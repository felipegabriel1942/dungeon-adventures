using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class GridManager : Node
{
    [Export]
    private TileMapLayer tileMapLayer;

    private int cellSize = 16;
    private AStarGrid2D grid;
    private List<TileMapLayer> allLayers = new();

    public override void _Ready()
    {
        InitGrid();
    }

    private void InitGrid()
    {
        grid = new AStarGrid2D();
        grid.Region = tileMapLayer.GetUsedRect();
        grid.CellSize = new Vector2I(cellSize, cellSize);
        grid.DiagonalMode = AStarGrid2D.DiagonalModeEnum.Never;
        grid.DefaultComputeHeuristic = AStarGrid2D.Heuristic.Manhattan;
        grid.Update();

        allLayers = FlattenMapLayer(tileMapLayer).ToList();   

        SetWalkableCells();
    }

    private void SetWalkableCells()
    {
        foreach (var tileMapLayer in allLayers)
        {
            foreach (var cell in tileMapLayer.GetUsedCells())
            {
                grid.SetPointSolid(cell, !GetCellCustomData(cell, "is_walkable").Item2);
            }
        }
    }

    private (TileMapLayer, bool) GetCellCustomData(Vector2I tilePosition, string dataName)
    {
        foreach (var layer in allLayers)
        {
            var customData = layer.GetCellTileData(tilePosition);

            if (customData == null) continue;

            return (layer, (bool) customData.GetCustomData(dataName));
        }

        return (null, false);
    }

    public Vector2 GetMousePosition()
    {
        return tileMapLayer.GetGlobalMousePosition();
    }

    private IEnumerable<TileMapLayer> FlattenMapLayer(TileMapLayer layer) =>
        new[] { layer }.Concat(
            layer.GetChildren()
                .OfType<TileMapLayer>()
                .SelectMany(FlattenMapLayer) ?? Enumerable.Empty<TileMapLayer>()
        );
}
