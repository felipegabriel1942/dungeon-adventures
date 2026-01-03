using Game.Enum;

namespace Game.Scripts;

public class CharacterStateMachine
{
    public CharacterState CurrentState { get; private set; } = CharacterState.IDLE;

    public bool CanMove() => CurrentState == CharacterState.IDLE;
    public bool CanAttack() => CurrentState == CharacterState.IDLE;

    public void SetState(CharacterState state)
    {
        CurrentState = state;
    }
}
