using Game.Autoload;
using Game.Enum;
using Game.Level;
using Game.Resources.Character;

using Godot;

namespace Game.UI;

public partial class GameUi : CanvasLayer
{
    
    [Export]
    private CharacterResource[] heroesResources;

    [Export]
    private PackedScene heroSectionScene;

    private Button moveButton;
    private Button attackButton;
    private Button endTurnButton;
    private Button healButton;
    private VBoxContainer heroSectionContainer;
    private PanelContainer actionsMenu;
    private PlayerState currentPlayerState;
    private Character currentCharacter;
    private LevelContext levelContext;

    public override void _Ready()
    {
        attackButton = GetNode<Button>("%AttackButton");
        endTurnButton = GetNode<Button>("%EndTurnButton");
        moveButton = GetNode<Button>("%MoveButton");
        heroSectionContainer = GetNode<VBoxContainer>("%HeroSectionContainer");
        actionsMenu = GetNode<PanelContainer>("%ActionsMenuContainer");
        healButton = GetNode<Button>("%HealButton");
        levelContext = GetTree().GetFirstNodeInGroup("level_context") as LevelContext;

        levelContext.CharactersReady += CreateHeroesSection;

        attackButton.MouseEntered += OnMouseEnter;
        attackButton.MouseExited += OnMouseExit;
        attackButton.Pressed += OnAttackButtonPressed;

        moveButton.MouseEntered += OnMouseEnter;
        moveButton.MouseExited += OnMouseExit;
        moveButton.Pressed += OnMoveButtonPressed;

        endTurnButton.MouseEntered += OnMouseEnter;
        endTurnButton.MouseExited += OnMouseExit;
        endTurnButton.Pressed += OnEndTurnButtonClicked;
        
        healButton.MouseEntered += OnMouseEnter;
        healButton.MouseExited += OnMouseExit;
        healButton.Pressed += OnHealButtonPressed;

        GameEvents.Instance.Connect(GameEvents.SignalName.PlayerStateChange, Callable.From<PlayerState>(OnPlayerStateChanged));
        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(OnTurnBegin));
    }

    private void OnHealButtonPressed()
    {
        GameEvents.EmitAttackButtonPressed(); 
    }

    private void OnTurnBegin(Character character)
    {
        currentCharacter = character;
    }

    private void OnMoveButtonPressed()
    {
        GameEvents.EmitMoveButtonPressed();
    }

    private void OnAttackButtonPressed()
    {
        GameEvents.EmitAttackButtonPressed();   
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor((Texture2D)GD.Load("res://assets/cursor-button.png"));
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor((Texture2D)GD.Load("res://assets/cursor.png"));
    }

    private void OnEndTurnButtonClicked()
    {
        GameEvents.EmitEndTurn();
    }

    private void CreateHeroesSection()
    {
        foreach (var hero in levelContext.Heroes)
        {
            var heroSection = heroSectionScene.Instantiate<HeroSection>();
            heroSectionContainer.AddChild(heroSection);
            heroSection.SetHero(hero);
        }
    }

    public override void _Process(double delta)
    {

        attackButton.Disabled = currentCharacter.HasAttacked;
        moveButton.Disabled = currentCharacter.HasMoved;

        switch (currentPlayerState)
        {
            case PlayerState.IDLE:
                actionsMenu.Visible = currentCharacter.resource.Team.Equals(TeamType.Hero);

                if (actionsMenu.Visible)
                {
                    switch (currentCharacter.resource.CombatRole)
                    {
                        case CombatRole.HEALER:
                            attackButton.Visible = false;
                            healButton.Visible = true;
                            break;
                        default:
                            attackButton.Visible = true;
                            healButton.Visible = false;
                            break;
                    }
                }

                break;
            case PlayerState.SELECT_MOVE:
                actionsMenu.Visible = false;
                break;
            case PlayerState.END_TURN:
                actionsMenu.Visible = false;
                break;
            default:
                actionsMenu.Visible = false;
                break;
        }
    }

    private void OnPlayerStateChanged(PlayerState newState)
    {
        currentPlayerState = newState;
    }
}
