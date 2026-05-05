extends Control

var BackButton: TextureButton

func _ready() -> void:
	add_to_group("Level")
	BackButton = get_node("BackButton")
	BackButton.pressed.connect(_on_back_pressed)

func _process(delta: float) -> void:
	pass

func _on_back_pressed() -> void:
	var board_manager = BoardManager
	var win_sound = get_node_or_null("WinSound")
	if win_sound:
		win_sound.stop()
	board_manager.emit_signal("change_menu", "MainMenu")
