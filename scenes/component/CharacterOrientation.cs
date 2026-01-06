using Game.Enum;
using Godot;

public partial class CharacterOrientation : Node
{
    [Signal]
    public delegate void OrientationChangedEventHandler(int orientation);

    public Direction CurrentDirection { get; private set; }

    public void SetDirection(Direction newDirection)
    {
        if (CurrentDirection == newDirection) 
            return;

        CurrentDirection = newDirection;

        EmitSignal(SignalName.OrientationChanged, (int)newDirection);
    }
}
