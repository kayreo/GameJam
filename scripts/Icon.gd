extends TextureRect
class_name Icon

signal change_active_node(which_node)

# Direction mapping:
# 0 = Left, 1 = Top, 2 = Right, 3 = Bottom

var adj := {
	0: null,
	1: null,
	2: null,
	3: null
}

var board_manager
var board

@export var is_clicked: bool = false

var wire_type: String = ""
var max_lifespan: int = 0

@export var enter_pos: int = 0
@export var exit_pos: int = 0
@export var target_connect: int = 0

var wire_anim: AnimatedSprite2D
var wire_filling: bool = false
var locked: bool = false
var paused: bool = false


func _ready():
	board = get_parent().get_parent().get_parent()
	board_manager = get_node("/root/BoardManager")

	board.set_grid_info.connect(_on_set_info)
	board.change_grid.connect(_on_change_grid)

	wire_anim = get_node("WireAnim")

	is_clicked = false

	change_active_node.connect(board.on_change_active_node)

	get_adjacents()


func _process(delta):
	if board_manager.game_running:

		if wire_filling:
			if paused:
				wire_anim.play()
				paused = false

			if wire_anim.frame == max_lifespan:
				wire_filling = false
				wire_anim.speed_scale = 0

				var exit_node: Icon = adj.get(exit_pos)

				if exit_node != null:
					exit_node.locked = true

					var reverse := false

					if target_connect == exit_node.enter_pos or target_connect == exit_node.exit_pos:

						if target_connect == exit_node.exit_pos:
							var temp = exit_node.enter_pos
							exit_node.enter_pos = exit_node.exit_pos
							exit_node.exit_pos = temp
							reverse = true

						_on_success_enter(exit_node, reverse)
					else:
						board_manager.game_running = false
						board._on_end_level()
				else:
					board_manager.game_running = false
					board._on_end_level()

	else:
		wire_anim.pause()
		paused = true


func _on_set_info(which_node: String, type_name: String):
	if which_node == name:
		wire_type = type_name
		max_lifespan = 8 if "Loop" in wire_type else 4


func _on_area_2d_input_event(viewport, event, shape_idx):
	if not board_manager.game_running:
		return

	if wire_anim.frame != 0 or locked:
		return

	if event is InputEventMouseButton and event.pressed:

		if event.button_index == MOUSE_BUTTON_LEFT:
			is_clicked = true
			board.click_grid_section(self)

		elif event.button_index == MOUSE_BUTTON_RIGHT:
			$Wire.rotation_degrees += 90
			$WireAnim.rotation_degrees += 90

			enter_pos = _rotate_dir(enter_pos)
			exit_pos = _rotate_dir(exit_pos)
			target_connect = _rotate_dir(target_connect)


func _rotate_dir(dir: int) -> int:
	dir += 1
	if dir >= 4:
		dir = 0
	return dir


func _on_area_2d_mouse_entered():
	$SelectBorder.show()


func _on_area_2d_mouse_exited():
	if not is_clicked:
		$SelectBorder.hide()


func _on_change_grid(which_node: String, node_to_swap: Icon):
	if which_node != name:
		return

	print("Swapping")

	# Animation swap
	var original_anim = $WireAnim.animation
	$WireAnim.animation = node_to_swap.get_node("WireAnim").animation
	node_to_swap.get_node("WireAnim").animation = original_anim

	# Texture swap
	var original_tex = $Wire.texture
	$Wire.texture = node_to_swap.get_node("Wire").texture
	node_to_swap.get_node("Wire").texture = original_tex

	# Rotation swap
	var original_rot = $Wire.rotation_degrees
	$Wire.rotation_degrees = node_to_swap.get_node("Wire").rotation_degrees
	$WireAnim.rotation_degrees = node_to_swap.get_node("Wire").rotation_degrees

	node_to_swap.get_node("Wire").rotation_degrees = original_rot
	node_to_swap.get_node("WireAnim").rotation_degrees = original_rot

	# Positions swap
	var temp = enter_pos
	enter_pos = node_to_swap.enter_pos
	node_to_swap.enter_pos = temp

	temp = exit_pos
	exit_pos = node_to_swap.exit_pos
	node_to_swap.exit_pos = temp

	temp = target_connect
	target_connect = node_to_swap.target_connect
	node_to_swap.target_connect = temp

	# Lifespan swap
	var temp_life = max_lifespan
	max_lifespan = node_to_swap.max_lifespan
	node_to_swap.max_lifespan = temp_life

	# Wire type swap
	var temp_type = wire_type
	wire_type = node_to_swap.wire_type
	node_to_swap.wire_type = temp_type

	$SelectBorder.hide()
	node_to_swap.get_node("SelectBorder").hide()

	is_clicked = false
	node_to_swap.is_clicked = false


func on_node_start():
	wire_anim.play()
	change_active_node.emit(self)
	wire_filling = true


func _on_success_enter(start_node: Icon, reverse: bool):
	if reverse:
		start_node.get_node("WireAnim").animation = start_node.wire_type + "R"

		match start_node.exit_pos:
			3:
				start_node.target_connect = 1
			1:
				start_node.target_connect = 3
			0:
				start_node.target_connect = 2
			2:
				start_node.target_connect = 0

	start_node.on_node_start()


func get_adjacents():
	var cur_row = String(get_parent().name)
	var cur_row_num = int(cur_row[3])

	var above_row = cur_row_num - 1
	var below_row = cur_row_num + 1

	var index_of_self = get_parent().get_children().find(self)

	var left_index = index_of_self - 1
	var right_index = index_of_self + 1

	if left_index >= 0:
		adj[0] = get_parent().get_child(left_index)

	if above_row >= 0:
		adj[1] = get_parent().get_parent().get_node("Row%d" % above_row).get_child(index_of_self)

	if right_index < get_parent().get_child_count():
		adj[2] = get_parent().get_child(right_index)

	if below_row < get_parent().get_parent().get_child_count():
		adj[3] = get_parent().get_parent().get_node("Row%d" % below_row).get_child(index_of_self)
