using Godot;
using System;

public partial class Navigation : Control
{

	private TextureButton BackButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BackButton = GetNode<TextureButton>("BackButton");
		BackButton.Pressed += OnBackPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBackPressed() {
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
}
