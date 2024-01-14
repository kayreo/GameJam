using Godot;
using System;

public partial class Board : Node
{
	private Icon ClickedNode;

	protected BoardManager BoardManager;

	protected Godot.Collections.Array<Node> GridNodes;

	protected Godot.Collections.Array<Node> Wires;

	[Signal]
	public delegate void SetDataEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		BoardManager.ClickNode += OnClickNode;
		SetData += BoardManager.OnSetData;
		EmitSignal("SetData");

		GridNodes = GetTree().GetNodesInGroup("Grid");
		Wires = GetTree().GetNodesInGroup("Wire");

		// Always set first tile to be a straight wire
		Icon GridNode = (Icon)GridNodes[0];
		TextureRect WireNode = (TextureRect)Wires[3];
		GridNode.GetNode<Sprite2D>("Wire").Texture = WireNode.Texture;
		GridNode.GetNode<AnimatedSprite2D>("WireAnim").Animation = WireNode.Name;

		BoardManager.EmitSignal("SetGridInfo", GridNode.Name, WireNode.Name);

		Godot.Collections.Array<BoardManager.Position> ToAddPos = BoardManager.WirePos[1];

		GridNode.EnterPos = ToAddPos[0];
		GridNode.ExitPos = ToAddPos[1];
		GridNode.TargetConnect = BoardManager.Position.Left;

		// Randomly fill other tiles
		for (int i = 1; i < GridNodes.Count; i++) {
			//GD.Print(GridNodes[i]);
			GridNode = (Icon)GridNodes[i];
			WireNode = (TextureRect)Wires.PickRandom();
			string WireName = WireNode.Name;

		//	GD.Print("Wire type: ", WireName);

			if (WireName.Contains("Elbow")) {
				ToAddPos = BoardManager.WirePos[0];
				GridNode.TargetConnect = BoardManager.Position.Bottom;
			}

			if (WireName.Contains("Straight")) {
				ToAddPos = BoardManager.WirePos[1];
				GridNode.TargetConnect = BoardManager.Position.Left;
			}

			GridNode.EnterPos = ToAddPos[0];
			GridNode.ExitPos = ToAddPos[1];

		//	GD.Print("Starting positions: ", GridNode.EnterPos, " ", GridNode.ExitPos);

			GridNode.GetNode<Sprite2D>("Wire").Texture = WireNode.Texture;
			GridNode.GetNode<AnimatedSprite2D>("WireAnim").Animation = WireName;
			//GridNode.GetNode<Sprite2D>("WireFilling").Texture = WireNode.GetNode<TextureRect>("Full").Texture;
			BoardManager.EmitSignal("SetGridInfo", GridNode.Name, WireNode.Name);
		}
		GridNode = (Icon)GridNodes[0];
		GridNode.GetNode<AnimatedSprite2D>("WireAnim").SpeedScale = 0.5F;
		GridNode.OnNodeStart();
	}

/*     var vec = get_viewport().get_mouse_position() - self.position # getting the vector from self to the mouse
    vec = vec.normalized() * delta * SPEED # normalize it and multiply by time and speed
    position += vec # move by that vector
	*/

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

	private void OnClickNode(Icon WhichNode) {
		ClickedNode = WhichNode;
		GD.Print("Node: ", WhichNode.Name);
	}

}
