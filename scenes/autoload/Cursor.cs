using Godot;

namespace Game.Autoload;

public partial class Cursor : CanvasLayer
{
    
    private static Sprite2D sprite2D;

    public override void _Ready()
    {
       Input.MouseMode = Input.MouseModeEnum.Hidden;
       sprite2D = GetNode<Sprite2D>("Sprite2D");
    }

    public override void _Process(double delta)
    {
        sprite2D.GlobalPosition = sprite2D.GetGlobalMousePosition();
    }

    public static void SetCursor(Texture2D texture)
    {
        sprite2D.Texture = texture;
    }
}
