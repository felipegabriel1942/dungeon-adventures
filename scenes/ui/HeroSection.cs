using Godot;

namespace Game.UI;

public partial class HeroSection : PanelContainer
{
    private TextureRect portrait;
    private Label name;
    private Label health;

    public override void _Ready()
    {
        portrait = GetNode<TextureRect>("%HeroPortraitTextureRect");
        name = GetNode<Label>("%HeroNameLabel");
        health = GetNode<Label>("%HeroHealthLabel");
        // GameEvents.Instance.Connect(GameEvents.SignalName.CharacterHealthChanged, Callable.From<Character>(OnCharacterHealthChanged));      
    }

    public void SetHero(Character hero)
    {
        portrait.Texture = hero.resource.Portrait;
        name.Text = hero.resource.DisplayName;
        health.Text = $"{hero.resource.Health}/{hero.resource.Health}";
    }

    private void OnCharacterHealthChanged(Character character)
    {
        if (character.resource.DisplayName.Equals(name.Text))
        {
            health.Text = $"{character.CurrentHealth}/{character.resource.Health}";
        }   
    }
}
