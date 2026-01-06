using Game.Autoload;
using Godot;

public partial class Skeleton : Character
{
    public override void _Ready()
    {
        // animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        
        base._Ready();
    }

    protected override void Die()
    {
        GD.Print($"{this.resource.DisplayName} dies.");
        GameEvents.EmitCharacterDied(this);
        QueueFree();
    }
}
