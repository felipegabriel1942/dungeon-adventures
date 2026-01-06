using Game;
using Game.Level;
using Game.UI;
using Godot;

public partial class BaseLevel : Node
{

    [Export]
    private LevelContext levelContext;

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

    private void SetCamera()
    {
        gameCamera.SetBoundingRect(tileMapLayer.GetUsedRect());
    }

}
