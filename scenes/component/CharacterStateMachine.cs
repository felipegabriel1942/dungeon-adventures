using Game.Enum;
using Godot;

namespace Game.Component;

public partial class CharacterStateMachine: Node
{

    [Signal]
    public delegate void StateChangedEventHandler(int newState);

    public CharacterState CurrentState { get; private set; } = CharacterState.IDLE;

    public void SetState(CharacterState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;

        EmitSignal(SignalName.StateChanged, (int)newState);
    }
}
