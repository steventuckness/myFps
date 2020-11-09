extends Spatial

var animation_player
var is_alive = true
var hitpoints = 100
var hitbox_head
var hitbox_body

const IDLE_LOOP = "zombie idle-loop"
const DEATH = "zombie stumbling-loop"

func _ready():
	animation_player = $AnimationPlayer
	hitbox_head = $Armature/Skeleton/head/zombiehitbox_head
	hitbox_body = $Armature/Skeleton/torso/zombiehitbox_torso
	hitbox_head.connect("area_entered", self, "area_entered_zombie_head")
	hitbox_body.connect("area_entered", self, "area_entered_zombie_body")

func _process(delta):
	if hitpoints > 0 && is_alive: 
		animation_player.play(IDLE_LOOP)
	else:
		if is_alive:
			is_alive = false
			animation_player.play(DEATH)
			
# ...
func area_entered_zombie_head(area):
	if area.get_parent().get_name() == "Bullet":
		hurt(25)
		print('zombie hit in head');
	
func area_entered_zombie_body(area):
	if (area.get_parent().get_name() == "Bullet"):
		hurt(15)
		print('zombie hit in body')

func hurt(damage):
	print('did ' + str(damage) + ' to zombie')
	hitpoints -= damage
