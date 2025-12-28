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

    public override void Attack(Character target)
    {
        HasAttacked = true;
        animatedSprite2D.Play("attack");
        weapon.Visible = true;
        IsAttacking = true;
        animatedSprite2D.FlipH = GetMapPosition().X < target.GetMapPosition().X;
        weapon.Attack();
        target.TakeDamage(CalculateDamage(target));
        weapon.AttackFinished += OnAttackFinished;
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
