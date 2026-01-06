using Game.Enum;
using Godot;

namespace Game.Component;

public partial class CharacterAnimationComponent : Node
{

    [Signal]
    public delegate void AnimationCompletedEventHandler(StringName anim);

    [Export]
    private AnimatedSprite2D animatedSprite2D;

    [Export]
    private CharacterOrientation characterOrientation;

    public override void _Ready()
    {
        animatedSprite2D.AnimationFinished += OnAnimationFinished;
        characterOrientation.OrientationChanged += OnOrientationChange;
    }

    public void PlayForState(CharacterState newState, Direction direction)
    {
        switch (newState)
        {
            case CharacterState.IDLE:
                PlayIdle();
            break;
            case CharacterState.MOVING:
                PlayMove(direction);
            break;
            case CharacterState.HURT:
                PlayHurt();
            break;    
        }
    }

    private void OnOrientationChange(int direction)
    {
        PlayMove((Direction)direction);
    }

    private void PlayHurt()
    {
        animatedSprite2D.Play("hurt");
    }

    private void PlayMove(Direction direction)
    {
        if (direction.Equals(Direction.LEFT))
        {
            animatedSprite2D.Play("walk_side");
            animatedSprite2D.FlipH = true;
        }

        if (direction.Equals(Direction.RIGHT))
        {
            animatedSprite2D.Play("walk_side");
            animatedSprite2D.FlipH = false;
        }

        if (direction.Equals(Direction.UP))
        {
            animatedSprite2D.Play("walk_up");
        }

        if (direction.Equals(Direction.DOWN))
        {
            animatedSprite2D.Play("walk_down");
        }
    }

    private void PlayIdle()
    {
        animatedSprite2D.Play("idle");
    }

    private void OnAnimationFinished()
    {
        EmitSignal(SignalName.AnimationCompleted, animatedSprite2D.Animation);
    }

}
