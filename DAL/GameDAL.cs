using System;
using System.Linq;

namespace DAL
{
    public class GameDAL
    {
        static public int AddOrUpdate(Entity.Game game)
        {
            using (GameContext data = new GameContext())
            {
                var databaseGame = data.Games.FirstOrDefault(x => x.Id == game.Id);
                if (databaseGame == null)
                {
                    databaseGame = new Game();
                    data.Games.Add(databaseGame);
                }
                databaseGame.Id = game.Id;
                databaseGame.Data = game.Data;
                databaseGame.Host = game.Host;
                databaseGame.Status = game.Status;

                data.SaveChanges();
                return databaseGame.Id;
            }
        }

        static public Entity.Game Get(int id)
        {
            using (GameContext data = new GameContext())
            {
                var databaseGame = data.Games.FirstOrDefault(x => x.Id == id);
                var game = new Entity.Game();
                game.Id = id;
                game.Data = databaseGame.Data;
                game.Host = databaseGame.Host;
                game.Status = databaseGame.Status;
                game.HostName = databaseGame.HostNavigation.Name;
                game.PlayersCount = databaseGame.Sessions.Select(x => x.Status == true).Count();

                return game;
            }
        }

        static public Entity.SearchOut.SearchOutGame Get(Entity.SearchIn.SearchingGame query)
        {
            var result = new Entity.SearchOut.SearchOutGame();

            using (GameContext data = new GameContext())
            {
                var temp = data.Games.Where(x => !query.Host.HasValue || x.Host.Equals(query.Host));
                result.Total = temp.Count();
                if (query.Top.HasValue)
                {
                    temp = temp.Take(query.Top.Value);
                }
                if (query.Skip.HasValue)
                {
                    temp = temp.Skip(query.Skip.Value);
                }
                result.Games = temp.Select(x => new Entity.Game()
                {
                    Id = x.Id,
                    Data = x.Data,
                    Host = x.Host,
                    Status = x.Status,
                    HostName = x.HostNavigation.Name,
                    PlayersCount = x.Sessions.Count(),
                }).ToList();
            }

            return result;
        }
    }
}
