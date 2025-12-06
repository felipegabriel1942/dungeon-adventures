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

}
