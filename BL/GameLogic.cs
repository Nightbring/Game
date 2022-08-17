using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace BL
{
    public static class GameLogic
    {
        public static ConcurrentDictionary<int, ConcurrentDictionary<string, ConcurrentDictionary<string, Entity.Ingame.BaseObject>>> allGames = new ConcurrentDictionary<int, ConcurrentDictionary<string, ConcurrentDictionary<string, Entity.Ingame.BaseObject>>>();

        public static ConcurrentDictionary<int, ConcurrentDictionary<int, (bool, (double, double))>> allFire = new ConcurrentDictionary<int, ConcurrentDictionary<int, (bool, (double, double))>>();

        public static ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>> allButtons = new ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>>();

        public static ConcurrentDictionary<int, ConcurrentDictionary<int, bool>> usersInGame = new ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>();

        public static void Initialization(Entity.Game game)
        {
            allButtons.TryAdd(game.Id, new ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>());
            allFire.TryAdd(game.Id, new ConcurrentDictionary<int, (bool, (double, double))>());
            usersInGame.TryAdd(game.Id, new ConcurrentDictionary<int, bool>());
            allGames.TryAdd(game.Id, new ConcurrentDictionary<string, ConcurrentDictionary<string, Entity.Ingame.BaseObject>>());


            var arr = new ConcurrentDictionary<string, Entity.Ingame.BaseObject>();

            for (int i = 0; i < 70; i++)
            {
                var wall = new Entity.Ingame.WallObject();
                wall.XPosition = i * 20;
                wall.YPosition = 0;
                arr.TryAdd(wall.Id, wall);
            }
            for (int i = 0; i < 70; i++)
            {
                var wall = new Entity.Ingame.WallObject();
                wall.XPosition = i * 20;
                wall.YPosition = 780;
                arr.TryAdd(wall.Id, wall);
            }
            for (int i = 1; i < 39; i++)
            {
                var wall = new Entity.Ingame.WallObject();
                wall.XPosition = 0;
                wall.YPosition = i * 20;
                arr.TryAdd(wall.Id, wall);
            }
            //for (int i = 1; i < 39; i++)
            //{
            //    var wall = new Entity.Ingame.WallObject();
            //    wall.XPosition = 1380;
            //    wall.YPosition = i * 20;
            //    arr.TryAdd(wall.Id, wall);
            //}

            var enemy = new Entity.Ingame.ShipObject();
            enemy.XPosition = 250;
            enemy.YPosition = 350;
            enemy.Name = "static dummy";
            enemy.Class = "cruiser-p";

            allGames[game.Id].TryAdd("static", arr);
            allGames[game.Id].TryAdd("ships", new ConcurrentDictionary<string, Entity.Ingame.BaseObject>());
            allGames[game.Id].TryAdd("ammo", new ConcurrentDictionary<string, Entity.Ingame.BaseObject>());
            allGames[game.Id].TryAdd("bg", new ConcurrentDictionary<string, Entity.Ingame.BaseObject>());

            allGames[game.Id]["ships"].TryAdd(enemy.Id, enemy);

            var star = new Entity.Ingame.DynamicBackground.BackgroundStar();
            star.Radius = 2000;
            star.Layer = 4;
            star.Planets = new Entity.Ingame.DynamicBackground.BackgroundPlanet[3]
            {
                new Entity.Ingame.DynamicBackground.BackgroundPlanet()
                {
                    Radius = 1000,
                    Layer = 4,
                    OrbitalRadius = 9000,
                    OrbitalSpeed = 2,
                    Angle = 0,
                    Moons = new Entity.Ingame.DynamicBackground.BackgroundMoon[2]
                    {
                        new Entity.Ingame.DynamicBackground.BackgroundMoon()
                        {
                            Radius = 120,
                            Layer = 4,
                            OrbitalRadius = 1400,
                            OrbitalSpeed = -2,
                            Angle = 0
                        },
                        new Entity.Ingame.DynamicBackground.BackgroundMoon()
                        {
                            Radius = 80,
                            Layer = 4,
                            OrbitalRadius = 2000,
                            OrbitalSpeed = -2,
                            Angle = 0
                        }
                    }
                },
                new Entity.Ingame.DynamicBackground.BackgroundPlanet()
                {
                    Radius = 600,
                    Layer = 4,
                    OrbitalRadius = 4500,
                    OrbitalSpeed = -3,
                },
                new Entity.Ingame.DynamicBackground.BackgroundPlanet()
                {
                    Radius = 500,
                    Layer = 4,
                    OrbitalRadius = 15000,
                    OrbitalSpeed = 2,
                }
            };
            allGames[game.Id]["bg"].TryAdd(star.Id, star);
        }

        public static void JoinUser(int gameId, int userId, string ship ="", string weapon = "")
        {
            bool exist = false;
            allButtons[gameId].TryAdd(userId, new ConcurrentDictionary<int, bool>());
            allFire[gameId].TryAdd(userId, (false, (0,0)));
            usersInGame[gameId].TryAdd(userId, true);

            foreach (var i in allGames[gameId]["ships"].Values)
            {
                var t = (Entity.Ingame.ShipObject)i;
                if (t.UserId == userId)
                {
                    exist = true;
                }
            }
            if (!exist && ship != "")
            {
                var userShip = new Entity.Ingame.ShipObject();
                userShip.XPosition = 150;
                userShip.YPosition = 150;
                userShip.UserId = userId;
                userShip.Name = UserBL.Get(userId).Name;
                userShip.Class = ship;
                
                if (weapon != "")
                {
                    foreach (var w in userShip.Weapons)
                    {
                        w.AmmoType = weapon;
                    }
                }
                
                allGames[gameId]["ships"].TryAdd(userShip.Id, userShip);
            }
        }

        public static void LeaveUser(int gameId, int userId)
        {
            if (usersInGame[gameId].ContainsKey(userId))
            {
                _ = usersInGame[gameId].Remove(userId, out _);
            }
        }

        public static bool Intersects(Entity.Ingame.DynamicObject o1, Entity.Ingame.DynamicObject o2)
        {
            var ax = o1.XPosition + o1.Speed * Math.Cos(o1.Rotation * Math.PI / 180);
            var ay = o1.YPosition + o1.Speed * Math.Sin(o1.Rotation * Math.PI / 180);
            var ar = o1.Radius;

            var bx = o2.XPosition + o2.Speed * Math.Cos(o1.Rotation * Math.PI / 180);
            var by = o2.YPosition + o2.Speed * Math.Sin(o1.Rotation * Math.PI / 180);
            var br = o2.Radius;

            return Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by)) < (ar + br);
        }

        public static bool Intersects(Entity.Ingame.DynamicObject o1, Entity.Ingame.StaticObject o2)
        {
            var ax = o1.XPosition - o1.Radius + o1.Speed * Math.Cos(o1.Rotation * Math.PI / 180);
            var ax1 = o1.XPosition + o1.Radius + o1.Speed * Math.Cos(o1.Rotation * Math.PI / 180);
            var ay = o1.YPosition - o1.Radius + o1.Speed * Math.Sin(o1.Rotation * Math.PI / 180);
            var ay1 = o1.YPosition + o1.Radius + o1.Speed * Math.Sin(o1.Rotation * Math.PI / 180);

            var bx = o2.XPosition - o2.Width/2;
            var bx1 = o2.XPosition + o2.Width/2;
            var by = o2.YPosition - o2.Height/2;
            var by1 = o2.YPosition + o2.Height/2;

            return (
                (
                    (
                        (ax >= bx && ax <= bx1) || (ax1 >= bx && ax1 <= bx1)
                    ) && (
                        (ay >= by && ay <= by1) || (ay1 >= by && ay1 <= by1)
                    )
                ) || (
                    (
                        (bx >= ax && bx <= ax1) || (bx1 >= ax && bx1 <= ax1)
                    ) && (
                        (by >= ay && by <= ay1) || (by1 >= ay && by1 <= ay1)
                    )
                )
            ) || (
                (
                    (
                        (ax >= bx && ax <= bx1) || (ax1 >= bx && ax1 <= bx1)
                    ) && (
                        (by >= ay && by <= ay1) || (by1 >= ay && by1 <= ay1)
                    )
                ) || (
                    (
                        (bx >= ax && bx <= ax1) || (bx1 >= ax && bx1 <= ax1)
                    ) && (
                        (ay >= by && ay <= by1) || (ay1 >= by && ay1 <= by1)
                    )
                )
            );
        }

        public static bool Intersects(Entity.Ingame.BaseObject o1, Entity.Ingame.BaseObject o2)
        {
            var ax = o1.XPosition - o1.Width / 2;
            var ax1 = o1.XPosition + o1.Width / 2;
            var ay = o1.YPosition - o1.Height / 2;
            var ay1 = o1.YPosition + o1.Height / 2;

            var bx = o2.XPosition - o2.Width / 2;
            var bx1 = o2.XPosition + o2.Width / 2;
            var by = o2.YPosition - o2.Height / 2;
            var by1 = o2.YPosition + o2.Height / 2;

            return (
                (
                    (
                        (ax >= bx && ax <= bx1) || (ax1 >= bx && ax1 <= bx1)
                    ) && (
                        (ay >= by && ay <= by1) || (ay1 >= by && ay1 <= by1)
                    )
                ) || (
                    (
                        (bx >= ax && bx <= ax1) || (bx1 >= ax && bx1 <= ax1)
                    ) && (
                        (by >= ay && by <= ay1) || (by1 >= ay && by1 <= ay1)
                    )
                )
            ) || (
                (
                    (
                        (ax >= bx && ax <= bx1) || (ax1 >= bx && ax1 <= bx1)
                    ) && (
                        (by >= ay && by <= ay1) || (by1 >= ay && by1 <= ay1)
                    )
                ) || (
                    (
                        (bx >= ax && bx <= ax1) || (bx1 >= ax && bx1 <= ax1)
                    ) && (
                        (ay >= by && ay <= by1) || (ay1 >= by && ay1 <= by1)
                    )
                )
            );
        }

        public static void PlayFire(int gameId)
        {
            //For every players
            foreach (var userId in usersInGame[gameId].Keys)
            {
                //Check fire
                foreach (var i in GameLogic.allGames[gameId]["ships"].Values)
                {
                    var ship = (Entity.Ingame.IterableObject)i;
                    if (ship.UserId == userId)
                    {
                        int j = 0;
                        foreach (var w in ship.Weapons)
                        {
                            w.X = Math.Cos((ship.Rotation + w.AnglePos) * Math.PI / 180) * w.Radius;// + ship.XPosition;
                            w.Y = Math.Sin((ship.Rotation + w.AnglePos) * Math.PI / 180) * w.Radius;// + ship.YPosition;

                            if (w.FireCount == w.FireCd)
                            {
                                if (GameLogic.allFire[gameId][userId].Item1)
                                {
                                    //Fire
                                    w.FireCount = 0;
                                    if (w.AmmoType == "missile")
                                    {
                                        if (ship.LockedTarget != "")
                                        {
                                            var missile = new Entity.Ingame.Ammo.Missile();
                                            var ammo = (Entity.Ingame.AmmoObject)missile;
                                            ammo.Shooter = ship;
                                            ammo.MyWeapon = j; // Array.IndexOf(ship.Weapons, w);

                                            w.Rotation = Math.Atan(-(allFire[gameId][userId].Item2.Item2 - (-w.Y + ship.YPosition)) / Math.Abs(allFire[gameId][userId].Item2.Item1 - (w.X + ship.XPosition))) * 180 / Math.PI;
                                            if (allFire[gameId][userId].Item2.Item1 < (w.X + ship.XPosition))
                                                w.Rotation = -1 * w.Rotation + 180;

                                            if (allGames[gameId]["ships"][ship.LockedTarget] != null)
                                            {
                                                missile.Target = (Entity.Ingame.IterableObject)allGames[gameId]["ships"][ship.LockedTarget];
                                            }
                                            ammo.SetPosition();

                                            allGames[gameId]["ammo"].TryAdd(ammo.Id, ammo);
                                        }
                                    }
                                    else if (w.AmmoType == "bullet")
                                    {
                                        var ammo = new Entity.Ingame.AmmoObject();
                                        ammo.Shooter = ship;
                                        ammo.MyWeapon = j; // Array.IndexOf(ship.Weapons, w);

                                        w.Rotation = Math.Atan(-(allFire[gameId][userId].Item2.Item2 - (-w.Y + ship.YPosition)) / Math.Abs(allFire[gameId][userId].Item2.Item1 - (w.X + ship.XPosition))) * 180 / Math.PI;

                                        if (allFire[gameId][userId].Item2.Item1 < (w.X + ship.XPosition))
                                            w.Rotation = -1 * w.Rotation + 180;

                                        ammo.SetPosition();

                                        allGames[gameId]["ammo"].TryAdd(ammo.Id, ammo);
                                    }
                                                                        

                                    
                                }
                            }
                            else
                            {
                                w.FireCount++;
                            }
                            j++;
                        }

                        

                        break;
                    }
                }
            }
        }

        public static void Control(int gameId)
        {
            foreach (var userId in usersInGame[gameId].Keys)
            {
                foreach (var i in allGames[gameId]["ships"].Values)
                {
                    if (i.ObjectType == "ship")
                    {
                        var ship = (Entity.Ingame.ShipObject)i;
                        if (ship.UserId == userId)
                        {
                            //left
                            if (GameLogic.allButtons[gameId][userId].ContainsKey(37) || GameLogic.allButtons[gameId][userId].ContainsKey(65))
                            {
                                ship.Rotation += ship.RSpeed;
                                if (ship.Rotation >= 360)
                                    ship.Rotation -= 360;
                            }
                            //right
                            if (GameLogic.allButtons[gameId][userId].ContainsKey(39) || GameLogic.allButtons[gameId][userId].ContainsKey(68))
                            {
                                ship.Rotation -= ship.RSpeed;
                                if (ship.Rotation < 0)
                                    ship.Rotation += 360;
                            }
                            //up
                            if (GameLogic.allButtons[gameId][userId].ContainsKey(38) || GameLogic.allButtons[gameId][userId].ContainsKey(87))
                            {
                                if (ship.Speed <= ship.MaxSpeed)
                                    ship.Speed += 5 * ship.Inertion;
                            }
                            //down
                            if (GameLogic.allButtons[gameId][userId].ContainsKey(40) || GameLogic.allButtons[gameId][userId].ContainsKey(83))
                            {
                                if (ship.Speed > 0)
                                    ship.Speed += -1 * ship.Inertion;
                                if (ship.Speed < 0)
                                    ship.Speed = 0;

                            }
                            break;
                        }
                    }
                }
            }
        }

        public static void MoveShip(int gameId)
        {
            foreach (var i in allGames[gameId]["ships"].Values) //Move ships
            {
                var ship = (Entity.Ingame.IterableObject)i;
                double velocityX = ship.Speed * Math.Cos(ship.Rotation * Math.PI / 180);
                double bX = Math.Cos(ship.Rotation * Math.PI / 180);
                double velocityY = ship.Speed * Math.Sin(ship.Rotation * Math.PI / 180);
                double bY = Math.Sin(ship.Rotation * Math.PI / 180);
                bool canMove = true;
                foreach (var j in allGames[gameId]["static"].Values)
                {
                    var s = (Entity.Ingame.StaticObject)j;
                    if (Intersects(ship, s) == true) //Collision
                    {
                        canMove = false;
                    }
                }

                if (canMove == true) //Move
                {
                    ship.XPosition += velocityX;
                    ship.YPosition += velocityY;
                }
                else
                {
                    ship.Speed = 0;
                    //Back
                    ship.XPosition -= (velocityX + bX);
                    ship.YPosition -= (velocityY + bY);
                }

                if (ship.Speed > 0) ship.Speed -= 1 * ship.Inertion;
                if (ship.Speed < 0) ship.Speed = 0;

                ship.TimeNotDamaged++;
                if (ship.TimeNotDamaged > 150 && ship.Shield < ship.FullShield)
                {
                    if (ship.TimeNotDamaged % 2 == 0)
                        ship.Shield += 1;
                    if (ship.Shield > ship.FullShield)
                        ship.Shield = ship.FullShield;
                }
            }
        }

        public static void MoveAmmo(int gameId)
        {
            foreach (var i in allGames[gameId]["ammo"].Values)
            {
                var ammo = (Entity.Ingame.AmmoObject)i;
                double velocityX = ammo.Speed * Math.Cos(ammo.Rotation * Math.PI / 180);
                double velocityY = ammo.Speed * Math.Sin(ammo.Rotation * Math.PI / 180);
                bool canMove = true;

                foreach (Entity.Ingame.IterableObject target in allGames[gameId]["ships"].Values)
                {
                    try { 
                        if (target.Id == ammo.Shooter.Id) continue;
                        if (Intersects(ammo, target) == true) //Hit
                        {

                            Hit(gameId, target.Id, ammo.Id);

                            canMove = false;

                            break;
                        }
                    } catch { break; }
                }

                foreach (var stat in allGames[gameId]["static"].Values)
                {
                    if (Intersects(ammo, stat) == true) //Collision
                    {
                        canMove = false;

                        break;
                    }
                }

                if (ammo.AmmoType == "missile")
                {
                    var missile = (Entity.Ingame.Ammo.Missile)ammo;
                    double targetAngle = -Math.Atan(-( missile.Target.YPosition - (missile.YPosition)) / Math.Abs(missile.Target.XPosition - (missile.XPosition))) * 180 / Math.PI;
                    if (missile.Target.XPosition < missile.XPosition)
                        targetAngle = -1 * targetAngle + 180;
                    if (targetAngle < 0)
                        targetAngle += 360;
                    if (targetAngle >= 360)
                        targetAngle -= 360;
                    if (missile.Rotation != targetAngle)
                    {
                        //if ()
                        missile.Rotation = targetAngle;
                    }
                }

                if (canMove == true) //Move
                {
                    ammo.XPosition += velocityX;
                    ammo.YPosition += velocityY;
                    ammo.Life -= 1;
                }
                else
                {
                    _ = allGames[gameId]["ammo"].Remove(ammo.Id, out _);
                }

                if (ammo.Life <= 0)
                {
                    _ = allGames[gameId]["ammo"].Remove(ammo.Id, out _);
                }
            }
        }

        public static void MoveOrbital(int gameId)
        {
            foreach (Entity.Ingame.DynamicBackground.BackgroundObject i in allGames[gameId]["bg"].Values) //For each star
            {
                if (i.Class == "star")
                {
                    var star = (Entity.Ingame.DynamicBackground.BackgroundStar)i;

                    if (star.Planets.Length > 0)
                        foreach (Entity.Ingame.DynamicBackground.BackgroundPlanet planet in star.Planets)
                        {
                            planet.Angle += planet.OrbitalSpeed/planet.OrbitalRadius;
                            if (planet.Angle >= 360)
                                planet.Angle -= 360;
                            if (planet.Angle < 0)
                                planet.Angle += 360;

                            planet.XPosition = star.XPosition + planet.OrbitalRadius * Math.Cos(planet.Angle / Math.PI);
                            planet.YPosition = star.YPosition + planet.OrbitalRadius * Math.Sin(planet.Angle / Math.PI);

                            if (planet.Moons.Length > 0)
                                foreach (Entity.Ingame.DynamicBackground.BackgroundMoon moon in planet.Moons)
                                {
                                    moon.Angle += moon.OrbitalSpeed / moon.OrbitalRadius;
                                    if (moon.Angle >= 360)
                                        moon.Angle -= 360;
                                    if (moon.Angle < 0)
                                        moon.Angle += 360;

                                    moon.XPosition = planet.XPosition + moon.OrbitalRadius * Math.Cos(moon.Angle / Math.PI);
                                    moon.YPosition = planet.YPosition + moon.OrbitalRadius * Math.Sin(moon.Angle / Math.PI);
                                }
                        }
                }
            }
        }

        public static void Hit(int gameId, string targetId, string ammoId)
        {
            var target = (Entity.Ingame.IterableObject)allGames[gameId]["ships"][targetId];
            try
            {
                target.TimeNotDamaged = 0;

                var ammo = (Entity.Ingame.AmmoObject)allGames[gameId]["ammo"][ammoId];

                if (target.Shield > 0)
                {
                    target.Shield -= ammo.Damage;
                    if (target.Shield < 0)
                        target.Shield = 0;
                }
                else
                {
                    target.HP -= ammo.Damage;
                }

                if (target.HP <= 0)
                {
                    target.HP = 0;
                    _ = allGames[gameId]["ships"].Remove(target.Id, out _);
                    foreach (Entity.Ingame.ShipObject ship in allGames[gameId]["ships"].Values)
                    {
                        //var ship
                        if (ship.LockedTarget == target.Id)
                        {
                            ship.LockedTarget = "";
                        }
                    }
                }
            } catch
            {
                Console.WriteLine("Error");
            }
        }

        public static void LockTarget(int gameId, int lockerId, string targetId, bool isLock)
        {
            if (usersInGame[gameId].ContainsKey(lockerId))
            {
                foreach (var i in allGames[gameId]["ships"].Values)
                {
                    if (isLock)
                    {
                        var ship = (Entity.Ingame.IterableObject)i;
                        if (ship.UserId == lockerId)
                        {
                            if (ship.Id != targetId)
                            {
                                ship.LockedTarget = targetId;
                                break;
                            } else
                            {
                                ship.LockedTarget = "";
                                break;
                            }
                            
                        } 
                    } else
                    {
                        var ship = (Entity.Ingame.IterableObject)i;
                        if (ship.UserId == lockerId)
                        {
                            ship.LockedTarget = "";
                            break;
                        }
                    }
                }
            }
        }
    }
}
