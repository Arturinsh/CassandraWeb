using CassandraWebTest.DAO;
using CassandraWebTest.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        public SignInStatus ExternalSignIn(ExternalLoginInfo loginInfo, HttpContextBase context)
        {
            if (loginInfo.Login.LoginProvider == "Facebook")
            {
                var user = usersDao.FindByFacebookKey(loginInfo.Login.ProviderKey);
                if (user != null)
                {
                    var ident = new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.NameIdentifier, user.username),
                        new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                        new Claim(ClaimTypes.Name, user.username)
                    },
                    DefaultAuthenticationTypes.ApplicationCookie);
                    context.GetOwinContext().Authentication.SignIn(
                     new AuthenticationProperties { IsPersistent = false }, ident);

                    return SignInStatus.Success;
                }
            }
            if (loginInfo.Login.LoginProvider == "Google")
            {
                var user = usersDao.FindByGoogleKey(loginInfo.Login.ProviderKey);
                if (user != null)
                {
                    var ident = new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.NameIdentifier, user.username),
                        new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                        new Claim(ClaimTypes.Name, user.username)
                    },
                    DefaultAuthenticationTypes.ApplicationCookie);
                    context.GetOwinContext().Authentication.SignIn(
                     new AuthenticationProperties { IsPersistent = false }, ident);

                    return SignInStatus.Success;
                }
            }

            var emailUser = usersDao.GetUserByName(loginInfo.Email);
            if (emailUser != null)
            {
                bool login = false;
                if (loginInfo.Login.LoginProvider == "Facebook")
                {
                    emailUser.fbkey = loginInfo.Login.ProviderKey;
                    usersDao.updateUser(emailUser);
                    login = true;
                }

                if (loginInfo.Login.LoginProvider == "Google")
                {
                    emailUser.gkey = loginInfo.Login.ProviderKey;
                    usersDao.updateUser(emailUser);
                    login = true;
                }

                if (login)
                {
                    var ident = new ClaimsIdentity(
                    new[] {
                        new Claim(ClaimTypes.NameIdentifier, emailUser.username),
                        new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
                        new Claim(ClaimTypes.Name, emailUser.username)
                    },
                    DefaultAuthenticationTypes.ApplicationCookie);
                    context.GetOwinContext().Authentication.SignIn(
                     new AuthenticationProperties { IsPersistent = false }, ident);

                    return SignInStatus.Success;
                }
            }

            return SignInStatus.Failure;
        }

        public IdentityResult AddExternalLogin(ExternalLoginInfo loginInfo, ApplicationUser user)
        {
            if (loginInfo.Login.LoginProvider == "Facebook")
            {
                UsersModels newUser = new UsersModels()
                {
                    username = user.Email,
                    fbkey = loginInfo.Login.ProviderKey
                };
                usersDao.addUser(newUser);
                return IdentityResult.Success;
            }

            if (loginInfo.Login.LoginProvider == "Google")
            {
                UsersModels newUser = new UsersModels()
                {
                    username = user.Email,
                    gkey = loginInfo.Login.ProviderKey
                };
                usersDao.addUser(newUser);
                return IdentityResult.Success;
            }
            return new IdentityResult("Error adding user");
        }

    }
}