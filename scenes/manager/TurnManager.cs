using System.Collections.Generic;
using System.Linq;
using Game.Autoload;
using Godot;

namespace Game.Manager;

public partial class TurnManager : Node
{
    
    [Export]
    private GridManager gridManager;

    private Queue<Character> turnOrder = new();
    
    public override void _Ready()
    {
        GameEvents.Instance.Connect(GameEvents.SignalName.EndTurn, Callable.From(EndTurn));
        GameEvents.Instance.Connect(GameEvents.SignalName.CharacterDied, Callable.From<Character>(OnCharacterDeath));

        CallDeferred(nameof(Init));
    }


    private void Init()
    {
        var characters = gridManager.GetAllCharacters()
            .OrderByDescending(c => c.Initiative)
            .ThenByDescending(c => c.resource.Agility)
            .ToList();

        foreach(var character in characters)
        {
            turnOrder.Enqueue(character);
        }

        GameEvents.EmitBeginTurn(turnOrder.Peek());
    }

    private async void EndTurn()
    {
        var finished = turnOrder.Dequeue();
        finished.EndMyTurn();

        turnOrder.Enqueue(finished);

        await ToSignal(GetTree().CreateTimer(1.5f), "timeout");

        GameEvents.EmitBeginTurn(turnOrder.Peek());
    }

    private void OnCharacterDeath(Character character)
    {
        turnOrder = new Queue<Character>(
            turnOrder.Where(c => c != character)
        );
    }
}
