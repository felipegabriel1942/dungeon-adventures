using Godot;

public partial class BaseLevel : Node
{

    private GridManager gridManager;

    public override void _Ready()
    {
        gridManager = GetNode<GridManager>("%GridManager");
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }

}
