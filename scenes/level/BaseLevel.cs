using Game;
using Game.UI;
using Godot;

public partial class BaseLevel : Node
{

    private GridManager gridManager;
    private GameCamera gameCamera;
    private TileMapLayer tileMapLayer;
    private GameUi gameUi;

    public override void _Ready()
    {
        gridManager = GetNode<GridManager>("%GridManager");
        gameCamera = GetNode<GameCamera>("GameCamera");
        tileMapLayer = GetNode<TileMapLayer>("%BaseTerrainLayer");
        gameUi = GetNode<GameUi>("%GameUI");        
    }

    public override void _PhysicsProcess(double delta)
    {
        
    }

    private void SetCamera()
    {
        gameCamera.SetBoundingRect(tileMapLayer.GetUsedRect());
    }

}
