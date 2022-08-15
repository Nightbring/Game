using System;
using System.Collections.Generic;

namespace Entity.Ingame.DynamicBackground
{
    public class BackgroundObject : BaseObject
    {
        public int Layer { get; set; }
        public double Radius { get; set; }
        public string Class { get; set; }

        public BackgroundObject() : base()
        {
            ObjectType = "bgobject";
            Class = "star";
            Layer = 1;
            Radius = 1;
        }
    }
}
