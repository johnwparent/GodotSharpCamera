using Godot;
using System;

public class testEnv : Spatial
{
    [Signal]
    public delegate void StartPCV();
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    // [Signal]
    // public delegate void StartPCV();
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var pc = (PCLoad)GetNode("/root/PCLoad");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
