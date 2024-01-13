using Godot;
using System;

public partial class BlackoutBoard : Board
{

	private int BlackOutNum = 5;

	Godot.Collections.Array<Icon> BlackOutNodes;

	// Timer for how long tiles stay blacked out
	Timer BlackOutTimer;

	// Timer for how long until another blackout
	Timer ResetBlackOutTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BlackOutNodes = new Godot.Collections.Array<Icon>();
		BlackOutTimer = GetNode<Timer>("BlackOutTimer");
		BlackOutTimer.Timeout += OnBlackOutTimeOut;
		ResetBlackOutTimer = GetNode<Timer>("ResetBlackOutTimer");
		ResetBlackOutTimer.Timeout += OnResetTimeOut;
		base._Ready();
		ResetBlackOutTimer.Start(3);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	private void PopulateNodeList() {
		for (int i = 0; i < BlackOutNum; i++) {
			Icon NodeToAdd = (Icon)GridNodes.PickRandom();
			while (NodeToAdd == GridNodes[0] || NodeToAdd == GridNodes[GridNodes.Count - 1] || 
				BlackOutNodes.Contains(NodeToAdd) || NodeToAdd.WireAnim.Frame != 0) {
					NodeToAdd = (Icon)GridNodes.PickRandom();
			}
			//GD.Print("Going to black out node: ", NodeToAdd.Name);
			BlackOutNodes.Add(NodeToAdd);
		}
	}

	private void OnBlackOut() {
		GD.Print("Black out starting");

	}

	private void OnBlackOutTimeOut() {
		GD.Print("Unblacking out");
		foreach (Icon ToBlackOut in BlackOutNodes) {
			ToBlackOut.GetNode<Sprite2D>("Wire").Show();
			ToBlackOut.GetNode<AnimatedSprite2D>("WireAnim").Show();
		}
		BlackOutNodes.Clear();
		BlackOutTimer.Stop();
		ResetBlackOutTimer.Start(3);
	}

	private void OnResetTimeOut() {
		GD.Print("Waiting for next black out");
		PopulateNodeList();
		foreach (Icon ToBlackOut in BlackOutNodes) {
			ToBlackOut.GetNode<Sprite2D>("Wire").Hide();
			ToBlackOut.GetNode<AnimatedSprite2D>("WireAnim").Hide();
		}
		ResetBlackOutTimer.Stop();
		BlackOutTimer.Start(2);
	}
}
