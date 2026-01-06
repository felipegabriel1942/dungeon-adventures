using System;
using Godot;

namespace Game.Component;

public partial class HealthComponent : Node
{
    [Signal]
    public delegate void HealthChangedEventHandler(int current, int max);

    [Signal]
    public delegate void HealedEventHandler(int amount);

    [Signal]
    public delegate void DiedEventHandler();

    private int maxHealth;
    public int CurrentHealth { get; private set; }

    private Character character;

    public override void _Ready()
    {
        character = GetParent<Character>();
        maxHealth = character.resource.Health;
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead()) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);

        EmitSignal(SignalName.HealthChanged, CurrentHealth, maxHealth);

        if (IsDead())
        {
            EmitSignal(SignalName.Died);
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead()) return;

        CurrentHealth = Math.Min(CurrentHealth + amount, maxHealth);

        EmitSignal(SignalName.HealthChanged, CurrentHealth, maxHealth);
        EmitSignal(SignalName.Healed, amount);
    }

    private bool IsDead() => CurrentHealth <= 0;
}
