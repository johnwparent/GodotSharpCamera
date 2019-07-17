using Godot;
using System;


    public class IR : RayCast
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";
        [Export]
        float fov = 0.10F;
        Array distances;

        Vector3 lastC;

        float maxDist;
        float minDist;
        string connection;
        int seq;
        string type = "IR";
        File saveData;
        string local = "c://Users/John Parent/Documents";
        
        public IR()
        {
            this.saveData = new File();
        }
        public IR(string connection)
        {
            this.connection = connection;
            this.saveData = new File();
            
        }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            //an rr connection will be established here given by the user in the contructor when they create an instance of this IR sensor
            //for now, and json will be written to a local repository so we can verify information.
            this.saveData = new File();
            saveData.Open("c://Users/John Parent/Dropbox/a/saveData.json", (int)File.ModeFlags.Write);
            saveData.StoreLine("hello world");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(float delta)
        {
            var n = GetNode<ImmediateGeometry>("path");
            n.Clear();
            n.Begin(Mesh.PrimitiveType.Lines,null);
            n.AddVertex(this.GetTranslation());
            n.AddVertex(lastC);
            n.End();


        }

        /// <summary>
        /// Called 60x per second by main, sends out raycast to simulate IR sensor,
        /// Collects and writes data
        /// </summary>
        public override void _PhysicsProcess(float delta)
        {
            
            var dir = -GlobalTransform.basis.z;
            //GD.Print(dir);
            this.SetCastTo(dir*100);
            //GD.Print(this.GetCastTo());
            this.Enabled = true;
            this.ForceRaycastUpdate();
            if(this.Enabled && this.IsColliding())
            {
                lastC = this.GetCollisionPoint();
                saveData.StoreLine(JSON.Print(this.GetCollisionPoint()));
            }
        }
        /// <summary>
        /// Turns off raycaster when sensor data is no longer needed
        /// </summary>
        public override void _ExitTree()
        {
            var ray = (RayCast)this;
            ray.Enabled = false;
            saveData.Close();
        }
    }
