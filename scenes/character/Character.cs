using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Autoload;
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

    public override void _Ready()
    {
        AddToGroup("characters");
        CalculateInitiative();

        turnIndicator = GetNode<Node2D>("TurnIndicator");

        CurrentHealth = resource.Health;

        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(SetMyTurn));
    }

    public abstract void Attack(Character target);

    public override void _PhysicsProcess(double delta)
    {
        // if (HasMoved)
        // {
        //     animatedSprite2D.Modulate = new Color(0.5f, 0.5f, 0.5f, 1);
        // } else
        // {
        //      animatedSprite2D.Modulate = new Color(1f, 1f, 1f, 1);
        // }

        turnIndicator.Visible = IsMyTurn;
    }

    public async Task Move(List<Vector2> path)
    {
        var tween = CreateTween()
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.InOut);

        foreach (var cell in path)
        {
            tween.TweenProperty(this, "position", cell, 0.2);
        }

        await ToSignal(tween, "finished");

        tween.Dispose();

        HasMoved = true;
        
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
}
