using Cassandra;
using Cassandra.Mapping;
using CassandraWebTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CassandraWebTest.DAO
{
    public interface IArticlesDAO
    {
        Task<IEnumerable<ArticlesModels>> getArticles();
        ArticlesModels ArticleById(string id);
        ArticlesModels updateArticle(ArticlesModels model);
        Task addArticle(ArticlesModels article);
        Task<IEnumerable<ArticlesModels>> getAuthorArticles(string author);
        void deleteArticle(ArticlesModels post);
    }

    public class ArticlesDAO : IArticlesDAO
    {
        protected readonly ISession session;
        protected readonly IMapper mapper;

        public ArticlesDAO()
        {
            ICassandraDAO cassandraDAO = new CassandraDAO();
            session = cassandraDAO.GetSession();
            mapper = new Mapper(session);
        }

        public async Task<IEnumerable<ArticlesModels>> getArticles()
        {
            return await mapper.FetchAsync<ArticlesModels>();
        }

        public ArticlesModels ArticleById(string id)
        {
            return mapper.FirstOrDefault<ArticlesModels>("WHERE id = ?", new Guid(id));
        }

        public ArticlesModels updateArticle(ArticlesModels model)
        {
            if (model.comments == null)
            {
                model.comments = new List<Comment>();
            }
            mapper.Update<ArticlesModels>("Set timestamp =?, view_count=?, author = ?, title = ?, content = ?, comments =? where id = ?",
                model.timestamp, model.view_count, model.author, model.title, model.content, model.comments, model.id);
            return model;
        }

        public async Task addArticle(ArticlesModels article)
        {
            article.id = Guid.NewGuid();
            await mapper.InsertAsync(article);
        }
        public async Task<IEnumerable<ArticlesModels>> getAuthorArticles(string author)
        {
            return await mapper.FetchAsync<ArticlesModels>("Where author = ?", author);
        }

        public void deleteArticle(ArticlesModels article)
        {
            mapper.Delete<ArticlesModels>("Where id = ?", article.id);
        }
    }
}