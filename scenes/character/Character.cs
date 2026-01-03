using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Autoload;
using Game.Enum;
using Game.Resources.Character;
using Game.Scripts;
using Godot;

public abstract partial class Character : Node2D
{

    [Export]
    public CharacterResource resource { get; private set; }

    public CharacterStateMachine StateMachine { get; private set; }

    protected AnimatedSprite2D animatedSprite2D;
    public bool IsAttacking;
    public bool HasMoved;
    public bool HasAttacked;
    public int Initiative { get; private set; }
    public bool IsMyTurn { get; protected set; }
    public int CurrentHealth;

    // private CharacterState characterState;
    private Vector2 currentPosition;
    private Node2D turnIndicator;

    public override void _Ready()
    {
        StateMachine = new CharacterStateMachine();

        AddToGroup("characters");
        CalculateInitiative();

        turnIndicator = GetNode<Node2D>("TurnIndicator");

        CurrentHealth = resource.Health;

        animatedSprite2D.AnimationFinished += OnAnimationFinished;

        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(SetMyTurn));
    }

    private void OnAnimationFinished()
    {
        if (animatedSprite2D.Animation == "hurt")
        {
            StateMachine.SetState(CharacterState.IDLE);
        }

        if (animatedSprite2D.Animation == "attack_side")
        {
            StateMachine.SetState(CharacterState.IDLE);
        }
    }

    public void Attack(Character target)
    {
        StateMachine.SetState(CharacterState.ATTACKING);

        IsAttacking = true;

        animatedSprite2D.FlipH = GetMapPosition().X < target.GetMapPosition().X;

        if (resource.CombatRole.Equals(CombatRole.HEALER))
        {
            _ = target.Heal(Dice.Roll());
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

        if (StateMachine.CurrentState == CharacterState.ATTACKING)
        {
            animatedSprite2D.Play("attack_side");
        }

        if (StateMachine.CurrentState == CharacterState.HURT)
        {
            animatedSprite2D.Play("hurt");
        }

        if (StateMachine.CurrentState == CharacterState.IDLE)
        {
            animatedSprite2D.Play("idle");
        }

        if (StateMachine.CurrentState != CharacterState.MOVING)
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

    public async Task Move(List<Vector2> path)
    {
        StateMachine.SetState(CharacterState.MOVING);

        var tween = CreateTween();

        foreach (var cell in path)
        {
            tween.TweenProperty(this, "position", cell, 0.4);
        }

        await ToSignal(tween, "finished");

        tween.Dispose();

        HasMoved = true;

        StateMachine.SetState(CharacterState.IDLE);
        
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
        StateMachine.SetState(CharacterState.HURT);

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

        await ToSignal(GetTree().CreateTimer(2.0), "timeout");

        GD.Print($"{resource.DisplayName} healed {healPoints} points.");

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
