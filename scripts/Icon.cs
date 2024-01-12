using Godot;
using System;
using System.Linq;

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
		GetAdjacents();
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

	public Godot.Collections.Dictionary GetAdjacents() {
		Godot.Collections.Dictionary result = new Godot.Collections.Dictionary() {
			{0, -1},
			{1, -1},
			{2, -1},
			{3, -1}
		};
		// Need to get icons left, right, above, below
		string CurRow = GetParent().Name;
		int CurRowNum = CurRow[3] - '0';
		int AboveRow = CurRowNum - 1;
		int BelowRow = CurRowNum + 1;
		int IndexOfSelf = GetParent().GetChildren().IndexOf(this);
		int LeftIndex = IndexOfSelf - 1;
		int RightIndex = IndexOfSelf + 1;

		if (LeftIndex >= 0) {
			result[0] = ((Icon)GetParent().GetChild(LeftIndex));
		}

		if (AboveRow >= 0) {
			//GD.Print("Getting: ", GetParent().GetParent().GetNode<HBoxContainer>("Row" + AboveRow).GetChild(IndexOfSelf).Name);
			result[1] = ((Icon)GetParent().GetParent().GetNode<HBoxContainer>("Row" + AboveRow).GetChild(IndexOfSelf));
		}

		if (RightIndex < GetParent().GetChildCount()) {
			result[2] = ((Icon)GetParent().GetChild(RightIndex));
		}

		if (BelowRow < GetParent().GetParent().GetChildCount()) {
			//GD.Print("Below adding ", " Row" + BelowRow);
			//GD.Print("Getting: ", GetParent().GetParent().GetNode<HBoxContainer>("Row" + BelowRow).GetChild(IndexOfSelf).Name);
			result[3] = ((Icon)GetParent().GetParent().GetNode<HBoxContainer>("Row" + BelowRow).GetChild(IndexOfSelf));
		}

		GD.Print("Adjacents loaded for ", Name);
		Icon printName1 = (Icon)result[(int)BoardManager.Position.Left];
		if (printName1 != null) {
			GD.Print("Left ", printName1.Name);
		}
		Icon printName2 = (Icon)result[(int)BoardManager.Position.Top];
		if (printName2 != null) {
			GD.Print("Above, ", printName2.Name);
		}
		Icon printName3 = (Icon)result[(int)BoardManager.Position.Right];
		if (printName3 != null) {
			GD.Print("Right, ", printName3.Name);
		}
		Icon printName4 = (Icon)result[(int)BoardManager.Position.Bottom];
		if (printName4 != null) {
			GD.Print("Below, ", printName4.Name);
		}

		//GD.Print("Index of me: ", IndexOfSelf);
		//GD.Print("Above: ", AboveRow);
		//GD.Print("Below: ", BelowRow);
		//GD.Print("Row" + CurRowNum);
		return result;
	}

}
