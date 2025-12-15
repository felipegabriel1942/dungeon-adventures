using Godot;

public partial class Magic : Node2D
{
    
    [Signal]
    public delegate void MagicFinishedEventHandler();

    [Export]
    private PackedScene ProjectileScene;

    private Projectile projectile;

    public void Attack()
    {
        if (ProjectileScene != null)
        {
           projectile = ProjectileScene.Instantiate<Projectile>();
           AddChild(projectile);
        }
    }

    private void OnMagicAnimationFinished()
    {
        // EmitSignal(SignalName.MagicFinished);
        // RemoveChild(projectile);
    }

}
