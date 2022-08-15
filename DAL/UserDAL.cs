using System;
using System.Linq;

namespace DAL
{
    public class UserDAL
    {
        static public int AddOrUpdate(Entity.User user)
        {
            using (GameContext data = new GameContext())
            {
                var databaseUser = data.Users.FirstOrDefault(x => x.Id == user.Id);
                if (databaseUser == null)
                {
                    databaseUser = new User();
                    data.Users.Add(databaseUser);
                }
                databaseUser.Id = user.Id;
                databaseUser.Name = user.Name;
                databaseUser.Password = user.Password;

                data.SaveChanges();

                return databaseUser.Id;
            }
        }

        static public Entity.User Get(int id)
        {
            using (GameContext data = new GameContext())
            {
                var databaseUser = data.Users.FirstOrDefault(x => x.Id == id);
                var user = new Entity.User();
                user.Id = id;
                user.Name = databaseUser.Name;
                user.Password = databaseUser.Password;

                return user;
            }
        }

        static public Entity.User Get(string login)
        {
            using (GameContext data = new GameContext())
            {
                var databaseUser = data.Users.FirstOrDefault(x => x.Name == login);
                if (databaseUser != null)
                {
                    var user = new Entity.User();
                    user.Id = databaseUser.Id;
                    user.Name = databaseUser.Name;
                    user.Password = databaseUser.Password;

                    return user;
                } return null;
            }
        }

        static public Entity.SearchOut.SearchOutUser Get(Entity.SearchIn.SearchingUser query)
        {
            var result = new Entity.SearchOut.SearchOutUser();

            using (GameContext data = new GameContext())
            {
                var temp = data.Users.Where(x => x.Name.StartsWith(query.Name));
                result.Total = temp.Count();
                if (query.Top.HasValue) {
                    temp = temp.Take(query.Top.Value);
                }
                if (query.Skip.HasValue)
                {
                    temp = temp.Skip(query.Skip.Value);
                }
                result.Users = temp.Select(x => new Entity.User()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Password = x.Password,
                }).ToList();
            }

            return result;
        }

    }
}
