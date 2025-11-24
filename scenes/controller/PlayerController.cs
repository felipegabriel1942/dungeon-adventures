using Game.Character;
using Godot;

public partial class PlayerController : Node
{
    [Export]
    private GridManager gridManager;

    private Character selectedCharacter;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("select")) {

            var clickedCharacter = gridManager.GetCharacterAtMousePosition();

            if (clickedCharacter != null)
            {
               GD.Print(clickedCharacter.Name); 
            }
            
        }
    }

}
