using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using NoteMarketPlace.Models;
namespace NoteMarketPlace.Controllers
{
    public class AdminController : Controller
    {
        private Notes_MarketplaceEntities Db = new Notes_MarketplaceEntities();


        public ActionResult Admin_Header()
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                var systemDisplay_image = db.SystemConfigurations.Where(s => s.K_ey == "Display_image").Select(s => s.Value).FirstOrDefault();
                var systemFacebook = db.SystemConfigurations.Where(s => s.K_ey == "Facebook").Select(s => s.Value).FirstOrDefault();
                var systemTwitter = db.SystemConfigurations.Where(s => s.K_ey == "Twitter").Select(s => s.Value).FirstOrDefault();
                var systemLinkedIn = db.SystemConfigurations.Where(s => s.K_ey == "LinkedIn").Select(s => s.Value).FirstOrDefault();
                try
                {
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name && x.IsActive==true);
                    var userdata = db.Users_Admin.FirstOrDefault(s => s.UserID == user.ID);
                    TempData["RoleId"] = user.RoleID;
                    TempData["Twitter"] = systemTwitter;
                    TempData["LinkedIn"] = systemLinkedIn;
                    TempData["Facebook"] = systemFacebook;
                    if (userdata != null)
                    {
                        if (userdata.Profile_Picture != null)
                        {
                            //ViewBag.userdetails = userdata.Profile_Picture;
                            TempData["Userdetails"] = userdata.Profile_Picture;
                        }
                        else
                        {
                            TempData["Userdetails"] = systemDisplay_image;
                        }
                    }
                    else
                    {
                        TempData["Userdetails"] = systemDisplay_image;
                    }
                }
                catch
                {
                    TempData["Twitter"] = systemTwitter;
                    TempData["LinkedIn"] = systemLinkedIn;
                    TempData["Facebook"] = systemFacebook;
                    TempData["Userdetails"] = systemDisplay_image;
                }
                return View("~/Views/Shared/Admin_Header.cshtml");
            }
        }
        [Authorize]
        public ActionResult Dashboard_admin(string SearchText, int? changeInMonth)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    ViewBag.downloadcount = 0;
                    var sellernote = db.SellerNotes.Where(x => (x.Status == "approved") && x.IsActive == true).OrderBy(x=>x.PublishedDate).ToList();
                    ViewBag.SellerNoteDetails = null;
                    var inReviewNotes = db.SellerNotes.Where(x => x.Status == "submitted" || x.Status == "inReview").ToList();
                    ViewBag.InReviewNotes = inReviewNotes.Count;
                    var dt = DateTime.Now.AddDays(-7);
                    var LastDaysDownloaded = db.Downloads.Where(d =>( d.AttachmentDownloadDate > dt ) && d.IsActive==true && d.IsSellerHasAllowedDownload==true).ToList();
                    ViewBag.LastDownloadednotes = LastDaysDownloaded.Count;
                    var LastRegistration = db.Users.Where(u => (u.JoiningDate > dt) && u.IsActive==true).ToList();
                    ViewBag.LastUsers = LastRegistration.Count;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var userdata = db.Users.ToList();
                    ViewBag.userdetails = userdata;
                    var downloaddata = db.Downloads.ToList();
                    ViewBag.downloaddetails = downloaddata;
                    if (changeInMonth != null)
                    {
                        sellernote = db.SellerNotes.Where(x => (x.PublishedDate.Month == changeInMonth) && (x.Status == "approved") && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }

                    }
                    else if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isfname = db.Users.Any(d => d.FirstName == SearchText);
                            bool islname = db.Users.Any(d => d.LastName == SearchText);
                            bool isPrice = decimal.TryParse(SearchText, out decimal price);
                            List<string> sellMode = new List<string>();
                            sellMode.Add("free");
                            sellMode.Add("paid");
                            bool isSellType = sellMode.Contains(SearchText.ToLower());
                            if (isTitle)
                            {
                                sellernote = db.SellerNotes.Where(x => (x.Title == SearchText) && (x.Status == "approved") && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive == true).Select(x => x.CategoryID).ToList();
                                sellernote = db.SellerNotes.Where(x => (cid.Contains(x.Category)) && (x.Status == "approved") && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }

                            }

                            if (isfname)
                            {
                                var userid = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsActive == true && (userid.Contains(x.SellerID))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (islname)
                            {
                                var userid = db.Users.Where(u => u.LastName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsActive == true && (userid.Contains(x.SellerID))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isPrice)
                            {

                                bool Price = db.SellerNotes.Any(d => d.SellingPrice == price && (d.Status == "approved"));
                                if (Price)
                                {
                                    sellernote = db.SellerNotes.Where(x => (x.Status == "approved") && x.SellingPrice == price && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                    if (sellernote.Count != 0)
                                    {
                                        ViewBag.SellerNoteDetails = sellernote;
                                    }
                                }
                            }
                                if (isSellType)
                                {
                                    if (SearchText.ToLower() == "free")
                                    {
                                        sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsPaid == false && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                    }
                                    if (SearchText.ToLower() == "paid")
                                    {
                                        sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsPaid == true && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                    }
                                    if (sellernote.Count != 0)
                                    {
                                        ViewBag.SellerNoteDetails = sellernote;
                                    }
                                }

                            }
                    }
                    else
                    {
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }
                    }
                    }
            }
                    return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult UserProfile_Admin()
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    var getCountryList = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList, CountryNameList;
                    User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    bool isusersigned = db.Users.Any(u => u.ID == user.ID);
                    Admin_Profile up = new Admin_Profile();
                    if (isusersigned)
                    {
                        up.Firstname = user.FirstName;
                        up.Lastname = user.LastName;
                        up.emailId = user.EmailID;
                    }
                    bool isRecordcreated = db.Users_Admin.Any(p => p.UserID == user.ID);
                    if (isRecordcreated)
                    {
                        var userList = db.Users_Admin.FirstOrDefault(p => p.UserID == user.ID);
                        up.Secondary_emailId = userList.Secondary_Email;
                        up.phoneNo = userList.Phone_Number;
                        int coun_code = Convert.ToInt32(userList.Country_Code);
                        Country countrycode = db.Countries.FirstOrDefault(c => c.CountryID == coun_code && c.IsActive == true);
                        if (countrycode != null)
                        {
                            object counId = countrycode.CountryID;
                            CountryList = new SelectList(getCountryList, "CountryID", "CountryCode", counId);
                            ViewBag.countrycode = CountryList;
                        }
                        else
                        {
                            CountryList = new SelectList(getCountryList, "CountryID", "CountryCode");
                            ViewBag.countrycode = CountryList;
                        }
                        up.profilePictureGet = userList.Profile_Picture;
                        string filename = Path.GetFileName(userList.Profile_Picture);
                        ViewBag.picname = filename;
                        return View(up);
                    }
                    CountryList = new SelectList(getCountryList, "CountryID", "CountryCode");
                    ViewBag.countrycode = CountryList;
                    ViewBag.picname = null;
                    return View(up);
                }
            }
                    return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult UserProfile_Admin(Users_Admin adminProfileDb ,Admin_Profile adminProfileModel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    int userProfileTemp = 0;
                    int userTemp = 0;

                    var user = db.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                    bool isRecordcreated = db.Users_Admin.Any(p => p.UserID == user.ID);


                    if (user != null)
                    {
                        userTemp = 1;
                    }
                    if (isRecordcreated)
                    {
                        adminProfileDb = db.Users_Admin.Where(u => u.UserID == user.ID).FirstOrDefault();
                        userProfileTemp = 1;
                    }
                    user.FirstName = adminProfileModel.Firstname;
                    user.LastName = adminProfileModel.Lastname;
                    adminProfileDb.UserID = user.ID;
                    adminProfileDb.Secondary_Email = adminProfileModel.Secondary_emailId;
                    adminProfileDb.Country_Code = adminProfileModel.CountryCode;
                    adminProfileDb.Phone_Number = adminProfileModel.phoneNo;
                    adminProfileDb.IsActive = true;
                    var path = Server.MapPath("~\\UploadedFiles\\") + user.ID;
                    if (!(Directory.Exists(path)))
                    {
                        Directory.CreateDirectory(path);
                        ViewBag.Mesage = "Directory created";
                    }
                    else
                    {
                        ViewBag.Mesage = "Directory exist";
                    }
                    string temp_path = "~\\UploadedFiles\\" + user.ID + "\\";
                    if (adminProfileModel.profilePicture != null && adminProfileModel.profilePicture.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(adminProfileModel.profilePicture.FileName);
                        string imgext = Path.GetExtension(_FileName);
                        if (imgext == ".jpg" || imgext == ".gif" || imgext == ".jpeg" || imgext == ".png")
                        {

                            string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                            adminProfileModel.profilePicture.SaveAs(_path);
                            string upload_picpath = temp_path + _FileName;
                            adminProfileDb.Profile_Picture = upload_picpath;
                        }
                        else
                        {
                            ViewBag.Extensionerror = "Please select image file";
                            return View();
                        }
                    }
                    else if (adminProfileModel.profilePictureGet != null)
                    {
                        adminProfileDb.Profile_Picture = adminProfileModel.profilePictureGet;
                    }
                    else
                    {
                        adminProfileDb.Profile_Picture = null;
                    }
                    adminProfileDb.ModifiedDate = DateTime.Now;
                    if (userTemp == 1)
                    {
                        db.Entry(user).State = EntityState.Modified;
                    }
                    else
                    {

                        db.Users.Add(user);
                    }
                    if (userProfileTemp == 1)
                    {
                        db.Entry(adminProfileDb).State = EntityState.Modified;
                    }
                    else
                    {
                        adminProfileDb.CreatedDate = DateTime.Now;
                        db.Users_Admin.Add(adminProfileDb);
                    }

                    db.SaveChanges();

                }
            }
            return RedirectToAction("Dashboard_admin", "Admin");
        }

        [Authorize]
        public ActionResult ManageCategory(string SearchText)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var categoryList = db.NoteCategories.OrderBy(x => x.CreatedDate).ToList();
                    ViewBag.categorydetails = null;
                    var admindata = db.Users.ToList();
                    ViewBag.admindetails = admindata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isuser = db.Users.Any(u => u.FirstName == SearchText && u.RoleID != 1);
                            List<string> Activetype = new List<string>();
                            Activetype.Add("yes");
                            Activetype.Add("no");
                            bool isactiveType = Activetype.Contains(SearchText);
                            if (isCat)
                            {
                                categoryList = db.NoteCategories.Where(x => x.CategoryName == SearchText).OrderBy(x => x.CreatedDate).ToList();
                                if (categoryList.Count != 0)
                                {
                                    ViewBag.categorydetails = categoryList;
                                }

                            }
                            if (isuser)
                            {
                                var user = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                categoryList = db.NoteCategories.Where(x => user.Contains(x.CreatedBy)).OrderBy(x => x.CreatedDate).ToList();
                                if (categoryList.Count != 0)
                                {
                                    ViewBag.categorydetails = categoryList;
                                }
                            }
                            if (isactiveType)
                            {
                                if (SearchText == "yes")
                                {
                                    categoryList = db.NoteCategories.Where(x => x.IsActive == true).OrderBy(x => x.CreatedDate).ToList();
                                }
                                if (SearchText == "no")
                                {
                                    categoryList = db.NoteCategories.Where(x => x.IsActive == false).OrderBy(x => x.CreatedDate).ToList();
                                }
                                if (categoryList.Count != 0)
                                {
                                    ViewBag.categorydetails = categoryList;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (categoryList.Count != 0)
                        {
                            ViewBag.categorydetails = categoryList;
                        }
                    }
                }
            }
                    return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult AddCategory(int?catid)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    if (catid != null)
                    {
                        bool isrecordCreated = db.NoteCategories.Any(n => n.CategoryID == catid);
                        if (isrecordCreated)
                        {
                            var categorydetail = db.NoteCategories.FirstOrDefault(c => c.CategoryID == catid);
                            Category category = new Category();
                            category.categoryID = categorydetail.CategoryID;
                            category.categoryName = categorydetail.CategoryName;
                            category.Description = categorydetail.Description;
                            return View(category);
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddCategory(Category category,NoteCategory cat_db)
        {
                using (var db=new Notes_MarketplaceEntities())
                {
                    int categoryTemp =0;
                    if(category.categoryID!=null)
                    {
                        cat_db = db.NoteCategories.Where(c => c.CategoryID == category.categoryID).FirstOrDefault();
                        if (cat_db != null)
                        {
                            categoryTemp = 1;
                        }
                    }
                    User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    cat_db.CategoryName = category.categoryName;
                    cat_db.Description = category.Description;
                    cat_db.IsActive = true;
                    if (categoryTemp == 1)
                    {
                        cat_db.ModifiedDate = DateTime.Now;
                        cat_db.ModifiedBy = user.ID;
                        db.Entry(cat_db).State = EntityState.Modified;
                    }
                    else
                    {
                        cat_db.CreatedDate = DateTime.Now;
                        cat_db.CreatedBy = user.ID;
                        db.NoteCategories.Add(cat_db);
                    }
                    db.SaveChanges();
                }
            
            return RedirectToAction("ManageCategory","Admin");
        }
        [Authorize]
        public ActionResult DeleteCategory(int?catid)
        {
            using(var db=new Notes_MarketplaceEntities())
            {
                NoteCategory cat_db = db.NoteCategories.Where(c => c.CategoryID == catid).FirstOrDefault();
                User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                cat_db.IsActive = false;
                cat_db.ModifiedDate = DateTime.Now;
                cat_db.ModifiedBy = user.ID;
                db.Entry(cat_db).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ManageCategory", "Admin");
        }

        [Authorize]
        public ActionResult ManageType(string SearchText)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var typeList = db.NoteTypes.OrderBy(x => x.CreatedDate).ToList();
                    ViewBag.typedetails = null;
                    var admindata = db.Users.ToList();
                    ViewBag.admindetails = admindata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isType = db.NoteTypes.Any(d => d.TypeName == SearchText);
                            bool isuser = db.Users.Any(u => u.FirstName == SearchText && u.RoleID != 1);
                            List<string> Activetype = new List<string>();
                            Activetype.Add("yes");
                            Activetype.Add("no");
                            bool isactiveType = Activetype.Contains(SearchText);
                            if (isType)
                            {
                                typeList = db.NoteTypes.Where(x => x.TypeName == SearchText).OrderBy(x => x.CreatedDate).ToList();
                                if (typeList.Count != 0)
                                {
                                    ViewBag.typedetails = typeList;
                                }

                            }
                            if (isuser)
                            {
                                var user = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                typeList = db.NoteTypes.Where(x => user.Contains(x.CreatedBy)).OrderBy(x => x.CreatedDate).ToList();
                                if (typeList.Count != 0)
                                {
                                    ViewBag.typedetails = typeList;
                                }
                            }
                            if (isactiveType)
                            {
                                if (SearchText == "yes")
                                {
                                    typeList = db.NoteTypes.Where(x => x.IsActive == true).OrderBy(x => x.CreatedDate).ToList();
                                }
                                if (SearchText == "no")
                                {
                                    typeList = db.NoteTypes.Where(x => x.IsActive == false).OrderBy(x => x.CreatedDate).ToList();
                                }
                                if (typeList.Count != 0)
                                {
                                    ViewBag.typedetails = typeList;
                                }

                            }

                        }
                    }
                    else
                    {
                        if (typeList.Count != 0)
                        {
                            ViewBag.typedetails = typeList;
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult AddType(int? typeid)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    if (typeid != null)
                    {
                        bool isrecordCreated = db.NoteTypes.Any(n => n.TypeID == typeid);
                        if (isrecordCreated)
                        {
                            var typedetail = db.NoteTypes.FirstOrDefault(c => c.TypeID == typeid);
                            Note_Type typeObj = new Note_Type();
                            typeObj.typeId = typedetail.TypeID;
                            typeObj.typeName = typedetail.TypeName;
                            typeObj.Description = typedetail.Description;
                            return View(typeObj);
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddType(Note_Type typeObj, NoteType type_db)
        {
                using (var db = new Notes_MarketplaceEntities())
                {
                    int typeTemp = 0;
                    if (typeObj.typeId != null)
                    {
                        type_db = db.NoteTypes.Where(c => c.TypeID == typeObj.typeId).FirstOrDefault();
                        if (type_db != null)
                        {
                            typeTemp = 1;
                        }
                    }
                    User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    type_db.TypeName = typeObj.typeName;
                    type_db.Description = typeObj.Description;
                    type_db.IsActive = true;
                    if (typeTemp == 1)
                    {
                        type_db.ModifiedDate = DateTime.Now;
                        type_db.ModifiedBy = user.ID;
                        db.Entry(type_db).State = EntityState.Modified;
                    }
                    else
                    {
                        type_db.CreatedDate = DateTime.Now;
                        type_db.CreatedBy = user.ID;
                        db.NoteTypes.Add(type_db);
                    }
                    db.SaveChanges();
                }
            return RedirectToAction("ManageType", "Admin");
        }
        [Authorize]
        public ActionResult DeleteType(int? typeid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                NoteType type_db = db.NoteTypes.Where(c => c.TypeID == typeid).FirstOrDefault();
                User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                type_db.IsActive = false;
                type_db.ModifiedDate = DateTime.Now;
                type_db.ModifiedBy = user.ID;
                db.Entry(type_db).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ManageType", "Admin");
        }

        [Authorize]
        public ActionResult ManageCountry(string SearchText)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var countryList = db.Countries.ToList();
                    ViewBag.countrydetails = null;
                    var admindata = db.Users.ToList();
                    ViewBag.admindetails = admindata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool iscountry = db.Countries.Any(d => d.CountryName == SearchText);
                            bool isuser = db.Users.Any(u => u.FirstName == SearchText && u.RoleID != 1);
                            List<string> ActiveCountry = new List<string>();
                            ActiveCountry.Add("yes");
                            ActiveCountry.Add("no");
                            bool isactiveCountry = ActiveCountry.Contains(SearchText);
                            if (iscountry)
                            {
                                countryList = db.Countries.Where(x => x.CountryName == SearchText).OrderBy(x => x.CreatedDate).ToList();
                                if (countryList.Count != 0)
                                {
                                    ViewBag.countrydetails = countryList;
                                }

                            }
                            if (isuser)
                            {
                                var user = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                countryList = db.Countries.Where(x => user.Contains(x.CreatedBy)).OrderBy(x => x.CreatedDate).ToList();
                                if (countryList.Count != 0)
                                {
                                    ViewBag.countrydetails = countryList;
                                }

                            }
                            if (isactiveCountry)
                            {
                                if (SearchText == "yes")
                                {
                                    countryList = db.Countries.Where(x => x.IsActive == true).OrderBy(x => x.CreatedDate).ToList();
                                }
                                if (SearchText == "no")
                                {
                                    countryList = db.Countries.Where(x => x.IsActive == false).OrderBy(x => x.CreatedDate).ToList();
                                }
                                if (countryList.Count != 0)
                                {
                                    ViewBag.countrydetails = countryList;
                                }

                            }

                        }
                    }
                    else { 
                    if (countryList.Count != 0)
                    {
                        ViewBag.countrydetails = countryList;
                    }
                    }
                }
            }
            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult AddCountry(int? countryid)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    if (countryid != null)
                    {
                        bool isrecordCreated = db.Countries.Any(n => n.CountryID == countryid);
                        if (isrecordCreated)
                        {
                            var countrydetail = db.Countries.FirstOrDefault(c => c.CountryID == countryid);
                            Note_Country countryObj = new Note_Country();
                            countryObj.countryID = countrydetail.CountryID;
                            countryObj.countryName = countrydetail.CountryName;
                            countryObj.countrycode = countrydetail.CountryCode;
                            return View(countryObj);
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddCountry(Note_Country countryObj, Country country_db)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                int countryTemp = 0;
                if (countryObj.countryID != null)
                {
                    country_db = db.Countries.Where(c => c.CountryID == countryObj.countryID).FirstOrDefault();
                    if (country_db != null)
                    {
                        countryTemp = 1;
                    }
                }
                User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                country_db.CountryName = countryObj.countryName;
                country_db.CountryCode = countryObj.countrycode;
                country_db.IsActive = true;
                if (countryTemp == 1)
                {
                    country_db.ModifiedDate = DateTime.Now;
                    country_db.ModifiedBy = user.ID;
                    db.Entry(country_db).State = EntityState.Modified;
                }
                else
                {
                    country_db.CreatedDate = DateTime.Now;
                    country_db.CreatedBy = user.ID;
                    db.Countries.Add(country_db);
                }
                db.SaveChanges();
            }
            return RedirectToAction("ManageCountry", "Admin");
        }
        [Authorize]
        public ActionResult DeleteCountry(int? countryid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                Country country_db = db.Countries.Where(c => c.CountryID == countryid).FirstOrDefault();
                User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                country_db.IsActive = false;
                country_db.ModifiedDate = DateTime.Now;
                country_db.ModifiedBy = user.ID;
                db.Entry(country_db).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ManageCountry", "Admin");
        }
        [Authorize]
        [HttpGet]
        public ActionResult AddAdministrator(int? adminid)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    var getCountryList = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList;
                    if (adminid != null)
                    {
                        bool isrecordCreated = db.Users.Any(n => n.ID == adminid);
                        if (isrecordCreated)
                        {
                            var admindetail = db.Users.FirstOrDefault(c => c.ID == adminid);
                            var adminprofiledetail = db.Users_Admin.FirstOrDefault(p => p.UserID == adminid);
                            Administrator adminObj = new Administrator();
                            adminObj.adminid = admindetail.ID;
                            adminObj.fname = admindetail.FirstName;
                            adminObj.lname = admindetail.LastName;
                            adminObj.email = admindetail.EmailID;
                            adminObj.phoneno = adminprofiledetail.Phone_Number;
                            int coun_code = Convert.ToInt32(adminprofiledetail.Country_Code);
                            Country country = db.Countries.FirstOrDefault(c => c.CountryID == coun_code && c.IsActive == true);
                            object counId = country.CountryID;
                            CountryList = new SelectList(getCountryList, "CountryID", "CountryCode", counId);
                            ViewBag.countrycode = CountryList;
                            return View(adminObj);
                        }
                    }
                    CountryList = new SelectList(getCountryList, "CountryID", "CountryCode");
                    ViewBag.countrycode = CountryList;
                }
            }
                    return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddAdministrator(Administrator administrator,User user,Users_Admin adminUserProfileDb)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                int userProfileTemp = 0;
                int userTemp = 0;
                var superAdmin = db.Users.FirstOrDefault(u => u.EmailID == User.Identity.Name);
                if (administrator.adminid != null)
                {
                    user = db.Users.Where(x => x.ID == administrator.adminid).FirstOrDefault();
                    adminUserProfileDb = db.Users_Admin.Where(u => u.UserID == administrator.adminid).FirstOrDefault();
                    if (user != null)
                    {
                        userTemp = 1;
                    }
                    if (adminUserProfileDb != null)
                    {
                        userProfileTemp = 1;
                    }
                }
                user.FirstName = administrator.fname;
                user.LastName = administrator.lname;
                user.EmailID = administrator.email;
                user.RoleID = 2;
                user.Password = "AdminPassword";
                user.IsEmailVerified = true;
                user.IsActive = true;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBY = superAdmin.ID;
                adminUserProfileDb.Country_Code = administrator.CountryCode;
                adminUserProfileDb.Phone_Number = administrator.phoneno;
                adminUserProfileDb.ModifiedDate = DateTime.Now;
                if (userTemp == 1)
                {
                    db.Entry(user).State = EntityState.Modified;
                }
                else
                {
                    user.JoiningDate = DateTime.Now;
                    db.Users.Add(user);
                }
                db.SaveChanges();
                if (userProfileTemp == 1)
                {
                    db.Entry(adminUserProfileDb).State = EntityState.Modified;
                }
                else
                {
                    adminUserProfileDb.CreatedDate = DateTime.Now;
                    user = db.Users.Where(x => x.EmailID==administrator.email).FirstOrDefault();
                    adminUserProfileDb.UserID = user.ID;
                    db.Users_Admin.Add(adminUserProfileDb);
                }
                db.SaveChanges();
            }
                return RedirectToAction("ManageAdministrator","Admin");
        }
        [Authorize]
        public ActionResult ManageAdministrator(string SearchText)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var adminList = db.Users.Where(u=>u.RoleID==2).OrderBy(u=>u.JoiningDate).ToList();
                    ViewBag.userdetails = null;
                    var admindata = db.Users_Admin.ToList();
                    ViewBag.admindetails = admindata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isfname = db.Users.Any(d => d.FirstName == SearchText && d.RoleID == 2);
                            bool islname = db.Users.Any(d => d.LastName == SearchText && d.RoleID == 2);
                            bool isemail = db.Users.Any(d => d.EmailID == SearchText && d.RoleID == 2);
                            bool isphone = db.Users_Admin.Any(d => d.Phone_Number == SearchText);
                            List<string> ActiveCountry = new List<string>();
                            ActiveCountry.Add("yes");
                            ActiveCountry.Add("no");
                            bool isactiveuser = ActiveCountry.Contains(SearchText);
                            if (isfname)
                            {
                                adminList = db.Users.Where(u => u.FirstName == SearchText && u.RoleID == 2).OrderBy(u => u.JoiningDate).ToList();
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }

                            }
                            if (islname)
                            {
                                adminList = db.Users.Where(u => u.LastName == SearchText && u.RoleID == 2).OrderBy(u => u.JoiningDate).ToList();
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }

                            }
                            if (isemail)
                            {
                                adminList = db.Users.Where(u => u.EmailID == SearchText && u.RoleID == 2).OrderBy(u => u.JoiningDate).ToList();
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }

                            }
                            if (isphone)
                            {
                                var pno = db.Users_Admin.Where(b => b.Phone_Number == SearchText).Select(b => b.UserID).ToList();
                                adminList = db.Users.Where(u => pno.Contains(u.ID) && u.RoleID == 2).OrderBy(u => u.JoiningDate).ToList();
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }
                            }
                            if (isactiveuser)
                            {
                                if (SearchText == "yes")
                                {
                                    adminList = db.Users.Where(x => x.IsActive == true && x.RoleID == 2).OrderBy(u => u.JoiningDate).ToList();
                                }
                                if (SearchText == "no")
                                {
                                    adminList = db.Users.Where(x => x.IsActive == false && x.RoleID == 2).OrderBy(u => u.JoiningDate).ToList();
                                }
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (adminList.Count != 0)
                        {
                            ViewBag.userdetails = adminList;
                        }
                    }
                    
                }
            }

            return View();
        }
        [Authorize]
        public ActionResult DeleteAdministrator(int? adminid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                User user_db = db.Users.Where(c => c.ID == adminid).FirstOrDefault();
                user_db.IsActive = false;
                db.Entry(user_db).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("ManageAdministrator", "Admin");
        }
        [Authorize]
        public ActionResult NotesUnderReview(string SearchText,int? changeInName)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var sellernote = db.SellerNotes.Where(x => (x.Status == "inReview") || (x.Status == "submitted")).OrderBy(x => x.PublishedDate).ToList();
                    ViewBag.SellerNoteDetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var userdata = db.Users.ToList();
                    ViewBag.userdetails = userdata;
                    var sellerid = db.SellerNotes.Where(x => (x.Status == "inReview") || (x.Status == "submitted")).Select(x => x.SellerID).ToList();
                    var getuserList = db.Users.Where(x => x.IsActive == true && (sellerid.Contains(x.ID))).ToList();
                    SelectList BuyerList = new SelectList(getuserList, "ID", "FirstName");
                    ViewBag.buyername = BuyerList;
                    if (changeInName != null)
                    {
                        sellernote = db.SellerNotes.Where(x => (x.SellerID == changeInName) && ((x.Status == "inReview") || (x.Status == "submitted"))).OrderBy(x => x.PublishedDate).ToList();
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }

                    }
                    else if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isfname = db.Users.Any(d => d.FirstName == SearchText);
                            bool islname = db.Users.Any(d => d.LastName == SearchText);
                            List<string> ActiveStatus = new List<string>();
                            ActiveStatus.Add("inreview");
                            ActiveStatus.Add("submitted");
                            bool isstatus = ActiveStatus.Contains(SearchText.ToLower());
                            if (isTitle)
                            {
                                sellernote = db.SellerNotes.Where(x => (x.Title == SearchText) && ((x.Status == "inReview") || (x.Status == "submitted"))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive == true).Select(x => x.CategoryID).ToList();
                                sellernote = db.SellerNotes.Where(x => (cid.Contains(x.Category)) && ((x.Status == "inReview") || (x.Status == "submitted"))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isfname)
                            {
                                var userid = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => (userid.Contains(x.SellerID)) && ((x.Status == "inReview") || (x.Status == "submitted"))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (islname)
                            {
                                var userid = db.Users.Where(u => u.LastName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => (userid.Contains(x.SellerID)) && ((x.Status == "inReview") || (x.Status == "submitted"))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isstatus)
                            {
                                if (SearchText.ToLower() == "inreview")
                                {
                                    sellernote = db.SellerNotes.Where(x => x.Status == "inReview").OrderBy(x => x.PublishedDate).ToList();
                                }
                                if (SearchText.ToLower() == "submitted")
                                {
                                    sellernote = db.SellerNotes.Where(x => x.Status == "submitted").OrderBy(x => x.PublishedDate).ToList();
                                }
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }


                        }
                    }
                    else
                    {
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }
                    }
                    
                }
            }
            return View();
        }
        [Authorize]
        public ActionResult approved(int? noteid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNote sellerNoteData = db.SellerNotes.Where(x=>x.ID==noteid).FirstOrDefault();
                sellerNoteData.Status = "approved";
                sellerNoteData.PublishedDate = DateTime.Now;
                sellerNoteData.IsActive = true;
                int userid = db.Users.Where(u => u.EmailID==User.Identity.Name).Select(x=>x.ID).FirstOrDefault();
                sellerNoteData.ActionedBy = userid;
                db.Entry(sellerNoteData).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("NotesUnderReview","Admin");
        }
        [Authorize]
        public ActionResult reject(int? noteid,string Message)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNote sellerNoteData = db.SellerNotes.Where(x => x.ID == noteid).FirstOrDefault();
                sellerNoteData.Status = "rejected";
                sellerNoteData.IsActive = false;
                int userid = db.Users.Where(u => u.EmailID == User.Identity.Name).Select(x => x.ID).FirstOrDefault();
                sellerNoteData.ActionedBy = userid;
                sellerNoteData.AdminRemarks = Message;
                db.Entry(sellerNoteData).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("NotesUnderReview", "Admin");
        }
        [Authorize]
        public ActionResult inReview(int? noteid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNote sellerNoteData = db.SellerNotes.Where(x => x.ID == noteid).FirstOrDefault();
                sellerNoteData.Status = "inReview";
                int userid = db.Users.Where(u => u.EmailID == User.Identity.Name).Select(x => x.ID).FirstOrDefault();
                sellerNoteData.ActionedBy = userid;
                db.Entry(sellerNoteData).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("NotesUnderReview", "Admin");
        }
        [Authorize]
        public ActionResult PublishedNotes(string SearchText, int? changeInName)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    ViewBag.downloadcount = 0;
                    var sellernote = db.SellerNotes.Where(x => (x.Status == "approved") && x.IsActive==true).OrderBy(x => x.PublishedDate).ToList();
                    ViewBag.SellerNoteDetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var userdata = db.Users.ToList();
                    ViewBag.userdetails = userdata;
                    var downloaddata = db.Downloads.ToList();
                    ViewBag.downloaddetails = downloaddata;
                    var sellerid = db.SellerNotes.Where(x => x.Status == "approved" && x.IsActive==true).Select(x => x.SellerID).ToList();
                    var getuserList = db.Users.Where(x => x.IsActive == true && sellerid.Contains(x.ID)).ToList();
                    SelectList BuyerList = new SelectList(getuserList, "ID", "FirstName");
                    ViewBag.buyername = BuyerList;
                    if (changeInName != null)
                    {
                        sellernote = db.SellerNotes.Where(x => (x.SellerID == changeInName) && (x.Status == "approved") && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }

                    }
                    else if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isfname = db.Users.Any(d => d.FirstName == SearchText);
                            bool islname = db.Users.Any(d => d.LastName == SearchText);
                            bool isPrice = decimal.TryParse(SearchText, out decimal price);
                            List<string> sellMode = new List<string>();
                            sellMode.Add("free");
                            sellMode.Add("paid");
                            bool isSellType = sellMode.Contains(SearchText.ToLower());
                            if (isTitle)
                            {
                                sellernote = db.SellerNotes.Where(x => (x.Title == SearchText) && (x.Status == "approved") && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive == true).Select(x => x.CategoryID).ToList();
                                sellernote = db.SellerNotes.Where(x => (cid.Contains(x.Category)) && (x.Status == "approved") && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isfname)
                            {
                                var userid = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsActive == true && (userid.Contains(x.SellerID) || userid.Contains((int)x.ActionedBy))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (islname)
                            {
                                var userid = db.Users.Where(u => u.LastName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsActive == true && (userid.Contains(x.SellerID) || userid.Contains((int)x.ActionedBy))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }
                            if (isPrice)
                            {

                                bool Price = db.SellerNotes.Any(d => d.SellingPrice == price && (d.Status == "approved"));
                                if (Price)
                                {
                                    sellernote = db.SellerNotes.Where(x => (x.Status == "approved") && x.SellingPrice == price && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                    if (sellernote.Count != 0)
                                    {
                                        ViewBag.SellerNoteDetails = sellernote;
                                    }
                                }
                            }
                            if (isSellType)
                            {
                                if (SearchText.ToLower() == "free")
                                {
                                    sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsPaid == false && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                }
                                if (SearchText.ToLower() == "paid")
                                {
                                    sellernote = db.SellerNotes.Where(x => x.Status == "approved" && x.IsPaid == true && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                }
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        public ActionResult unPublish(int? noteid,string Message)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNote sellerNoteData = db.SellerNotes.Where(x => x.ID == noteid).FirstOrDefault();
                sellerNoteData.Status = "removed";
                sellerNoteData.IsActive = false;
                int userid = db.Users.Where(u => u.EmailID == User.Identity.Name).Select(x => x.ID).FirstOrDefault();
                sellerNoteData.ActionedBy = userid;
                sellerNoteData.AdminRemarks = Message;
                db.Entry(sellerNoteData).State = EntityState.Modified;
                db.SaveChanges();
                var sellerEmail = db.Users.FirstOrDefault(u => u.ID == sellerNoteData.SellerID);
                MailMessage mail = new MailMessage();
                mail.To.Add(sellerEmail.EmailID);
                mail.From = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya");
                mail.Subject = "Sorry! we need to remove your notes from our portal.";
                string Body = "<p><p style=" + "margin-bottom:0px;" + ">Hello "+sellerEmail.FirstName+" "+sellerEmail.LastName+",<br>We want to inform you that, your note " + sellerNoteData.Title+ " has been removed from the portal.<br>Please find our remarks as below -<br>"+Message+"<br><p><b>Regards,<br>NotesMarketplace</b></p></p>";
                mail.Body = Body;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = UTF8Encoding.UTF8;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("parejiyadevanshi@gmail.com", "jhmswewvcofgnyfr"); // Enter seders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return RedirectToAction("PublishedNotes", "Admin");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Admin_ViewNotes(int noteid)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    ViewBag.review = 0;
                    ViewBag.rating = 0;
                    ViewBag.star = 0;
                    ViewBag.month = null;
                    ViewBag.day = null;

                    var bookdata = db.SellerNotes.Where(x => x.ID == noteid).ToList();
                    ViewBag.bookdetails = bookdata;
                    var countrydata = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList = new SelectList(countrydata, "CountryName", "CountryID");
                    ViewBag.countrydetails = CountryList;
                    var notesReview = db.SellerNotesReviews.Where(n => n.NoteID == noteid && n.IsActive == true).OrderBy(n => Guid.NewGuid()).Take(3).ToList();
                    ViewBag.NoteReview = notesReview;
                    var reviewerList = db.Users.ToList();
                    ViewBag.reviewerList = reviewerList;
                    var reviewerProfileList = db.UserProfiles.ToList();
                    ViewBag.reviewrProfileList = reviewerProfileList;
                    var note_review = db.SellerNotesReviews.Where(x=>x.IsActive == true && x.NoteID==noteid).ToList();
                    SelectList ReviewList = new SelectList(note_review, "Ratings", "NoteID");
                    ViewBag.reviewMsg = ReviewList;
                    var inappropriate_message = db.SellerNotesReportedIssues.Where(x=>x.IsActive == true && x.NoteID==noteid).ToList();
                    SelectList ReportedList = new SelectList(inappropriate_message, "ID", "NoteID");
                    ViewBag.inappropriateMsg = ReportedList;
                }
            }

            return View();
        }
        [Authorize]
        public ActionResult Admin_DownloadNotes(string SearchText, int? changeInBuyerName ,int? changeInSellerName ,string changeInTitle)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var downloadList = db.Downloads.Where(x=>x.IsSellerHasAllowedDownload==true && x.IsActive == true).OrderBy(x=>x.AttachmentDownloadDate).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var userdata = db.Users.ToList();
                    ViewBag.userdetails = userdata;
                    var getNoteList = db.Downloads.GroupBy(s => s.NoteTitle).Select(s => s.FirstOrDefault()).ToList();
                    SelectList NoteList = new SelectList(getNoteList, "NoteTitle", "NoteTitle");
                    ViewBag.notetitle = NoteList;
                    var sellerid = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.IsActive == true).Select(x => x.SellerId).ToList();
                    var getsellerList = db.Users.Where(x => x.IsActive == true && sellerid.Contains(x.ID)).ToList();
                    SelectList SellerNameList = new SelectList(getsellerList, "ID", "FirstName");
                    ViewBag.sellername = SellerNameList;
                    var buyerid = db.Downloads.Where(x => x.IsSellerHasAllowedDownload==true).Select(x => x.DownloaderId).ToList();
                    var getbuyerList = db.Users.Where(x => x.IsActive == true && buyerid.Contains(x.ID)).ToList();
                    SelectList BuyerNameList = new SelectList(getbuyerList, "ID", "FirstName");
                    ViewBag.buyername = BuyerNameList;
                    if (changeInBuyerName != null)
                    {
                        downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.DownloaderId == changeInBuyerName && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }

                    }
                    else if (changeInSellerName != null)
                    {
                        downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.SellerId == changeInSellerName && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }

                    }
                    else if (changeInTitle != null)
                    {
                        downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.NoteTitle == changeInTitle && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }

                    }
                    else if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.Downloads.Any(d => d.NoteTitle == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isFname = db.Users.Any(d => d.FirstName == SearchText);
                            bool isLname = db.Users.Any(d => d.LastName == SearchText);
                            bool isPrice = decimal.TryParse(SearchText, out decimal price);
                            List<string> sellMode = new List<string>();
                            sellMode.Add("free");
                            sellMode.Add("paid");
                            bool isSellType = sellMode.Contains(SearchText.ToLower());
                            if (isTitle)
                            {
                                downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.NoteTitle == SearchText && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }

                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive == true).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.NoteCategory == cid && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }


                            }
                            if (isFname)
                            {
                                var bname = db.Users.Where(b => b.FirstName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.IsActive == true && (bname.Contains(x.DownloaderId) || bname.Contains(x.SellerId))).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }

                            }
                            if (isLname)
                            {
                                var bname = db.Users.Where(b => b.LastName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.IsActive == true && (bname.Contains(x.DownloaderId) || bname.Contains(x.SellerId))).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                            }
                            if (isPrice)
                            {

                                bool Price = db.Downloads.Any(d => d.PurchasedPrice == price);
                                if (Price)
                                {
                                    downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.IsActive == true && x.PurchasedPrice == price).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                    if (downloadList.Count != 0)
                                    {
                                        ViewBag.downloaddetails = downloadList;
                                    }
                                }
                            }
                            if (isSellType)
                            {
                                if (SearchText.ToLower() == "free")
                                {
                                    downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.IsActive == true && x.IsPaid == false).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                }
                                if (SearchText.ToLower() == "paid")
                                {
                                    downloadList = db.Downloads.Where(x => x.IsSellerHasAllowedDownload == true && x.IsActive == true && x.IsPaid == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                }
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }
                    }

                }
            }
            return View();
        }
        [Authorize]
        public ActionResult Admin_RejectedNotes(string SearchText,int? changeInName)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var sellernote = db.SellerNotes.Where(x => (x.Status == "rejected")).OrderBy(x => x.PublishedDate).ToList();
                    ViewBag.SellerNoteDetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var userdata = db.Users.ToList();
                    ViewBag.userdetails = userdata;
                    var sellerid = db.SellerNotes.Where(x => x.Status == "rejected").Select(x => x.SellerID).ToList();
                    var getuserList = db.Users.Where(x => x.IsActive == true && sellerid.Contains(x.ID)).ToList();
                    SelectList BuyerList = new SelectList(getuserList, "ID", "FirstName");
                    ViewBag.buyername = BuyerList;
                    if (changeInName != null)
                    {
                        sellernote = db.SellerNotes.Where(x => (x.SellerID == changeInName) && (x.Status == "rejected")).OrderBy(x => x.PublishedDate).ToList();
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }

                    }
                    else if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isfname = db.Users.Any(d => d.FirstName == SearchText);
                            bool islname = db.Users.Any(d => d.LastName == SearchText);
                            if (isTitle)
                            {
                                sellernote = db.SellerNotes.Where(x => (x.Title == SearchText) && (x.Status == "rejected")).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }

                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive == true).Select(x => x.CategoryID).ToList();
                                sellernote = db.SellerNotes.Where(x => (cid.Contains(x.Category)) && (x.Status == "rejected")).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }

                            }
                            if (isfname)
                            {
                                var userid = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => x.Status == "rejected" && (userid.Contains(x.SellerID) || userid.Contains((int)x.ActionedBy))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }


                            }
                            if (islname)
                            {
                                var userid = db.Users.Where(u => u.LastName == SearchText).Select(u => u.ID).ToList();
                                sellernote = db.SellerNotes.Where(x => x.Status == "rejected" && (userid.Contains(x.SellerID) || userid.Contains((int)x.ActionedBy))).OrderBy(x => x.PublishedDate).ToList();
                                if (sellernote.Count != 0)
                                {
                                    ViewBag.SellerNoteDetails = sellernote;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (sellernote.Count != 0)
                        {
                            ViewBag.SellerNoteDetails = sellernote;
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        public ActionResult SpamReports(string SearchText)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var report = db.SellerNotesReportedIssues.Where(x => x.IsActive == true).OrderBy(x=>x.CreatedDate).ToList();
                    ViewBag.ReportDetails = null;
                    var sellerdetails = db.SellerNotes.ToList();
                    ViewBag.sellerdetails = sellerdetails;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var userdata = db.Users.ToList();
                    ViewBag.userdetails = userdata;
                    var sellerid = db.SellerNotesReportedIssues.Where(x => x.IsActive == true).Select(x => x.ReportedBYID).ToList();
                    var getuserList = db.Users.Where(x => x.IsActive == true && sellerid.Contains(x.ID)).ToList();
                    SelectList BuyerList = new SelectList(getuserList, "ID", "FirstName");
                    ViewBag.buyername = BuyerList;

                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isfname = db.Users.Any(d => d.FirstName == SearchText);
                            bool islname = db.Users.Any(d => d.LastName == SearchText);
                            if (isTitle)
                            {
                                var reportNoteId = db.SellerNotes.Where(u => u.Title == SearchText).Select(u => u.ID).ToList();
                                report = db.SellerNotesReportedIssues.Where(x => x.IsActive == true && reportNoteId.Contains(x.NoteID)).OrderBy(x => x.CreatedDate).ToList();
                                if (report.Count != 0)
                                {
                                    ViewBag.ReportDetails = report;
                                }

                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive == true).Select(x => x.CategoryID).ToList();
                                var reportNoteId = db.SellerNotes.Where(u => cid.Contains(u.Category)).Select(u => u.ID).ToList();
                                report = db.SellerNotesReportedIssues.Where(x => x.IsActive == true && reportNoteId.Contains(x.NoteID)).OrderBy(x => x.CreatedDate).ToList();
                                if (report.Count != 0)
                                {
                                    ViewBag.ReportDetails = report;
                                }

                            }
                            if (isfname)
                            {
                                var userid = db.Users.Where(u => u.FirstName == SearchText).Select(u => u.ID).ToList();
                                report = db.SellerNotesReportedIssues.Where(x => x.IsActive == true && userid.Contains(x.ReportedBYID)).OrderBy(x => x.CreatedDate).ToList();
                                if (report.Count != 0)
                                {
                                    ViewBag.ReportDetails = report;
                                }

                            }
                            if (islname)
                            {
                                var userid = db.Users.Where(u => u.LastName == SearchText).Select(u => u.ID).ToList();
                                report = db.SellerNotesReportedIssues.Where(x => x.IsActive == true && userid.Contains(x.ReportedBYID)).OrderBy(x => x.CreatedDate).ToList();
                                if (report.Count != 0)
                                {
                                    ViewBag.ReportDetails = report;
                                }
                            }

                        }
                    }
                    else
                    {
                        if (report.Count != 0)
                        {
                            ViewBag.ReportDetails = report;
                        }
                    }
                }
            }
            return View();
        }
        [Authorize]
        public ActionResult DeleteSpamReports(int? reportid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNotesReportedIssue notesReportedIssue = db.SellerNotesReportedIssues.Where(s => s.ID == reportid).FirstOrDefault();
                notesReportedIssue.IsActive = false;
                db.Entry(notesReportedIssue).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("SpamReports", "Admin");
        }
        [Authorize]
        public ActionResult Members( string SearchText)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    ViewBag.Review = 0;
                    ViewBag.Published = 0;
                    ViewBag.Download = 0;
                    ViewBag.Expenses = 0;
                    ViewBag.Earnings = 0;
                    var IsRecordCreated = db.UserProfiles.Select(u=>u.UserId).ToList();
                    var adminList = db.Users.Where(u => u.RoleID == 1 && u.IsActive==true && IsRecordCreated.Contains(u.ID)).OrderBy(u=>u.JoiningDate).ToList();
                    ViewBag.userdetails = null;
                    var admindata = db.UserProfiles.ToList();
                    var adminNotes = db.SellerNotes.ToList();
                    ViewBag.adminNoteDetails = adminNotes;
                    var adminDownload = db.Downloads.ToList();
                    ViewBag.adminDownloadDetails = adminDownload;
                    ViewBag.admindetails = admindata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isfname = db.Users.Any(d => d.FirstName == SearchText && d.RoleID == 1 && d.IsActive == true);
                            bool islname = db.Users.Any(d => d.LastName == SearchText && d.RoleID == 1 && d.IsActive == true);
                            bool isemail = db.Users.Any(d => d.EmailID == SearchText && d.RoleID == 1 && d.IsActive == true);
                            if (isfname)
                            {
                                adminList = db.Users.Where(u => u.FirstName == SearchText && u.RoleID == 1 && u.IsActive == true && IsRecordCreated.Contains(u.ID)).OrderBy(u => u.JoiningDate).ToList();
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }

                            }
                            if (islname)
                            {
                                adminList = db.Users.Where(u => u.LastName == SearchText && u.RoleID == 1 && u.IsActive == true && IsRecordCreated.Contains(u.ID)).OrderBy(u => u.JoiningDate).ToList();
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }


                            }
                            if (isemail)
                            {
                                adminList = db.Users.Where(u => u.EmailID == SearchText && u.RoleID == 1 && u.IsActive == true && IsRecordCreated.Contains(u.ID)).OrderBy(u => u.JoiningDate).ToList();
                                if (adminList.Count != 0)
                                {
                                    ViewBag.userdetails = adminList;
                                }

                            }

                        }
                    }
                    else
                    {
                        if (adminList.Count != 0)
                        {
                            ViewBag.userdetails = adminList;
                        }
                    }
                }
            }

            return View();
        }
        [Authorize]
        public ActionResult MemberDetails (int memberId)
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    ViewBag.countDownload = 0;
                    var memberdata = db.UserProfiles.Where(u => u.UserId == memberId ).ToList();
                    ViewBag.memberdetails = memberdata;
                    var user = db.Users.Where(u => u.ID == memberId && u.IsActive == true).ToList();
                    ViewBag.userdetails = user;
                    var bookdata = db.SellerNotes.Where(x => x.SellerID == memberId && x.Status !="draft").OrderBy(x=>x.PublishedDate).ToList();
                    ViewBag.bookdetails = null;
                    var downloaddata = db.Downloads.Where(x=>x.SellerId==memberId && x.IsSellerHasAllowedDownload==true && x.IsActive==true).ToList();
                    ViewBag.downloaddetails = downloaddata;
                    var countrydata = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList = new SelectList(countrydata, "CountryName", "CountryID");
                    ViewBag.countrydetails = CountryList;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    if (bookdata.Count != 0)
                    {
                        ViewBag.bookdetails = bookdata;
                    }
                }
            }
                    return View();
        }
        [Authorize]
        public ActionResult Deactivate(int? memberId)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                User user = db.Users.Where(s => s.ID == memberId).FirstOrDefault();
                int admin = db.Users.Where(u => u.EmailID == User.Identity.Name).Select(u => u.ID).FirstOrDefault();
                UserProfile userProfile = db.UserProfiles.Where(u => u.UserId == memberId).FirstOrDefault();
                var notes = db.SellerNotes.Where(n => n.SellerID == memberId && n.Status=="approved").ToList();
                for(int i = 1;i<=notes.Count;i++)
                {
                    SellerNote status = db.SellerNotes.Where(n => n.SellerID == memberId && n.Status == "approved").FirstOrDefault();
                    status.Status = "removed";
                    status.AdminRemarks = "Account Deactivated";
                    status.IsActive = false;
                    status.ActionedBy = admin;
                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();
                    
                }
                userProfile.IsActive = false;
                db.Entry(userProfile).State = EntityState.Modified;
                user.IsActive = false;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBY = admin;
                db.Entry(user).State = EntityState.Modified;
                
                db.SaveChanges();
            }

            return RedirectToAction("Members", "Admin");
        }
        [Authorize]
        public ActionResult DeleteReview(int reviewid,int note_id)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNotesReview review_db = db.SellerNotesReviews.Where(c => c.ID==reviewid).FirstOrDefault();
                User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                review_db.IsActive = false;
                review_db.ModifiedDate = DateTime.Now;
                review_db.ModifiedBy = user.ID;
                db.Entry(review_db).State = EntityState.Modified;
                db.SaveChanges();
                var review = db.SellerNotesReviews.Where(r => r.NoteID == note_id && r.IsActive == true).ToList();
                var count = 0;
                decimal star = 0;
                foreach (var r in review)
                {
                    star += r.Ratings;
                    count += 1;
                }
                decimal note_star = star / count;
                SellerNote sellerNote = db.SellerNotes.FirstOrDefault(n => n.ID == note_id);
                sellerNote.rating = note_star;
                db.Entry(sellerNote).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Admin_ViewNotes", "Admin", new { noteid = note_id });
        }

        [Authorize]
        [HttpGet]
        public ActionResult ManageConfiguration()
        {
            Admin_Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    var systemEmail = db.SystemConfigurations.Where(x=>x.K_ey=="Email").Select(x=>x.Value).FirstOrDefault();
                    var systempno = db.SystemConfigurations.Where(x=>x.K_ey=="pno").Select(x=>x.Value).FirstOrDefault();
                    var systemAdmin_email = db.SystemConfigurations.Where(x=>x.K_ey=="admin_Email").Select(x=>x.Value).FirstOrDefault();
                    var systemFacebook = db.SystemConfigurations.Where(x=>x.K_ey=="Facebook").Select(x=>x.Value).FirstOrDefault();
                    var systemTwitter = db.SystemConfigurations.Where(x=>x.K_ey=="Twitter").Select(x=>x.Value).FirstOrDefault();
                    var systemLinkedIn = db.SystemConfigurations.Where(x=>x.K_ey=="LinkedIn").Select(x=>x.Value).FirstOrDefault();
                    var systemNote_image = db.SystemConfigurations.Where(x=>x.K_ey=="Note_image").Select(x=>x.Value).FirstOrDefault();
                    var systemDisplay_image = db.SystemConfigurations.Where(x=>x.K_ey=="Display_image").Select(x=>x.Value).FirstOrDefault();
                    Models.SystemConfiguration system = new Models.SystemConfiguration();
                    system.emailAddress = systemEmail;
                    system.adminEmails = systemAdmin_email;
                    system.phoneNo = systempno;
                    system.FacebookUrl = systemFacebook;
                    system.TwitterUrl = systemTwitter;
                    system.LinkedInUrl = systemLinkedIn;
                    system.uploadPictureGet = systemNote_image;
                    string notefile = Path.GetFileName(systemNote_image);
                    ViewBag.uploadname = notefile;
                    system.profilePictureGet = systemDisplay_image;
                    string picfile = Path.GetFileName(systemDisplay_image);
                    ViewBag.profilename = picfile;
                    return View(system);
                }
            }
                    return View();
        }
        [HttpPost]
        public ActionResult ManageConfiguration(Models.SystemConfiguration systeConfiguration)
        {
            using(var db=new Notes_MarketplaceEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                var systemEmail = db.SystemConfigurations.Where(x => x.K_ey == "Email").FirstOrDefault();
                var systempno = db.SystemConfigurations.Where(x => x.K_ey == "pno").FirstOrDefault();
                var systemAdmin_email = db.SystemConfigurations.Where(x => x.K_ey == "admin_Email").FirstOrDefault();
                var systemFacebook = db.SystemConfigurations.Where(x => x.K_ey == "Facebook").FirstOrDefault();
                var systemTwitter = db.SystemConfigurations.Where(x => x.K_ey == "Twitter").FirstOrDefault();
                var systemLinkedIn = db.SystemConfigurations.Where(x => x.K_ey == "LinkedIn").FirstOrDefault();
                var systemNote_image = db.SystemConfigurations.Where(x => x.K_ey == "Note_image").FirstOrDefault();
                var systemDisplay_image = db.SystemConfigurations.Where(x => x.K_ey == "Display_image").FirstOrDefault();
                systemEmail.Value = systeConfiguration.emailAddress;
                systemEmail.CreatedDate = DateTime.Now;
                systemEmail.CreatedBy = user.ID;
                db.Entry(systemEmail).State = EntityState.Modified;
                systempno.Value = systeConfiguration.phoneNo;
                systempno.CreatedDate = DateTime.Now;
                systempno.CreatedBy = user.ID;
                db.Entry(systempno).State = EntityState.Modified;
                systemAdmin_email.Value = systeConfiguration.adminEmails;
                systemAdmin_email.CreatedDate = DateTime.Now;
                systemAdmin_email.CreatedBy = user.ID;
                db.Entry(systemAdmin_email).State = EntityState.Modified;
                systemFacebook.Value = systeConfiguration.FacebookUrl;
                systemFacebook.CreatedDate = DateTime.Now;
                systemFacebook.CreatedBy = user.ID;
                db.Entry(systemFacebook).State = EntityState.Modified;
                systemTwitter.Value = systeConfiguration.TwitterUrl;
                systemTwitter.CreatedDate = DateTime.Now;
                systemTwitter.CreatedBy = user.ID;
                db.Entry(systemTwitter).State = EntityState.Modified;
                systemLinkedIn.Value = systeConfiguration.LinkedInUrl;
                systemLinkedIn.CreatedDate = DateTime.Now;
                systemLinkedIn.CreatedBy = user.ID;
                db.Entry(systemLinkedIn).State = EntityState.Modified;
                var path = Server.MapPath("~\\UploadedFiles\\defaultfile");
                    if (!(Directory.Exists(path)))
                    {
                        Directory.CreateDirectory(path);
                        ViewBag.Mesage = "Directory created";
                    }
                    else
                    {
                        ViewBag.Mesage = "Directory exist";
                    }
                    string temp_path = "~\\UploadedFiles\\defaultfile\\";
                    if (systeConfiguration.uploadPicture != null && systeConfiguration.uploadPicture.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(systeConfiguration.uploadPicture.FileName);
                        string imgext = Path.GetExtension(_FileName);
                        if (imgext == ".jpg" || imgext == ".gif" || imgext == ".jpeg" || imgext == ".png")
                        {

                            string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                            systeConfiguration.uploadPicture.SaveAs(_path);
                            string upload_picpath = temp_path + _FileName;
                            systemNote_image.Value=upload_picpath;
                            systemNote_image.CreatedDate=DateTime.Now;
                        systemNote_image.CreatedBy = user.ID;
                        }
                        else
                        {
                            ViewBag.Extensionerror = "Please select image file";
                            return View();
                        }
                    }
                    else if (systeConfiguration.uploadPictureGet != null)
                    {
                        systemNote_image.Value = systeConfiguration.uploadPictureGet;
                    }
                    else
                    {
                        systemNote_image.Value = "~\\UploadedFiles\\defaultfile\\computer-science.png";
                    }
                db.Entry(systemNote_image).State = EntityState.Modified;
                if (systeConfiguration.profilePicture != null && systeConfiguration.profilePicture.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(systeConfiguration.profilePicture.FileName);
                    string imgext = Path.GetExtension(_FileName);
                    if (imgext == ".jpg" || imgext == ".gif" || imgext == ".jpeg" || imgext == ".png")
                    {

                        string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                        systeConfiguration.profilePicture.SaveAs(_path);
                        string upload_picpath = temp_path + _FileName;
                        systemDisplay_image.Value = upload_picpath;
                        systemDisplay_image.CreatedDate = DateTime.Now;
                        systemDisplay_image.CreatedBy = user.ID;
                    }
                    else
                    {
                        ViewBag.Extensionerror = "Please select image file";
                        return View();
                    }
                }
                else if (systeConfiguration.profilePictureGet != null)
                {
                    systemDisplay_image.Value = systeConfiguration.profilePictureGet;
                }
                else
                {
                    systemDisplay_image.Value = "~\\UploadedFiles\\defaultfile\\computer-science.png";
                }
                db.Entry(systemDisplay_image).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Dashboard_admin", "Admin");
        }
    }
}