extends Board
class_name BlackoutBoard

# ----------------------------
# SETTINGS
# ----------------------------
var blackout_num: int = 5
var blackout_nodes: Array[Icon] = []

# Timers
@onready var blackout_timer: Timer = get_node("BlackOutTimer")
@onready var reset_blackout_timer: Timer = get_node("ResetBlackOutTimer")


# ----------------------------
# READY
# ----------------------------
func _ready():
	blackout_nodes = []

	blackout_timer.timeout.connect(_on_blackout_timeout)
	reset_blackout_timer.timeout.connect(_on_reset_timeout)

	super._ready()

	reset_blackout_timer.start(3)

# ----------------------------
# POPULATE BLACKOUT NODES
# ----------------------------
func populate_node_list():
	grid_nodes = get_tree().get_nodes_in_group("Grid")
	blackout_nodes.clear()

	for i in range(blackout_num):

		var node_to_add: Icon = grid_nodes.pick_random()

		while node_to_add == grid_nodes[0] \
		or node_to_add == grid_nodes[grid_nodes.size() - 1] \
		or blackout_nodes.has(node_to_add) \
		or node_to_add.get_node("WireAnim").frame != 0:

			node_to_add = grid_nodes.pick_random()

		blackout_nodes.append(node_to_add)


# ----------------------------
# BLACKOUT EVENTS
# ----------------------------
func _on_blackout():
	print("Black out starting")


func _on_blackout_timeout():
	print("Unblacking out")

	for node: Icon in blackout_nodes:
		node.get_node("Wire").show()
		node.get_node("WireAnim").show()

	blackout_nodes.clear()

	blackout_timer.stop()
	reset_blackout_timer.start(3)


func _on_reset_timeout():
	print("Waiting for next black out")

	populate_node_list()

	for node: Icon in blackout_nodes:
		node.get_node("Wire").hide()
		node.get_node("WireAnim").hide()

	reset_blackout_timer.stop()
	blackout_timer.start(2)
