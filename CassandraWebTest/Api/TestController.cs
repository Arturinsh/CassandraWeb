using CassandraWebTest.DAO;
using CassandraWebTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CassandraWebTest.Api
{
    public class TestController : ApiController
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

        public class testUser
        {
            public string username { get; set; }
            public string password { get; set; }
        }

        [HttpPost]
        public bool TestPost(testUser user)
        {
            return false;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Mvc.Route("api/Test/List")]
        public async Task<IEnumerable<UsersModels>> Get()
        {
            IEnumerable<UsersModels> users = await usersDao.getUsers();
            return users.ToArray();
        }

        //public IEnumerable<string> Get()
        //{

        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }
    }
}