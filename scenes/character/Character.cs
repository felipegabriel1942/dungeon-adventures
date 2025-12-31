using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Autoload;
using Game.Enum;
using Game.Resources.Character;
using Godot;

public abstract partial class Character : Node2D
{

    [Export]
    public CharacterResource resource { get; private set; }

    private Node2D turnIndicator;

    protected AnimatedSprite2D animatedSprite2D;
    public bool IsAttacking;
    public bool HasMoved;
    public bool HasAttacked;

    public int Initiative { get; private set; }
    public bool IsMyTurn { get; protected set; }

    public int CurrentHealth;

    private CharacterState characterState;

    public override void _Ready()
    {
        characterState = CharacterState.IDLE;

        AddToGroup("characters");
        CalculateInitiative();

        turnIndicator = GetNode<Node2D>("TurnIndicator");

        CurrentHealth = resource.Health;

        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(SetMyTurn));
    }

    public abstract void Attack(Character target);

    public override void _PhysicsProcess(double delta)
    {
        turnIndicator.Visible = IsMyTurn;

        if (characterState == CharacterState.IDLE)
        {
            animatedSprite2D.Play("idle");
        }

        if (characterState != CharacterState.MOVING)
            return;

        Vector2 movement = Position - currentPosition;

        if (movement.Length() > 0.001f)
        {
            if (Mathf.Abs(movement.X) > Mathf.Abs(movement.Y))
            {
                if (movement.X > 0)
                {
                    animatedSprite2D.FlipH = false;
                    animatedSprite2D.Play("walk_side");
                } else
                {
                    animatedSprite2D.FlipH = true;
                    animatedSprite2D.Play("walk_side");
                }
            } else
            {
                if (movement.Y > 0)
                {
                    animatedSprite2D.Play("walk_down");
                } else
                {
                    animatedSprite2D.Play("walk_up");
                }
            } 
        }

        currentPosition = Position;

    }

    private Vector2 currentPosition;

    public async Task Move(List<Vector2> path)
    {
        characterState = CharacterState.MOVING;

        var tween = CreateTween();

        foreach (var cell in path)
        {
            tween.TweenProperty(this, "position", cell, 0.4);
        }

        await ToSignal(tween, "finished");

        tween.Dispose();

        HasMoved = true;

        characterState = CharacterState.IDLE;
        
        // TODO: Verificar se esse trecho de codigo pode ir para o enemy controller
        if (resource.Team.Equals(TeamType.Enemy))
        {
            IsMyTurn = false;

            GameEvents.EmitEndTurn();
        }
    }

    public void CalculateInitiative()
    {
        Initiative = resource.Agility + Dice.Roll();
    }

    private void SetMyTurn(Character character)
    {
        if (character == this)
        {
            IsMyTurn = true;
        }
    }

    public void EndMyTurn()
    {
        IsMyTurn = false;
    }

    public override string ToString()
    {
        return $"{{ \"name\": {this.Name}, \"currentHealht\": {this.CurrentHealth}, initiative: {this.Initiative}, agility: {this.resource.Agility}, isMyTurn: {this.IsMyTurn} }}";
    }

    public void TakeDamage(int damage)
    {

        CurrentHealth -= damage;

        addFloatingPoints(damage, "");

        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        GameEvents.EmitCharacterHealthChanged(this);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected abstract void Die();

    protected int CalculateDamage(Character target)
    {
        var attackerRoll = this.resource.Attack + Dice.Roll();
        var defenseRoll = target.resource.Defense + Dice.Roll();
        var damage =  attackerRoll - defenseRoll;

        GD.Print($"{this.resource.DisplayName} rolls an {attackerRoll} for attack and {target.resource.DisplayName} rolls {defenseRoll} for defense.");

        if (damage <= 0)
        {
            GD.Print($"{this.resource.DisplayName} misses attack.");
        } else
        {
            GD.Print($"{target.resource.DisplayName} suffer {damage} points of damage.");
        }

        return damage < 0 ? 0 : damage;
    }

    public Vector2I GetMapPosition()
    {
        return (Vector2I) GlobalPosition / 16;
    }

    public async Task Heal(int healPoints)
    {

        PackedScene healingEffectScene = GD.Load<PackedScene>("res://scenes/HealingEffect.tscn");

        var healingEffect = healingEffectScene.Instantiate();

        AddChild(healingEffect);

        CurrentHealth += healPoints;

        if (CurrentHealth > resource.Health)
        {
            CurrentHealth = resource.Health;
        } 

        GameEvents.EmitCharacterHealthChanged(this);

        GD.Print($"{resource.DisplayName} healed {healPoints} points.");

        await ToSignal(GetTree().CreateTimer(2.0), "timeout");

        RemoveChild(healingEffect);

    }

    private void addFloatingPoints(int points, string type)
    {
        var floatingTextScene = GD.Load<PackedScene>("res://scenes/ui/FloatingText.tscn");
        var floatingTextInstance = floatingTextScene.Instantiate<FloatingText>();

        GetParent().AddChild(floatingTextInstance);

        floatingTextInstance.GlobalPosition = new Vector2(GlobalPosition.X + 14, GlobalPosition.Y - 14);
        floatingTextInstance.SetText(points.ToString(), "");
    } 
}
