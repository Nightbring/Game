using System;
using System.Collections.Generic;

namespace Entity.Ingame
{
    public class AmmoObject : DynamicObject
    {
        public string AmmoType { get; set; }

        public IterableObject Shooter { get; set; }
        public int MyWeapon { get; set; }
        public int Life { get; set; }
        public int Damage { get; set; }
        public AmmoObject() : base()
        {
            ObjectType = "ammo";
            AmmoType = "bullet";
            Radius = 2;
            Height = 5;
            Width = 5;
            Rotation = 0;
            Life = 40;
            Damage = 5;
            Speed = 20;
        }
        
        public void SetPosition()
        {
            YPosition = Shooter.Weapons[MyWeapon].Y + Shooter.YPosition;
            XPosition = Shooter.Weapons[MyWeapon].X + Shooter.XPosition;
            Rotation = Shooter.Weapons[MyWeapon].Rotation;
        }
    }
}
