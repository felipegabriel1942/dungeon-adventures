using System;
using Godot;

public partial class Dice : Node
{
    public static int Roll()
    {
        return new Random().Next(1, 7);
    }
}
