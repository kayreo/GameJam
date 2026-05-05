extends Board
class_name ReducedVisBoard


# ----------------------------
# READY
# ----------------------------
func _ready():
	super._ready()

	_setup_reveal_system()


# ----------------------------
# SETUP REVEAL SYSTEM
# ----------------------------
func _setup_reveal_system():

	if grid_nodes.is_empty():
		return

	for i in range(1, grid_nodes.size()):

		var cur_node: Icon = grid_nodes[i]

		cur_node.get_node("Wire").hide()

		# Connect safely (avoid duplicate connections)
		if not cur_node.is_connected("change_active_node", Callable(self, "_on_reveal_nodes")):
			cur_node.connect("change_active_node", Callable(self, "_on_reveal_nodes"))

	# reveal first node immediately
	var first_node: Icon = grid_nodes[0]
	_reveal_nodes(first_node)


# ----------------------------
# SIGNAL HANDLER
# ----------------------------
func _on_reveal_nodes(which_node: Icon):
	_reveal_nodes(which_node)


# ----------------------------
# REVEAL LOGIC
# ----------------------------
func _reveal_nodes(which_node: Icon):

	for key in which_node.adj.keys():

		var node_to_reveal: Icon = which_node.adj[key]

		if node_to_reveal != null:
			print("Revealing: ", node_to_reveal.name)
			node_to_reveal.get_node("Wire").show()
