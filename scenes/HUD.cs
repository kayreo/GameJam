using Godot;
using System;
using System.IO;
using System.Linq;

public partial class HUD : CanvasLayer
{
	private TextureRect SubMenu;

	private TextureRect DialogueBox;

	private AnimatedSprite2D Sidebar;

	private AnimatedSprite2D Phone;

	private Json jsonLoader;

	private Godot.Collections.Dictionary DialogueScenarios;

	private Godot.Collections.Dictionary DialogueData;

	protected BoardManager BoardManager;

	private RichTextLabel Dialogue;

	private AnimatedSprite2D Speaker0;

	private AnimatedSprite2D Speaker1;

	private TextureButton MusicButton;

	private TextureButton HelpButton;

	private TextureButton PauseButton;

	private AudioStreamPlayer MusicPlayer;

	private string CurrentScenario;

	[Export]
	public string CurSpeaker0 = "0";

	[Export]
	public string CurSpeaker1 = "0";

	private bool InProgress = false;

	private int CurrentLineIndex = 0;


	[Signal]
	public delegate void EndFirstDialogueEventHandler();

	[Signal]
	public delegate void EndDialogueEventHandler();

	[Signal]
	public delegate void TriggerDialogueEventHandler(string DialogueText);

	
	[Signal]
	public delegate void StartGameEventHandler();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BoardManager = GetNode<BoardManager>("/root/BoardManager");
		EndDialogue += BoardManager.OnEndDialogue;
		EndFirstDialogue += BoardManager.OnFirstEndDialogue;
		SubMenu = GetNode<TextureRect>("SubMenu");
		DialogueBox = GetNode<TextureRect>("DialogueBox");
		Sidebar = GetNode<AnimatedSprite2D>("Border/Sidebar");
		Phone = GetNode<AnimatedSprite2D>("Border/Phone");
		foreach (Node n in SubMenu.GetChildren()) {
			TextureButton b = (TextureButton)n;
			GD.Print("Button: ", b.Name);
		}

		MusicButton = SubMenu.GetNode<TextureButton>("MusicButton");
		MusicButton.Pressed += OnMusicPressed;

		HelpButton = SubMenu.GetNode<TextureButton>("HelpButton");
		HelpButton.Pressed += BoardManager.OnHelpPressed;

		PauseButton = SubMenu.GetNode<TextureButton>("PauseButton");
		PauseButton.Pressed += BoardManager.OnPausePressed;

		MusicPlayer = GetNode<AudioStreamPlayer>("Music");

		jsonLoader = new Json();

		jsonLoader.Parse(File.ReadAllText("sprites/HUD/DialogueData.json"));
		DialogueScenarios = (Godot.Collections.Dictionary)jsonLoader.Data;

		GD.Print("Scenarios: ", DialogueScenarios);

		TriggerDialogue += OnDialogueTrigger;
		Dialogue = DialogueBox.GetNode<RichTextLabel>("Dialogue");
		Speaker0 = DialogueBox.GetNode<AnimatedSprite2D>("Speaker0");
		GD.Print("Speaker got: ", Speaker0.Name);
		Speaker0.Animation = "player" + CurSpeaker0;
		Speaker1 = DialogueBox.GetNode<AnimatedSprite2D>("Speaker1");
		Speaker1.Animation = "player" + CurSpeaker1;
		//Dialogue.VisibleCharacters = 0;
		if (!MusicPlayer.Playing && !BoardManager.Muted) {
			MusicPlayer.Play();
		}
		EmitSignal("TriggerDialogue", BoardManager.CurLevel + "Start");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (BoardManager.DialogueActive) {
			Dialogue.Show();
			//Printing out a line
			if (InProgress) {
				// scroll text
				if (Dialogue.VisibleCharacters < Dialogue.Text.Length) {
					Dialogue.VisibleCharacters += 1;
					if (Input.IsActionJustReleased("click")) {
						Dialogue.VisibleCharacters = -1;
						InProgress = false;
					}
				} 
				// text done scrolling, set visible chars to all
				else if (Dialogue.VisibleCharacters >= Dialogue.Text.Length) {
					Dialogue.VisibleCharacters = -1;
					InProgress = false;
				}
			}
			//Move to next line
			else {
				if (Input.IsActionJustReleased("click")) {
					ContinueDialogue();
				}
			}
		} else {
			Dialogue.Hide();
			CurrentLineIndex = 0;
			Speaker0.Frame = 0;
			Speaker1.Frame = 0;
		}
	}

	// Triggers to begin to dialogue and to continue dialogue
	// Loads secnario to be printed
	// DialogueLine parameter is the group of lines to grab from the JSON file
	private void OnDialogueTrigger(string Scenario) {
		GD.Print("Received signal", Scenario);
		DialogueData = (Godot.Collections.Dictionary)DialogueScenarios[Scenario];
		CurrentScenario = Scenario;
		BoardManager.DialogueActive = true;
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
			BoardManager.DialogueActive = false;
			if (CurrentScenario.Contains("Start")) {
				EmitSignal("EndFirstDialogue");
				EmitSignal("StartGame");
			} else {
				EmitSignal("EndDialogue");
			}
		}
	}

	// X: 0: Neutral, 1: Shocked, 2: Sad, 3: Happy
	// Y: 0: Alchemist, 1: Demon, 2: Fox
	private void ChangePortrait(Vector2I WhichPortrait) {
		GD.Print("Chef");
		GetNode<TileMap>("PortraitControl/Portrait").SetCell(0, new Vector2I(0,0), 0, WhichPortrait);
	}

	public void OnMusicPressed() {
		GD.Print("Music pressed");
		MusicPlayer.Playing = !MusicPlayer.Playing;
		BoardManager.Muted = !BoardManager.Muted;
	}
}
