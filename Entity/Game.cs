using System;

namespace Entity
{
    public class Game
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public int? Host { get; set; }
        public bool? Status { get; set; }
        public string HostName { get; set; }
        public int PlayersCount { get; set; }
    }
}
