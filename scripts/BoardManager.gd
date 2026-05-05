extends Node

# ----------------------------
# SIGNALS
# ----------------------------
signal change_level(which_level: String)
signal change_menu(which_menu: String)

# ----------------------------
# STATE
# ----------------------------
var dialogue_active: bool = false
var game_running: bool = false
var level_end: bool = false
var level_success: bool = false

var cur_level: String = ""
var cur_level_index: int = 0

var muted: bool = false


# ----------------------------
# READY (HOOK SIGNALS)
# ----------------------------
func _ready():
	# IMPORTANT: connect internal signal
	connect("change_level", Callable(self, "on_change_level"))
	connect("change_menu", Callable(self, "on_change_menu"))


# ----------------------------
# LEVEL LOADING
# ----------------------------
func on_change_level(which_level: String):
	cur_level = which_level
	print("Entering ", which_level)

	var container = get_tree().root.get_node("SceneManager")

	# remove existing level
	for child in container.get_children():
		if child.is_in_group("Level"):
			child.queue_free()
	await get_tree().process_frame
	# load new level
	reset_runtime_state()
	var path_to_level = "res://scenes/levels/%s.tscn" % which_level
	var packed_scene: PackedScene = load(path_to_level)

	if packed_scene:
		var level_instance = packed_scene.instantiate()
		container.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
		container.add_child(level_instance)
	else:
		push_error("Failed to load level: " + path_to_level)
		
func on_change_menu(which_menu: String):
	print("Entering ", which_menu)

	var container = get_tree().root.get_node("SceneManager")

	# remove existing level
	for child in container.get_children():
		if child.is_in_group("Level"):
			child.queue_free()
	await get_tree().process_frame
	# load new level
	reset_runtime_state()
	var path_to_scene = "res://scenes/%s.tscn" % which_menu
	var packed_scene: PackedScene = load(path_to_scene)

	if packed_scene:
		var level_instance = packed_scene.instantiate()
		container.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
		container.add_child(level_instance)
	else:
		push_error("Failed to load scene: " + path_to_scene)

func reset_runtime_state():
	game_running = false
	level_end = false
	level_success = false
	dialogue_active = false

# ----------------------------
# DIALOGUE FLOW (START / END)
# ----------------------------
func on_first_end_dialogue():
	print("Dialogue over")
	dialogue_active = false
	game_running = true


func on_end_dialogue():
	print("Dialogue over")
	dialogue_active = false
	game_running = false
	get_tree().get_first_node_in_group("JinglePlayer").stop()
	if not muted:
		get_tree().get_first_node_in_group("MusicPlayer").volume_db = -10

	cur_level_index += 1

	if cur_level_index >= 3:
		cur_level_index = 0
		emit_signal("change_level", "WinScreen")
	else:
		emit_signal("change_level", "Level%d" % cur_level_index)
