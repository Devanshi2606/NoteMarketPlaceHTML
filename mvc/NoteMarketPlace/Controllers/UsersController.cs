using System;   
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Security;
using NoteMarketPlace;
using NoteMarketPlace.Models;


namespace NoteMarketPlace.Controllers
{
    public class UsersController : Controller
    {
        private Notes_MarketplaceEntities Db = new Notes_MarketplaceEntities();

        [HttpGet]
        public ActionResult Signup(User user)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(User user,Signup signupobj)
        {
            if (ModelState.IsValid)
            {
                if (signupobj.password == signupobj.confirmPassword)
                {
                    using (var db = new Notes_MarketplaceEntities())

                    {
                        var IsCheck = db.Users.Where(email => email.EmailID == signupobj.emailId).FirstOrDefault();
                        if(IsCheck!=null)
                        {
                            ModelState.AddModelError("EmailExists", "Email Id Already Exists");
                            return View();
                        }
                        user.FirstName = signupobj.firstname;
                            user.LastName = signupobj.lastname;
                            user.EmailID = signupobj.emailId;
                            user.Password = signupobj.password;
                            user.ActivationCode = Guid.NewGuid();
                            user.RoleID = 1;
                            db.Users.Add(user);
                            db.SaveChanges();
                        SendEmailToUser(user.FirstName,user.EmailID,user.ActivationCode.ToString());
                            ViewBag.msg = "Your account has been successfully created. Verification Link has been sent to your Email Id.";
                        
                }
                }
                else
                {
                    ViewBag.ConfirmPassMsg = "Password does Not Match";
                }
            }
            return View();
        }

        public void SendEmailToUser(string username,string emailId, string activationCode)
        {
            var GenarateUserVerificationLink = "/Users/UserVerification/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

            var fromMail = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya"); 
            var fromEmailpassword = "jhmswewvcofgnyfr";      
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);
            var Message = new MailMessage(fromMail, toEmail);
            Message.Subject = "Registration Completed-Demo";
            
            Message.AlternateViews.Add(Mail_Body(username,link));
            Message.IsBodyHtml = true;
            smtp.Send(Message);
        }
        private AlternateView Mail_Body(string username,string link)
        {
            string path = Server.MapPath("~/Content/images/login/logo.png");
            LinkedResource Img = new LinkedResource(path, MediaTypeNames.Image.Jpeg);
            Img.ContentId = "MyImage";
            string str = "<p style=\"margin: 30px;\"> <table> <tr><img src=cid:MyImage  id=\"img\" alt=\"Logo\" width=\"100%\" height=\"100%\"/>   </tr> </table></p> <h1 style=\"color:#6255a5; font size:26px;\">Email Verification</h1><p><p style=\"color: #000;\"><b>Dear "+username+",</b><p style=\"color: #000;\">Thanks for Signing up!</p><p style=\"color: #000;\">Simply click below for email verification.</p></p><p><a href="+link+">Click here to verify email address</a></p>";

            //"<p style=" + "margin: 30px;" + "> <table> <tr><img src=cid:MyImage  id=\"img\" alt=\"Logo\" width=\"100%\" height=\"100%\"/>   </tr> </table></p> <h1 style=" + "color:#6255a5; font size:26px;" + ">Email Verification</h1><p><p style=" + "color: #000;" + "><b>Dear Smith,</b><p style=" + "color: #000;" + ">Thanks for Signing up!</p><p style=" + "color: #000;" + ">Simply click below for email verification.</p></p>";
         
            AlternateView AV =
            AlternateView.CreateAlternateViewFromString(str, null, MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);
            return AV;
        }
        public ActionResult UserVerification(string id)
        {   using (var db = new Notes_MarketplaceEntities())
            {
                bool Status = false;

                db.Configuration.ValidateOnSaveEnabled = false;
                var IsVerify = db.Users.Where(u => u.ActivationCode == new Guid(id)).FirstOrDefault();

                if (IsVerify != null)
                {
                    IsVerify.IsEmailVerified = true;
                    db.SaveChanges();
                    ViewBag.Message = "Email Verification completed";
                    Status = true;
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Message = "Invalid Request...Email not verify";
                    ViewBag.Status = false;
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Login l)
        {
            using(var db=new Notes_MarketplaceEntities())
            {
                bool isValid = db.Users.Any(x => x.EmailID == l.emailId && x.Password == l.password && x.IsEmailVerified );
                User user = db.Users.FirstOrDefault(x=> x.EmailID == l.emailId);
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(l.emailId, false);
                    if (user.RoleID==1)
                    {
                        return RedirectToAction("Home", "Users");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard_admin", "Admin");
                    }
                }
                else
                {
                    ViewBag.incorrectpass = "The password that you've entered is incorrect";
                }
            }
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Users");
        }
        
        [HttpGet]
        public ActionResult Forgot()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgot(Forgot f)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                if (ModelState.IsValid)
                {
                    if (db.Users.Any(x => x.EmailID == f.emailId))
                    {
                        string allowedChars = "";
                        allowedChars = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
                        allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
                        allowedChars += "1,2,3,4,5,6,7,8,9,0,!,@,#,$,%,&,?";
                        char[] sep = { ',' };
                        string[] arr = allowedChars.Split(sep);
                        string passwordString = "";
                        string temp = "";
                        Random rand = new Random();
                        for (int i = 0; i < 8; i++)
                        {
                            temp = arr[rand.Next(0, arr.Length)];
                            passwordString += temp;
                        }

                        var user = db.Users.FirstOrDefault(x => x.EmailID == f.emailId);
                        user.Password = passwordString.ToString();
                        ViewBag.forgotpassMsg = "Your Password has been changed successfully and system generated passward is sent to your registered email address.";
                        MailMessage mail = new MailMessage();
                        mail.To.Add(f.emailId);
                        mail.From = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya");
                        mail.Subject = "New Temporary Password has been created for you";
                        string Body = "<p><p style="+"margin-bottom:0px;"+ ">Hello,<br>We have generated a new password for you</p> Password: "+passwordString+"<br><p><b>Regards,<br>NotesMarketplace</b></p></p>";
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
                        db.SaveChanges();
                        
                        
                        //return RedirectToAction("Login", "Users");
                    }
                    else
                    {
                        ViewBag.forgotpassMsg = "Incorrent Email address.";
                    }
                }
                return View();
            }
        }

        [HttpGet]
        public ActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SearchNotes()
        {
            using(var db=new Notes_MarketplaceEntities())
            {
                ViewBag.count = 0;
                ViewBag.review = 0;
                ViewBag.rating = 0;
                ViewBag.star = 0;
                ViewBag.month = null;
                ViewBag.day = null;
                var bookdata = db.SellerNotes.ToList();
                ViewBag.bookdetails = bookdata;
                var countrydata = db.Countries.ToList();
                SelectList CountryList = new SelectList(countrydata, "CountryName", "CountryID");
                ViewBag.countrydetails = CountryList;
                var inappropriate_message = db.SellerNotesReportedIssues.ToList();
                SelectList ReportedList = new SelectList(inappropriate_message, "ID", "NoteID");
                ViewBag.inappropriateMsg = ReportedList;
                var note_review = db.SellerNotesReviews.ToList();
                SelectList ReviewList = new SelectList(note_review, "Ratings", "NoteID");
                ViewBag.reviewMsg = ReviewList;
                //var uni_name = db.SellerNotes.Include(u=>u.UniversityName).GroupBy(u=>u.UniversityName).ToList();
                //ViewBag.uni_name = uni_name;
                //var category_name = db.SellerNotes.Include(u => u.Category).GroupBy(u => u.Category).ToList();
                //ViewBag.category_name = category_name;
                //var type_name = db.SellerNotes.Include(u => u.NoteType).GroupBy(u => u.NoteType).ToList();
                //ViewBag.type_name = type_name;
                //var course_name = db.SellerNotes.Include(u => u.Course).GroupBy(u => u.Course).ToList();
                //ViewBag.course_name = course_name;
                //var country_name = db.SellerNotes.Include(u => u.Country).GroupBy(u => u.Country).ToList();
                //ViewBag.country_name = country_name;
                //var rating_name = db.SellerNotes.Include(u => u.rating).GroupBy(u => u.rating).ToList();
                //ViewBag.rating_name = rating_name;
            }
            return View();
        }
        [HttpPost]
        public ActionResult SearchNotes(Searchnotes searchnotes)
        {   using (var db = new Notes_MarketplaceEntities())
            {
                //var uni_name = db.SellerNotes.Include(u => u.UniversityName).GroupBy(u => u.UniversityName).ToList();
                //ViewBag.uni_name = uni_name;
                //var category_name = db.SellerNotes.Include(u => u.Category).GroupBy(u => u.Category).ToList();
                //ViewBag.category_name = category_name;
                //var type_name = db.SellerNotes.Include(u => u.NoteType).GroupBy(u => u.NoteType).ToList();
                //ViewBag.type_name = type_name;
                //var course_name = db.SellerNotes.Include(u => u.Course).GroupBy(u => u.Course).ToList();
                //ViewBag.course_name = course_name;
                //var country_name = db.SellerNotes.Include(u => u.Country).GroupBy(u => u.Country).ToList();
                //ViewBag.country_name = country_name;
                //var rating_name = db.SellerNotes.Include(u => u.rating).GroupBy(u => u.rating).ToList();
                //ViewBag.rating_name = rating_name;
            }
            return View();

        }


        [HttpGet]
        [Authorize]
        public ActionResult SellyourNotes()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ViewNotes()
        {   using (var db = new Notes_MarketplaceEntities())
            {
                ViewBag.count = 0;
                ViewBag.review = 0;
                ViewBag.rating = 0;
                ViewBag.star = 0;
                ViewBag.month = null;
                ViewBag.day = null;

                var bookdata = db.SellerNotes.ToList();
                ViewBag.bookdetails = bookdata;
                var countrydata = db.Countries.ToList();
                SelectList CountryList = new SelectList(countrydata, "CountryName", "CountryID");
                ViewBag.countrydetails = CountryList;
                var note_review = db.SellerNotesReviews.ToList();
                SelectList ReviewList = new SelectList(note_review, "Ratings", "NoteID");
                ViewBag.reviewMsg = ReviewList;
                var inappropriate_message = db.SellerNotesReportedIssues.ToList();
                SelectList ReportedList = new SelectList(inappropriate_message, "ID", "NoteID");
                ViewBag.inappropriateMsg = ReportedList;
            }
            return View();
        }
        [HttpGet]
        public ActionResult Faq()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactUs contactUs)
        {
            if (ModelState.IsValid)
            {
                
                    MailMessage mail = new MailMessage();
                    mail.To.Add("parejiyadevanshi@gmail.com");
                    mail.From = new MailAddress("parejiyadevanshi@gmail.com","Devanshi Parejiya");
                    mail.Subject = contactUs.firstName+" -Query";
                    string Body = "<p><p>Hello,</p>"+contactUs.comment+"<p><b>Regards,<br>"+contactUs.firstName+"</b></p></p>";
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
            return View();
        }

        [HttpGet]
        public ActionResult AddNotes()
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                
                var getCategoryList = db.NoteCategories.ToList();
                SelectList CategoryList = new SelectList(getCategoryList, "CategoryID", "CategoryName");
                ViewBag.categoryname = CategoryList;

                var getTypeList = db.NoteTypes.ToList();
                SelectList TypeList = new SelectList(getTypeList, "TypeID", "TypeName");
                ViewBag.typename = TypeList;
                var getCountryList = db.Countries.ToList();
                SelectList CountryList = new SelectList(getCountryList, "CountryID", "CountryName");
                ViewBag.countryname = CountryList;
                
            }
            return View();
        }
        public ActionResult AddNotes(Addnotes addnotes,SellerNote sellerNote,string submit)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    var getCategoryList = db.NoteCategories.ToList();
                    SelectList CategoryList = new SelectList(getCategoryList, "CategoryID", "CategoryName");
                    ViewBag.categoryname = CategoryList;

                    var getTypeList = db.NoteTypes.ToList();
                    SelectList TypeList = new SelectList(getTypeList, "TypeID", "TypeName");
                    ViewBag.typename = TypeList;
                    var getCountryList = db.Countries.ToList();
                    SelectList CountryList = new SelectList(getCountryList, "CountryID", "CountryName");
                    ViewBag.countryname = CountryList;

                    {
                        var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                        sellerNote.SellerID = user.ID;
                        sellerNote.Title = addnotes.Title;
                        sellerNote.Category = Convert.ToInt32(addnotes.CategoryName);
                        sellerNote.NoteType = Convert.ToInt32(addnotes.TypeName);
                        sellerNote.NumberofPages = addnotes.NoOfPages;
                        sellerNote.Description = addnotes.Description;
                        sellerNote.Country = Convert.ToInt32(addnotes.CountryName);
                        sellerNote.UniversityName = addnotes.InstitutionName;
                        sellerNote.Course = addnotes.CourseName;
                        sellerNote.CourseCode = addnotes.CourseCode;
                        sellerNote.Professor = addnotes.ProfessorName;
                        sellerNote.ModifiedDate = DateTime.Now;
                        if (addnotes.SellMode == "Free")
                        {
                            sellerNote.IsPaid = false;
                            sellerNote.SellingPrice = Convert.ToDecimal(0);
                        }
                        else
                        {
                            sellerNote.IsPaid = true;
                            sellerNote.SellingPrice = Convert.ToDecimal(addnotes.SellPrice);
                        }

                        var path = Server.MapPath("~/UploadedFiles/") + sellerNote.SellerID;
                        if (!(Directory.Exists(path)))
                        {
                            Directory.CreateDirectory(path);
                            ViewBag.Mesage = "Directory created";
                        }
                        else
                        {
                            ViewBag.Mesage = "Directory exist";
                        }

                        if (addnotes.uploadPicture.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(addnotes.uploadPicture.FileName);
                            string imgext = Path.GetExtension(_FileName);
                            if (imgext == ".jpg" || imgext == ".gif" || imgext == ".jpeg" || imgext == ".png")
                            {
                                string temp_path = "~/UploadedFiles/" + sellerNote.SellerID + "/";
                                string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                                addnotes.uploadPicture.SaveAs(_path);
                                sellerNote.DisplayPicture = _path;
                            }
                            else
                            {
                                ViewBag.Extensionerror = "Please select image file";
                                return View();
                            }
                        }
                        if (addnotes.uploadNotes.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(addnotes.uploadNotes.FileName);
                            string _path = Path.Combine(Server.MapPath("~/UploadedFiles/" + sellerNote.SellerID + "/"), _FileName);
                            addnotes.uploadNotes.SaveAs(_path);
                            sellerNote.FileName = _FileName;
                            sellerNote.FilePath = _path;
                        }
                        if (addnotes.uploadPreview.ContentLength > 0)
                        {
                            string _FileName = Path.GetFileName(addnotes.uploadPreview.FileName);
                            string _path = Path.Combine(Server.MapPath("~/UploadedFiles/" + sellerNote.SellerID + "/"), _FileName);
                            addnotes.uploadPreview.SaveAs(_path);
                            sellerNote.NotesPreview = _path;
                        }



                        switch (submit)
                        {
                            case "save":
                                sellerNote.Status = "drafted";
                                break;
                            case "publish":
                                sellerNote.Status = "submitted";
                                sellerNote.PublishedDate = DateTime.Now;
                                break;
                        }


                        db.SellerNotes.Add(sellerNote);
                        db.SaveChanges();
                    }
                   
                }
            }
            return View();
        }
        
        // GET: Users
        /*
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.UserRole);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.RoleID = new SelectList(db.UserRoles, "ID", "Name");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,RoleID,FirstName,LastName,EmailID,Password,IsEmailVerified,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,IsActive")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RoleID = new SelectList(db.UserRoles, "ID", "Name", user.RoleID);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.RoleID = new SelectList(db.UserRoles, "ID", "Name", user.RoleID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,RoleID,FirstName,LastName,EmailID,Password,IsEmailVerified,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,IsActive")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RoleID = new SelectList(db.UserRoles, "ID", "Name", user.RoleID);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/
    }
}
