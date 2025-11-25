using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game.Character;
using Godot;

public partial class GridManager : Node
{
    [Export]
    private TileMapLayer tileMapLayer;

    private int cellSize = 16;
    private AStarGrid2D grid;
    private List<TileMapLayer> allLayers = new();
    private Node2D highlightLayer;

    public override void _Ready()
    {
        highlightLayer = GetNode<Node2D>("%HighlightLayer");

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
            var cell = tileMapLayer.LocalToMap(character.GlobalPosition);
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
        ).Reverse();

    public List<Character> GetAllCharacters() => GetTree()
        .GetNodesInGroup("characters")
        .Cast<Character>()
        .ToList();

    public async Task MoveCharacter(Character character, Vector2 targetPos)
    {
        if (!CanMove(character, targetPos))
        {
            return;
        }

        var currentCell = tileMapLayer.LocalToMap(character.GlobalPosition);
        var targetCell = tileMapLayer.LocalToMap(targetPos);

        var path = grid.GetPointPath(currentCell, targetCell, true);

        grid.SetPointSolid(currentCell, false);
        grid.SetPointSolid(targetCell, true);

        await character.Move(path.Skip(1).ToList());
    }

    private bool CanMove(Character character, Vector2 targetPos)
    {
        return !character.HasMoved && GetMovableTiles(character)
            .Contains(tileMapLayer.LocalToMap(GetMousePosition()));
    }


    public bool CanMoveToSelectedCell(Character character, Vector2 targetPosition)
    {
        return GetMovableTiles(character).Contains(tileMapLayer.LocalToMap(targetPosition));
    }

    public List<Vector2I> GetMovableTiles(Character character)
    {
        Vector2I startCell = tileMapLayer.LocalToMap(character.GlobalPosition);

        Queue<Vector2I> toCheck = new();
        List<Vector2I> result = new();

        toCheck.Enqueue(startCell);
        result.Add(startCell);

        if (!character.HasMoved)
        {
            while(toCheck.Count > 0)
            {
                var current = toCheck.Dequeue();

                foreach(var direction in new Vector2I[] { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right })
                {
                    Vector2I next = current + direction;

                    int distance = GetManhattanDistance(startCell, next);

                    if (!result.Contains(next) && !grid.IsPointSolid(next) && grid.Region.HasPoint(next))
                    {

                        if (distance <= character.Speed)
                        {
                            result.Add(next);
                            toCheck.Enqueue(next);
                        }
                    } 
                }
            }
        }

        result.Remove(startCell);

        return result;
    }

    private int GetManhattanDistance(Vector2I a, Vector2I b)
    {
        return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
    }

    public void HighlightMovableCells(Character character)
    {

        ClearHighlights();

        List<Vector2I> movableCells = GetMovableTiles(character);

        foreach (var cell in movableCells)
        {
            var rect = new ColorRect
            {
                Color = new Color(0, 0, 1, 0.25f),
                Size = new Vector2(cellSize, cellSize),
                Position = tileMapLayer.MapToLocal(cell) - new Vector2(cellSize / 2, cellSize / 2),
                ZIndex = 2,
            };

            rect.MouseFilter = Control.MouseFilterEnum.Ignore;

            highlightLayer.AddChild(rect);
        }
    }

    public void ClearHighlights()
    {
        highlightLayer.QueueFree();
        highlightLayer = new Node2D();
        AddChild(highlightLayer);
    }
}
