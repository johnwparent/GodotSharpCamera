using Godot;
using System;
using System.IO;
using System.IO.Pipes;

///<summary>
///A server class to send lidar data as a stream to localhost
///</summary>
public class PCLoad : Godot.Node
{

    public NamedPipeServerStream stream {get; set;}
    
    public override void _Ready()
    {        
        //Signal setup should be handled via the lidar script
        this.stream = new NamedPipeServerStream("LIO",PipeDirection.Out);
    }
    ///<summary>
    ///Starts the stream of LIDAR data
    ///Only to be called by a signal from the LIDAR _Ready() function
    ///</summary>
    public void _PointCloudServerEnable()
    {
            this.stream.WaitForConnection();
            GD.Print("Client connected to server pipe");
    }
    ///<summary>
    ///Pushes the Lidar data to the stream
    ///</summary>
    public void _LiveUpdates(Vector3 pt,Vector3 dp)
    {
         try
            {
                using(StreamWriter sw = new StreamWriter(this.stream))
                {
                    sw.WriteLine(pt);
                }
            }
            catch(Exception e)
            {
                GD.PrintErr(e);
            }
        
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