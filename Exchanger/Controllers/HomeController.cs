using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Exchanger.DB;
using Exchanger.Models;

namespace Exchanger.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home/Index
        public ActionResult Index()
        {
            return View();
        }

        // GET: Home/HomePage
        public ActionResult HomePage()
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = Session["Id"];
            using (var db = new ExchangedbEntities())
            {
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(userId.ToString()));
                var dbAddress = db.Address.FirstOrDefault(a => a.Id.ToString().Equals(dbUser.AddressId.ToString()));

                var model = new EditUserModel
                {
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    ParentName = dbUser.ParentName,
                    Address = dbAddress.Address1,
                    Phone = dbAddress.Phone,
                    Email = dbAddress.Email,
                    Website = dbAddress.Website,
                    Files = dbUser.Documents.Count
                };

                return View(model);
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}