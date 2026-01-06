using Game.Component;
using Godot;

public partial class VFXComponent : Node
{

    public override void _Ready()
    {
        var health = GetParent<Character>().GetNode<HealthComponent>("HealthComponent");
        health.Healed += OnHealed;
    }

    private async void OnHealed(int amount)
    {
        PackedScene healingEffectScene = GD.Load<PackedScene>("res://scenes/HealingEffect.tscn");
        var healingEffect = healingEffectScene.Instantiate();

        GetParent().AddChild(healingEffect);

        await ToSignal(GetTree().CreateTimer(2.0), "timeout");

        healingEffect.QueueFree();
    }
}
