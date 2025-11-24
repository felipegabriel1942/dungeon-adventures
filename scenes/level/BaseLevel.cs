using Game;
using Godot;

public partial class BaseLevel : Node
{

    private GridManager gridManager;
    private GameCamera gameCamera;
    private TileMapLayer tileMapLayer;

    public override void _Ready()
    {
        gridManager = GetNode<GridManager>("%GridManager");
        gameCamera = GetNode<GameCamera>("GameCamera");
        tileMapLayer = GetNode<TileMapLayer>("%BaseTerrainLayer");

        CallDeferred(nameof(SetCamera));
        
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }


    private void SetCamera()
    {
        gameCamera.SetBoundingRect(tileMapLayer.GetUsedRect());
    }

}
