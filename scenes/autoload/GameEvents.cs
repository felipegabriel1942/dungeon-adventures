using Game.Enum;
using Godot;

namespace Game.Autoload;

public partial class GameEvents : Node
{

    public static GameEvents Instance { get; private set; }

    [Signal]
    public delegate void BeginTurnEventHandler(Character character);

    [Signal]
    public delegate void EndTurnEventHandler();

    [Signal]
    public delegate void CharacterSelectedOnGridEventHandler(Character character);

    [Signal]
    public delegate void PlayerStateChangeEventHandler(PlayerState state);

    [Signal]
    public delegate void AttackButtonPressedEventHandler();

    [Signal]
    public delegate void MoveButtonPressedEventHandler();

    [Signal]
    public delegate void CharacterDamagedEventHandler(Character character);

    [Signal]
    public delegate void ProjectileHitTargetEventHandler(Character target);

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            Instance = this;
        }
    }

    public static void EmitBeginTurn(Character character)
    {
        Instance.EmitSignal(SignalName.BeginTurn, character);
    }

    public static void EmitEndTurn()
    {
        Instance.EmitSignal(SignalName.EndTurn);
    }

    public static void EmitCharacterSelectedOnGrid(Character character) {
        Instance.EmitSignal(SignalName.CharacterSelectedOnGrid, character);
    }

    public static void EmitPlayerStateChanged(int newState)
    {
        Instance.EmitSignal(SignalName.PlayerStateChange, newState);
    }

    public static void EmitAttackButtonPressed()
    {
        Instance.EmitSignal(SignalName.AttackButtonPressed);
    }

    public static void EmitMoveButtonPressed()
    {
        Instance.EmitSignal(SignalName.MoveButtonPressed);
    }

    public static void EmitCharacterDamaged(Character character)
    {
        Instance.EmitSignal(SignalName.CharacterDamaged, character);
    }

    public static void EmitProjectileHitTarget(Character target)
    {
        Instance.EmitSignal(SignalName.ProjectileHitTarget, target);
    }
}
