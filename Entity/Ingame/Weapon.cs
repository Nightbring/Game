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

        private string ammoType { get; set; }
        public string AmmoType { get {
                return ammoType;
            }
            set {
                ammoType = value;
                switch (ammoType) {
                    case "bullet": FireCd = 10; break;
                    case "missile": FireCd = 100; break;
                    default: FireCd = 1; break;
                }
            } 
        }
        public AmmoObject Ammo { get; set; }

        public double X { get; set; }
        public double Y { get; set; }

        public Weapon()
        {
            AmmoType = "bullet";
            FireCount = 0;
        }
    }
}
