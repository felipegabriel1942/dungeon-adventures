using Godot;

namespace Game.Resources.Character;

[GlobalClass]
public partial class CharacterResource : Resource
{

    [Export]
    public string DisplayName { get; private set; }

    [Export]
    public int Health { get; private set; }

    [Export]
    public int Agility { get; private set; }

    [Export]
    public int Speed { get; private set; }

    [Export]
    public int Attack { get; private set; }

    [Export]
    public int Defense { get; private set; }

    [Export]
    public int AttackRange { get; private set; }

    [Export]
    public TeamType Team { get; private set; }

    [Export]
    public Texture2D Portrait { get; private set; }
}
