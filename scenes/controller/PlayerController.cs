using Game.Autoload;
using Game.Enum;
using Godot;

namespace Game.Controller;

public partial class PlayerController : Node
{

    private readonly StringName ACTION_LEFT_CLICK = "select";

    [Export]
    private GridManager gridManager;

    private Character selectedCharacter;

    private Character hoveredCharacter;

    private PlayerStates currentState;

    public override void _Ready()
    {
        GameEvents.Instance.Connect(GameEvents.SignalName.EndTurn, Callable.From(OnCharacterTurnEnded));
    }

    public override async void _UnhandledInput(InputEvent evt)
    {
        switch(currentState)
        {
            case PlayerStates.IDLE:
                if (evt.IsActionPressed(ACTION_LEFT_CLICK))
                {
                     GameEvents.EmitCharacterSelectedOnGrid(hoveredCharacter);

                    if (hoveredCharacter != null && hoveredCharacter.resource.Team.Equals(TeamType.Hero) && hoveredCharacter.IsMyTurn)
                    {
                        ChangeState(PlayerStates.SELECT_MOVE);
                        selectedCharacter = hoveredCharacter;
                    }
                }
                break;
            case PlayerStates.SELECT_MOVE:
                if (evt.IsActionPressed(ACTION_LEFT_CLICK))
                {
                    if (gridManager.CanMoveToTargetPosition(selectedCharacter, gridManager.GetMousePosition()))
                    {
                        MoveCharacter();
                    }
                }
                break;
            case PlayerStates.MOVING:
                break;
            default:
                break;
        }
    }

    public override void _Process(double delta)
    {
        switch (currentState)
        {
            case PlayerStates.IDLE:
                hoveredCharacter = gridManager.GetCharacterAtCell(gridManager.GetMouseGridCellPosition());
                break;
            case PlayerStates.SELECT_MOVE:
                gridManager.HighlightMovableCells(selectedCharacter);
                break;
            case PlayerStates.END_TURN:
                selectedCharacter = null;
                break;
        }
    }

    private async void MoveCharacter()
    {
        ChangeState(PlayerStates.MOVING);

        await gridManager.MoveCharacter(selectedCharacter, gridManager.GetMousePosition());
    
        ChangeState(PlayerStates.END_TURN);
    }

    private void OnCharacterTurnEnded()
    {
        gridManager.ClearHighlights();
        ChangeState(PlayerStates.IDLE);
    }

    private void ChangeState(PlayerStates state)
    {
        currentState = state;

        GameEvents.EmitPlayerStateChanged((int) currentState);
    }

}
