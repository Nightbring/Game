using System;
using System.Collections.Generic;

namespace Entity.Ingame
{
    public class BaseObject
    {
        public string Id { get; set; }
        public double XPosition { get; set; }
        public double YPosition { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public string ObjectType { get; set; }
        private static int CurentId = 0;

        public BaseObject()
        {
            CurentId++;
            Id = CurentId.ToString();
        }
    }
}
