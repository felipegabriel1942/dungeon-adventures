using Godot;

public partial class Warrior : Character
{
    private Weapon weapon;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        weapon = GetNode<Weapon>("%Weapon");
        base._Ready();
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }

}
