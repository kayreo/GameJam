using Godot;
using System;

public partial class MainMenu : Control
{
	private TextureButton StartButton;

	private TextureButton CreditButton;

	protected BoardManager BoardManager;

	[Signal]
	public delegate void ChangeLevelEventHandler(string WhichLevel);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");

		StartButton = GetNode<TextureButton>("PlayButton");

		CreditButton = GetNode<TextureButton>("CreditsButton");

		ChangeLevel += BoardManager.OnChangeLevel;
		StartButton.Pressed += OnStartPressed;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnStartPressed() {
		EmitSignal("ChangeLevel", "Level0");
	}
}
