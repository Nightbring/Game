using System;
using System.Collections.Generic;

namespace Entity.Ingame.DynamicBackground
{
    public class BackgroundMoon : BackgroundDO
    {
        public BackgroundMoon() : base()
        {
            Class = "moon";
            Layer = 1;
            Radius = 10;
        }
    }
}
