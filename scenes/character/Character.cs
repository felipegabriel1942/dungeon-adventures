using Godot;

namespace Game.Character;

public abstract partial class Character : Node2D
{

    public override void _Ready()
    {
        AddToGroup("characters");
    }

    protected bool isAttacking;

    public abstract void Attack();
}
