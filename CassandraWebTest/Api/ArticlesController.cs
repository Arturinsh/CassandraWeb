using CassandraWebTest.DAO;
using CassandraWebTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CassandraWebTest.Api
{
    public class ArticlesController : ApiController
    {
        private static IArticlesDAO dao;

        protected IArticlesDAO articlesDao
        {
            get
            {
                if (dao == null)
                {
                    dao = new ArticlesDAO();
                }
                return dao;
            }
        }

        // GET: api/Articles
        public async Task<IEnumerable<ArticlesModels>> Get()
        {
            IEnumerable<ArticlesModels> articles = await articlesDao.getArticles();
            return articles.ToList().OrderByDescending(x => x.timestamp);
        }

        // GET: api/Articles/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Articles
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Articles/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Articles/5
        public void Delete(int id)
        {
        }
    }
}
