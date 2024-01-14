using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BoardManager : Node
{
	public enum Position {
		Left,
		Top,
		Right,
		Bottom
	}

	// Index 0: Elbow, Index 1: Straight
	// Sub Index 0: Entrance, Index 1: Exit
	public Godot.Collections.Array<Godot.Collections.Array<Position>> WirePos = new Godot.Collections.Array<Godot.Collections.Array<Position>>();
	private Node CurClick;
	
	private Node SwapClick;

	private Icon ActiveIcon;

	Godot.Collections.Array<Node> GridNodes;

	Godot.Collections.Array<Node> Wires;

	[Signal]
	public delegate void SetGridInfoEventHandler(string WhichNode, string TypeNam);

	[Signal]
	public delegate void ChangeGridEventHandler(string WhichNode, Icon NodeToSwap);

	[Signal]
	public delegate void ClickNodeEventHandler(Icon WhichNode);

	[Signal]
	public delegate void TriggerDialogueEventHandler(string DialogueText);

	[Signal]
	public delegate void EndLevelEventHandler();

	public bool DialogueActive = false;

	public bool GameRunning = false;

	public bool LevelEnd = false;

	public bool LevelSuccess = false;

	private string CurLevel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EndLevel += OnEndLevel;
	}

	public void OnSetData() {
		GridNodes = GetTree().GetNodesInGroup("Grid");
		Wires = GetTree().GetNodesInGroup("Wire");
		Godot.Collections.Array<Position> ToAddPos = new Godot.Collections.Array<Position>();

		// Elbow
		ToAddPos.Add(Position.Left);
		ToAddPos.Add(Position.Top);
		WirePos.Add(ToAddPos);

		// Straight
		ToAddPos = new Godot.Collections.Array<Position>();
		ToAddPos.Add(Position.Left);
		ToAddPos.Add(Position.Right);
		WirePos.Add(ToAddPos);

		GD.Print("Wire positions: ", WirePos);
	}

	/*     var vec = get_viewport().get_mouse_position() - self.position # getting the vector from self to the mouse
    vec = vec.normalized() * delta * SPEED # normalize it and multiply by time and speed
    position += vec # move by that vector

	*/

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void ClickGridSection(Node Who) {
		GD.Print("Clicked ", Who.Name);
		int GetIndex;
		if (CurClick == null) {
			GD.Print("First click");
			GetIndex = GridNodes.IndexOf(Who);
			EmitSignal("ClickNode", (Icon)Who);
			CurClick = GridNodes[GetIndex];
		} else {
			GD.Print("Second click");
			GetIndex = GridNodes.IndexOf(Who);
			SwapClick = GridNodes[GetIndex];
			EmitSignal("ChangeGrid", CurClick.Name, (Icon)SwapClick);
			CurClick = null;
			SwapClick = null;
			EmitSignal("TriggerDialogue", "Level0");
		}
	}

	public void OnChangeActiveNode(Icon WhichNode) {
		GD.Print("Active Node!", WhichNode.Name);
		ActiveIcon = WhichNode;
		if (GridNodes.IndexOf((Node)ActiveIcon) == GridNodes.Count - 1) {
			if (ActiveIcon.ExitPos == Position.Right) {
				GD.Print("Reached end and successful!!!");
				LevelSuccess = true;
			}
			else {
				GD.Print("Reached end and NOT successful");
			}
			LevelEnd = true;
			EmitSignal("EndLevel");
		}
	}

	private void OnEndLevel() {
		GD.Print("Woop");
		EmitSignal("TriggerDialogue", CurLevel + "End");
	}

	public void OnChangeLevel(string WhichLevel) {
		GD.Print(WhichLevel);
		CurLevel = WhichLevel;
		string PathToLevel = "res://scenes/levels/" + WhichLevel + ".tscn";
		GetTree().ChangeSceneToFile(PathToLevel);
	}

	public void OnEndDialogue() {
		GD.Print("Dialogue over");
		DialogueActive = false;
	}

	public void OnHelpPressed() {
		GD.Print("Help pressed");
	}

	public void OnPausePressed() {
		GD.Print("Pause Pressed");
	}

}
