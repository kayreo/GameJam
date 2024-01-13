using Godot;
using System;
using System.IO;
using System.Linq;

public partial class HUD : CanvasLayer
{
	private TextureRect SubMenu;

	private TextureRect DialogueBox;

	private TextureRect Sidebar;

	private Json jsonLoader;

	private Godot.Collections.Dictionary DialogueScenarios;

	private Godot.Collections.Dictionary DialogueData;

	protected BoardManager BoardManager;

	private RichTextLabel Dialogue;

	private AnimatedSprite2D Speaker0;

	private AnimatedSprite2D Speaker1;

	[Export]
	public string CurSpeaker0 = "0";

	[Export]
	public string CurSpeaker1 = "0";

	private bool DialogueActive = false;

	private bool InProgress = false;

	private int CurrentLineIndex = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");

		SubMenu = GetNode<TextureRect>("SubMenu");
		DialogueBox = GetNode<TextureRect>("DialogueBox");
		Sidebar = GetNode<TextureRect>("Sidebar");
		foreach (Node n in SubMenu.GetChildren()) {
			Button b = (Button)n;
			GD.Print("Button: ", b.Name);
		}

		jsonLoader = new Json();

		jsonLoader.Parse(File.ReadAllText("sprites/HUD/DialogueData.json"));
		DialogueScenarios = (Godot.Collections.Dictionary)jsonLoader.Data;

		GD.Print("Scenarios: ", DialogueScenarios);

		BoardManager.TriggerDialogue += OnDialogueTrigger;
		Dialogue = DialogueBox.GetNode<RichTextLabel>("Dialogue");
		Speaker0 = DialogueBox.GetNode<AnimatedSprite2D>("Speaker0");
		Speaker0.Animation = "player" + CurSpeaker0;
		Speaker1 = DialogueBox.GetNode<AnimatedSprite2D>("Speaker1");
		Speaker1.Animation = "player" + CurSpeaker1;
		//Dialogue.VisibleCharacters = 0;
		BoardManager.EmitSignal("TriggerDialogue", "Level0");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (DialogueActive) {
			Dialogue.Show();
			// Printing out a line
			// if (InProgress) {
			// 	// scroll text
			// 	if (Dialogue.VisibleCharacters < Dialogue.Text.Length) {
			// 		Dialogue.VisibleCharacters += 1;
			// 		if (Input.IsActionJustReleased("click")) {
			// 			Dialogue.VisibleCharacters = -1;
			// 			InProgress = false;
			// 		}
			// 	} 
			// 	// text done scrolling, set visible chars to all
			// 	else if (Dialogue.VisibleCharacters >= Dialogue.Text.Length) {
			// 		Dialogue.VisibleCharacters = -1;
			// 		InProgress = false;
			// 	}
			// }
			// Move to next line
			//else {
				if (Input.IsActionJustReleased("click")) {
					ContinueDialogue();
				}
			//}
		} else {
			Dialogue.Hide();
			Speaker0.Frame = 0;
			Speaker1.Frame = 0;
		}
	}

	// Triggers to begin to dialogue and to continue dialogue
	// Loads secnario to be printed
	// DialogueLine parameter is the group of lines to grab from the JSON file
	private void OnDialogueTrigger(string Scenario) {
		DialogueData = (Godot.Collections.Dictionary)DialogueScenarios[Scenario];
		DialogueActive = true;
		ContinueDialogue();
	}

	private void ContinueDialogue() {
		if (DialogueData.ContainsKey(CurrentLineIndex.ToString())) {
			String[] DialogueLine = (String[])DialogueData[CurrentLineIndex.ToString()];
			//GD.Print("Dialogue: ", DialogueLine[0]);
			string Who0 = (string)DialogueLine[0];
			//Speaker0.Animation = "player" + Who0[0];
			Speaker0.Frame = (int)Who0[1];
			//GD.Print("Dialogue2: ", DialogueLine[1]);
			string Who1 = (string)DialogueLine[1];
			//Speaker1.Animation = "player" + Who1[0];
			Speaker1.Frame = (int)Who1[1];

			Dialogue.Text = DialogueLine[2];
			
			InProgress = true;
			
			CurrentLineIndex++;
			// GD.Print("Visible: ", DialogueBox.VisibleCharacters);
			// GD.Print("Total: ", DialogueBox.Text.Length);
		} else {
			// key not found, stop dialogue
			DialogueActive = false;
		}
	}

	// X: 0: Neutral, 1: Shocked, 2: Sad, 3: Happy
	// Y: 0: Alchemist, 1: Demon, 2: Fox
	private void ChangePortrait(Vector2I WhichPortrait) {
		GD.Print("Chef");
		GetNode<TileMap>("PortraitControl/Portrait").SetCell(0, new Vector2I(0,0), 0, WhichPortrait);
	}
}
