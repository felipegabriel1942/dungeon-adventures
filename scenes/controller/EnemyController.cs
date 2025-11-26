using System.Linq;
using Godot;

namespace Game.Controller;

public partial class EnemyController : Node
{
    [Export]
    private GridManager gridManager;

    private Character selectedCharacter;

    public override void _Ready()
    {
        CallDeferred(nameof(Init));
    }

    private async void Init()
    {

        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

        // TODO: Essa logica devera ser feita pelo TurnManager
        var characters = gridManager.GetAllCharacters()
            .Where(c => c.Team.Equals(TeamType.Enemy))
            .ToList();

        selectedCharacter = characters.FirstOrDefault();

        var target = FindNearestTarget();

        var targetPos = GetReachableCellClosestToTarget(selectedCharacter.GlobalPosition, target.GlobalPosition);

        await gridManager.MoveCharacter(selectedCharacter, targetPos.GetValueOrDefault());

        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
    }

    private Character FindNearestTarget()
    {
        var targets = gridManager.GetAllCharacters()
            .Where(c => c.Team == TeamType.Hero)
            .ToList();

        if (!targets.Any())
        {  
           return null; 
        }        

        targets
            .Sort((a, b) =>
            {
                return a.Position
                    .DistanceTo(selectedCharacter.Position)
                    .CompareTo(b.Position.DistanceTo(selectedCharacter.Position));
            });

        return targets.First();
    }

    public Vector2? GetReachableCellClosestToTarget(Vector2 current, Vector2 target)
    {
        var currentPos = gridManager.LocalToMap(current);
        var targetPos = gridManager.LocalToMap(target);

        var path = gridManager.GetPathBetweenPoints(currentPos, targetPos)
            .Skip(1)
            .Select(gridManager.LocalToMap);

        var movableTiles = gridManager.GetMovableTiles(selectedCharacter).ToHashSet();

        var closest = path.LastOrDefault(movableTiles.Contains);

        return closest == default ? null : gridManager.MapToLocal(closest);   
    }
}
