using Godot;
using System;

public partial class MainMenu : Control
{
	private TextureButton StartButton;

	private TextureButton CreditButton;

	protected BoardManager BoardManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		BoardManager.CurLevelIndex = 0;

		StartButton = GetNode<TextureButton>("PlayButton");

		CreditButton = GetNode<TextureButton>("CreditsButton");

		BoardManager.ChangeLevel += BoardManager.OnChangeLevel;
		StartButton.Pressed += OnStartPressed;

		CreditButton.Pressed += OnCreditsPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnStartPressed() {
		BoardManager.CurLevel = "Level0";
		BoardManager.EmitSignal("ChangeLevel", "Level0");
	}

	private void OnCreditsPressed() {
		GetTree().ChangeSceneToFile("res://scenes/Credits.tscn");
	}
}
