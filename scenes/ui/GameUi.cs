
using Game.Autoload;
using Game.Enum;
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

    private Button endTurnButton;

    private VBoxContainer heroSectionContainer;

    private PanelContainer actionsMenu;

    public override void _Ready()
    {
        attackButton = GetNode<Button>("%AttackButton");
        endTurnButton = GetNode<Button>("%EndTurnButton");
        heroSectionContainer = GetNode<VBoxContainer>("%HeroSectionContainer");
        actionsMenu = GetNode<PanelContainer>("%ActionsMenuContainer");

        CreateHeroesSection();

        endTurnButton.Pressed += OnEndTurnButtonClicked;
        // GameEvents.Instance.Connect(GameEvents.SignalName.CharacterSelectedOnGrid, Callable.From<Character>(OnCharacterSelected));
        GameEvents.Instance.Connect(GameEvents.SignalName.PlayerStateChange, Callable.From<PlayerStates>(OnPlayerStateChanged));
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


    private void OnPlayerStateChanged(PlayerStates newState)
    {
        switch (newState)
        {
            case PlayerStates.SELECT_MOVE:
                actionsMenu.Visible = true;
                break;
            case PlayerStates.END_TURN:
                actionsMenu.Visible = true;
                break;
            default:
                actionsMenu.Visible = false;
                break;
        }
    }
}
