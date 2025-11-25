using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Game.Character;

public abstract partial class Character : Node2D
{
    [Export]
    public int Speed = 1;

    protected AnimatedSprite2D animatedSprite2D;
    public bool IsAttacking;
    public bool HasMoved;
    

    public override void _Ready()
    {
        AddToGroup("characters");
    }

    public abstract void Attack();

    public override void _PhysicsProcess(double delta)
    {
        if (HasMoved)
        {
            animatedSprite2D.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
        } else
        {
             animatedSprite2D.Modulate = new Color(1f, 1f, 1f, 1);
        }
    }

    public async Task Move(List<Vector2> path)
    {
        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        foreach (var cell in path)
        {
            tween.TweenProperty(this, "position", cell, 0.2);
        }

        await ToSignal(tween, "finished");

        tween.Dispose();

        HasMoved = true;
    }

}
