using Godot;

namespace Game.Character;

public abstract partial class Character : Node
{
    
    protected bool isAttacking;

    public abstract void Attack();
}
