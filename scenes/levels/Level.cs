using Godot;
using System;

public partial class Level : Node
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print("Removing: ", GetParent().GetChild(0));
		//GetParent().GetChild(0).QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
