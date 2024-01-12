using Godot;
using System;

public partial class Icon : TextureRect
{
	/*
	Exit points: Top, bottom, left, right

	Entrance points: Top, bottom, left, right

	A tile's exit point must be the opposite of the connecting entrance point to connect successfully
	If not, game over
	*/
	private BoardManager BoardManager;

	[Export]
	public bool IsClicked = false;

	string WireType;

	[Export]
	public double Lifespan = 0.0;

	[Export]
	public BoardManager.Position EnterPos;

	[Export]
	public BoardManager.Position ExitPos;

	private AnimatedSprite2D WireAnim;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		BoardManager.SetGridInfo += OnSetInfo;
		BoardManager.ChangeGrid += OnChangeGrid;
		WireAnim = GetNode<AnimatedSprite2D>("WireAnim");
		WireAnim.Play();
		IsClicked = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void OnSetInfo(string WhichNode, string TypeName) {
		if (WhichNode.Equals(Name)) {
			//GD.Print("Setting ", Name);
			//GD.Print("Setting type to: ", TypeName);
			WireType = TypeName;
		}
	}

	private void _OnArea2DInputEvent(Node viewport, InputEvent @event, int shape_idx) {
		if (Lifespan == 0.0) {
			if (@event is InputEventMouseButton && @event.IsPressed()) {
				var btn = @event as InputEventMouseButton;
				if (btn.ButtonIndex == MouseButton.Left) {
					GD.Print("Meow");
					IsClicked = true;
					BoardManager.ClickGridSection(this);
				} else if (btn.ButtonIndex == MouseButton.Right) {
					GD.Print("Rotating");
					GetNode<Sprite2D>("Wire").RotationDegrees += 90;
					GetNode<AnimatedSprite2D>("WireAnim").RotationDegrees += 90;
					EnterPos += 1;
					if ((int)EnterPos >= 4) {
						EnterPos = BoardManager.Position.Left;
					}
					ExitPos += 1;
					if ((int)ExitPos >= 4) {
						ExitPos = BoardManager.Position.Left;
					}
					//EnterPos = (EnterPos + 1) >= BoardManager.Position.Bottom ? EnterPos + 1 : 0;
					//ExitPos = (ExitPos + 1) >= BoardManager.Position.Bottom ? ExitPos + 1 : 0;
					GD.Print("New Enter Exit: ", EnterPos, " ", ExitPos);
				}
			}
		}
	} 

	private void _OnArea2DMouseEntered() {
		//GetNode<TextureRect>("SelectBorder").Show();
	}

	private void _OnArea2DMouseExited() {
		if (!IsClicked) {
			GetNode<TextureRect>("SelectBorder").Hide();
		}
	}

	private void OnChangeGrid(string WhichNode, Icon NodeToSwap) {
		if (WhichNode.Equals(Name)) {
			GD.Print("Swapping");
			Texture2D OriginalSprite = GetNode<Sprite2D>("Wire").Texture;
			float OriginalRot = GetNode<Sprite2D>("Wire").RotationDegrees;
			GetNode<Sprite2D>("Wire").Texture = NodeToSwap.GetNode<Sprite2D>("Wire").Texture;
			GetNode<Sprite2D>("Wire").RotationDegrees = NodeToSwap.GetNode<Sprite2D>("Wire").RotationDegrees;
			NodeToSwap.GetNode<Sprite2D>("Wire").Texture = OriginalSprite;
			NodeToSwap.GetNode<Sprite2D>("Wire").RotationDegrees = OriginalRot;
			IsClicked = false;
			NodeToSwap.IsClicked = false;
			GetNode<TextureRect>("SelectBorder").Hide();
			NodeToSwap.GetNode<TextureRect>("SelectBorder").Hide();
		}
	}

}
