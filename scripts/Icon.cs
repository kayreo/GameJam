using Godot;
using System;

public partial class Icon : TextureRect
{
	private BoardManager BoardManager;

	string WireType;

	[Export]
	public double Lifespan = 0.0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		BoardManager.SetGridInfo += OnSetInfo;
		BoardManager.ChangeGrid += OnChangeGrid;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void OnSetInfo(string WhichNode, string TypeName, float CurLifetime) {
		if (WhichNode.Equals(Name)) {
			GD.Print("Setting ", Name);
			GD.Print("Setting type to: ", TypeName);
			WireType = TypeName;
			Lifespan = CurLifetime;
		}
	}

	private void _OnArea2DInputEvent(Node viewport, InputEvent @event, int shape_idx) {
		if (@event is InputEventMouseButton && @event.IsPressed()) {
			GD.Print("Meow");
			BoardManager.ClickGridSection(this);
		}
	} 

	private void OnChangeGrid(string WhichNode, Icon NodeToSwap) {
		if (WhichNode.Equals(Name)) {
			GD.Print("Swapping");
			Texture2D OriginalTexture = Texture;
			double OriginalLifespan = Lifespan;
			Texture = NodeToSwap.Texture;
			Lifespan = NodeToSwap.Lifespan;
			NodeToSwap.Lifespan = Lifespan;
			NodeToSwap.Texture = OriginalTexture;
		}
	}

}
