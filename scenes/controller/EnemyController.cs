using System.Linq;
using Game.Autoload;
using Godot;

namespace Game.Controller;

public partial class EnemyController : Node
{
    [Export]
    private GridManager gridManager;

    public override void _Ready()
    {
        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(ExecuteTurn));
    }

    private async void ExecuteTurn(Character character)
    {
        if (TeamType.Enemy.Equals(character.resource.Team)) {
            
            await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

            var target = FindNearestTarget(character);

            if (gridManager.IsTargetInAttackArea(character, target))
            {
                await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

                // TODO: Implementar logica de combate aqui!!!!!!
                character.Attack();
                GD.Print("Atacou o " + target.ToString());
            } else
            {
                var targetPos = GetReachableCellClosestToTarget(character, target.GlobalPosition);

                await gridManager.MoveCharacter(character, targetPos.GetValueOrDefault());
            }

            await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
        }
    }

    // TODO: Melhorar a logica do alvo, colocando condições, por exemplo, alvo com menos pontos de vida, tipo
    // preferido de alvo e etc.
    private Character FindNearestTarget(Character character)
    {
        var targets = gridManager.GetAllCharacters()
            .Where(c => c.resource.Team == TeamType.Hero)
            .ToList();

        if (!targets.Any())
        {  
           return null; 
        }        

        targets
            .Sort((a, b) =>
            {
                return a.Position
                    .DistanceTo(character.Position)
                    .CompareTo(b.Position.DistanceTo(character.Position));
            });

        return targets.First();
    }

    public Vector2? GetReachableCellClosestToTarget(Character character, Vector2 target)
    {
        var currentPos = gridManager.LocalToMap(character.GlobalPosition);
        var targetPos = gridManager.LocalToMap(target);

        var path = gridManager.GetPathBetweenPoints(currentPos, targetPos)
            .Skip(1)
            .Select(gridManager.LocalToMap);

        var movableTiles = gridManager.GetMovableTiles(character).ToHashSet();

        var closest = path.LastOrDefault(movableTiles.Contains);

        return closest == default ? null : gridManager.MapToLocal(closest);   
    }
}
