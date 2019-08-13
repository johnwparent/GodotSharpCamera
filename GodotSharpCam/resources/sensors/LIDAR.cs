using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

    public class LIDAR : Spatial
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";
        public struct GOBeamData
        {


        }
        static readonly object locker = new object();
        PCLoad server;
        ImmediateGeometry im;
        bool first = true;
        RayCast cast;
        double _minAng;
        double _maxAng;
        float _resolution;
        int _beamNum;
        float _beamMax;
        float dataSampleSize;
        bool interpolation;
        int refresh;
        float maxLR;
        float space;
        double _maxDegree;
        double _minDegree;
        String or;
        Godot.Collections.Array pointCloud;
        Godot.Collections.Array distCloud;
        Godot.File resultPCD;

        int INTR_CTR = 0;
        // Called when the node enters the scene tree for the first time.
        
        //Empty constructor to silence godot errors
        private LIDAR()
        {
            
        }
        /// <summary>
        /// Creates a LIDAR object to be instantiated on a model by a model parser
        /// </summary>
        /// <param name = "minAng", name = "maxAng">minAng and maxAng define the yz limits of the LIDAR's scanning<param>
        /// <param name = "pos">pos: Vector3 coordinate of the LIDAR sensor</param>
        public LIDAR(double minAng,double maxAng, float _maxLR,float resolution,int beamNum,float beamMax,Vector3 pos)
        {
            this._minAng=minAng;
            this._maxAng = maxAng;
            this._resolution=resolution;
            this._beamNum=beamNum;
            this._beamMax=beamMax;
            this.dataSampleSize=this._beamNum*this._resolution;
            this.maxLR = _maxLR;
            if(resolution<1)
            {
                interpolation=true;
            }
            else{
                interpolation=false;
            }
            
            this.Translation = pos;
        }
        
        public override void _Ready()
        {
            
            pointCloud = new Godot.Collections.Array();
            //needs to be rewritten to send a pipe to RR
            resultPCD = new Godot.File();
            
            resultPCD.Open("c://Users/John Parent/Dropbox/a/pc.xyz", (int)Godot.File.ModeFlags.Write);
            this.server = (PCLoad)GetNode("/root/PCLoad");
            
            this._minAng = -0.53529248;
            this._maxAng = 0.18622663;
            this._maxDegree = (180/Mathf.Pi)*_maxAng;
            this._minDegree = (180/Mathf.Pi)*_minAng;
            this._resolution = 1f;
            this._beamNum = 32;
            this._beamMax = 100;
            this.Translation = this.Translation;
            this.maxLR = 360;
            this.dataSampleSize = 100;            
            this.space = spacing(Mathf.Abs((float)this._minAng)+(float)this._maxAng);
            this.Rotate(Vector3.Down,this.space*_beamNum);
            this.cast = (RayCast)GetNode("RayCast");

            for(int j = 0;j<this._beamNum;j++)
            {
                Spatial beam = new Spatial();
                beam.AddToGroup("beams");
                this.AddChild(beam);
                beam.Rotate(new Vector3(1,0,0),(float)(this.space*j));
                //GD.Print(beam.GetRotationDegrees().x+" "+ beam.GetRotationDegrees().y);
                for(int i = 1;i<6;i++)
                {
                    RayCast tmp = (RayCast)cast.Duplicate();
                    beam.AddChild(tmp);
                    tmp.Rotate(new Vector3(1,0,0),(float)(this.space/5)*i);
                }
                
                
            }
        }

        /// <summary>
        /// Is called by the main process 60x per second, but varies based on framerate, calls the LIDAR detector to collect data, and then resets the LIDAR's oritentation
        /// </summary>
        public override void _PhysicsProcess(float delta)
        {
            
            if(this.first)
            {
                this.im = new ImmediateGeometry();
                AddChild(this.im);
                this.im.Clear();

                this.server._PointCloudServerEnable();
                //thread the call to execute the visualizer, so we dont run into a deadlock
                //this.server.Exec();
                GD.Print("LIDAR is enabled and scanning");
                //WriteHeader();
            }
            this.Rotate(new Vector3(0,1,0),0.0174533f);
            if(INTR_CTR%12==0)
            {
                INTR_CTR = 0;
                this.im.Clear();
            }
            LIDAR_DRIVER();
            this.first = false;  
            INTR_CTR++;
        }
        /// <summary>
        /// Drives the LIDAR process, writes to file, closes file, handles interpolation logic and  LIDAR camera movement control
        /// </summary>
        public void LIDAR_DRIVER()
        {
            foreach(Spatial b in GetTree().GetNodesInGroup("beams"))
            {               
                GO(b);
                
                this.im.Begin(Mesh.PrimitiveType.Lines);
                this.im.AddVertex(b.GetTranslation());
                var dir = -b.GetChild<RayCast>(0).GlobalTransform.basis.z;
                b.GetChild<RayCast>(0).SetCastTo(dir*100);
                this.im.AddVertex(b.GetChild<RayCast>(0).GetCastTo());
                this.im.End();
            }
        }
        void GO(Spatial beam)
        {
            foreach(RayCast r in beam.GetChildren())
            {
                var dir = -r.GlobalTransform.basis.z;
                //GD.Print(dir.x + " " + dir.y + " " + dir.z);
                r.SetCastTo(dir*100);
                r.SetCollideWithAreas(true);
                r.SetCollideWithBodies(true);
                r.Enabled = true;

                r.ForceRaycastUpdate();
                // GD.Print(r.GetCastTo());
                
                if(r.IsColliding())
                {
                    //GD.Print("Collision Detected at: "+ r.GetCollisionPoint());
                    //pointCloud.Add(r.GetCollisionPoint());
                    //GD.Print("x");
                    this.server._LiveUpdates(r.GetCollisionPoint(),r.GetCollisionPoint());
                    //resultPCD.StoreLine(r.GetCollisionPoint().x + " " + r.GetCollisionPoint().y + " " + r.GetCollisionPoint().z);
                    //distCloud.Add(this.Translation.DistanceTo(r.GetCollisionPoint()));
                }
                
                
            }
            
        }
        private float spacing(float fov)
        {
           return fov/_beamNum;
        }
        private void WriteHeader()
        {
            resultPCD.StoreLine("VERSION .7");
            resultPCD.StoreLine("FIELDS x y z");
            resultPCD.StoreLine("SIZE 4 4 4");
            resultPCD.StoreLine("TYPE F F F");
            resultPCD.StoreLine("COUNT 1 1 1");
            resultPCD.StoreLine("WIDTH "+_beamNum);
            resultPCD.StoreLine("Height 1");
            resultPCD.StoreLine("VIEWPOINT 0 0 0 1 0 0 0");
            resultPCD.StoreLine("POINTS "+_beamNum);
            resultPCD.StoreLine("DATA ascii");
            
        }

        private float distancing(Vector3 collisionLocation)
        {   
            return this.Translation.DistanceTo(collisionLocation);
        }
        
        /// <summary>
        /// when LIDAR exits and data is no longer needed the file written to is closed
        /// </summary>
        public override void _ExitTree()
        {
            resultPCD.Close();
        }
    }