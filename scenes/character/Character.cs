using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Autoload;
using Godot;

public abstract partial class Character : Node2D
{

    [Export]
    public int Agility = 10;

    [Export]
    public int Speed = 1;

    [Export]
    public TeamType Team;

    [Export]
    public int AttackRange = 1;

    private Node2D turnIndicator;

    protected AnimatedSprite2D animatedSprite2D;
    public bool IsAttacking;
    public bool HasMoved;

    public int Initiative { get; private set; }
    public bool IsMyTurn { get; protected set; }

    public override void _Ready()
    {
        AddToGroup("characters");
        CalculateInitiative();

        turnIndicator = GetNode<Node2D>("TurnIndicator");

        GameEvents.Instance.Connect(GameEvents.SignalName.BeginTurn, Callable.From<Character>(SetMyTurn));
    }

    public abstract void Attack();

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
        IsMyTurn = false;

        GameEvents.EmitEndTurn();
    }

    public void CalculateInitiative()
    {
        Random random = new Random();
        Initiative = Agility + random.Next(1, 7);
    }

    private void SetMyTurn(Character character)
    {
        if (character == this)
        {
            IsMyTurn = true;
        }
    }

    public override string ToString()
    {
        return $"{{ \"name\": {this.Name}, initiative: {this.Initiative}, agility: {this.Agility}, isMyTurn: {this.IsMyTurn} }}";
    }

}
