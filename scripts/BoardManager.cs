using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BoardManager : Node
{
	Godot.Collections.Array<Node> GridNodes;

	Godot.Collections.Array<Node> Wires;
	
	[Signal]
	public delegate void ChangeLevelEventHandler(string WhichLevel);

	public bool DialogueActive = false;

	public bool GameRunning = false;

	public bool LevelEnd = false;

	public bool LevelSuccess = false;

	public string CurLevel;

	public int CurLevelIndex = 0;

	public bool Muted = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	/*     var vec = get_viewport().get_mouse_position() - self.position # getting the vector from self to the mouse
    vec = vec.normalized() * delta * SPEED # normalize it and multiply by time and speed
    position += vec # move by that vector

	*/

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}


	public void OnChangeLevel(string WhichLevel) {
		CurLevel = WhichLevel;
		GD.Print("Entering ", WhichLevel);
		string PathToLevel = "res://scenes/levels/" + WhichLevel + ".tscn";
		//var scene = GD.Load<PackedScene>(PathToLevel);
		//var instance = scene.Instantiate();
	
		//GetParent().GetNode<Node>("Game").AddChild(instance);
		GetTree().ChangeSceneToFile(PathToLevel);
	}

	public void OnFirstEndDialogue() {
		GD.Print("Dialogue over");
		DialogueActive = false;
		GameRunning = true;
	}

	public void OnEndDialogue() {
		GD.Print("Dialogue over");
		DialogueActive = false;
		GameRunning = false;
		CurLevelIndex++;
		if (CurLevelIndex >= 3) {
			EmitSignal("ChangeLevel", "WinScreen");
		} else {
			EmitSignal("ChangeLevel", "Level" + CurLevelIndex);
		}
	}



}
