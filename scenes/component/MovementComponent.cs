using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Enum;
using Godot;

public partial class MovementComponent : Node
{
    private Character character;

    public override void _Ready()
    {
        character = GetParent<Character>();
    }

    public async Task MoveAlongPath(List<Vector2> path)
    {
        foreach (var cell in path)
        {
            Vector2 start = character.GlobalPosition;
            Vector2 end = cell;

            Vector2 delta = end - start;

            UpdateDirectionFromDelta(delta);

            var tween = CreateTween();
            tween.TweenProperty(character, "global_position", cell, 0.4);

            await ToSignal(tween, "finished");
            tween.Dispose();
        }
    }

    private void UpdateDirectionFromDelta(Vector2 delta)
    {
         if (delta == Vector2.Zero) return;

        if (Mathf.Abs(delta.X) > Mathf.Abs(delta.Y))
            character.UpdateFacingDirection(delta.X > 0 ? Direction.RIGHT : Direction.LEFT);
        else
            character.UpdateFacingDirection(delta.Y > 0 ? Direction.DOWN : Direction.UP);
    }
}
