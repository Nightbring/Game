using System;
using System.Collections.Generic;

namespace Entity.Ingame.DynamicBackground
{
    public class BackgroundDO : BackgroundObject
    {
        private double orbitalradius { get; set; }

        public double OrbitalRadius { 
            get { 
                return orbitalradius;
            } 
            set { 
                orbitalradius = value;
                XPosition = value * Math.Cos(Angle / Math.PI);
                YPosition = value * Math.Sin(Angle / Math.PI);
            } 
        }
        public double OrbitalSpeed { get; set; }
        public double Angle { get; set; }

        public BackgroundDO() : base()
        {
            Angle = 0;
            OrbitalRadius = 1000;
            OrbitalSpeed = 1;
        }
    }
}
