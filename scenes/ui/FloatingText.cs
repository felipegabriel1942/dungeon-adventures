using Godot;

public partial class FloatingText : Node2D
{
    
    private Label label;

    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        label = GetNode<Label>("%Label");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("default");
        animationPlayer.AnimationFinished += OnAnimationFinished;
    }

    private void OnAnimationFinished(StringName animName)
    {
        if (animName.Equals("default"))
        {
            QueueFree();
        }
    }

    public void SetText(string text, string type)
    {
        label.Text = text;
    }

}
