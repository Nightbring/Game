using System;
using System.Linq;

namespace DAL
{
    public class SessionDAL
    {
        static public int AddOrUpdate(Entity.Session session)
        {
            using (GameContext data = new GameContext())
            {
                var databaseSession = data.Sessions.FirstOrDefault(x => x.Id == session.Id);
                if (databaseSession == null)
                {
                    databaseSession = new Session();
                    data.Sessions.Add(databaseSession);
                }
                databaseSession.Id = session.Id;
                databaseSession.Game = session.Game;
                databaseSession.User = session.User;
                databaseSession.Status = session.Status;

                data.SaveChanges();

                return databaseSession.Id;
            }
        }
        static public Entity.Session Get(int id)
        {
            using (GameContext data = new GameContext())
            {
                var databaseSession = data.Sessions.FirstOrDefault(x => x.Id == id);
                var session = new Entity.Session();
                session.Id = id;
                session.Game = databaseSession.Game;
                session.User = databaseSession.User;
                session.Status = databaseSession.Status;

                return session;
            }
        }

        static public Entity.Session Get(int user, int game)
        {
            using (GameContext data = new GameContext())
            {
                var databaseSession = data.Sessions.FirstOrDefault(x => x.User == user && x.Game == game && x.Status == true);
                var session = new Entity.Session();

                if (databaseSession != null)
                {
                    session.Id = databaseSession.Id;
                    session.Game = databaseSession.Game;
                    session.User = databaseSession.User;
                    session.Status = databaseSession.Status;

                }

                return session;
            }
        }

        static public Entity.SearchOut.SearchOutSession Get(Entity.SearchIn.SearchingSession query)
        {
            var result = new Entity.SearchOut.SearchOutSession();

            using (GameContext data = new GameContext())
            {
                var temp = data.Sessions.Where(x => x.Game.Equals(query.Game) && x.Status.Equals(true)); // ?
                result.Total = temp.Count();
                if (query.Top.HasValue)
                {
                    temp = temp.Take(query.Top.Value);
                }
                if (query.Skip.HasValue)
                {
                    temp = temp.Skip(query.Skip.Value);
                }
                result.Sessions = temp.Select(x => new Entity.Session()
                {
                    Id = x.Id,
                    Game = x.Game,
                    User = x.User,
                }).ToList();
            }

            return result;
        }
    }
}
