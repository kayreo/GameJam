using Godot;
using System;

public partial class Board : Node
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

	private Icon ClickedNode;

	protected BoardManager BoardManager;

	protected Godot.Collections.Array<Node> GridNodes;

	protected Godot.Collections.Array<Node> Wires;

	private Node CurClick;
	
	private Node SwapClick;

	private Icon ActiveIcon;

	[Signal]
	public delegate void SetDataEventHandler();

	[Signal]
	public delegate void SetGridInfoEventHandler(string WhichNode, string TypeNam);

	[Signal]
	public delegate void ChangeGridEventHandler(string WhichNode, Icon NodeToSwap);

	[Signal]
	public delegate void ClickNodeEventHandler(Icon WhichNode);

	[Signal]
	public delegate void EndLevelEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		ClickNode += OnClickNode;
		SetData += OnSetData;
		EmitSignal("SetData");

		EndLevel += OnEndLevel;

		HUD HUD = (HUD)GetParent().GetNode<CanvasLayer>("HUD");

		HUD.StartGame += OnGameStart;

		GridNodes = GetTree().GetNodesInGroup("Grid");
		Wires = GetTree().GetNodesInGroup("Wire");
	//	GD.Print("Nodes: ", GridNodes);

		// Always set first tile to be a straight wire
		Icon GridNode = (Icon)GridNodes[0];
		TextureRect WireNode = (TextureRect)Wires[3];
		GridNode.GetNode<Sprite2D>("Wire").Texture = WireNode.Texture;
		GridNode.GetNode<AnimatedSprite2D>("WireAnim").Animation = WireNode.Name;

		EmitSignal("SetGridInfo", GridNode.Name, WireNode.Name);

		Godot.Collections.Array<Position> ToAddPos = WirePos[1];

		GridNode.EnterPos = ToAddPos[0];
		GridNode.ExitPos = ToAddPos[1];
		GridNode.TargetConnect = Position.Left;

		// Randomly fill other tiles
		for (int i = 1; i < GridNodes.Count; i++) {
			//GD.Print(GridNodes[i].Name);
			GridNode = (Icon)GridNodes[i];
			WireNode = (TextureRect)Wires.PickRandom();
			string WireName = WireNode.Name;

		//	GD.Print("Wire type: ", WireName);

			if (WireName.Contains("Elbow")) {
				ToAddPos = WirePos[0];
				GridNode.TargetConnect = Position.Bottom;
			}

			if (WireName.Contains("Straight")) {
				ToAddPos = WirePos[1];
				GridNode.TargetConnect = Position.Left;
			}

			GridNode.EnterPos = ToAddPos[0];
			GridNode.ExitPos = ToAddPos[1];

		//	GD.Print("Starting positions: ", GridNode.EnterPos, " ", GridNode.ExitPos);

			GridNode.GetNode<Sprite2D>("Wire").Texture = WireNode.Texture;
			GridNode.GetNode<AnimatedSprite2D>("WireAnim").Animation = WireName;
			//GridNode.GetNode<Sprite2D>("WireFilling").Texture = WireNode.GetNode<TextureRect>("Full").Texture;
			EmitSignal("SetGridInfo", GridNode.Name, WireNode.Name);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// if (ClickedNode != null) {
		// 	GD.Print(GetViewport().GetMousePosition());
		// 	Sprite2D Follow = GetNode<Sprite2D>("FollowBox");
		// 	Follow.Texture = ClickedNode.Texture;
		// 	Vector2 Pos = GetViewport().GetMousePosition() - Follow.Position;
		// 	Follow.Position = Pos * (float)delta * 10000;
		// 	//GD.Print("Sprite pos: ", Follow.Position);
		// }
	}

	public void OnGameStart() {
	//	GD.Print("Starting Game");
		Icon GridNode = (Icon)GridNodes[0];
		BoardManager.GameRunning = true;
		GridNode.GetNode<AnimatedSprite2D>("WireAnim").SpeedScale = 0.25F;
		GridNode.OnNodeStart();
	}

	private void OnClickNode(Icon WhichNode) {
		ClickedNode = WhichNode;
	//	GD.Print("Node: ", WhichNode.Name);
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

	//	GD.Print("Wire positions: ", WirePos);
	}


	public void ClickGridSection(Node Who) {
	//	GD.Print("Clicked ", Who.Name);
		int GetIndex;
		if (CurClick == null) {
		//	GD.Print("First click");
			GetIndex = GridNodes.IndexOf(Who);
			EmitSignal("ClickNode", (Icon)Who);
			CurClick = GridNodes[GetIndex];
		} else {
		//	GD.Print("Second click");
			GetIndex = GridNodes.IndexOf(Who);
			SwapClick = GridNodes[GetIndex];
			EmitSignal("ChangeGrid", CurClick.Name, (Icon)SwapClick);
			CurClick = null;
			SwapClick = null;
		}
	}


	public void OnChangeActiveNode(Icon WhichNode) {
	//	GD.Print("Active Node!", WhichNode.Name);
		ActiveIcon = WhichNode;
		if (GridNodes.IndexOf((Node)ActiveIcon) == GridNodes.Count - 1) {
			if (ActiveIcon.ExitPos == Board.Position.Right) {
				GD.Print("Reached end and successful!!!");
				BoardManager.LevelSuccess = true;
			}
			else {
				GD.Print("Reached end and NOT successful");
			}
			BoardManager.LevelEnd = true;
			EmitSignal("EndLevel");
		}
	}

	public void OnEndLevel() {
		GD.Print("Woop");
		if (BoardManager.LevelSuccess) {
			GetParent().GetNode<CanvasLayer>("HUD").EmitSignal("TriggerDialogue", BoardManager.CurLevel + "End");
			GetParent().GetNode<CanvasLayer>("GameOver").GetNode<TextureRect>("Win").Show();
			GetParent().GetNode<CanvasLayer>("HUD").GetNode<TextureRect>("Border").GetNode<AnimatedSprite2D>("Phone").Frame = 1;
			if (!BoardManager.Muted) {
				GetParent().GetNode<CanvasLayer>("HUD").GetNode<AudioStreamPlayer>("Music").Stop();
				GetParent().GetNode<CanvasLayer>("HUD").GetNode<AudioStreamPlayer>("WinJingle").Play();
			}
		} else {
			GetParent().GetNode<CanvasLayer>("GameOver").GetNode<TextureRect>("Lose").Show();
			GetParent().GetNode<CanvasLayer>("GameOver").GetNode<TextureButton>("BackButton").Show();
		}
	}

}
