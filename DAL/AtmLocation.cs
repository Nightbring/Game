using System;
using System.Collections.Generic;

#nullable disable

namespace DAL
{
    public partial class AtmLocation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longtitude { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? CloseTime { get; set; }
    }
}
