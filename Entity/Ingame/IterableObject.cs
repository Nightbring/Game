using System;
using System.Collections.Generic;

namespace Entity.Ingame
{
    public class IterableObject : DynamicObject
    {
        public int? UserId { get; set; }
        public string Name { get; set; }

        public int FullShield { get; set; }
        public int Shield { get; set; }
        public int FullHP { get; set; }
        public int HP { get; set; }
        public int FullEnergy { get; set; }
        public int Energy { get; set; }

        public double MaxSpeed { get; set; }
        public double RSpeed { get; set; }
        public double Inertion { get; set; }

        private string myClass { get; set; }
        public string Class {
            get {
                return myClass;
            } 
            set {
                myClass = value;
                switch (myClass)
                {
                    case "frigate-p":
                        Radius = 45;
                        Width = 2 * Radius;
                        Height = 2 * Radius;

                        FullShield = 300;
                        Shield = FullShield;
                        FullHP = 200;
                        HP = FullHP;
                        FullEnergy = 100;
                        Energy = FullEnergy;
                        MaxSpeed = 15;
                        RSpeed = 10;
                        Inertion = 0.8;

                        Weapons = new Weapon[3] { 
                            new Weapon() { Radius = 25, AnglePos = 120 }, 
                            new Weapon() { Radius = 10, AnglePos = 0 }, 
                            new Weapon() { Radius = 25, AnglePos = -120 } 
                        }; 
                        break;
                    case "frigate-r":
                        Radius = 50;
                        Width = 2 * Radius;
                        Height = 2 * Radius;

                        FullShield = 200;
                        Shield = FullShield;
                        FullHP = 350;
                        HP = FullHP;
                        FullEnergy = 100;
                        Energy = FullEnergy;
                        MaxSpeed = 14;
                        RSpeed = 7.5;
                        Inertion = 0.7;

                        Weapons = new Weapon[3] {
                            new Weapon() { Radius = 44, AnglePos = 110 },
                            new Weapon() { Radius = 28, AnglePos = 180 },
                            new Weapon() { Radius = 44, AnglePos = -110 }
                        };
                        break;
                    case "cruiser-p":
                        Radius = 75;
                        Width = 2 * Radius;
                        Height = 2 * Radius;

                        FullShield = 650;
                        Shield = FullShield;
                        FullHP = 400;
                        HP = FullHP;
                        FullEnergy = 100;
                        Energy = FullEnergy;
                        MaxSpeed = 13;
                        RSpeed = 5;
                        Inertion = 0.5;

                        Weapons = new Weapon[4] {
                            new Weapon() { Radius = 25, AnglePos = 153 },
                            new Weapon() { Radius = 27, AnglePos = 10 },
                            new Weapon() { Radius = 27, AnglePos = -10 },
                            new Weapon() { Radius = 25, AnglePos = -153 }
                        };
                        break;
                    case "cruiser-r":
                        Radius = 80;
                        Width = 2 * Radius;
                        Height = 2 * Radius;

                        FullShield = 400;
                        Shield = FullShield;
                        FullHP = 750;
                        HP = FullHP;
                        FullEnergy = 100;
                        Energy = FullEnergy;
                        MaxSpeed = 11;
                        RSpeed = 5;
                        Inertion = 0.4;

                        Weapons = new Weapon[4] {
                            new Weapon() { Radius = 56, AnglePos = 155 },
                            new Weapon() { Radius = 13, AnglePos = 145 },
                            new Weapon() { Radius = 13, AnglePos = -145 },
                            new Weapon() { Radius = 56, AnglePos = -155 }
                        };
                        break;
                    case "battleship-p":
                        Radius = 125;
                        Width = 2 * Radius;
                        Height = 2 * Radius;
                        FullShield = 900;
                        Shield = FullShield;
                        FullHP = 800;
                        HP = FullHP;
                        FullEnergy = 100;
                        Energy = FullEnergy;
                        MaxSpeed = 8;
                        RSpeed = 2.5;
                        Inertion = 0.3;

                        Weapons = new Weapon[5] { 
                            new Weapon() { Radius = 50, AnglePos = 130 }, 
                            new Weapon() { Radius = 25, AnglePos = 85 },
                            new Weapon() { Radius = 10, AnglePos = 0 },
                            new Weapon() { Radius = 25, AnglePos = -85 }, 
                            new Weapon() { Radius = 50, AnglePos = -130 } 
                        };
                        break;
                    case "battleship-r":
                        Radius = 125;
                        Width = 2 * Radius;
                        Height = 2 * Radius;
                        FullShield = 500;
                        Shield = FullShield;
                        FullHP = 1300;
                        HP = FullHP;
                        FullEnergy = 100;
                        Energy = FullEnergy;
                        MaxSpeed = 7;
                        RSpeed = 2.5;
                        Inertion = 0.25;

                        Weapons = new Weapon[5] {
                            new Weapon() { Radius = 55, AnglePos = 30 },
                            new Weapon() { Radius = 75, AnglePos = 135 },
                            new Weapon() { Radius = 65, AnglePos = 180 },
                            new Weapon() { Radius = 75, AnglePos = -135 },
                            new Weapon() { Radius = 55, AnglePos = -30 }
                        };
                        break;
                    default:
                        Radius = 10;
                        FullShield = 0;
                        Shield = FullShield;
                        FullHP = 100;
                        HP = FullHP;
                        FullEnergy = 0;
                        Energy = FullEnergy;

                        Weapons = new Weapon[0];
                        break;
                }
            } 
        }
        public string Party { get; set; }

        public Weapon[] Weapons {get; set;}

        public string LockedTarget { get; set; }

        public int TimeNotDamaged { get; set; }

        public IterableObject() : base()
        {
            LockedTarget = "";
            Radius = 0;
            Width = 2 * Radius;
            Height = 2 * Radius;
            YPosition = 0;
            XPosition = 0;
            Speed = 0;
            Rotation = 0;
            UserId = 0;
            TimeNotDamaged = 0;
        }
    }
}
