using Godot;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

///<summary>
///A server class to send lidar data as a stream to localhost
///</summary>
public class PCLoad : Godot.Node
{

    public PacketPeerUDP stream {get; set;}
    
    public override void _Ready()
    {        
        //Signal setup should be handled via the lidar script
    }
    ///<summary>
    ///Starts the stream of LIDAR data
    ///Only to be called by a signal from the LIDAR _Ready() function
    ///</summary>
    public void _PointCloudServerEnable()
    {
        this.stream = new PacketPeerUDP();
        if(this.stream.SetDestAddress("127.0.0.1",42561)!=Error.Ok)
        {
            GD.PrintErr(this.stream.SetDestAddress("127.0.0.1",42561)!=Error.Ok);
        }        
    }
    ///<summary>
    ///Pushes the Lidar data to the stream
    ///</summary>
    public void _LiveUpdates(Vector3 pt,Vector3 dp)
    {
        this.stream.PutVar(pt);
        
    }
    ///<summary>
    ///Executes the godot executables that reads off the stream and produces LIDAR point clouds as a mesh
    ///</summary>
    public void Exec()
    {
        String[] p = new String[]{"--path C:/pointCloud/pointCloud.exe","--main-pck C:/pointCloud/pointCloud.pck"};
        OS.Execute("C:/pointCloud/pointCloud.exe",p,false);
        GD.Print(OS.GetExecutablePath());
        
    }
}