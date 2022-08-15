using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class Weapon
    {
        public double Radius { get; set; }
        public double AnglePos { get; set; }

        public double Rotation { get; set; }

        public int FireCd { get; set; }
        public int FireCount { get; set; }

        public string AmmoType { get; set; }
        public string Ammo { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public Weapon()
        {
            AmmoType = "bullet";
            FireCd = 10;
            FireCount = 10;
        }
    }
}
