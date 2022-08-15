using System;
using System.Collections.Generic;

namespace Entity.Ingame.DynamicBackground
{
    public class BackgroundStar : BackgroundObject
    {
        public BackgroundPlanet[] Planets { get; set; }

        public BackgroundStar() : base()
        {
            Layer = 1;
            Radius = 1000;
            XPosition = 0;
            YPosition = 0;
            Planets = new BackgroundPlanet[0];
        }
    }
}
