using Game.Autoload;
using Godot;

public partial class Skeleton : Character
{
    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.AnimationFinished += OnAnimationFinished;
        base._Ready();
    }

    public override void Attack()
    {
        animatedSprite2D.Play("attack");
        IsAttacking = true;
        IsMyTurn = false; 
        GameEvents.EmitEndTurn();   
    }

    private void OnAnimationFinished()
    {
        if (animatedSprite2D.Animation == "attack")
        {
            OnAttackFinished();
        }
    }

    private void OnAttackFinished()
    {
        IsAttacking = false;
        animatedSprite2D.Play("idle");
    }

}
