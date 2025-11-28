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

        CallDeferred(nameof(Init));
    }

    private void Init()
    {
        var characters = gridManager.GetAllCharacters()
            .OrderByDescending(c => c.Initiative)
            .ThenByDescending(c => c.Agility)
            .ToList();

        foreach(var character in characters)
        {
            turnOrder.Enqueue(character);
        }

        GameEvents.EmitBeginTurn(turnOrder.Peek());
    }

    private void EndTurn()
    {
        var finished = turnOrder.Dequeue();
        turnOrder.Enqueue(finished);

        GameEvents.EmitBeginTurn(turnOrder.Peek());
    } 
}
