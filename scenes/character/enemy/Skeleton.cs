using Game.Character;
using Godot;

public partial class Skeleton : Character
{
    private AnimatedSprite2D animatedSprite2D;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        base._Ready();
    }

    public override void Attack()
    {
        animatedSprite2D.Play("attack");
        animatedSprite2D.AnimationFinished += OnAttackFinished;
        isAttacking = true;
    }

    private void OnAttackFinished()
    {
        isAttacking = false;
        animatedSprite2D.Play("idle");
    }

}
