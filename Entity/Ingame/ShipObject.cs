using System;
using System.Collections.Generic;

namespace Entity.Ingame
{
    public class ShipObject : IterableObject
    {
        public ShipObject() : base()
        {
            YPosition = 0;
            XPosition = 0;
            ObjectType = "ship";
            Speed = 0;
            Rotation = 0;
            UserId = 0;

            //Weapons = new Weapon[4] { new Weapon() { Radius = 25, AnglePos = 153 }, new Weapon() { Radius = 25, AnglePos = -153 }, new Weapon() { Radius = 27, AnglePos = 10 }, new Weapon() { Radius = 27, AnglePos = -10 } };
        }
    }
}
