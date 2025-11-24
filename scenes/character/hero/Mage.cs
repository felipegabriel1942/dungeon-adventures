using Game.Character;
using Godot;

public partial class Mage : Character
{
    
    private AnimatedSprite2D animatedSprite2D;
    private Magic magic;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        magic = GetNode<Magic>("%Magic");
        base._Ready();
    }

    public override void Attack()
    {
        animatedSprite2D.Play("attack");
        magic.Visible = true;
        isAttacking = true;
        magic.Attack();
        magic.MagicFinished += OnAttackFinished;
    }

    private void OnAttackFinished()
    {
        isAttacking = false;
        animatedSprite2D.Play("idle");
    }
}
