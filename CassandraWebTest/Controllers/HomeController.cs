﻿using CassandraWebTest.DAO;
using CassandraWebTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CassandraWebTest.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private static IArticlesDAO dao;
        private static IUsersDAO uDao;

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

        protected IUsersDAO usersDao
        {
            get
            {
                if (uDao == null)
                {
                    uDao = new UsersDAO();
                }
                return uDao;
            }
        }


        public async Task<ActionResult> Index()
        {
            IEnumerable<ArticlesModels> articles = await articlesDao.getArticles();
            return View("Index", articles.ToList().OrderByDescending(x => x.timestamp));
        }

        public ActionResult Watch(string id)
        {
            ArticlesModels model = articlesDao.ArticleById(id);
            if (Request.IsAuthenticated && model.author != User.Identity.Name)
                model.view_count = model.view_count + 1;
            articlesDao.updateArticle(model);
            return View(model);
        }

        public ActionResult PostComment(string comment, string postid)
        {
            ArticlesModels model = articlesDao.ArticleById(postid);
            Comment com = new Comment();
            com.votes = new List<Vote>();
            com.commentary = comment;
            com.author = User.Identity.Name;

            if (model.comments == null)
                model.comments = new List<Comment>();

            model.comments.Add(com);

            articlesDao.updateArticle(model);

            return RedirectToAction("Watch", "Home", new { @id = postid });
        }

        [Authorize]
        public ActionResult CreatePost()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreatePost(ArticlesModels model)
        {
            model.author = User.Identity.Name;
            model.timestamp = DateTimeOffset.Now;
            model.view_count = 0;
            await articlesDao.addArticle(model);

            var articles = await articlesDao.getAuthorArticles(User.Identity.Name);

            if (articles.Count() > 5)
            {
                await usersDao.SetUserNotification(User.Identity.Name, true);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<ActionResult> MyPosts()
        {
            IEnumerable<ArticlesModels> articles = await articlesDao.getAuthorArticles(User.Identity.Name);
            return View(articles.ToList().OrderByDescending(x => x.timestamp));
        }

        [HttpPost]
        [Authorize]
        public ActionResult Delete(string postid)
        {
            var post = articlesDao.ArticleById(postid);

            if (post.author == User.Identity.Name)
                articlesDao.deleteArticle(post);

            return RedirectToAction("MyPosts");
        }

        [Authorize]
        public ActionResult EditPost(string id)
        {
            var post = articlesDao.ArticleById(id);

            if (post.author == User.Identity.Name)
                return View(post);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditPost(ArticlesModels model)
        {
            var post = articlesDao.ArticleById(model.id.ToString());

            if (post.author != User.Identity.Name)
                return RedirectToAction("Index");

            post.title = model.title;
            post.content = model.content;
            post.timestamp = DateTimeOffset.Now;
            articlesDao.updateArticle(post);

            return RedirectToAction("Watch", "Home", new { @id = model.id });
        }

        [HttpPost]
        [Authorize]
        public ActionResult VoteComment(int vote, string postid, string commentid)
        {
            var post = articlesDao.ArticleById(postid);
            var comment = post.comments.Where(x => x.id == new Guid(commentid)).FirstOrDefault();

            if (comment.votes == null)
                comment.votes = new List<Vote>();

            var userVote = comment.votes.Where(x => x.username == User.Identity.Name);
            if (userVote.Count() > 0)
            {
                var tempList = comment.votes.ToList();
                tempList.Remove(userVote.FirstOrDefault());
                comment.votes = tempList;
            }
            else
            {
                Vote newVote = new Vote();
                newVote.username = User.Identity.Name;
                newVote.vote = vote;
                var tempVotes = comment.votes.ToList();
                tempVotes.Add(newVote);
                comment.votes = tempVotes;
            }

            articlesDao.updateArticle(post);

            return RedirectToAction("Watch", "Home", new { @id = postid });
        }

        public bool showNotification()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = usersDao.GetUserByName(User.Identity.Name);
                return user.notification;
            }
            return false;
        }

        public async Task removeNotification()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = usersDao.GetUserByName(User.Identity.Name);
                await usersDao.SetUserNotification(User.Identity.Name, false);
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}