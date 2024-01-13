using Godot;
using Godot.NativeInterop;
using System;

// This board type reveals pieces of the board as the wire fills
public partial class ReducedVisBoard : Board
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Icon CurNode;
		for (int i = 1; i < GridNodes.Count; i++) {
			CurNode = (Icon)GridNodes[i];
			CurNode.GetNode<Sprite2D>("Wire").Hide();
			CurNode.ChangeActiveNode += RevealNodes;
			//CurNode.GetNode<AnimatedSprite2D>("WireAnim").Hide();
		}
		CurNode = (Icon)GridNodes[0];
		RevealNodes(CurNode);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	private void RevealNodes(Icon WhichNode) {
		foreach (int i in WhichNode.Adj.Keys) {
			Icon NodeToReveal = (Icon)WhichNode.Adj[i];
			if (NodeToReveal != null) {
				GD.Print("Revealing: ", NodeToReveal.Name);
				NodeToReveal.GetNode<Sprite2D>("Wire").Show();
			}
		}
	}
}
