using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStruct;
using EntityStruct.EntityTable;

namespace BusinessStruct
{
    public class UserLogin
    {
        private static string _loginedUser;

        public static string LoginedUser
        {
            get
            {
                if (_loginedUser == null) _loginedUser = "";
                return _loginedUser;
            }
        }

        public static bool IsEmptyUserDatabase()
        {
            return UsersProvider.IsEmptyUserDatabase();
        }

        public static bool IsValidUserId(string userId)
        {
            return UsersProvider.IsValidUserId(userId);
        }

        public static bool IsValidPassword(string userId, string password)
        {
            return UsersProvider.IsValidPAssword(userId, password);
        }

        public static bool AddUser(string userId, string password)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("Ошибка логина.", "userId");
            }
            else if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Ошибка пароля.", "userId");
            }

            return UsersProvider.NewUser(userId, password);
        }

        public static bool RemoveUser(string userId)
        {
            return UsersProvider.DeleteUser(userId);
        }

        public static void Login(string userId, string password)
        {
            if (string.IsNullOrWhiteSpace(userId) || !UserLogin.IsValidUserId(userId))
            {
                throw new ArgumentException("Неправильный логин", "userId");
            }
            else if (string.IsNullOrWhiteSpace(password) || !UserLogin.IsValidPassword(userId, password))
            {
                throw new ArgumentException("Неправильный пароль", "password");
            }

            _loginedUser = userId;
        }

        public static List<UsersEntity> ListUsers()
        {
            return UsersProvider.ListUsers();
        }

        public static bool ModifyUser(string userId, string oldPassword, string newUserId, string password,
                                      string confirm)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Неверный пароль", "password");
            }
            else if (string.IsNullOrWhiteSpace(confirm) || password != confirm)
            {
                throw new ArgumentException("Неверный повторный пароль", "confirm");
            }
            else if (string.IsNullOrWhiteSpace(userId) || !UserLogin.IsValidUserId(userId))
            {
                throw new ArgumentException("Неправильный старый логин", "userId");
            }
            else if (string.IsNullOrWhiteSpace(oldPassword) || !UserLogin.IsValidPassword(userId, oldPassword))
            {
                throw new ArgumentException("Неправильный старый пароль", "oldPassword");
            }
            else if (string.IsNullOrWhiteSpace(newUserId) || (userId != newUserId && UserLogin.IsValidUserId(newUserId)))
            {
                throw new ArgumentException("Неправильный новый логин", "newUserId");
            }
            else if (UsersProvider.Modify(userId, newUserId, password))
            {
                if (_loginedUser == userId) _loginedUser = newUserId;
                return true;
            }
            else
                return false;
        }
    }
}
