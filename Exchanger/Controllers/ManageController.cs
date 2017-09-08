using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI;
using Exchanger.DB;
using Exchanger.Helpers;
using Exchanger.Models;

namespace Exchanger.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage/Edit
        public ActionResult EditUser(string id)
        {
            if (Session["Id"] == null || id == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["Id"].ToString() != id && Session["UserTypeId"].ToString() != "2")
            {
                return RedirectToAction("HomePage", "Home");
            }
            using (var db = new ExchangedbEntities())
            {
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id));
                
                var userType = from type in db.UserType.ToList()
                            select type.Name;

                var model = new EditUserModel
                {
                    Id = dbUser.Id,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    ParentName = dbUser.ParentName,
                    Address = dbUser.Address.Address1,
                    Phone = dbUser.Address.Phone,
                    Email = dbUser.Address.Email,
                    Website = dbUser.Address.Website,
                    //UserType = dbUser.UserTypeId,
                    UserType = userType.ToList()
                };
                return PartialView(model);
            }
        }

        // POST: Manage/Edit
        [HttpPost]
        public ActionResult EditUser(ShowEditUserModel model)
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new ExchangedbEntities())
            {
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(model.Id.ToString()));
                try
                {
                    var host = (model.Email.Split('@'));
                    var hostname = host[1];

                    Dns.GetHostEntry(hostname);
                }
                catch
                {
                    ModelState.AddModelError("", "Not real Email");
                    return PartialView();
                }

                dbUser.FirstName = model.FirstName;
                dbUser.LastName = model.LastName;
                dbUser.ParentName = model.ParentName;
                dbUser.Address.Address1 = model.Address;
                dbUser.Address.Phone = model.Phone;
                dbUser.Address.Email = model.Email;
                dbUser.Address.Website = model.Website;
                dbUser.UserTypeId = db.UserType.FirstOrDefault(a => a.Name.Equals(model.UserType)).Id;

                db.SaveChanges();

                return Session["Id"].ToString() != dbUser.Id.ToString() ? 
                    RedirectToAction("AdminPage") : RedirectToAction("HomePage", "Home");
            }
        }

        //GET: Manage/AdminPage
        public ActionResult AdminPage()
        {
            if (Session["Id"] == null || Session["UserTypeId"].ToString() != "2")
            {
                return RedirectToAction("HomePage", "Home");
            }

            using (var db = new ExchangedbEntities())
            {
                //var users = db.Users.ToList();
                var users = db.Users.Select(user => new DisplayUserModel()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ParentName = user.ParentName,
                    Login = user.Login,
                    Files = user.Documents.Count,
                    UserType = user.UserType.Name
                }).ToList();

                return View(users);
            }
        }

        //GET: Manage/DeleteUser
        public ActionResult DeleteUser(string id)
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["UserTypeId"].ToString() != "2" && Session["Id"].ToString() != id.ToString())
            {
                return RedirectToAction("HomePage", "Home");
            }

            using (var db = new ExchangedbEntities())
            {
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id));

                var model = new ShowEditUserModel
                {
                    Id = dbUser.Id,
                    FirstName = dbUser.FirstName,
                    LastName = dbUser.LastName,
                    ParentName = dbUser.ParentName,
                    Address = dbUser.Address.Address1,
                    Phone = dbUser.Address.Phone,
                    Email = dbUser.Address.Email,
                    Files = dbUser.Documents.Count,
                    Website = dbUser.Address.Website,
                    UserType = dbUser.UserType.Name
                };

                return PartialView(model);
            }
        }

        //POST: Manage/DeleteUser
        [HttpPost]
        public ActionResult DeleteUser(Guid id)
        {
            using (var db = new ExchangedbEntities())
            {
                var addressId = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id.ToString())).AddressId;
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id.ToString()));

                var documents = dbUser.Documents.ToList();
                
                foreach (var doc in documents)
                {
                    System.IO.File.Delete(Server.MapPath("~/Files/" + dbUser.Login + "/" + doc.FileName));
                    dbUser.Documents.Remove(doc);
                    db.Documents.Remove(doc);
                }

                db.Users.Remove(db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id.ToString())));
                db.Address.Remove(db.Address.FirstOrDefault(a => a.Id.ToString().Equals(addressId.ToString())));

                db.SaveChanges();

                System.IO.Directory.Delete(Server.MapPath("~/Files/" + dbUser.Login));

                if (Session["UserTypeId"].ToString() == "2"
                    && Session["Id"].ToString() != dbUser.Id.ToString())
                {
                    return RedirectToAction("AdminPage");
                }
                Session.Abandon();
                return RedirectToAction("Register", "Account");
            }
        }

        //GET: Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            if (Session["Id"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }

        //POST: Manage/ChangePassword
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid) return View();
            using (var db = new ExchangedbEntities())
            {
                var id = Session["Id"].ToString();
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id));

                if (MD5Helper.GetHashString(model.OldPassword) != dbUser.Password)
                {
                    ModelState.AddModelError("", "Wrong old password");
                    return View();
                }

                dbUser.Password = MD5Helper.GetHashString(model.NewPassword);
                db.SaveChanges();

            }
            return RedirectToAction("HomePage", "Home");
        }
    }
}