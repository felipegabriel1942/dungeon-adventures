using System.Collections.Generic;
using System.Linq;
using Game.Character;
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

        CallDeferred(nameof(SetOccupiedCells));
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

    private void SetOccupiedCells()
    {
        foreach (var character in GetAllCharacters())
        {
            var cell = ToVector2I(character.GlobalPosition);
            grid.SetPointSolid(cell, true);
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

    public Character GetCharacterAtMousePosition()
    {    
        Vector2 mousePosition = GetMousePosition();
        Vector2I mouseCell = ToVector2I(mousePosition);
        return GetCharacterAtCell(mouseCell);
    }

    public Vector2 GetMousePosition()
    {
        return tileMapLayer.GetGlobalMousePosition();
    }

    public Vector2I ToVector2I(Vector2 pos)
    {
        return new Vector2I(
            Mathf.FloorToInt(pos.X / cellSize),
            Mathf.FloorToInt(pos.Y / cellSize)
        );   
    }

    public Character GetCharacterAtCell(Vector2I cell)
    {
        return GetAllCharacters().FirstOrDefault(c => ToVector2I(c.Position) == cell);
    }

    private IEnumerable<TileMapLayer> FlattenMapLayer(TileMapLayer layer) =>
        new[] { layer }.Concat(
            layer.GetChildren()
                .OfType<TileMapLayer>()
                .SelectMany(FlattenMapLayer) ?? Enumerable.Empty<TileMapLayer>()
        );

    public List<Character> GetAllCharacters() => GetTree()
        .GetNodesInGroup("characters")
        .Cast<Character>()
        .ToList();
}
