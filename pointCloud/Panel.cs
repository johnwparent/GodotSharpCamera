using Godot;
using System;

public class Panel : Godot.Panel
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
         var stream = new PacketPeerUDP();
         stream.Listen(42661,"127.0.0.1");
         if(stream.GetAvailablePacketCount()>0)
        {
            var data = stream.GetVar();
            Label lb = new Label();
            lb.SetText((string)data);
            this.AddChild(lb);
        }
    }
}
