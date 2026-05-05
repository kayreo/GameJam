extends Control

var start_button: TextureButton
var credit_button: TextureButton
var music_button: TextureButton

var music_player: AudioStreamPlayer

var board_manager


func _ready():
	add_to_group("Level")
	board_manager = BoardManager  # autoload singleton

	board_manager.cur_level_index = 0

	start_button = get_node("PlayButton")
	credit_button = get_node("CreditsButton")
	music_button = get_node("MusicButton")

	start_button.pressed.connect(_on_start_pressed)
	credit_button.pressed.connect(_on_credits_pressed)
	music_button.pressed.connect(_on_music_pressed)
	music_player = get_parent().get_node_or_null("Music")


func _process(delta):
	if board_manager.muted:
		music_button.texture_normal = load("res://sprites/HUD/SubMenuMusicOff.png")
	else:
		music_button.texture_normal = load("res://sprites/HUD/SubMenuMusic.png")


# ----------------------------
# START GAME
# ----------------------------
func _on_start_pressed():
	board_manager.cur_level = "Level0"
	board_manager.emit_signal("change_level", "Level0")


# ----------------------------
# CREDITS
# ----------------------------
func _on_credits_pressed():
	board_manager.emit_signal("change_menu", "Credits")
	
	
# ----------------------------
# MUSIC
# ----------------------------
func _on_music_pressed():
	print(board_manager.muted)
	board_manager.muted = !board_manager.muted
	if board_manager.muted:
		music_player.volume_db = -80
	else:
		music_player.volume_db = -10
