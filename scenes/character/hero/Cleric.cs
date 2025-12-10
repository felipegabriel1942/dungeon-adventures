using Godot;

public partial class Cleric : Character
{
    private Magic magic;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        magic = GetNode<Magic>("%Magic");
        base._Ready();
    }

    public override void Attack(Character target)
    {
        animatedSprite2D.Play("attack");
        magic.Visible = true;
        IsAttacking = true;
        magic.Attack();
        magic.MagicFinished += OnAttackFinished;
    }

    private void OnAttackFinished()
    {
        IsAttacking = false;
        animatedSprite2D.Play("idle");
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
