using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Exchanger.DB;
using Exchanger.Models;

namespace Exchanger.Controllers
{
    public class FileController : Controller
    {
        //GET: Files/UserFiles
        public ActionResult UserFiles()
        {
            using (var db = new ExchangedbEntities())
            {
                if (Session["Id"] == null) return RedirectToAction("Login", "Account");

                var id = Session["Id"].ToString();
                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id));

                //var documents = dbUser.Documents;
                var documents = dbUser.Documents.Select(file => new FileModel()
                {
                    Id = file.Id,
                    FileName = file.FileName,
                    Size = file.Size,
                    CreationDate = file.CreationDate,
                    LastEditDate = file.LastEditDate,
                    CreatedBy = file.CreatedBy,
                    Access = file.Access.Access1,
                    FileType = file.FileType.Type
                }).ToList();

                return View(documents);
            }
        }
        
        // GET: File/Upload
        public ActionResult Upload()
        {
            if (Session["Id"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return PartialView("Upload");
        }

        // POST: File/Upload
        [HttpPost]
        public ActionResult Upload(string baseData)
        {
            using (var db = new ExchangedbEntities())
            {
                if (HttpContext.Request.Files.AllKeys.Any())
                {
                    for (var i = 0; i < HttpContext.Request.Files.Count; i++)
                    {
                        var file = HttpContext.Request.Files["files" + i];
                        if (file != null)
                        {
                            var fileName = Path.GetFileName(file.FileName);

                            if (System.IO.File.Exists(Server.MapPath("~/Files/" + Session["Login"] + "/" + fileName)))
                            {
                                ModelState.AddModelError("", "File with " + fileName + " name exists \n");
                                //return PartialView("Upload");
                            }
                            else
                            {
                                file.SaveAs(Server.MapPath("~/Files/" + Session["Login"] + "/" + fileName));

                                var filesize =
                                    new FileInfo(Server.MapPath("~/Files/" + Session["Login"] + "/" + fileName)).Length;

                                var mimetype = MimeMapping.GetMimeMapping(fileName);

                                var dbFileType = db.FileType.FirstOrDefault(a => a.MIMEType.Equals(mimetype));

                                if (dbFileType == null && mimetype != null)
                                {
                                    var filetypeid = db.FileType.OrderByDescending(a => a.Id).FirstOrDefault().Id;

                                    dbFileType = new FileType
                                    {
                                        Id = filetypeid + 1,
                                        MIMEType = mimetype,
                                        Type = mimetype.Split('/')[0]
                                    };

                                    db.FileType.Add(dbFileType);
                                }

                                var document = new Documents
                                {
                                    Id = Guid.NewGuid(),
                                    AccessId = 1,
                                    CreatedBy = Session["Login"].ToString(),
                                    CreationDate = DateTime.Now,
                                    LastEditDate = DateTime.Now,
                                    FileName = fileName,
                                    Size = Convert.ToInt32(filesize),
                                    FileTypeId = dbFileType.Id
                                };


                                var id = Session["Id"].ToString();
                                var dbUser = db.Users.FirstOrDefault(a => a.Id.ToString().Equals(id));

                                dbUser.Documents.Add(document);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }
            return PartialView("Upload");
        }

        //POST: File/CheckFileExist
        [HttpPost]
        public ActionResult CheckFileExist(string name)
        {
            using (var db = new ExchangedbEntities())
            {
                var data = "";
                var login = Session["Login"].ToString();
                var dbUser = db.Users.FirstOrDefault(a => a.Login.Equals(login));
                var dbdocument = dbUser.Documents.FirstOrDefault(a => a.FileName.Equals(name));
                if (dbdocument != null)
                {
                    data = "Success";
                    return Json(data);
                }
                return Json(data);
            }
        }

        //GET: File/DeleteFile
        public ActionResult DeleteFile(string id)
        {
            if (Session["Id"] == null) return RedirectToAction("Login", "Account");

            using (var db = new ExchangedbEntities())
            {
                var dbDocument = db.Documents.FirstOrDefault(a => a.Id.ToString().Equals(id));
                var document = new FileModel()
                {
                    Id = dbDocument.Id,
                    FileName = dbDocument.FileName,
                    Size = dbDocument.Size,
                    CreationDate = dbDocument.CreationDate,
                    LastEditDate = dbDocument.LastEditDate,
                    CreatedBy = dbDocument.CreatedBy,
                    Access= dbDocument.CreatedBy,
                    FileType = dbDocument.FileType.Type
                };

                return PartialView("DeleteFile", document);
            }
        }

        //POST: File/DeleteFile
        [HttpPost]
        public ActionResult DeleteFile(Guid id)
        {
            using (var db = new ExchangedbEntities())
            {
                var dbDocument = db.Documents.FirstOrDefault(a => a.Id.ToString().Equals(id.ToString()));

                var login = Session["Login"].ToString();

                var dbUser = db.Users.FirstOrDefault(a => a.Login.Equals(login));

                if (dbDocument.CreatedBy != dbUser.Login)
                {
                    dbUser.Documents.Remove(dbDocument);

                    if (dbDocument.Users.Count == 2)
                    {
                        dbDocument.AccessId = 1;
                    }
                    db.SaveChanges();
                }
                else
                {
                    System.IO.File.Delete(Server.MapPath("~/Files/" + Session["Login"] + "/" + dbDocument.FileName));

                    dbUser.Documents.Remove(dbDocument);
                    db.Documents.Remove(dbDocument);

                    db.SaveChanges();
                }
            }

            return RedirectToAction("UserFiles");
        }

        //GET: File/EditFile
        public ActionResult EditFile(string id)
        {
            if (Session["Id"] == null) return RedirectToAction("Login", "Account");

            using (var db = new ExchangedbEntities())
            {
                var dbDocument = db.Documents.FirstOrDefault(a => a.Id.ToString().Equals(id));

                var document = new EditFileModel()
                {
                    Id = dbDocument.Id,
                    FileName = dbDocument.FileName,
                    Size = dbDocument.Size,
                    CreationDate = dbDocument.CreationDate,
                    LastEditDate = dbDocument.LastEditDate,
                    CreatedBy = dbDocument.CreatedBy,
                    Access = db.Access.Select(a => a.Access1).ToList(),
                    FileType = dbDocument.FileType.Type
                };

                return PartialView(document);
            }
        }

        //POST: File/EditFile
        [HttpPost]
        public ActionResult EditFile(FileModel model)
        {
            using (var db = new ExchangedbEntities())
            {
                var dbFile = db.Documents.FirstOrDefault(a => a.Id.ToString().Equals(model.Id.ToString()));
                
                System.IO.File.Move(Server.MapPath("~/Files/" + Session["Login"] + "/" + dbFile.FileName),
                    Server.MapPath("~/Files/" + Session["Login"] + "/" + model.FileName));

                dbFile.FileName = model.FileName;

                var mimetype = MimeMapping.GetMimeMapping(model.FileName);

                var dbFileType = db.FileType.FirstOrDefault(a => a.MIMEType.Equals(mimetype));

                if (dbFileType == null && mimetype != null)
                {
                    var filetypeid = db.FileType.OrderByDescending(a => a.Id).FirstOrDefault().Id;

                    dbFileType = new FileType
                    {
                        Id = filetypeid + 1,
                        MIMEType = mimetype,
                        Type = mimetype.Split('/')[0]
                    };

                    db.FileType.Add(dbFileType);
                }

                dbFile.FileTypeId = dbFileType.Id;
                dbFile.LastEditDate = DateTime.Now;

                if (model.Access == "Private")
                {
                    dbFile.Users.Clear();
                    var owner = db.Users.FirstOrDefault(a => a.Login.Equals(dbFile.CreatedBy));
                    dbFile.Users.Add(owner);
                }

                dbFile.AccessId = db.Access.FirstOrDefault(a => a.Access1.Equals(model.Access)).Id;

                db.SaveChanges();

                return RedirectToAction("UserFiles");
            }
        }
        
        //GET: File/Download
        public FileResult Download(string id)
        {
            using (var db = new ExchangedbEntities())
            {
                var doc = db.Documents.FirstOrDefault(a => a.Id.ToString().Equals(id));

                var filepath = Path.Combine(Server.MapPath("~/Files/" + doc.CreatedBy + "/" + doc.FileName));

                return File(filepath, MimeMapping.GetMimeMapping(filepath), doc.FileName);
            }
        }

        //GET: File/ShareFiles
        public ActionResult ShareFile(string id)
        {
            if (Session["Id"] == null) return RedirectToAction("Login", "Account");

            using (var db = new ExchangedbEntities())
            {
                var doc = db.Documents.FirstOrDefault(a => a.Id.ToString().Equals(id));

                var model = new ShareFileModel()
                {
                    Id = doc.Id,
                    FileName = doc.FileName,
                    Login = db.Users.OrderBy(a => a.Login).Select(a => a.Login).ToList()
                };
                
                return PartialView(model);
            }
        }

        //POST: File/ShareFiles
        [HttpPost]
        public ActionResult ShareFile(ShareModel model)
        {
            using (var db = new ExchangedbEntities())
            {
                var user = db.Users.FirstOrDefault(a => a.Login.Equals(model.Login));
                var doc = db.Documents.FirstOrDefault(a => a.Id.ToString().Equals(model.Id.ToString()));

                user.Documents.Add(doc);
                doc.AccessId = 2;
                db.SaveChanges();
            }

            return RedirectToAction("UserFiles");
        }
    }
}