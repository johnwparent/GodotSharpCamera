// using Godot;
// using System;
// using System.Threading;
// using System.Threading.Tasks;
// public class PointCloud : Godot.Node
// {
//     Godot.Collections.Array pointCloud;
//     Godot.Collections.Array distCloud;
    
//     Popup Pop;
//     Viewport v;
//     Node pc;
//     public override void _Ready()
//     {
//         pc = GetNode("root");
//         this.Pop = new Popup();
//         this.v = new Viewport();
//         this.v.OwnWorld = true;
//         this.v.UpdateWorlds();
//         PackedScene sharpCam = (PackedScene)GD.Load("res://resources/scenes/sharpCam.tscn");
//         Node camChild = sharpCam.Instance();
//         ViewportContainer vc = new ViewportContainer();
//         Spatial spatial = new Spatial();
//         spatial.AddChild(camChild);
//         this.v.AddChild(spatial);
//         vc.AddChild(this.v);
//         this.Pop.AddChild(vc);
//         pc.AddChild(Pop);
//         GD.Print("hello mark");
//     }

//     public bool PointCloudOn(bool io)
//     {
//         switch(io)
//         {
//             case true:
//                 this.Pop.PopupCentered(new Vector2(0,0));
//                 return true;                
//             default:
//                 this.Pop.Hide();
//                 return false;
//         }
//     }
//     public bool _LiveUpdates(Vector3 pt)
//     {
       
       
//         return false;
//     }
// }

