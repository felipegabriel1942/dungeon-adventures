using System.Linq;
using Game.Autoload;
using Godot;

public partial class Mage : Character
{
    
    [Export]
    private PackedScene[] Spells;

    private Magic magic;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        magic = GetNode<Magic>("%Magic");

        GameEvents.Instance.Connect(GameEvents.SignalName.ProjectileHitTarget, Callable.From<Character>(OnSpellHit));
        base._Ready();
    }

    private void OnSpellHit(Character target)
    {
        target.TakeDamage(CalculateDamage(target));
        IsAttacking = false;
        animatedSprite2D.Play("idle");
    }

    // public override void Attack(Character target)
    // {
    //     HasAttacked = true;
    //     animatedSprite2D.Play("attack");
        
    //     IsAttacking = true;
    //     var spell = Spells.First().Instantiate<Projectile>();

    //     animatedSprite2D.FlipH = GetMapPosition().X < target.GetMapPosition().X;

    //     spell.Initialize(GlobalPosition, target);
    //     AddChild(spell);
    // }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
    

}
