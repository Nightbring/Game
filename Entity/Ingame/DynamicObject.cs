using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class DynamicObject : BaseObject
    {
        public double Rotation { get; set; }
        public double Speed { get; set; }
        public double Radius { get; set; }

        public DynamicObject() : base()
        {
        }
    }
}
