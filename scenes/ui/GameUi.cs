
using Game.Autoload;
using Game.Resources.Character;

using Godot;

namespace Game.UI;

public partial class GameUi : CanvasLayer
{
    
    [Export]
    private CharacterResource[] heroesResources;

    [Export]
    private PackedScene heroSectionScene;

    private Button attackButton;

    private Button endturnButton;

    private VBoxContainer heroSectionContainer;

    private PanelContainer actionsMenu;


    public override void _Ready()
    {
        attackButton = GetNode<Button>("%AttackButton");
        endturnButton = GetNode<Button>("%EndTurnButton");
        heroSectionContainer = GetNode<VBoxContainer>("%HeroSectionContainer");
        actionsMenu = GetNode<PanelContainer>("%ActionsMenuContainer");

        CreateHeroesSection();

        endturnButton.Pressed += OnEndTurnButtonClicked;
        GameEvents.Instance.Connect(GameEvents.SignalName.CharacterSelectedOnGrid, Callable.From<Character>(OnCharacterSelected));
    
    }

    private void OnEndTurnButtonClicked()
    {
        GameEvents.EmitEndTurn();
    }

    private void CreateHeroesSection()
    {
        foreach (var heroResource in heroesResources)
        {
            var heroSection = heroSectionScene.Instantiate<HeroSection>();
            heroSectionContainer.AddChild(heroSection);
            heroSection.SetHeroResource(heroResource);
        }
    }

    private void OnCharacterSelected(Character character)
    {
        if (character != null)
        {
            // characterSheet.Visible = true;

            if (character.resource.Team.Equals(TeamType.Hero))
            {
                actionsMenu.Visible = true;
            }

            if (character.resource.Team.Equals(TeamType.Enemy))
            {
                actionsMenu.Visible = false;
            }
        } else
        {
            // characterSheet.Visible = false;
            actionsMenu.Visible = false;
        }

        GD.Print(character);
    }


}
