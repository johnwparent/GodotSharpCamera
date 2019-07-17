using Godot;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
public class PCLoad : Godot.Node
{
    [Signal]
    public delegate void StartPCV();
    Hashtable pointCloud;    
    WindowDialog Pop;
    Viewport v;
    Node pc;
    Spatial space;
    public override void _Ready()
    {
        pc = GetNode("/root");
        this.Pop = new WindowDialog();
        this.v = new Viewport();
        this.v.OwnWorld = true;
        this.v.UpdateWorlds();
        PackedScene sharpCam = (PackedScene)GD.Load("res://resources/scenes/sharpCam.tscn");
        Node camChild = sharpCam.Instance();
        ViewportContainer vc = new ViewportContainer();
        this.space = new Spatial();
        Task t1 = Task.Factory.StartNew(()=>{space.AddChild(new OmniLight());});
        Task t = Task.Factory.StartNew(()=>{space.AddChild(camChild);});
        Task t2 = Task.Factory.StartNew(()=>{this.v.AddChild(space);});
        Task t3 = Task.Factory.StartNew(()=>{vc.AddChild(this.v);});
        this.Pop.AddChild(vc);
        this.Pop.Connect("tree_entered",this,"_PointCloudOn");
        GetTree().GetRoot().Connect("ready",this,"addToTree");        
        t.Wait();
        t2.Wait();
        t3.Wait();
    }

    public void _PointCloudOn()
    {
        GD.Print("sjdhas");
        GD.Print(this.Pop.GetChildCount());
        this.Pop.SetCustomMinimumSize(new Vector2(10,10));
        this.Pop.SetResizable(true);
        this.Pop.ShowOnTop = true;
        this.Pop.PopupCentered();
    }
    public bool _LiveUpdates(Vector3 pt,Vector3 dp)
    {
       if(!pointCloud.ContainsKey(pt))
       {
           pointCloud.Add(pt,dp);
           ImmediateGeometry im = new ImmediateGeometry();
           var m = new SpatialMaterial();
           im.SetMaterialOverride(m);
           im.Clear();
           im.Begin(Godot.Mesh.PrimitiveType.Points);
           im.AddVertex(pt);
           im.End();
           return true;
       }       
        return false;
    }
    public void addToTree()
    {
        GD.Print("Root ready");
        GetTree().GetRoot().GetNode("/root/testEnv/Control").AddChild(this.Pop);
    }
}