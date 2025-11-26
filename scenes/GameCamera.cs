using Godot;

namespace Game;

public partial class GameCamera : Camera2D
{
    
    private const int TILE_SIZE = 16;

    public override void _Process(double delta)
    {
        //GlobalPosition = GetScreenCenterPosition();
    }

    public void SetBoundingRect(Rect2I boundingRect)
    {
        LimitLeft = boundingRect.Position.X * TILE_SIZE;
        LimitRight = boundingRect.End.X * TILE_SIZE;
        LimitTop = boundingRect.Position.Y * TILE_SIZE;
        LimitBottom = boundingRect.End.Y * TILE_SIZE;
    }

    public void CenterOnPosition(Vector2 position)
    {
        GlobalPosition = position;
    }

}
