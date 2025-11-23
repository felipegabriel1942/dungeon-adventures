using Godot;

public partial class Weapon : Node2D
{
    [Signal]
    public delegate void AttackFinishedEventHandler();

    [Export]
    private PackedScene weaponScene;

    private AnimatedSprite2D animatedSprite2D;

    public void Attack()
    {
        if (weaponScene != null)
        {
           animatedSprite2D = weaponScene.Instantiate<AnimatedSprite2D>();
           AddChild(animatedSprite2D);
           animatedSprite2D.Centered = false;
           animatedSprite2D.Offset = new Vector2(-8, -23);
           animatedSprite2D.Play("attack");
           animatedSprite2D.AnimationFinished += OnWeaponAnimationFinished;
        }
    }

    private void OnWeaponAnimationFinished()
    {
        EmitSignal(SignalName.AttackFinished);
        RemoveChild(animatedSprite2D);
    }

}

