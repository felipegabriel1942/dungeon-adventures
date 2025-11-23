using Godot;

public partial class Magic : Node2D
{
    
    [Signal]
    public delegate void MagicFinishedEventHandler();

    [Export]
    private PackedScene magicScene;

    private AnimatedSprite2D animatedSprite2D;

        public void Attack()
    {
        if (magicScene != null)
        {
           animatedSprite2D = magicScene.Instantiate<AnimatedSprite2D>();
           AddChild(animatedSprite2D);
           animatedSprite2D.Centered = false;
           animatedSprite2D.Offset = new Vector2(-8, -23);
           animatedSprite2D.Play("attack");
           animatedSprite2D.AnimationFinished += OnMagicAnimationFinished;
        }
    }

    private void OnMagicAnimationFinished()
    {
        EmitSignal(SignalName.MagicFinished);
        RemoveChild(animatedSprite2D);
    }

}
