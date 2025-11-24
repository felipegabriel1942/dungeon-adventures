using Godot;
using Game.Character;

public partial class Warrior : Character
{
    private AnimatedSprite2D animatedSprite2D;
    private Weapon weapon;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        weapon = GetNode<Weapon>("%Weapon");
        base._Ready();
    }

    public override void Attack()
    {
        animatedSprite2D.Play("attack");
        weapon.Visible = true;
        isAttacking = true;
        weapon.Attack();
        weapon.AttackFinished += OnAttackFinished;
    }

    private void OnAttackFinished()
    {
        isAttacking = false;
        animatedSprite2D.Play("idle");
    }
}
