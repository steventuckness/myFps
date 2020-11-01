using Godot;
using System;

public class Player : KinematicBody
{
	[Export]
	public float Gravity = -24.8f;
	[Export]
	public float MaxSpeed = 20.0f;
	[Export]
	public float JumpSpeed = 18.0f;
	[Export]
	public float Accel = 4.5f;
	[Export]
	public float Deaccel = 16.0f;
	[Export]
	public float MaxSlopeAngle = 40.0f;
	[Export]
	public float MouseSensitivity = 0.05f;

	[Export]
	public float MaxSprintSpeed = 30.0f;
	[Export]
	public float SprintAccel = 18.0f;

	private Vector3 _vel = new Vector3();
	private Vector3 _dir = new Vector3();

	private Camera _camera;
	private Spatial _rotationHelper;

	private bool _isSprinting = false;

	private SpotLight _flashlight;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera>("Rotation_Helper/Camera");
		_rotationHelper = GetNode<Spatial>("Rotation_Helper");
		_flashlight = GetNode<SpotLight>("Rotation_Helper/Flashlight");

		Input.SetMouseMode(Input.MouseMode.Captured);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
			ProcessInput(delta);
			ProcessMovement(delta);
	}

	 private void ProcessInput(float delta)
	{
		//  -------------------------------------------------------------------
		// Sprinting
		if (Input.IsActionPressed("movement_sprint")) 
		{
			_isSprinting = true;
		} else 
		{
			_isSprinting = false;
		}
		//  -------------------------------------------------------------------
		
	 	//  -------------------------------------------------------------------
		// Turning the flashlight on/off
		if (Input.IsActionJustPressed("flashlight")) 
		{
			if (_flashlight.IsVisibleInTree())
			{
				_flashlight.Hide();
			}
			else {
				_flashlight.Show();
			}
		}

		//  -------------------------------------------------------------------
		//  Walking
		_dir = new Vector3();
		Transform camXform = _camera.GlobalTransform;

		Vector2 inputMovementVector = new Vector2();

		if (Input.IsActionPressed("movement_forward"))
			inputMovementVector.y += 1;
		if (Input.IsActionPressed("movement_backward"))
			inputMovementVector.y -= 1;
		if (Input.IsActionPressed("movement_left"))
			inputMovementVector.x -= 1;
		if (Input.IsActionPressed("movement_right"))
			inputMovementVector.x += 1;

		inputMovementVector = inputMovementVector.Normalized();

		// Basis vectors are already normalized.
		_dir += -camXform.basis.z * inputMovementVector.y;
		_dir += camXform.basis.x * inputMovementVector.x;
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//  Jumping
		if (IsOnFloor())
		{
			if (Input.IsActionJustPressed("movement_jump"))
				_vel.y = JumpSpeed;
		}
		//  -------------------------------------------------------------------

		//  -------------------------------------------------------------------
		//  Capturing/Freeing the cursor
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (Input.GetMouseMode() == Input.MouseMode.Visible)
				Input.SetMouseMode(Input.MouseMode.Captured);
			else
				Input.SetMouseMode(Input.MouseMode.Visible);
		}
		//  -------------------------------------------------------------------
	}

	private void ProcessMovement(float delta)
	{
		_dir.y = 0;
		_dir = _dir.Normalized();

		_vel.y += delta * Gravity;

		Vector3 hvel = _vel;
		hvel.y = 0;

		Vector3 target = _dir;

		if (_isSprinting) 
		{
			target *= MaxSprintSpeed;
		} 
		else 
		{
			target *= MaxSpeed;
		}

		float accel;
		if (_dir.Dot(hvel) > 0)
			if (_isSprinting) 
			{
			   accel = SprintAccel; 
			}
			else 
			{
				accel = Accel;
			}
		else
			accel = Deaccel;

		hvel = hvel.LinearInterpolate(target, accel * delta);
		_vel.x = hvel.x;
		_vel.z = hvel.z;
		_vel = MoveAndSlide(_vel, new Vector3(0, 1, 0), false, 4, Mathf.Deg2Rad(MaxSlopeAngle));
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			InputEventMouseMotion mouseEvent = @event as InputEventMouseMotion;
			_rotationHelper.RotateX(Mathf.Deg2Rad(mouseEvent.Relative.y * MouseSensitivity));
			RotateY(Mathf.Deg2Rad(-mouseEvent.Relative.x * MouseSensitivity));

			Vector3 cameraRot = _rotationHelper.RotationDegrees;
			cameraRot.x = Mathf.Clamp(cameraRot.x, -70, 70);
			_rotationHelper.RotationDegrees = cameraRot;
		}
	}
}
