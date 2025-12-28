using Godot;

public partial class Cleric : Character
{

    [Export]
    private PackedScene[] Spells;

    private Magic magic;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        magic = GetNode<Magic>("%Magic");

        base._Ready();
    }

    public override async void Attack(Character target)
    {
        HasAttacked = true;
        animatedSprite2D.Play("attack");

        IsAttacking = true;

        animatedSprite2D.FlipH = GetMapPosition().X < target.GetMapPosition().X;

        await target.Heal(Dice.Roll());

        IsAttacking = false;
        animatedSprite2D.Play("idle");
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
