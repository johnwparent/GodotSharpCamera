using Godot;
using System;
using System.IO;
using System.IO.Pipes;
public class cloud : Spatial
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    NamedPipeClientStream stream;
    // Called when the node enters the scene tree for the first time.
    ///<summary>
    ///Establishes the input stream to generate the cloud from
    ///</summary>
    public override void _Ready()
    {
        this.stream = new NamedPipeClientStream("LIO");
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
                using (StreamReader sr = new StreamReader(this.stream))
                {
                    String tmp = "";
                    while((tmp = sr.ReadLine())!=null)
                    {
                        //get the command, command args, and RID from the stream
                        //will need some string parsing
                        //call the command w/ the args on the given root node, or whatever given RID
                    }
                }
            }
            catch(Exception e)
            {
                GD.PrintErr(e);
            }
        GD.Print("Exit _Process");
    }
}
