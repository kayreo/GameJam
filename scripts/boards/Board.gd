extends Node
class_name Board

# ----------------------------
# ENUM
# ----------------------------
enum Position {
	LEFT,
	TOP,
	RIGHT,
	BOTTOM
}

# ----------------------------
# SIGNALS
# ----------------------------
signal set_data
signal set_grid_info(which_node: String, type_name: String)
signal change_grid(which_node: String, node_to_swap: Icon)
signal click_node(which_node: Icon)
signal end_level

# ----------------------------
# DATA
# ----------------------------
var wire_pos: Array = []

var clicked_node: Icon = null
var active_icon: Icon = null

var board_manager: Node

var grid_nodes: Array[Node] = []
var wires: Array[Node] = []

var cur_click: Node = null
var swap_click: Node = null

# safety flag to prevent calls after deletion
var _is_destroyed := false

var _level_ended := false

# Audio nodes
var music
var jingle


# ----------------------------
# READY
# ----------------------------
func _ready():
	board_manager = get_node("/root/BoardManager")

	click_node.connect(_on_click_node)
	set_data.connect(_on_set_data)
	end_level.connect(_on_end_level)

	emit_signal("set_data")

	# SAFE HUD CONNECT (prevents invalid reference crash)
	var hud = get_parent().get_node_or_null("HUD")
	if hud and not hud.start_game.is_connected(_on_game_start):
		hud.start_game.connect(_on_game_start)

	grid_nodes = get_tree().get_nodes_in_group("Grid")
	wires = get_tree().get_nodes_in_group("Wire")

	music = get_tree().get_first_node_in_group("MusicPlayer")
	jingle = get_tree().get_first_node_in_group("JinglePlayer")

	_setup_initial_grid()


# ----------------------------
# CLEANUP ON LEVEL EXIT
# ----------------------------
func _exit_tree():
	_is_destroyed = true

	var hud = get_parent().get_node_or_null("HUD")
	if hud and hud.start_game.is_connected(_on_game_start):
		hud.start_game.disconnect(_on_game_start)


# ----------------------------
# INITIAL GRID SETUP
# ----------------------------
func _setup_initial_grid():
	if _is_destroyed:
		return
	if grid_nodes.is_empty() or wires.is_empty():
		return

	var grid_node: Icon = grid_nodes[0]
	var wire_node: TextureRect = wires[3]

	grid_node.get_node("Wire").texture = wire_node.texture
	grid_node.get_node("WireAnim").animation = wire_node.name

	print(grid_node.name, wire_node.name)
	emit_signal("set_grid_info", grid_node.name, wire_node.name)

	var to_add = wire_pos[1]

	grid_node.enter_pos = to_add[0]
	grid_node.exit_pos = to_add[1]
	grid_node.target_connect = Position.LEFT

	for i in range(1, grid_nodes.size()):
		grid_node = grid_nodes[i]
		wire_node = wires.pick_random()

		var wire_name = wire_node.name
		var pos_data: Array

		if "Elbow" in wire_name:
			pos_data = wire_pos[0]
			grid_node.target_connect = Position.BOTTOM

		elif "Straight" in wire_name:
			pos_data = wire_pos[1]
			grid_node.target_connect = Position.LEFT

		grid_node.enter_pos = pos_data[0]
		grid_node.exit_pos = pos_data[1]

		grid_node.get_node("Wire").texture = wire_node.texture
		grid_node.get_node("WireAnim").animation = wire_name

		emit_signal("set_grid_info", grid_node.name, wire_name)


# ----------------------------
# SET DATA
# ----------------------------
func _on_set_data():
	if _is_destroyed:
		return

	grid_nodes = get_tree().get_nodes_in_group("Grid")
	wires = get_tree().get_nodes_in_group("Wire")

	wire_pos.clear()

	wire_pos.append([Position.LEFT, Position.TOP])      # Elbow
	wire_pos.append([Position.LEFT, Position.RIGHT])    # Straight


# ----------------------------
# GAME START
# ----------------------------
func _on_game_start():
	if _is_destroyed:
		return
	if grid_nodes.is_empty():
		return

	var grid_node: Icon = grid_nodes[0]

	board_manager.game_running = true
	grid_node.get_node("WireAnim").speed_scale = 0.25
	grid_node.on_node_start()


# ----------------------------
# CLICK HANDLING
# ----------------------------
func _on_click_node(which_node: Icon):
	if _is_destroyed:
		return
	clicked_node = which_node


func click_grid_section(who: Node):
	if _is_destroyed:
		return

	if cur_click == null:
		cur_click = who
		emit_signal("click_node", who)
	else:
		swap_click = who
		emit_signal("change_grid", cur_click.name, swap_click)

		cur_click = null
		swap_click = null


# ----------------------------
# ACTIVE NODE CHECK
# ----------------------------
func on_change_active_node(which_node: Icon):
	if _is_destroyed:
		return

	active_icon = which_node

	if grid_nodes.find(active_icon) == grid_nodes.size() - 1:

		if active_icon.exit_pos == Position.RIGHT:
			print("Reached end SUCCESS")
			board_manager.level_success = true
		else:
			print("Reached end FAIL")

		board_manager.level_end = true
		emit_signal("end_level")


# ----------------------------
# END LEVEL
# ----------------------------
func _on_end_level():
	if _is_destroyed:
		return

	print("Woop")
	print("Woop from:", self.name, " id:", get_instance_id())
	var hud = get_parent().get_node_or_null("HUD")
	
	if _level_ended:
		return

	if board_manager.level_success:
		_level_ended = true
		if hud:
			hud.emit_signal("trigger_dialogue", board_manager.cur_level + "End")

		get_parent().get_node("GameOver/Win").show()

		if hud:
			hud.get_node("Border/Phone").frame = 1

		if is_instance_valid(music):
			music.volume_db = -80
		
		if is_instance_valid(jingle):
			jingle.play()

	else:
		print("Failure")
		get_parent().get_node("GameOver/Lose").show()
		get_parent().get_node("GameOver/BackButton").show()
		get_parent().get_node("GameOver/RetryButton").show()
