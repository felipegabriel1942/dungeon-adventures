using System;
using Game.Autoload;
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
        GameEvents.Instance.Connect(GameEvents.SignalName.CharacterDamaged, Callable.From<Character>(OnCharacterDamaged));      
    }

    private void OnCharacterDamaged(Character character)
    {
        if (character.resource.DisplayName.Equals(heroNameLabel.Text))
        {
            heroHealthLabel.Text = $"{character.CurrentHealth}/{character.resource.Health}";
        }   
    }


    public void SetHeroResource(CharacterResource characterResource)
    {
        heroPortraitTextureRect.Texture = characterResource.Portrait;
        heroNameLabel.Text = characterResource.DisplayName;
        heroHealthLabel.Text = $"{characterResource.Health}/{characterResource.Health}";
    }

}
