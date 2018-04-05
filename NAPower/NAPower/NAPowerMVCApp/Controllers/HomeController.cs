using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NAPowerMVCApp.Models;

namespace NAPowerMVCApp.Controllers
{
    public class HomeController : Controller
    {
        private UserAccountModel _model = new UserAccountModel();

        public ActionResult Index()
        {
            List<UserAccount> accounts = _model.GetAll();
            return View(accounts);
        }

        public ActionResult UserAccount(int id = 0)
        {
            UserAccount account = _model.GetUserAccount(id);
            return View("UserAccount", account);
        }

        public ActionResult Add()
        {
            List<UserAccount> accounts = _model.GetAll();
            var maxid = accounts.Max(r => r.Id);
            UserAccount account = new UserAccount();
            account.Id = maxid + 1;
            return View("UserAccount", account);
        }

        [HttpPost]
        public ActionResult Add(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                _model.AddAccount(account);
                return RedirectToAction("Index");
            }

            return View("UserAccount", account);
        }

        public ActionResult Delete(int id = 0)
        {
            _model.DeleteAccount(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UserAccount(UserAccount account)
        {
            if (ModelState.IsValid)
            {
                _model.UpdateAccount(account);
                return RedirectToAction("Index");
            }

            return View("UserAccount", account);
        }

    }
}