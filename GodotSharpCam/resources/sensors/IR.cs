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

            var dir = -GlobalTransform.basis.z;
            GD.Print(dir);
            this.SetCastTo(dir*100);
            GD.Print(this.GetCastTo());
            this.Enabled = true;
            this.ForceRaycastUpdate();
            saveData.StoreLine("hello world");
            
            if(this.Enabled && this.IsColliding())
            {
                GD.Print("ye");
                saveData.StoreLine(JSON.Print(this.GetCollisionPoint()));

            }
        
            
            saveData.Close();
            this.Enabled = false;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        // public override void _Process(float delta)
        // {
        //     this.SetCurrent(true);
        //     saveData.Open("user://savegame.save", (int)File.ModeFlags.Write);





        //     saveData.Close();
        //     this.SetCurrent(false);
        // }

        /// <summary>
        /// Called 60x per second by main, sends out raycast to simulate IR sensor,
        /// Collects and writes data
        /// </summary>
        public override void _PhysicsProcess(float delta)
        {
            
            var dir = -GlobalTransform.basis.z;
            GD.Print(dir);
            this.SetCastTo(dir*100);
            GD.Print(this.GetCastTo());
            this.Enabled = true;
            this.ForceRaycastUpdate();
            
            
            if(this.Enabled && this.IsColliding())
            {
                
                saveData.StoreLine(JSON.Print(this.GetCollisionPoint()));

            }
        
            
            saveData.Close();
            this.Enabled = false;
        }
        /// <summary>
        /// Turns off raycaster when sensor data is no longer needed
        /// </summary>
        public override void _ExitTree()
        {
            var ray = (RayCast)this;
            ray.Enabled = false;
            
        }
    }
