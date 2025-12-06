using Game.Resources.Character;
using Godot;

namespace Game.UI;

public partial class HeroSection : PanelContainer
{
    
    private TextureRect heroPortraitTextureRect;

    private Label heroNameLabel;

    private Label heroHealthLabel;

    public override void _Ready()
    {
        heroPortraitTextureRect = GetNode<TextureRect>("%HeroPortraitTextureRect");
        heroNameLabel = GetNode<Label>("%HeroNameLabel");
        heroHealthLabel = GetNode<Label>("%HeroHealthLabel");      
    }

    public void SetHeroResource(CharacterResource characterResource)
    {
        heroPortraitTextureRect.Texture = characterResource.Portrait;
        heroNameLabel.Text = characterResource.DisplayName;
        heroHealthLabel.Text = $"{characterResource.Health}/{characterResource.Health}";
    }

}
