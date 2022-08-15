using System;
using System.Collections.Generic;

namespace Entity.Ingame.DynamicBackground
{
    public class BackgroundPlanet : BackgroundDO
    {
        public BackgroundMoon[] Moons { get; set; }

        public BackgroundPlanet() : base()
        {
            Class = "planet";
            Layer = 1;
            Radius = 100;
            Moons = new BackgroundMoon[0];
        }
    }
}
