using Game.Autoload;
using Game.Enum;
using Godot;

namespace Game.Controller;

public partial class PlayerController : Node
{

    private readonly StringName ACTION_LEFT_CLICK = "select";

    private readonly StringName CANCEL_ACTION = "cancel";

    [Export]
    private GridManager gridManager;

    private Character characterOnTurn;

    private Character hoveredCharacter;

    private PlayerState currentState;

    public override void _Ready()
    {
        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(OnCharacterTurnBegin));
        GameEvents.Instance.Connect(GameEvents.SignalName.EndTurn, Callable.From(OnCharacterTurnEnded));
        GameEvents.Instance.Connect(GameEvents.SignalName.AttackButtonPressed, Callable.From(OnAttackButtonPressed));
        GameEvents.Instance.Connect(GameEvents.SignalName.MoveButtonPressed, Callable.From(OnMoveButtonPressed));
    }

    private void OnMoveButtonPressed()
    {
        if (!characterOnTurn.HasMoved)
        {
            ChangeState(PlayerState.SELECT_MOVE);
        }
    }

    private void OnCharacterTurnBegin(Character character)
    {
        ChangeState(PlayerState.IDLE);
        characterOnTurn = character;
    }

    private void OnAttackButtonPressed()
    {
        if (!characterOnTurn.HasAttacked)
        {
            ChangeState(PlayerState.SELECT_TARGET);
        }
    }

    public override async void _UnhandledInput(InputEvent evt)
    {
        switch(currentState)
        {
            case PlayerState.SELECT_MOVE:
                if (evt.IsActionPressed(ACTION_LEFT_CLICK))
                {
                    if (gridManager.CanMoveToTargetPosition(characterOnTurn, gridManager.GetMousePosition()))
                    {
                        MoveCharacter();
                    }
                }

                if (evt.IsActionPressed(CANCEL_ACTION))
                {
                    gridManager.ClearHighlights();
                    ChangeState(PlayerState.IDLE);
                }

                break;
            case PlayerState.SELECT_TARGET:
                if (evt.IsActionPressed(CANCEL_ACTION))
                {
                    gridManager.ClearHighlights();
                    ChangeState(PlayerState.IDLE);
                } else if (evt.IsActionPressed(ACTION_LEFT_CLICK))
                {             
                    if (hoveredCharacter != null)
                    {
                        characterOnTurn.Attack(hoveredCharacter);
                        gridManager.ClearHighlights();
                        ChangeState(PlayerState.IDLE);
                    }
                }
                
                break;
            default:
                break;
        }
    }

    public override void _Process(double delta)
    {
        hoveredCharacter = gridManager.GetCharacterAtCell(gridManager.GetMouseGridCellPosition());

        switch (currentState)
        {
            case PlayerState.SELECT_MOVE:
                gridManager.HighlightMovableCells(characterOnTurn);
                break;
            case PlayerState.SELECT_TARGET:
                gridManager.HighlightAttackArea(characterOnTurn);
                break;
        }
    }

    private async void MoveCharacter()
    {
        ChangeState(PlayerState.MOVING);

        await gridManager.MoveCharacter(characterOnTurn, gridManager.GetMousePosition());
    
        ChangeState(PlayerState.IDLE);
    }

    private void OnCharacterTurnEnded()
    {
        ChangeState(PlayerState.END_TURN);
        gridManager.ClearHighlights();
        characterOnTurn.HasMoved = false;
        characterOnTurn = null;
    }

    private void ChangeState(PlayerState state)
    {
        currentState = state;

        GameEvents.EmitPlayerStateChanged((int) currentState);
    }

}
