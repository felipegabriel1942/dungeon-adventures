using System.Collections.Generic;
using Game.Autoload;
using Game.Component;
using Game.Enum;
using Game.Level;
using Game.Resources.Character;
using Godot;

public abstract partial class Character : Node2D
{

    [Export]
    public CharacterResource resource { get; private set; }

    public bool IsAttacking;
    public bool HasMoved;
    public bool HasAttacked;
    public int Initiative { get; private set; }
    public bool IsMyTurn { get; protected set; }
    public int CurrentHealth;

    private Vector2 currentPosition;
    private Node2D turnIndicator;

    private CharacterStateMachine stateMachine;
    private CharacterAnimationComponent animation;
    private CharacterOrientation orientation;
    private MovementComponent movement;
    private HealthComponent health;

    public override void _Ready()
    {
        stateMachine = GetNode<CharacterStateMachine>("CharacterStateMachine");
        animation = GetNode<CharacterAnimationComponent>("CharacterAnimationComponent");
        movement = GetNode<MovementComponent>("MovementComponent");
        orientation = GetNode<CharacterOrientation>("CharacterOrientation");
        health = GetNode<HealthComponent>("HealthComponent");


        // Talvez altere 
        AddToGroup("characters");


        var levelContext = GetTree().GetFirstNodeInGroup("level_context") as LevelContext;
        levelContext.Register(this);

        CalculateInitiative();

        turnIndicator = GetNode<Node2D>("TurnIndicator");

        CurrentHealth = resource.Health;

        animation.AnimationCompleted += OnAnimationFinished;
        stateMachine.StateChanged += OnStateChange;

        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(SetMyTurn));
    }

    private void OnAnimationFinished(StringName anim)
    {
        if (anim.Equals("walk_side") || anim.Equals("hurt"))
        {
            stateMachine.SetState(CharacterState.IDLE);
        }
    }

    private void OnStateChange(int newState)
    {
       animation.PlayForState((CharacterState) newState, orientation.CurrentDirection);
    }

    public void Attack(Character target)
    {
        stateMachine.SetState(CharacterState.ATTACKING);

        IsAttacking = true;

        if (resource.CombatRole.Equals(CombatRole.HEALER))
        {
            target.Heal(Dice.Roll());
        } else
        {
            target.TakeDamage(CalculateDamage(target));
        }

        if (resource.Team.Equals(TeamType.Hero))
        {
            HasAttacked = true;
        }

        if (resource.Team.Equals(TeamType.Enemy))
        {
            IsMyTurn = false;
            GameEvents.EmitEndTurn();   
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        turnIndicator.Visible = IsMyTurn;

        // if (stateMachine.CurrentState == CharacterState.ATTACKING)
        // {
        //     animatedSprite2D.Play("attack_side");
        // }

    }

    public async void Move(List<Vector2> path)
    {
        stateMachine.SetState(CharacterState.MOVING);

        await movement.MoveAlongPath(path);

        HasMoved = true;

        stateMachine.SetState(CharacterState.IDLE);
        
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
        stateMachine.SetState(CharacterState.HURT);

        addFloatingPoints(damage, "");

        health.TakeDamage(damage);

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

    public void Heal(int amount)
    {
        health.Heal(amount);
    }

    //  TODO: Vai para o VXF Component
    private void addFloatingPoints(int points, string type)
    {
        var floatingTextScene = GD.Load<PackedScene>("res://scenes/ui/FloatingText.tscn");
        var floatingTextInstance = floatingTextScene.Instantiate<FloatingText>();

        GetParent().AddChild(floatingTextInstance);

        floatingTextInstance.GlobalPosition = new Vector2(GlobalPosition.X + 14, GlobalPosition.Y - 14);
        floatingTextInstance.SetText(points.ToString(), "");
    }

    public void UpdateFacingDirection(Direction newDirection)
    {
        orientation?.SetDirection(newDirection);
    }
}
