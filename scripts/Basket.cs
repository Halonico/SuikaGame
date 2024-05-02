using Godot;
using System;

public partial class Basket : Node2D
{
    [Signal]
    public delegate void OnTopCollisionEventHandler();
    private void OnBodyEntered(Node2D node)
    {
        if (node is Fruit)
        {
            EmitSignal(SignalName.OnTopCollision);
        }
    }
}
