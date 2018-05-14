using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityStruct;
using EntityStruct.EntityTable;

namespace DataStruct
{
    public class UsersProvider
    {
        /// <summary>
        /// Проверяем, пустая ли база данных пользователя.
        /// </summary>
        /// <returns></returns>
        public static bool IsEmptyUserDatabase()
        {
            bool exists = false;
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                exists = (0 < db.Users.Count());
            }
            return !exists;
        }

        public static bool IsValidUserId(string userID)
        {
            bool exists = false;
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var q = from u in db.Users
                        where u.Username == userID
                        select u;
                exists = (0 < q.Count());
            }

            return exists;
        }

        public static bool IsValidPAssword(string userID, string pw)
        {
            bool exists = false;
            using (var db = new InventoryContext(DatabaseConnection.ConnectionString))
            {
                var q = from u in db.Users
                        where u.Username == userID
                        select u;
                foreach (var user in q)
                {
                    exists = exists || EncriptionData.Confirm(pw, user.Password, EncriptionData.Supported_HA.SHA256);
                }
            }

            return exists;
        }

        public static bool NewUser(string userID, string password)
        {
            UsersEntity User = new UsersEntity(userID, EncriptionData.ComputeHash(password, EncriptionData.Supported_HA.SHA256, null));
            return DatabaseConnection.Add<UsersEntity>(User);
        }

        public static bool DeleteUser(string userID)
        {
            return DatabaseConnection.Remove<UsersEntity>(p => p.Username == userID);
        }

        public static bool Modify(string oldUserId, string newUserId, string password)
        {
            UsersEntity User = new UsersEntity(newUserId, EncriptionData.ComputeHash(password, EncriptionData.Supported_HA.SHA256,null));
            if (oldUserId != newUserId)
            {
                return (DeleteUser(oldUserId) && NewUser(newUserId, password));
            }

            return DatabaseConnection.Modify<UsersEntity>(User, p => p.Username == oldUserId);
        }

        public static List<UsersEntity> ListUsers()
        {
            var list = DatabaseConnection.ListTable<UsersEntity>(p => true);
            foreach (var u in list)
                u.Password = "******";
            return list;
        }
    }
}
