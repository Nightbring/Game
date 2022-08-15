using System;
using System.Collections.Generic;

namespace Entity.Ingame.Ammo
{
    public class Missile : AmmoObject
    {
        public new IterableObject Shooter { get; set; }

        public IterableObject Target { get; set; } 

        public Missile() : base()
        {
            ObjectType = "ammo";
            AmmoType = "missile";
            Radius = 7.5;
            Height = 15;
            Width = 15;
            Rotation = 0;
            Life = 80;
            Damage = 20;
            Speed = 18;
        }
        public new void  SetPosition()
        {
            YPosition = Shooter.Weapons[MyWeapon].Y + Shooter.YPosition;
            XPosition = Shooter.Weapons[MyWeapon].X + Shooter.XPosition;
            Rotation = Shooter.Weapons[MyWeapon].Rotation;
        }
    }
}
