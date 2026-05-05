extends Node

func _ready() -> void:
	add_to_group("Level")
	# print("Removing: ", get_parent().get_child(0))
	# get_parent().get_child(0).queue_free()
	pass


func _process(delta: float) -> void:
	pass
