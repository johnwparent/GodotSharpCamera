using Godot;
using System;

public class cloud : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    PacketPeerUDP stream;
    // Called when the node enters the scene tree for the first time.
    ///<summary>
    ///Establishes the input stream to generate the cloud from
    ///</summary>
    public override void _Ready()
    {
        this.stream = new PacketPeerUDP();
        if(this.stream.Listen(42561,"127.0.0.1")!=Error.Ok)
        {
            GD.PrintErr(this.stream.Listen(42561,"127.0.0.1"));
        }
        GD.Print("LIDAR RENDER: READY");
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
        GD.Print("Enter _Process");
        // if((Error)this.stream.GetVar()==Error.Ok)
        // {
            
        //     // Vector3 pt = new Vector3((Vector3)data);
        //     // var im = new ImmediateGeometry();
        //     // AddChild(im);
        //     // im.Clear();
        //     // im.Begin(Mesh.PrimitiveType.Points);
        //     // im.AddVertex(pt);
        //     // im.End();
        // }
        
        try
        {
            var data = (Error)this.stream.GetVar();
            GD.Print("Unable to aquire data, buffer overrun, or no packets");
        }
        catch(Exception e)
        {
            GD.Print("No Errors");
        }
        try
        {
            var data = this.stream.GetVar();
            GD.Print((Vector3)data);
        }
        catch(Exception e)
        {
            GD.Print("No data");
        }        
        GD.Print("Exit _Process");
    }
}
