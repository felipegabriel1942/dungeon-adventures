using Game.Autoload;
using Godot;

public partial class Skeleton : Character
{
    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        base._Ready();
    }

    public override void Attack()
    {
        animatedSprite2D.Play("attack");
        animatedSprite2D.AnimationFinished += OnAttackFinished;
        IsAttacking = true;
        IsMyTurn = false; 
        GameEvents.EmitEndTurn();   
    }

    private void OnAttackFinished()
    {
        IsAttacking = false;
        animatedSprite2D.Play("idle");
    }

}
