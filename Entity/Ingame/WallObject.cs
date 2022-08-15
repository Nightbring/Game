using System;


namespace Entity.Ingame
{
    public class WallObject : StaticObject
    {
        public WallObject() : base()
        {
            ObjectType = "wall";
            Width = 20;
            Height = 20;
            XPosition = 0;
            YPosition = 0;
        }
    }
}
