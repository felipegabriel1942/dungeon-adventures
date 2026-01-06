public class TurnState
{
    public bool HasMoved { get; set; }
    public bool HasActed { get; set; }

    public bool CanMove => !HasMoved;
    public bool CanAct => !HasActed;
}