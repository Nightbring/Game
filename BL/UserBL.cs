using System;

namespace BL
{
    static public class UserBL
    {
        static public int AddOrUpdate(Entity.User user)
        {
            return DAL.UserDAL.AddOrUpdate(user);
        }

        public static bool Authorization(Entity.User user)
        {
            var temp = DAL.UserDAL.Get(user.Name);
            if (temp != null)
            {
                if (user.Password == temp.Password) { 
                    return true; 
                }
            } 
            return false;
        }

        static public Entity.User Get(int id)
        {
            return DAL.UserDAL.Get(id);
        }

        static public Entity.User Get(string login)
        {
            return DAL.UserDAL.Get(login);
        }

        static public Entity.SearchOut.SearchOutUser Get(Entity.SearchIn.SearchingUser query)
        {
            return DAL.UserDAL.Get(query);
        }
    }
}
