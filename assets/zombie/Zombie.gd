extends Spatial

var animation_player

# Called when the node enters the scene tree for the first time.
func _ready():
	animation_player = $AnimationPlayer


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	animation_player.play("zombie idle-loop")
