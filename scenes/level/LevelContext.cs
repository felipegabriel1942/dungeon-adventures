using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Game.Level;

public partial class LevelContext : Node
{
    [Signal]
    public delegate void CharactersReadyEventHandler();

    private readonly List<Character> characters = new();

    public IReadOnlyList<Character> Characters => characters;

    public override void _Ready()
    {
        AddToGroup("level_context");

        CallDeferred(nameof(FinalizeRegistration));
    }

    public void Register(Character character)
    {
        if (!characters.Contains(character))
        {
            characters.Add(character);
        }
    }

    private void FinalizeRegistration()
    {
        EmitSignal(SignalName.CharactersReady);
    }

    public IEnumerable<Character> Heroes => characters.Where(c => c.resource.Team.Equals(TeamType.Hero));
}
