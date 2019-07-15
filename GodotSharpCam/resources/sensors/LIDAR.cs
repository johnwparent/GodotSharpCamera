using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

    public class LIDAR : Spatial
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";
        public struct GOBeamData
        {


        }
        static readonly object locker = new object();

        
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
        File resultPCD;
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
            resultPCD = new File();
            
            resultPCD.Open("c://Users/John Parent/Dropbox/a/pc.json", (int)File.ModeFlags.Write);
            
            
            this._minAng = -0.53529248;
            this._maxAng = 0.18622663;
            this._maxDegree = (180/Mathf.Pi)*_maxAng;
            this._minDegree = (180/Mathf.Pi)*_minAng;
            this._resolution = 1f;
            this._beamNum = 32;
            this._beamMax = 100;
            this.Translation = this.Translation;
            this.maxLR = 100;
            this.dataSampleSize = 100;
            cast = (Godot.RayCast)this.GetChild(0);
            this.Rotate(Vector3.Left,maxLR);
            this.Rotate(Vector3.Down,(float)_maxAng);

            String orientation = "lr";
            if(Mathf.Abs((float)_maxDegree)+Mathf.Abs((float)_minDegree)>maxLR)
            {
                orientation = "ud";


            }
            //if beams need to be horizontal, spacing is determine by splitting the vertical fov
            //if beams are vertiacal, split horizontal fov
            switch(orientation)
            {
                case "lr":
                    this.space = spacing(maxLR);
                    this.or = "lr";
                    break;
                default:
                    this.space = spacing(Mathf.Abs((float)_maxDegree)+Mathf.Abs((float)_minDegree));
                    this.or = "ud";
                    break;
            }


            for(int j = 0;j<_beamNum;j++)
            {
                Spatial beam = new Spatial();
                beam.AddToGroup("beams");
                beam.Rotate(Vector3.Up,(float)(this.space*j));
                for(int i = 1;i<5;i++)
                {
                    RayCast tmp = (RayCast)cast.Duplicate();
                    beam.AddChild(tmp);
                    tmp.Rotate(Vector3.Down,(float)(this.space/5)*i);
                }
                this.AddChild(beam);
            }
            
            
            
        }

        /// <summary>
        /// Is called by the main process 60x per second, but varies based on framerate, calls the LIDAR detector to collect data, and then resets the LIDAR's oritentation
        /// </summary>
        public override void _Process(float delta)
        {
            if(this.first)
            {
                GD.Print("hello");
                WriteHeader();
            }
            switch(or)
            {
                case "lr":
                    this.RotateObjectLocal(new Vector3(0,1,0),maxLR/1800);
                    break;
                default:
                    this.RotateObjectLocal(Vector3.Right,(float)(_maxDegree+_minDegree)/1800);
                    break;
            }
            LIDAR_DRIVER();
            writeFile();
            this.pointCloud.Clear();
            this.first = false;          
        }
        /// <summary>
        /// Drives the LIDAR process, writes to file, closes file, handles interpolation logic and  LIDAR camera movement control
        /// </summary>
        public void LIDAR_DRIVER()
        {
            foreach(Spatial b in GetTree().GetNodesInGroup("beams"))
            {               
                GO(b);
            }
        }
        void GO(Spatial beam)
        {
            foreach(RayCast r in beam.GetChildren())
            {
                var dir = -GlobalTransform.basis.z;
                r.SetCastTo(dir*100);
                r.Enabled = true;
                r.ForceRaycastUpdate();
                if(r.Enabled && r.IsColliding())
                {
                    pointCloud.Add(r.GetCollisionPoint());
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
        //responsible for actually taking the raycast and returning the collision data
        private Vector3 imaging(RayCast r)
        {
            var dir = -GlobalTransform.basis.z;
            
            r.SetCastTo(dir*100);
            r.Enabled = true;
            r.ForceRaycastUpdate();
            if(r.Enabled && r.IsColliding())
            {
                return r.GetCollisionPoint();

            }
            return new Vector3(0,0,0);
        }

        private float distancing(Vector3 collisionLocation)
        {   
            return this.Translation.DistanceTo(collisionLocation);
        }
        //handles interpolation
        private void interp()
        {
            int numOfInterp = _beamNum-(int)dataSampleSize;
            int dataRatio = pointCloud.Count/numOfInterp;
            int interps=0;
            int itr = 0;
            while(interps<numOfInterp)
            {
                Vector3 node1 = (Vector3)pointCloud[itr];
                Vector3 node2 = (Vector3)pointCloud[itr+1];
                if(node1.DistanceTo(node2) < 1)
                {
                    float dist1 = (float)distCloud[itr];
                    float dist2 = (float)distCloud[itr+1];
                    if(Math.Abs(dist1-dist2)<2)
                    {   
                        float x = (node1.x+node2.x)/2;
                        float y = (node1.y + node2.y)/2;
                        float z = (node1.z + node2.z)/2;
                        pointCloud.Add(new Vector3(x,y,z));
                        distCloud.Add(distancing(new Vector3(x,y,z)));
                        interps++;
                        itr += dataRatio;
                    }
                    else{
                        itr++;
                    }
                }
                else{
                    itr++;
                }

                if(itr >= pointCloud.Count-1+dataRatio)
                {
                    itr = 0;
                }
            }

        }
        //handles actually writing to the file with the data from the class variable array set aside for temporarily storing this data
        private void writeFile()
        {
            
            foreach(Vector3 xyz in pointCloud)
            {
                resultPCD.StoreLine((JSON.Print(xyz.x + " " +xyz.y + " " +xyz.z )));
            }
        }
        /// <summary>
        /// when LIDAR exits and data is no longer needed the file written to is closed
        /// </summary>
        public override void _ExitTree()
        {
            resultPCD.Close();
        }
    }