using CassandraWebTest.DAO;
using CassandraWebTest.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CassandraWebTest.App_Start
{
    public class MyUserManager
    {
        private static IUsersDAO dao;

        protected IUsersDAO usersDao
        {
            get
            {
                if (dao == null)
                {
                    dao = new UsersDAO();
                }
                return dao;
            }
        }

        public bool IsValid(string username, string password)
        {
            return usersDao.checkUserValidation(username, password);
        }

        public IdentityResult CreateAsync(ApplicationUser user, string password)
        {
            bool freeUsername = usersDao.checkUserNameAvailability(user.Email);
            if (freeUsername)
            {
                UsersModels newUser = new UsersModels()
                {
                    username = user.Email,
                    password = password
                };

                usersDao.addUser(newUser);
                return IdentityResult.Success;
            }
            return new IdentityResult("Username is already taken");
        }

        public UsersModels FindById(string id)
        {
            return usersDao.GetUserByName(id);
        }

        public IdentityResult ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (IsValid(username, oldPassword))
            {
                var user = usersDao.GetUserByName(username);
                user.password = newPassword;
                usersDao.updateUser(user);

                return IdentityResult.Success;
            }
            return new IdentityResult("Failed to change password");
        }

        public IdentityResult AddPassword(string username, string newPassword)
        {
            var user = usersDao.GetUserByName(username);
            if (string.IsNullOrEmpty(user.password))
            {
                user.password = newPassword;
                usersDao.updateUser(user);

                return IdentityResult.Success;
            }
            return new IdentityResult("User already have password");
        }
    }
}