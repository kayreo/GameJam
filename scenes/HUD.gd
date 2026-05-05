extends CanvasLayer

signal end_first_dialogue
signal end_dialogue
signal trigger_dialogue(dialogue_text: String)
signal start_game


var sub_menu: TextureRect
var dialogue_box: TextureRect
var sidebar: AnimatedSprite2D
var phone: AnimatedSprite2D

var json_loader
var dialogue_scenarios: Dictionary = {}
var dialogue_data: Dictionary = {}

var board_manager

var dialogue: RichTextLabel
var speaker0: AnimatedSprite2D
var speaker1: AnimatedSprite2D

var music_button: TextureButton
var help_button: TextureButton
var pause_button: TextureButton

var music_player: AudioStreamPlayer

var current_scenario: String = ""
var cur_speaker0: String = "0"
var cur_speaker1: String = "0"

var in_progress: bool = false
var current_line_index: int = 0

var start_button: TextureButton

# ----------------------------
# READY
# ----------------------------
func _ready():
	board_manager = BoardManager

	# connect board manager reactions
	if not end_dialogue.is_connected(board_manager.on_end_dialogue):
		end_dialogue.connect(board_manager.on_end_dialogue)

	if not end_first_dialogue.is_connected(board_manager.on_first_end_dialogue):
		end_first_dialogue.connect(board_manager.on_first_end_dialogue)

	sub_menu = get_node("SubMenu")
	dialogue_box = get_node("DialogueBox")
	sidebar = get_node("Border/Sidebar")
	phone = get_node("Border/Phone")

	music_button = sub_menu.get_node("MusicButton")
	help_button = sub_menu.get_node("HelpButton")
	pause_button = sub_menu.get_node("PauseButton")
	
	start_button = get_node("PlayButton")

	music_button.pressed.connect(on_music_pressed)
	help_button.pressed.connect(on_help_pressed)
	pause_button.pressed.connect(on_pause_pressed)

	music_player = get_tree().get_first_node_in_group("MusicPlayer")

	dialogue = dialogue_box.get_node("Dialogue")
	speaker0 = dialogue_box.get_node("Speaker0")
	speaker1 = dialogue_box.get_node("Speaker1")

	speaker0.animation = "player" + cur_speaker0
	speaker1.animation = "player" + cur_speaker1
	
	start_button.pressed.connect(_on_start_pressed)

	_load_dialogue_json()

	trigger_dialogue.connect(on_dialogue_trigger)

	if music_player and music_player.volume_db == -80 and not board_manager.muted:
		music_player.volume_db = -10

	emit_signal("trigger_dialogue", board_manager.cur_level + "Start")


# ----------------------------
# JSON LOADING
# ----------------------------
func _load_dialogue_json():
	var file = FileAccess.open("res://dialoguedata.json", FileAccess.READ)

	if file:
		var text = file.get_as_text()
		var parsed = JSON.parse_string(text)

		if parsed:
			dialogue_scenarios = parsed


# ----------------------------
# PROCESS (DIALOGUE LOOP)
# ----------------------------
func _process(delta):
	if board_manager.muted:
		music_button.texture_normal = load("res://sprites/HUD/SubMenuMusicOff.png")
	else:
		music_button.texture_normal = load("res://sprites/HUD/SubMenuMusic.png")

	if board_manager.dialogue_active:
		dialogue.show()
		if in_progress:
			if dialogue.visible_characters < dialogue.text.length():
				dialogue.visible_characters += 1

				if Input.is_action_just_released("click"):
					dialogue.visible_characters = -1
					in_progress = false

			elif dialogue.visible_characters >= dialogue.text.length():
				dialogue.visible_characters = -1
				in_progress = false

		else:
			if Input.is_action_just_released("click"):
				continue_dialogue()

	else:
		dialogue.hide()
		current_line_index = 0
		speaker0.frame = 0
		speaker1.frame = 0


# ----------------------------
# DIALOGUE START
# ----------------------------
func on_dialogue_trigger(scenario: String):
	dialogue_data = dialogue_scenarios.get(scenario, {})
	current_scenario = scenario

	board_manager.dialogue_active = true

	in_progress = true
	current_line_index = 0

	continue_dialogue()


# ----------------------------
# DIALOGUE FLOW
# ----------------------------
func continue_dialogue():
	if dialogue_data.has(str(current_line_index)):

		var line = dialogue_data[str(current_line_index)]

		var who0 = line[0]
		var who1 = line[1]

		# Speaker 0
		if "none" in who0:
			speaker0.animation = "none"
		else:
			speaker0.animation = "player" + str(who0[0])
			speaker0.frame = int(who0[1])

		# Speaker 1
		if "none" in who1:
			speaker1.animation = "none"
		else:
			speaker1.animation = "player" + str(who1[0])
			speaker1.frame = int(who1[1])

		dialogue.text = line[2]
		in_progress = true
		current_line_index += 1

	else:
		board_manager.dialogue_active = false

		if "Start" in current_scenario:
			emit_signal("end_first_dialogue")
			emit_signal("start_game")

			if "Level0" in board_manager.cur_level:
				on_help_pressed()

		else:
			speaker0.animation = "player" + cur_speaker0
			speaker1.animation = "player" + cur_speaker1
			emit_signal("end_dialogue")


# ----------------------------
# UI BUTTONS
# ----------------------------
func on_music_pressed():
	board_manager.muted = !board_manager.muted
	if board_manager.muted:
		music_player.volume_db = -80
	else:
		music_player.volume_db = -10

func on_help_pressed():
	on_pause_pressed()
	get_node("HelpWindow").visible = true

func on_pause_pressed():
	board_manager.game_running = false
	start_button.visible = true
	

func _on_start_pressed():
	board_manager.game_running = true
	get_node("HelpWindow").visible = false
	start_button.visible = false
