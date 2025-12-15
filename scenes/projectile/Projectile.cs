using Game.Autoload;
using Godot;

public partial class Projectile : Area2D
{
    
    [Export]
    public float Speed = 100.0f;

    private AnimatedSprite2D animatedSprite2D;

    public Vector2 Direction {get; private set; }

    private Character target;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        animatedSprite2D.FlipH = true;

        AreaEntered += OnAreaEntered;

        animatedSprite2D.AnimationFinished += OnAnimationFinished;
    }


    public void Initialize(Vector2 from, Character target)
    {
        Direction = (target.Position - from).Normalized();
        this.target = target;
               
    }

    private void OnAnimationFinished()
    {
        if (animatedSprite2D.Animation == "explosion")
        {   
            GameEvents.EmitProjectileHitTarget(target);
            QueueFree();
        }
    }

    private void OnAreaEntered(Area2D area)
    {
        animatedSprite2D.Play("explosion");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (animatedSprite2D.Animation != "explosion")
        {
            Vector2 movement = Direction * Speed * (float) delta;
            animatedSprite2D.FlipH = Direction.X == 1;
            Position += movement;
        }
    }
}
