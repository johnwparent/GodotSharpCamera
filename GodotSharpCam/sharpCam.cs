
using Godot;
using System;

public class sharpCam : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    private Godot.Camera cam;
    [Export(PropertyHint.Range,"0.0, 1.0")]
    public float sensitivity = 0.5f;
    [Export]
    private bool enabled = true;
    [Export]
    private bool pressed = false;
    [Export]
    private String forward = "wasdForward";
    [Export]
    private String back = "wasdBack";
    [Export]
    private String left = "wasdLeft";
    [Export]
    private String right = "wasdRight";
    [Export]
    private String up = "wasdUp";
    [Export]
    private String down = "wasdDown";

    [Export]
    Vector3 direction = new Vector3(0.0f,0.0f,0.0f);
    [Export]
    Vector3 speed = new Vector3(0.0f, 0.0f, 0.0f);
    [Export]
    float acceleration = 1.0f;
    [Export]
    float deceleration = 0.1f;
    [Export]
    Vector3 max_speed = new Vector3(2.0f, 2.0f, 2.0f);
    [Export]
    private bool local = true;
    [Export(PropertyHint.Range,"0.0 , 1.0")]
    float smoothness = 0.5f;
    
    float distance = 5.0f;
    bool collisions = true;
    int yaw_limit = 360;
    int pitch_limit = 360;

    float _yaw = 0.0f;
    float _pitch = 0.0f;
    float _total_yaw = 0.0f;
    float _total_pitch = 0.0f;
    Vector2 mousePosition = new Vector2(0.0f,0.0f);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.cam = this.GetNode<Godot.Camera>("camobj");
        this.cam.SetCurrent(enabled);


    }
    
 // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
      
        
        updateMouselook();
        
        updateMovement(delta);
    }
    /// <summary>
    /// An input event handler for the c# camera
    ///
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        
        
        if(@event is InputEventMouseButton && @event.IsAction("mouseRightClick"))
        {
            pressed=!pressed;
            
        }
        if(@event is InputEventMouseMotion && pressed)
        {
           
            InputEventMouseMotion _event = (InputEventMouseMotion)@event;
            mousePosition = _event.Relative;

        }

        
        
        if(@event.IsActionPressed("wasdForward"))
        {
            direction.z = -1f;
        }
        else if(@event.IsActionPressed("wasdBack"))
        {
            direction.z = 1f;
        }
        else if(!@event.IsActionPressed("wasdBack") && !@event.IsActionPressed("wasdForward")&&!@event.IsPressed())
        {
            
            direction.z = 0f;
        }
        if(@event.IsActionPressed("wasdLeft"))
        {
            direction.x = -1f;
        }
        else if(@event.IsActionPressed("wasdRight"))
        {
            direction.x = 1f;
        }
        else if(!@event.IsActionPressed("wasdLeft") && !@event.IsActionPressed("wasdRight")&&!@event.IsPressed())
        {
            direction.x = 0f;
        }
        if(@event.IsActionPressed("wasdUp"))
        {
            direction.y = 1f;
        }
        else if(@event.IsActionPressed("wasdDown"))
        {
            direction.y = -1f;
        }
        else if(!@event.IsActionPressed("wasdUp") &&!@event.IsActionPressed("wasdDown")&&!@event.IsPressed())
        {
            direction.y = 0f;   
        }
        
    }


    private void updateMouselook()
    {
        
        mousePosition *= sensitivity;
	_yaw = (float)_yaw * smoothness + mousePosition.x * (1.0f - smoothness);
	_pitch = _pitch * smoothness + mousePosition.y * (1.0f - smoothness);
	mousePosition = new Vector2(0, 0);

	if (yaw_limit < 360)
    {
        _yaw = Mathf.Clamp(_yaw, -yaw_limit - _total_yaw, yaw_limit - _total_yaw);
    }
		
	if (pitch_limit < 360)
    {
        _pitch = Mathf.Clamp(_pitch, -pitch_limit - _total_pitch, pitch_limit - _total_pitch);
    }
		

	_total_yaw += _yaw;
	_total_pitch += _pitch;
    this.RotateObjectLocal(new Vector3(1,0,0), Mathf.Deg2Rad(-_pitch));
	this.RotateY(Mathf.Deg2Rad(-_yaw));
    }

    private void updateMovement(float delta)
    {
        Vector3 offset = max_speed * acceleration * direction;
	
	    speed.x = Mathf.Clamp(speed.x + offset.x, -max_speed.x, max_speed.x);
	    speed.y = Mathf.Clamp(speed.y + offset.y, -max_speed.y, max_speed.y);
	    speed.z = Mathf.Clamp(speed.z + offset.z, -max_speed.z, max_speed.z);
	
	    
	    if (direction.x == 0)
        {
            speed.x *= (1.0f - deceleration);
        }
		
	    if (direction.y == 0)
        {
            speed.y *= (1.0f - deceleration);
        }
		
	    if (direction.z == 0)
        {
            speed.z *= (1.0f - deceleration);
        }
		

	    if (local)
        {
            this.Translate(speed * delta);
        }
	    	
	    else
        {
            this.GlobalTranslate(speed * delta);
        }
		    
    }
}