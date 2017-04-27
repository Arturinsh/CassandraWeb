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
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private static IUsersDAO dao;

        public UsersController() { }

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

        // GET: Users
        //[Authorize]
        public async Task<ActionResult> Index()
        {
            IEnumerable<UsersModels> users = await usersDao.getUsers();

            return View("Index", users.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(UsersModels model)
        {
            await usersDao.addUser(model);
            return RedirectToAction("Index");
        }

        public  ActionResult Edit(string id)
        {
            //TODO like this below same for delete
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //USER uSER = db.USERS.Find(id);
            //if (uSER == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(uSER);
            UsersModels model = usersDao.FindById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UsersModels model)
        {
            if (ModelState.IsValid)
            {
                usersDao.updateUser(model);
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult Delete(string id)
        {
            //TODO like this below same for delete
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //USER uSER = db.USERS.Find(id);
            //if (uSER == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(uSER);
            UsersModels model = usersDao.FindById(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(UsersModels user)
        {
            //UsersModels user = usersDao.getUser(id);
            usersDao.deleteUser(user);
            return RedirectToAction("Index");
        }


    }
}