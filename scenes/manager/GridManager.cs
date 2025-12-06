using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        Vector2I mouseCell = tileMapLayer.LocalToMap(mousePosition);
        return GetCharacterAtCell(mouseCell);
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
        var currentCell = tileMapLayer.LocalToMap(character.GlobalPosition);
        var targetCell = tileMapLayer.LocalToMap(targetPos);

        var path = grid.GetPointPath(currentCell, targetCell, true);

        grid.SetPointSolid(currentCell, false);
        grid.SetPointSolid(targetCell, true);

        ClearHighlights();
        await character.Move(path.Skip(1).ToList());
    }

    public bool CanMove(Character character, Vector2 targetPos)
    {
        return GetMovableTiles(character).Contains(tileMapLayer.LocalToMap(targetPos));
    }

    public bool CanMoveToSelectedCell(Character character, Vector2 targetPosition)
    {
        return GetMovableTiles(character).Contains(tileMapLayer.LocalToMap(targetPosition));
    }

    public List<Vector2I> GetMovableTiles(Character character)
    {
        var start = tileMapLayer.LocalToMap(character.GlobalPosition);
        var reachable = new List<Vector2I>();
        var visited = new HashSet<Vector2I>();

        var toVisit = new Queue<Vector2I>();
        toVisit.Enqueue(start);
        visited.Add(start);

        // if (character.HasMoved)
        // {
        //     return new List<Vector2I>();
        // }

        while (toVisit.Count > 0)
        {
            var current = toVisit.Dequeue();

            foreach (var direction in Directions4)
            {
                var next = current + direction;

                if (IsValidMovableTile(next, start, character.resource.Speed, visited))
                {
                    visited.Add(next);
                    reachable.Add(next);
                    toVisit.Enqueue(next);
                }
            }
        }

        return reachable;
    }

    private bool IsValidMovableTile(Vector2I tile, Vector2I start, int maxDistance, HashSet<Vector2I> visited)
    {
        if (visited.Contains(tile))
            return false;

        if (!grid.Region.HasPoint(tile))
            return false;

        if (grid.IsPointSolid(tile))
            return false;

        if (GetManhattanDistance(start, tile) > maxDistance)
            return false;

        return true;
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
                ZIndex = 1,
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

    public bool IsTargetInAttackArea(Character attacker, Character target)
    {
        return GetCellsInCharacterAttackRange(attacker).Contains(tileMapLayer.LocalToMap(target.Position));
    }

    private List<Vector2I> GetCellsInCharacterAttackRange(Character character)
    {
        var cells = GetCellsInRange(tileMapLayer.LocalToMap(character.GlobalPosition), character.resource.AttackRange);
        cells.RemoveAll(t => GetCharacterAtCell(t) == null || GetCharacterAtCell(t).resource.Team.Equals(character.resource.Team));
        return cells;
    }

    private List<Vector2I> GetCellsInRange(Vector2I center, int range)
    {
        return (
            from dx in Enumerable.Range(-range, range * 2 + 1)
            from dy in Enumerable.Range(-range, range * 2 +1)
            where Mathf.Abs(dx) + Mathf.Abs(dy) <= range
            select center + new Vector2I(dx, dy)
        ).ToList();
    }

    public Vector2I LocalToMap(Vector2 position) => tileMapLayer.LocalToMap(position);
    
    public Vector2 MapToLocal(Vector2I position) => tileMapLayer.MapToLocal(position);
    
    public Vector2[] GetPathBetweenPoints(Vector2I current, Vector2I target) => grid.GetPointPath(current, target, true);

    public Character GetCharacterAtCell(Vector2I cell) => GetAllCharacters().FirstOrDefault(c => LocalToMap(c.Position) == cell);

    private int GetManhattanDistance(Vector2I a, Vector2I b) => Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);

    public Vector2 GetMousePosition() => tileMapLayer.GetGlobalMousePosition();

    private static readonly Vector2I[] Directions4 =
    {
        Vector2I.Up,
        Vector2I.Down,
        Vector2I.Left,
        Vector2I.Right
    };
}
