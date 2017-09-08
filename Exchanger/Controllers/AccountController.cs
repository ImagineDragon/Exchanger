using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Profile;
using System.Web.UI.WebControls;
using Exchanger.DB;
using Exchanger.Helpers;
using Exchanger.Models;

namespace Exchanger.Controllers
{
    public class AccountController : Controller
    {
        //GET: Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Session["Id"] != null)
                return RedirectToAction("HomePage", "Home");
            return View();
        }

        //POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            using (var db = new ExchangedbEntities())
            {
                if (ModelState.IsValid)
                {
                    var dbUser = db.Users.FirstOrDefault(a => a.Login.Equals(model.Login)) ??
                                 db.Users.FirstOrDefault(a => a.Address.Email.Equals(model.Login));

                    if (dbUser != null)
                    {
                        if (dbUser.Password == MD5Helper.GetHashString(model.Password) &&
                            dbUser.ConfirmEmail)
                        {
                            Session["Id"] = dbUser.Id.ToString();
                            Session["Login"] = dbUser.Login;
                            Session["UserTypeId"] = dbUser.UserTypeId;
                            return RedirectToAction("HomePage", "Home");
                            //return RedirectToAction("HomePage", "Home", new { userId = dbUser.Id});
                        }
                        if (!dbUser.ConfirmEmail)
                        {
                            ModelState.AddModelError("", "Email is not confirmed");
                        }
                        ModelState.AddModelError("", "Wrong password");
                    }
                    else ModelState.AddModelError("", "Wrong login");
                }
                else ModelState.AddModelError("", "Wrong input");

                return View(model);
            }
        }

        //GET: Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //POST: Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            using (var db = new ExchangedbEntities())
            {
                var dbUser = db.Users.FirstOrDefault(a => a.Address.Email.Equals(model.Email));
                if (dbUser != null)
                {
                    var temp = Guid.NewGuid().ToString();
                    Session["temp"] = temp;

                    MailHelper.SendMail(dbUser.Address.Email, "Restore password",
                        string.Format("Restore your password : <a href=\"{0}\" title=\"Restore password\">{0}</a>",
                        Url.Action("RestorePassword", "Account", new { Token = temp, Email = dbUser.Address.Email },
                        Request.Url.Scheme)));
                }
                return RedirectToAction("Confirm", "Account", new { Email = dbUser.Address.Email });
            }
        }

        //GET: Account/RestorePassword
        [AllowAnonymous]
        public ActionResult RestorePassword(string token, string email)
        {
            using (var db = new ExchangedbEntities())
            {
                var dbUser = db.Users.FirstOrDefault(a => a.Address.Email.Equals(email));
                if (dbUser != null && Session["temp"].ToString() == token)
                {
                    Session["Id"] = dbUser.Id;
                    return View();
                }
                return RedirectToAction("Login");
            }
        }

        //POST: Account/RestorePassword
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RestorePassword(RestorePasswordModel model)
        {
            if (!ModelState.IsValid) return View();
            using (var db = new ExchangedbEntities())
            {
                var id = Session["Id"].ToString();
                Session.Abandon();
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id));
                if (dbUser != null)
                {
                    dbUser.Password = MD5Helper.GetHashString(model.NewPassword);
                    db.SaveChanges();
                }
                return RedirectToAction("Login");
            }
        }

        //GET: Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (Session["Id"] != null)
                return RedirectToAction("HomePage", "Home");
            return View();
        }

        //POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            using (var db = new ExchangedbEntities())
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var dbUser = db.Users.FirstOrDefault(a => a.Login.ToLower().Equals(model.Login.ToLower()));
                if (dbUser != null)
                {
                    ModelState.AddModelError("", "This login is used");
                    return View();
                }
                var dbAddress = db.Address.FirstOrDefault(a => a.Email.Equals(model.Email));
                if (dbAddress != null)
                {
                    ModelState.AddModelError("", "This email is used");
                    return View();
                }
                try
                {
                    var host = (model.Email.Split('@'));
                    var hostname = host[1];

                    Dns.GetHostEntry(hostname);
                }
                catch
                {
                    ModelState.AddModelError("", "Not real Email");
                    return View();
                }

                var address = new Address {Id = Guid.NewGuid(), Email = model.Email};
                db.Address.Add(address);
                db.SaveChanges();

                var user = new Users
                {
                    Id = Guid.NewGuid(),
                    Login = model.Login,
                    Password = MD5Helper.GetHashString(model.Password),
                    AddressId = address.Id,
                    UserTypeId = 1,
                    ConfirmEmail = false
                };
                db.Users.Add(user);
                db.SaveChanges();

                Directory.CreateDirectory(Server.MapPath("~/Files/" + user.Login + "/"));

                MailHelper.SendMail(user.Address.Email, "Email confirmation",
                    string.Format("Confirm your email : <a href=\"{0}\" title=\"Confirm email\">{0}</a>",
                        Url.Action("ConfirmEmail", "Account", new {Token = user.Id, Email = user.Address.Email},
                            Request.Url.Scheme)));
                
                //Session["Id"] = user.Id.ToString();
                //Session["Login"] = user.Login;
                //Session["UserTypeId"] = user.UserTypeId;

                //return RedirectToAction("Edit", "Manage", new {id = user.Id});

                return RedirectToAction("Confirm", "Account", new {Email = address.Email});
            }
        }

        [AllowAnonymous]
        public string Confirm(string email)
        {
            return "Check your email " + email;
        }

        [AllowAnonymous]
        public ActionResult ConfirmEmail(string token, string email)
        {
            using (var db = new ExchangedbEntities())
            {
                var user = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(token));

                if (user == null) return RedirectToAction("Register", "Account");

                if (user.Address.Email != email) return RedirectToAction("Register", "Account");

                user.ConfirmEmail = true;
                db.SaveChanges();

                Session["Id"] = user.Id.ToString();
                Session["Login"] = user.Login;
                Session["UserTypeId"] = user.UserTypeId;

                return RedirectToAction("HomePage", "Home");
            }
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}