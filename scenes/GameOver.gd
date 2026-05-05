extends CanvasLayer

var board_manager
var BackButton: TextureButton
var RetryButton: TextureButton

func _ready() -> void:
	board_manager = BoardManager
	BackButton = get_node("BackButton")
	BackButton.pressed.connect(_on_back_pressed)
	
	RetryButton = get_node("RetryButton")
	RetryButton.pressed.connect(_on_retry_pressed)

func _process(delta: float) -> void:
	pass

func _on_back_pressed() -> void:

	board_manager.emit_signal("change_menu", "MainMenu")

func _on_retry_pressed() -> void:
	board_manager.emit_signal("change_level", board_manager.cur_level)
