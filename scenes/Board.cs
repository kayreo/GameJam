using Godot;
using System;

public partial class Board : Node
{
	private Icon ClickedNode;

	private BoardManager BoardManager;

	Godot.Collections.Array<Node> GridNodes;

	Godot.Collections.Array<Node> Wires;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		BoardManager.ClickNode += OnClickNode;
		GridNodes = GetTree().GetNodesInGroup("Grid");
		Wires = GetTree().GetNodesInGroup("Wire");
		for (int i = 0; i < GridNodes.Count; i++) {
			//GD.Print(GridNodes[i]);
			TextureRect GridNode = (TextureRect)GridNodes[i];
			TextureRect WireNode = (TextureRect)Wires.PickRandom();
			GridNode.Texture = WireNode.Texture;
			BoardManager.EmitSignal("SetGridInfo", GridNode.Name, WireNode.Name, 0.0);
		}
	}

/*     var vec = get_viewport().get_mouse_position() - self.position # getting the vector from self to the mouse
    vec = vec.normalized() * delta * SPEED # normalize it and multiply by time and speed
    position += vec # move by that vector
	*/

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (ClickedNode != null) {
			GD.Print(GetViewport().GetMousePosition());
			Sprite2D Follow = GetNode<Sprite2D>("FollowBox");
			Follow.Texture = ClickedNode.Texture;
			Vector2 Pos = GetViewport().GetMousePosition() - Follow.Position;
			Follow.Position = Pos * (float)delta * 10000;
			//GD.Print("Sprite pos: ", Follow.Position);
		}
	}

	private void OnClickNode(Icon WhichNode) {
		ClickedNode = WhichNode;
		GD.Print("Node: ", WhichNode.Name);
	}

}
