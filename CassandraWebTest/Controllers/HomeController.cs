using CassandraWebTest.DAO;
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

            if (articles.Count() > 4)
            {
                var user = usersDao.GetUserByName(User.Identity.Name);
                if (user.employeeid == 0)
                {
                    await usersDao.SetUserNotification(User.Identity.Name, true);
                }
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

        public async Task<JsonResult> getEmployees()
        {
            List<Employee> employees = new List<Employee>();
            employees.Add(new Employee() { Email = User.Identity.Name });
            if (User.Identity.IsAuthenticated)
            {
                var articles = await articlesDao.getAuthorArticles(User.Identity.Name);
                foreach (var article in articles)
                {
                    foreach (var comment in article.comments)
                    {
                        var user = usersDao.GetUserByName(comment.author);
                        if (user.employeeid == 0)
                        {
                            if (!employees.Exists(x => x.Email == user.username))
                            {
                                employees.Add(new Employee() { Email = user.username });
                            }
                        }
                    }
                }
            }
            return Json(employees, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task setUserEmployeeIds(Company model)
        {
            foreach (var emp in model.Employees)
            {
                var usr = usersDao.GetUserByName(emp.Email);
                usr.employeeid = emp.Id;
                usersDao.updateUser(usr);
            }
            if (User.Identity.IsAuthenticated)
            {
                var user = usersDao.GetUserByName(User.Identity.Name);
                await usersDao.SetUserNotification(User.Identity.Name, false);
            }
        }

        //TEST code
        [HttpPost]
        public JsonResult testCreateCompany(Company model)
        {
            if (model.Employees != null)
            {
                foreach (var emp in model.Employees)
                {
                    emp.Id = 1;
                }
            }
            return Json(model);
        }

        public async Task<JsonResult> getEmployeeComments(int id)
        {
            List<CommentModel> comments = new List<CommentModel>();

            var articles = await articlesDao.getArticles();

            foreach (var article in articles)
            {
                foreach (var comment in article.comments)
                {
                    var usr = usersDao.GetUserByName(comment.author);
                    if (usr.employeeid == id)
                    {
                        int voteValue = 0;
                        foreach (var vote in comment.votes)
                        {
                            voteValue += vote.vote;
                        }

                        comments.Add(new CommentModel() { commentary = comment.commentary, vote = voteValue });
                    }
                }
            }
            return Json(comments, JsonRequestBehavior.AllowGet);
        }

        public bool showReaddEmployees()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = usersDao.GetUserByName(User.Identity.Name);
                if (user.employeeid != 0)
                    return true;
            }
            return false;
        }

        public async Task<JsonResult> getReAddEmp()
        {
            EmployeeReAdd reAddList = new EmployeeReAdd();
            List<Employee> employees = new List<Employee>();
            if (User.Identity.IsAuthenticated)
            {
                var articles = await articlesDao.getAuthorArticles(User.Identity.Name);
                foreach (var article in articles)
                {
                    foreach (var comment in article.comments)
                    {
                        var user = usersDao.GetUserByName(comment.author);
                        if (user.employeeid == 0)
                        {
                            if (!employees.Exists(x => x.Email == user.username))
                            {
                                employees.Add(new Employee() { Email = user.username });
                            }
                        }
                    }
                }
                reAddList.EmployeeId = usersDao.GetUserByName(User.Identity.Name).employeeid;
                reAddList.Employees = employees;
            }
            return Json(reAddList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void setUserReEmployeeIds(EmployeeReAdd model)
        {
            if (model.Employees != null)
            {
                foreach (var emp in model.Employees)
                {
                    if (emp != null)
                    {
                        var usr = usersDao.GetUserByName(emp.Email);
                        usr.employeeid = emp.Id;
                        usersDao.updateUser(usr);
                    }
                }
            }
        }


        //TEST code
        [HttpPost]
        public JsonResult testReAdd(EmployeeReAdd model)
        {

            if (model.Employees != null)
            {
                foreach (var emp in model.Employees)
                {
                    emp.Id = 2;
                }
            }
            return Json(model);
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