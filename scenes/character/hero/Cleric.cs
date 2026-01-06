using Godot;

public partial class Cleric : Character
{

    [Export]
    private PackedScene[] Spells;

    private Magic magic;

    public override void _Ready()
    {
        // animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        magic = GetNode<Magic>("%Magic");

        base._Ready();
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
