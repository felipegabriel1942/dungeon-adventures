using Godot;

namespace Game.Controller;

public partial class PlayerController : Node
{
    [Export]
    private GridManager gridManager;

    private Character selectedCharacter;

    public override async void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("select")) {

            var clickedCharacter = gridManager.GetCharacterAtMousePosition();

            if (clickedCharacter != null)
            {
               selectedCharacter = clickedCharacter;
               gridManager.HighlightMovableCells(clickedCharacter);
            }

            if (selectedCharacter != null && clickedCharacter == null)
            {
                await gridManager.MoveCharacter(selectedCharacter, gridManager.GetMousePosition());
                selectedCharacter = null;     
            }
        }
    }

}
