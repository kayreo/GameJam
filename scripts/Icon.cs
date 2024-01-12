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

	private Godot.Collections.Dictionary Adj = new Godot.Collections.Dictionary() {
			{0, -1},
			{1, -1},
			{2, -1},
			{3, -1}
		};

	private BoardManager BoardManager;

	[Export]
	public bool IsClicked = false;

	string WireType;

	[Export]
	public int Lifespan = 0;
	
	private int MaxLifespan = 0;

	[Export]
	public BoardManager.Position EnterPos;

	[Export]
	public BoardManager.Position ExitPos;

	[Export]
	public BoardManager.Position TargetConnect;

	private AnimatedSprite2D WireAnim;

	private bool WireFilling = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		BoardManager.SetGridInfo += OnSetInfo;
		BoardManager.ChangeGrid += OnChangeGrid;
		WireAnim = GetNode<AnimatedSprite2D>("WireAnim");
		IsClicked = false;
		GetAdjacents();
		if (Name == "0") {
			OnNodeStart();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Name == "0") {
			//GD.Print(WireAnim.Frame);
		}
		if (WireFilling) { 
			if (WireAnim.Frame == MaxLifespan) {
				WireFilling = false;
				WireAnim.SpeedScale = 0;
				// get exit position grid
				Icon Exit = (Icon)Adj[(int)ExitPos];
				GD.Print("Exiting into: ", Exit.Name);
				GD.Print("Targeting: ", TargetConnect);
				GD.Print("Entering from: ", Exit.EnterPos);
				GD.Print("Or: ", Exit.ExitPos);
				bool reverse = false;
				// Can enter from Left, Right, Top, Bottom
				if (TargetConnect == Exit.EnterPos || TargetConnect == Exit.ExitPos) {
					GD.Print("Connecting");
					if (TargetConnect == Exit.ExitPos) {
						BoardManager.Position OriginalPos = Exit.EnterPos;
						Exit.EnterPos = Exit.ExitPos;
						Exit.ExitPos = OriginalPos;
						GD.Print("Need to reverse");
						reverse = true;
					}
					OnSuccessEnter(Exit, reverse);
				}
			}
			// 	if (WireType.Contains("Straight")) {
			// 		// Two straights: 
			// 		if (Exit.WireType.Contains("Straight")) {

			// 		} 
			// 		else if (Exit.WireType.Contains("Elbow")) {

			// 		}
			// 	} else if (WireType.Contains("Elbow")) {
			// 		if (Exit.WireType.Contains("Straight")) {

			// 		} 
			// 		else if (Exit.WireType.Contains("Elbow")) {

			// 		}
			// 	}
			// }
		}
	}

	private void OnSetInfo(string WhichNode, string TypeName) {
		if (WhichNode.Equals(Name)) {
			//GD.Print("Setting ", Name);
			//GD.Print("Setting type to: ", TypeName);
			WireType = TypeName;
			if (WireType.Contains("Loop")) {
				MaxLifespan = 9;
			} else {
				MaxLifespan = 5;
			}
		}
	}

	private void _OnArea2DInputEvent(Node viewport, InputEvent @event, int shape_idx) {
		if (WireAnim.Frame == 0) {
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
					TargetConnect += 1;
					if ((int)TargetConnect >= 4) {
						TargetConnect = BoardManager.Position.Left;
					}
					//EnterPos = (EnterPos + 1) >= BoardManager.Position.Bottom ? EnterPos + 1 : 0;
					//ExitPos = (ExitPos + 1) >= BoardManager.Position.Bottom ? ExitPos + 1 : 0;
					GD.Print("New 1 2: ", EnterPos, " ", ExitPos);
					GD.Print("Target: ", TargetConnect);
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

			// Animation
			string OriginalAnim = GetNode<AnimatedSprite2D>("WireAnim").Animation;
			GetNode<AnimatedSprite2D>("WireAnim").Animation = NodeToSwap.GetNode<AnimatedSprite2D>("WireAnim").Animation;
			NodeToSwap.GetNode<AnimatedSprite2D>("WireAnim").Animation = OriginalAnim;

			// Texture
			Texture2D OriginalSprite = GetNode<Sprite2D>("Wire").Texture;
			GetNode<Sprite2D>("Wire").Texture = NodeToSwap.GetNode<Sprite2D>("Wire").Texture;
			NodeToSwap.GetNode<Sprite2D>("Wire").Texture = OriginalSprite;

			// Rotations
			float OriginalRot = GetNode<Sprite2D>("Wire").RotationDegrees;
			GetNode<Sprite2D>("Wire").RotationDegrees = NodeToSwap.GetNode<Sprite2D>("Wire").RotationDegrees;
			GetNode<AnimatedSprite2D>("WireAnim").RotationDegrees = NodeToSwap.GetNode<Sprite2D>("Wire").RotationDegrees;

			NodeToSwap.GetNode<Sprite2D>("Wire").RotationDegrees = OriginalRot;
			NodeToSwap.GetNode<AnimatedSprite2D>("WireAnim").RotationDegrees = OriginalRot;

			// Enter Exit positions
			BoardManager.Position OriginalPos = EnterPos;
			EnterPos = NodeToSwap.EnterPos;
			NodeToSwap.EnterPos = OriginalPos;
			OriginalPos = ExitPos;
			ExitPos = NodeToSwap.ExitPos;
			NodeToSwap.ExitPos = OriginalPos;
			OriginalPos = TargetConnect;
			TargetConnect = NodeToSwap.TargetConnect;
			NodeToSwap.TargetConnect = OriginalPos;

			// Lifespan
			int OriginalMaxLifespan = MaxLifespan;
			MaxLifespan = NodeToSwap.MaxLifespan;
			NodeToSwap.MaxLifespan = OriginalMaxLifespan;

			// Wire type
			string OriginalWireType = WireType;
			WireType = NodeToSwap.WireType;
			NodeToSwap.WireType = OriginalWireType;

			// Reset
			GetNode<TextureRect>("SelectBorder").Hide();
			NodeToSwap.GetNode<TextureRect>("SelectBorder").Hide();
			IsClicked = false;
			NodeToSwap.IsClicked = false;
		}
	}

	private void OnNodeStart() {
		WireAnim.Play();
		WireFilling = true;
	}

	private void OnSuccessEnter(Icon StartNode, bool reverse) {
		if (reverse) {
			StartNode.GetNode<AnimatedSprite2D>("WireAnim").Animation = StartNode.WireType + "R";
			if (StartNode.ExitPos == BoardManager.Position.Bottom) {
				StartNode.TargetConnect = BoardManager.Position.Top;
			} else if (StartNode.ExitPos == BoardManager.Position.Top) {
				StartNode.TargetConnect = BoardManager.Position.Bottom;
			} else if (StartNode.ExitPos == BoardManager.Position.Left) {
				StartNode.TargetConnect = BoardManager.Position.Right;
			} else if (StartNode.ExitPos == BoardManager.Position.Right) {
				StartNode.TargetConnect = BoardManager.Position.Left;
			}
			GD.Print("Reversed: ", StartNode.TargetConnect);
		}
		StartNode.OnNodeStart();
	}

	public void GetAdjacents() {

		// Need to get icons left, right, above, below
		string CurRow = GetParent().Name;
		int CurRowNum = CurRow[3] - '0';
		int AboveRow = CurRowNum - 1;
		int BelowRow = CurRowNum + 1;
		int IndexOfSelf = GetParent().GetChildren().IndexOf(this);
		int LeftIndex = IndexOfSelf - 1;
		int RightIndex = IndexOfSelf + 1;

		if (LeftIndex >= 0) {
			Adj[0] = ((Icon)GetParent().GetChild(LeftIndex));
		}

		if (AboveRow >= 0) {
			//GD.Print("Getting: ", GetParent().GetParent().GetNode<HBoxContainer>("Row" + AboveRow).GetChild(IndexOfSelf).Name);
			Adj[1] = ((Icon)GetParent().GetParent().GetNode<HBoxContainer>("Row" + AboveRow).GetChild(IndexOfSelf));
		}

		if (RightIndex < GetParent().GetChildCount()) {
			Adj[2] = ((Icon)GetParent().GetChild(RightIndex));
		}

		if (BelowRow < GetParent().GetParent().GetChildCount()) {
			//GD.Print("Below adding ", " Row" + BelowRow);
			//GD.Print("Getting: ", GetParent().GetParent().GetNode<HBoxContainer>("Row" + BelowRow).GetChild(IndexOfSelf).Name);
			Adj[3] = ((Icon)GetParent().GetParent().GetNode<HBoxContainer>("Row" + BelowRow).GetChild(IndexOfSelf));
		}

		// GD.Print("Adjacents loaded for ", Name);
		// Icon printName1 = (Icon)result[(int)BoardManager.Position.Left];
		// if (printName1 != null) {
		// 	GD.Print("Left ", printName1.Name);
		// }
		// Icon printName2 = (Icon)result[(int)BoardManager.Position.Top];
		// if (printName2 != null) {
		// 	GD.Print("Above, ", printName2.Name);
		// }
		// Icon printName3 = (Icon)result[(int)BoardManager.Position.Right];
		// if (printName3 != null) {
		// 	GD.Print("Right, ", printName3.Name);
		// }
		// Icon printName4 = (Icon)result[(int)BoardManager.Position.Bottom];
		// if (printName4 != null) {
		// 	GD.Print("Below, ", printName4.Name);
		// }

		//GD.Print("Index of me: ", IndexOfSelf);
		//GD.Print("Above: ", AboveRow);
		//GD.Print("Below: ", BelowRow);
		//GD.Print("Row" + CurRowNum);
	}

}
