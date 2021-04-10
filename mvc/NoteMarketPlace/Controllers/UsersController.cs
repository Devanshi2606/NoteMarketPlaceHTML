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
using System.Web.UI;
using PagedList.Mvc;
using PagedList;
using NoteMarketPlace;
using NoteMarketPlace.Models;
using System.Web.UI.WebControls;

namespace NoteMarketPlace.Controllers
{
    public class UsersController : Controller
    {
        private Notes_MarketplaceEntities Db = new Notes_MarketplaceEntities();
        public ActionResult Header()
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
                    var userdata = db.UserProfiles.FirstOrDefault(s => s.UserId == user.ID);
                    TempData["Twitter"] = systemTwitter;
                    TempData["LinkedIn"] = systemLinkedIn;
                    TempData["Facebook"] = systemFacebook;
                    if (userdata != null)
                    {
                        if (userdata.Profile_Picture != null)
                        {
                            //ViewBag.userdetails = userdata.Profile_Picture;u want to delete reported issue
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
                return View("~/Views/Shared/Header.cshtml");
            }
            
        }

        [HttpGet]
        public ActionResult Signup()
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
                            user.JoiningDate=DateTime.Now;
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
                    IsVerify.IsActive = true;
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
            Models.Login model = new Models.Login();
            if(Request.Cookies["Login"]!=null)
            {
                model.emailId = Request.Cookies["Login"].Values["EmailID"];
                model.password= Request.Cookies["Login"].Values["Password"];
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Login(Models.Login l)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    User user = db.Users.Where(x => x.EmailID == l.emailId && x.Password == l.password && x.IsEmailVerified==true && x.IsActive==true).FirstOrDefault();
                    
                    if (user!=null)
                    {
                     
                        FormsAuthentication.SetAuthCookie(l.emailId, l.RememberMe);
                        Session["EmailID"] = user.EmailID;
                        Session["Password"] = user.Password;
                        if(l.RememberMe)
                        {
                            HttpCookie cookie = new HttpCookie("Login");
                            cookie.Values.Add("EmailID", user.EmailID);
                            cookie.Values.Add("Password", user.Password);
                            cookie.Expires = DateTime.Now.AddDays(15);
                            Response.Cookies.Add(cookie);
                        }
                        if (user.RoleID == 1)
                        {
                            bool isRecordcreated = db.UserProfiles.Any(p => p.UserId == user.ID);
                            if (isRecordcreated)
                            {
                                return RedirectToAction("Home","Users");
                            }
                            return RedirectToAction("Userprofile", "Users");
                        }
                        else if(user.RoleID==2)
                        {
                            bool isRecordcreated = db.Users_Admin.Any(p => p.UserID == user.ID);
                            if (isRecordcreated)
                            {
                                return RedirectToAction("Dashboard_admin","Admin");
                            }
                            return RedirectToAction("UserProfile_Admin", "Admin");
                        }
                        else if(user.RoleID==3)
                        {
                            RedirectToAction("Dashboard_admin","Admin");
                        }
                    }
                    else
                    {
                        ViewBag.incorrectpass = "The password that you've entered is incorrect";
                    }
                }
            }
            return View(l);
        }
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Users");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Userprofile()
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    var getCountryList = db.Countries.Where(x=>x.IsActive==true).ToList();
                    SelectList CountryList, CountryNameList;
                    User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    bool isusersigned = db.Users.Any(u => u.ID == user.ID);
                    User_Profile up = new User_Profile();
                    if (isusersigned)
                    {
                        up.Firstname = user.FirstName;
                        up.Lastname = user.LastName;
                        up.emailId = user.EmailID;
                        up.DOB = DateTime.Now;
                    }
                    bool isRecordcreated = db.UserProfiles.Any(p => p.UserId == user.ID);
                    if(isRecordcreated)
                    {
                        var userList = db.UserProfiles.FirstOrDefault(p => p.UserId == user.ID);
                        
                        up.DOB = Convert.ToDateTime(userList.DOB);
                        up.phoneNo = userList.Phone_Number;
                        up.address1 = userList.Address_Line1;
                        up.address2 = userList.Address_Line2;
                        up.city = userList.City;
                        up.state = userList.State;
                        up.University = userList.University;
                        up.College = userList.College;
                        up.zipCode = userList.Zip_Code;
                        Country countryname = db.Countries.FirstOrDefault(c => c.CountryID == userList.Country && c.IsActive==true);
                        int coun_code = Convert.ToInt32(userList.Country_Code);
                        Country countrycode = db.Countries.FirstOrDefault(c => c.CountryID == coun_code && c.IsActive==true);
                        if(countryname!=null)
                        {
                            object counId = countryname.CountryID;
                            CountryNameList = new SelectList(getCountryList, "CountryID", "CountryName", counId);
                            ViewBag.countryname = CountryNameList;
                        }
                        else
                        {
                            CountryNameList = new SelectList(getCountryList, "CountryID", "CountryName");
                            ViewBag.countryname = CountryNameList;
                            ViewBag.countrydefValue = "Select Your Country";
                        }
                        if(countrycode!=null)
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
                        ViewBag.gender = userList.Gender;
                        up.profilePictureGet = userList.Profile_Picture;
                        string filename = Path.GetFileName(userList.Profile_Picture);
                        ViewBag.picname = filename;
                        return View(up);
                    }
                    CountryList = new SelectList(getCountryList, "CountryID", "CountryCode");
                    ViewBag.countrycode = CountryList;
                    CountryNameList = new SelectList(getCountryList, "CountryID", "CountryName");
                    ViewBag.countryname = CountryNameList;
                    ViewBag.countrydefValue= "Select Your Country";
                    ViewBag.gender = null;
                    ViewBag.picname = null;
                    return View(up);
                }
            }
            return View();

        }
        [Authorize]
        [HttpPost]
        public ActionResult Userprofile(User_Profile userProfilemodel, UserProfile userProfileDb)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    int userProfileTemp = 0;
                    int userTemp = 0;
                    
                    var user = db.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                    bool isRecordcreated = db.UserProfiles.Any(p => p.UserId == user.ID);

                    
                    if(user!=null)
                    {
                        userTemp = 1;
                    }
                    if(isRecordcreated)
                    {
                        userProfileDb = db.UserProfiles.Where(u => u.UserId == user.ID).FirstOrDefault();
                        userProfileTemp = 1;
                    }
                    user.FirstName = userProfilemodel.Firstname;
                    user.LastName = userProfilemodel.Lastname;
                    userProfileDb.DOB = userProfilemodel.DOB;
                    userProfileDb.Gender = userProfilemodel.Gender;
                    userProfileDb.Country_Code = userProfilemodel.CountryCode;
                    userProfileDb.UserId = user.ID;
                    userProfileDb.Phone_Number = userProfilemodel.phoneNo;
                    userProfileDb.IsActive = true;
                    userProfileDb.ModifiedDate = DateTime.Now;
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
                    if (userProfilemodel.profilePicture != null && userProfilemodel.profilePicture.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(userProfilemodel.profilePicture.FileName);
                        string imgext = Path.GetExtension(_FileName);
                        if (imgext == ".jpg" || imgext == ".gif" || imgext == ".jpeg" || imgext == ".png")
                        {

                            string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                            userProfilemodel.profilePicture.SaveAs(_path);
                            string upload_picpath = temp_path + _FileName;
                            userProfileDb.Profile_Picture = upload_picpath;
                        }
                        else
                        {
                            ViewBag.Extensionerror = "Please select image file";
                            return View();
                        }
                    }
                    else if(userProfilemodel.profilePictureGet!=null)
                    {
                        userProfileDb.Profile_Picture = userProfilemodel.profilePictureGet;
                    }
                    else
                    {
                        userProfileDb.Profile_Picture = null;
                    }

                    userProfileDb.Address_Line1 = userProfilemodel.address1;
                    userProfileDb.Address_Line2 = userProfilemodel.address2;
                    userProfileDb.City = userProfilemodel.city;
                    userProfileDb.State = userProfilemodel.state;
                    userProfileDb.Zip_Code = userProfilemodel.zipCode;
                    userProfileDb.Country = Convert.ToInt32(userProfilemodel.CountryName);
                    userProfileDb.University = userProfilemodel.University;
                    userProfileDb.College = userProfilemodel.College;
                    userProfileDb.Total_expenses = 0;
                    userProfileDb.Total_earnings = 0;
                    

                    if (userTemp == 1)
                    {
                        db.Entry(user).State = EntityState.Modified;
                    }
                    else
                    {
                        
                        db.Users.Add(user);
                    }
                    if (userProfileTemp==1)
                    {
                        db.Entry(userProfileDb).State = EntityState.Modified;
                    }
                    else
                    {
                        userProfileDb.CreatedDate = DateTime.Now;
                        db.UserProfiles.Add(userProfileDb);
                    }
                    
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Home","Users");
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
            Header();
            return View();
        }

        [HttpGet]
        public ActionResult SearchNotes(Searchnotes searchnotes,string changeInTitle,int? changeInType,int?changeINCategory,string changeInUniversity,string changeInCourse,int? changeInCountry,int? changeInRating)
        {
                Header();
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
                    var bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.IsActive == true).ToList();
                    ViewBag.bookdetails = null;
                    var inappropriate_message = db.SellerNotesReportedIssues.Where(r=>r.IsActive == true).ToList();
                    SelectList ReportedList = new SelectList(inappropriate_message, "ID", "NoteID");
                    ViewBag.inappropriateMsg = ReportedList;
                    var note_review = db.SellerNotesReviews.Where(r=>r.IsActive == true).ToList();
                    SelectList ReviewList = new SelectList(note_review, "Ratings", "NoteID");
                    ViewBag.reviewMsg = ReviewList;
                    var getCountryList = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList = new SelectList(getCountryList, "CountryID", "CountryName");
                    ViewBag.countryname = CountryList;
                    var getCategoryList = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(getCategoryList, "CategoryID", "CategoryName");
                    ViewBag.categoryname = CategoryList;
                    var getTypeList = db.NoteTypes.Where(x => x.IsActive == true).ToList();
                    SelectList TypeList = new SelectList(getTypeList, "TypeID", "TypeName");
                    ViewBag.typename = TypeList;
                    var getUniversityList = db.SellerNotes.GroupBy(s => s.UniversityName).Select(s => s.FirstOrDefault()).ToList();
                    SelectList UniversityList = new SelectList(getUniversityList, "UniversityName", "UniversityName");
                    ViewBag.universityname = UniversityList;

                    var getCourseList = db.SellerNotes.GroupBy(s => s.Course).Select(s => s.FirstOrDefault()).ToList();
                    SelectList CourseList = new SelectList(getCourseList, "Course", "Course");
                    ViewBag.coursename = CourseList;
                    if (changeInTitle != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Title == changeInTitle && s.IsActive == true).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        
                    }
                    else if (changeInType!=null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.NoteType==changeInType && s.IsActive == true).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        
                    }
                    else if (changeINCategory != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Category == changeINCategory && s.IsActive == true).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        
                    }
                    else if (changeInUniversity != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.UniversityName == changeInUniversity && s.IsActive == true).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                    }
                    else if (changeInCourse != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Course == changeInCourse && s.IsActive == true).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        
                    }
                    else if (changeInCountry != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Country == changeInCountry && s.IsActive == true).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        
                    }
                    else if (changeInRating != null)
                    {
                        //var r = db.SellerNotes.Where(s => s.Status == "approved" && s.IsActive == true).Select(s=>s.rating).ToList();
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.rating >=changeInRating && s.IsActive == true).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        
                    }
                    else
                    {
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        
                    }
                    


                }
            }
            return View();
        }

        
        [Authorize]
        public ActionResult SellyourNotes(string progressText,string publishedText,int? page)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    //ViewBag.month = null;
                    //ViewBag.day = null;
                    //ViewBag.count = 0;
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && (x.Status == "draft" || x.Status == "inReview" || x.Status == "submitted")).OrderBy(x=>x.ModifiedDate).ToList().ToPagedList(page??1,3);
                    var publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                    ViewBag.progressdetails = null;
                    ViewBag.publisheddetails = null;
                    var SoldNotes = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == true && x.IsActive == true).ToList();
                    ViewBag.Soldnotes = SoldNotes.Count;
                    var earning = db.UserProfiles.Where(x=>x.UserId==user.ID).Select(x=>x.Total_earnings).FirstOrDefault();
                    ViewBag.Earning = earning;
                    var downloadNotes = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsSellerHasAllowedDownload == true && x.IsActive == true).ToList();
                    ViewBag.downloadNotes = downloadNotes.Count;
                    var rejectedNotes = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected").ToList();
                    ViewBag.rejectedNotes = rejectedNotes.Count;
                    var buyerNotes = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == false && x.IsActive == false).ToList();
                    ViewBag.buyerNotes = buyerNotes.Count;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    if (!String.IsNullOrEmpty(progressText))
                    {
                        if (progressText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == progressText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == progressText);
                            List<string> StatusType = new List<string>();
                            StatusType.Add("draft");
                            StatusType.Add("inReview");
                            StatusType.Add("submitted");
                            bool isStatus = StatusType.Contains(progressText);
                            if (isTitle)
                            {
                                progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && (x.Status == "draft" || x.Status == "inReview" || x.Status == "submitted") && x.Title == progressText ).OrderBy(x => x.ModifiedDate).ToList().ToPagedList(page ?? 1, 3);
                                if (progressList.Count != 0)
                                {
                                    ViewBag.progressdetails = progressList;
                                }
                            }
                             if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == progressText && x.IsActive==true).Select(x => x.CategoryID).FirstOrDefault();
                                progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && (x.Status == "draft" || x.Status == "inReview" || x.Status == "submitted") && x.Category == cid).OrderBy(x => x.ModifiedDate).ToList().ToPagedList(page ?? 1, 3);
                                if (progressList.Count != 0)
                                {
                                    ViewBag.progressdetails = progressList;
                                }
                                
                            }
                             if (isStatus)
                            {
                                progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == progressText).OrderBy(x => x.ModifiedDate).ToList().ToPagedList(page ?? 1, 3);
                                if (progressList.Count != 0)
                                {
                                    ViewBag.progressdetails = progressList;
                                }
                            }
                            
                        }
                    }
                    else
                    {
                        if (progressList.Count != 0)
                        {
                            ViewBag.progressdetails = progressList;
                        }
                    }

                    if (!String.IsNullOrEmpty(publishedText))
                    {
                        if (publishedText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == publishedText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == publishedText);
                            bool isPrice = decimal.TryParse(publishedText, out decimal price);
                            List<string> sellMode = new List<string>();
                            sellMode.Add("free");
                            sellMode.Add("paid");
                            bool isSellType = sellMode.Contains(publishedText);
                            if (isTitle)
                            {
                                publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.Title == publishedText && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                if (publishedList.Count != 0)
                                {
                                    ViewBag.publisheddetails = publishedList;
                                }
                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == publishedText && x.IsActive==true).Select(x => x.CategoryID).FirstOrDefault();
                                publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.Category == cid && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                if (publishedList.Count != 0)
                                {
                                    ViewBag.publisheddetails = publishedList;
                                }
                                
                            }
                            if (isPrice)
                            {

                                bool Price = db.SellerNotes.Any(d => d.SellingPrice == price);
                                if (Price)
                                {
                                    publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.SellingPrice == price && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                    if (publishedList.Count != 0)
                                    {
                                        ViewBag.publisheddetails = publishedList;
                                    }
                                }
                            }
                            if (isSellType)
                            {
                                if (publishedText == "free")
                                {
                                    publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.IsPaid == false && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                }
                                 if (publishedText == "paid")
                                {
                                    publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.IsPaid == true && x.IsActive == true).OrderBy(x => x.PublishedDate).ToList();
                                }
                                if (publishedList.Count != 0)
                                {
                                    ViewBag.publisheddetails = publishedList;
                                }
                                
                            }
                            
                        }
                    }
                    else
                    {
                        if (publishedList.Count != 0)
                        {
                            ViewBag.publisheddetails = publishedList;
                        }
                    }
                    return View(publishedList.ToPagedList(page??1,3));
                }
                
            }
            return View();
        }
       

        [HttpGet]
        public ActionResult ViewNotes(int noteid)
        {
            Header();
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
                    var systemPhoneno = db.SystemConfigurations.Where(s => s.K_ey == "pno").Select(s=>s.Value).FirstOrDefault();
                    ViewBag.systemPhoneNo = systemPhoneno;
                    var bookdata = db.SellerNotes.Where(x => x.ID == noteid ).ToList();
                    ViewBag.bookdetails = bookdata;
                    var countrydata = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList = new SelectList(countrydata, "CountryName", "CountryID");
                    ViewBag.countrydetails = CountryList;
                    var notesReview = db.SellerNotesReviews.Where(n => n.NoteID == noteid && n.IsActive==true).OrderBy(n=>Guid.NewGuid()).Take(3).ToList();
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
                    var sellerdata = db.SellerNotes.Where(x => x.ID == noteid && x.IsActive == true).Select(x=>x.SellerID).ToList();
                    var userdata = db.Users.Where(u=> sellerdata.Contains(u.ID) || u.EmailID == User.Identity.Name).ToList();
                    var userProfiledata = db.UserProfiles.Where(u=> sellerdata.Contains(u.UserId) ).ToList();
                    ViewBag.UserProfileDetails = userProfiledata;
                    var Downloderdata = db.Users.Where(u => u.EmailID == User.Identity.Name).Select(u=>u.ID).FirstOrDefault();
                    bool isRecordCreated = db.Downloads.Any(d => d.NoteID == noteid && d.DownloaderId == Downloderdata && d.IsSellerHasAllowedDownload==true);
                    if (!isRecordCreated)
                    { ViewBag.RecordCreated = "no"; }
                    else { ViewBag.RecordCreated = "yes"; }
                    ViewBag.UserDetails = userdata;
                }
            }
            return View();
        }
        
        [Authorize]
        public ActionResult Downloading(int noteid,Download download)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    //var bookdata = db.SellerNotes.Where(x => x.ID == noteid).ToList();
                    var sellernote = db.SellerNotes.FirstOrDefault(s => s.ID == noteid && s.IsActive == true);
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var sellerid = db.Users.FirstOrDefault(x => x.ID == sellernote.SellerID);
                    var buyerDetail = db.UserProfiles.Where(b => b.UserId == user.ID).Select(b => b.Gender).FirstOrDefault();
                    bool isUserPaid = db.Downloads.Any(d => d.NoteID == noteid && d.DownloaderId == user.ID && d.IsSellerHasAllowedDownload == true);
                    bool isRecordCreated = db.Downloads.Any(d=>d.NoteID==noteid && d.DownloaderId==user.ID);
                    if (!isRecordCreated)
                    {
                        if (ModelState.IsValid)
                        {
                            download.NoteID = noteid;
                            download.SellerId = sellernote.SellerID;
                            download.DownloaderId = user.ID;
                            download.IsSellerHasAllowedDownload = false;
                            download.IsActive = false;
                            download.AttachmentName = sellernote.FileName;
                            download.AttachmentPath = sellernote.FilePath;
                            download.IsAttachmentDownloaded = false;
                            download.AttachmentDownloadDate = DateTime.Now;
                            download.IsPaid = sellernote.IsPaid;
                            download.PurchasedPrice = sellernote.SellingPrice;
                            download.NoteTitle = sellernote.Title;
                            download.NoteCategory = Convert.ToInt32(sellernote.Category);
                            db.Downloads.Add(download);
                            string gender = "";
                            switch (buyerDetail)
                            {
                                case "Male": gender = "him"; break;
                                case "Female": gender = "her"; break;
                            }
                            db.SaveChanges();
                            if (sellernote.SellingPrice == 0)
                            {
                                return RedirectToAction("DownloadFile", "Users", new { noteid = noteid });
                            }
                            else
                            {
                                MailMessage mail = new MailMessage();
                                mail.To.Add(sellerid.EmailID);
                                mail.From = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya");
                                mail.Subject = user.FirstName + " " + user.LastName + " wants to purchase your notes";
                                string Body = "<p><p style=" + "margin-bottom:0px;" + ">Hello," + sellerid.FirstName + " " + sellerid.LastName + "<br>We would like to inform you that, " + user.FirstName + " " + user.LastName + " wants to purchase your notes,Please see Buyer Requests tab and allow download access to Buyer if you have received the payment from " + gender + ".<br><p><b>Regards,<br>NotesMarketplace</b></p></p>";
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
                        }
                    }
                    if(isUserPaid)
                    {
                        return  RedirectToAction("DownloadFile", "Users",new { noteid=noteid});
                    }

                }
            }
            return RedirectToAction("SearchNotes","Users");
        }
        [Authorize]
        public ActionResult BuyerRequest(string SearchText)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.month = null;
                    ViewBag.day = null;
                    ViewBag.count = 0;
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == false).OrderBy(x => x.AttachmentDownloadDate).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var buyerdata = db.Users.ToList();
                    SelectList BuyerList = new SelectList(buyerdata, "EmailID", "ID");
                    ViewBag.buyerdetails = BuyerList;
                    var countrydata = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList = new SelectList(countrydata, "CountryCode", "CountryID");
                    ViewBag.countrydetails = CountryList;
                    var userdata = db.UserProfiles.ToList();
                    ViewBag.userdetails = userdata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.Downloads.Any(d => d.NoteTitle == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isBuyer = db.Users.Any(d => d.FirstName == SearchText);
                            bool isphoneno = db.UserProfiles.Any(d => d.Phone_Number == SearchText);
                            bool isPrice = decimal.TryParse(SearchText, out decimal price);

                            if (isTitle)
                            {
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == false && x.NoteTitle == SearchText && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                              
                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive==true).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == false && x.NoteCategory == cid && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                
                            }
                            if (isBuyer)
                            {
                                var bname = db.Users.Where(b => b.FirstName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == false && bname.Contains(x.DownloaderId) && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                            }
                            if (isphoneno)
                            {
                                var pno = db.UserProfiles.Where(b => b.Phone_Number == SearchText).Select(b => b.UserId).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == false && x.DownloaderId == pno && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
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
                                    downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == false && x.PurchasedPrice == price && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                    if (downloadList.Count != 0)
                                    {
                                        ViewBag.downloaddetails = downloadList;
                                    }
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
        public ActionResult DownloadNotes(string SearchText)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.month = null;
                    ViewBag.day = null;
                    ViewBag.count = 0;
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsSellerHasAllowedDownload == true && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var buyerdata = db.Users.ToList();
                    SelectList BuyerList = new SelectList(buyerdata, "EmailID", "ID");
                    ViewBag.buyerdetails = BuyerList;
                    var countrydata = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList = new SelectList(countrydata, "CountryCode", "CountryID");
                    ViewBag.countrydetails = CountryList;
                    var userdata = db.UserProfiles.ToList();
                    ViewBag.userdetails = userdata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.Downloads.Any(d => d.NoteTitle == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isBuyer = db.Users.Any(d => d.FirstName == SearchText);
                            bool isPrice = decimal.TryParse(SearchText, out decimal price);

                            if (isTitle)
                            {
                                downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsSellerHasAllowedDownload == true && x.NoteTitle == SearchText && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive==true).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsSellerHasAllowedDownload == true && x.NoteCategory == cid && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                            }
                            if (isBuyer)
                            {
                                var bname = db.Users.Where(b => b.FirstName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsSellerHasAllowedDownload == true && bname.Contains(x.DownloaderId) && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
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
                                    downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsSellerHasAllowedDownload == true && x.PurchasedPrice == price && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                    if (downloadList.Count != 0)
                                    {
                                        ViewBag.downloaddetails = downloadList;
                                    }
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
        public ActionResult SoldNotes(string SearchText)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.month = null;
                    ViewBag.day = null;
                    ViewBag.count = 0;
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == true && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var buyerdata = db.Users.ToList();
                    SelectList BuyerList = new SelectList(buyerdata, "EmailID", "ID");
                    ViewBag.buyerdetails = BuyerList;
                    var countrydata = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CountryList = new SelectList(countrydata, "CountryCode", "CountryID");
                    ViewBag.countrydetails = CountryList;
                    var userdata = db.UserProfiles.ToList();
                    ViewBag.userdetails = userdata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.Downloads.Any(d => d.NoteTitle == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isBuyer = db.Users.Any(d => d.FirstName == SearchText);
                            bool isPrice = decimal.TryParse(SearchText, out decimal price);

                            if (isTitle)
                            {
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == true && x.NoteTitle == SearchText && x.IsActive == true).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                
                            }
                             if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive==true).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == true && x.IsActive == true && x.NoteCategory == cid).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                            }
                            if (isBuyer)
                            {
                                var bname = db.Users.Where(b => b.FirstName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == true && x.IsActive == true && bname.Contains(x.DownloaderId)).OrderBy(x => x.AttachmentDownloadDate).ToList();
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
                                    downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsSellerHasAllowedDownload == true && x.IsActive == true && x.PurchasedPrice == price).OrderBy(x => x.AttachmentDownloadDate).ToList();
                                    if (downloadList.Count != 0)
                                    {
                                        ViewBag.downloaddetails = downloadList;
                                    }
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
        public ActionResult RejectedNotes(string SearchText)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    ViewBag.count = 0;
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected").OrderBy(x => x.PublishedDate).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var userdata = db.UserProfiles.ToList();
                    ViewBag.userdetails = userdata;
                    if (!String.IsNullOrEmpty(SearchText))
                    {
                        if (SearchText.GetType() == typeof(string))
                        {
                            bool isTitle = db.SellerNotes.Any(d => d.Title == SearchText);
                            bool isCat = db.NoteCategories.Any(d => d.CategoryName == SearchText);
                            bool isRemark = db.SellerNotes.Any(d => d.AdminRemarks == SearchText);

                            if (isTitle)
                            {
                                downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected" && x.Title == SearchText).OrderBy(x => x.PublishedDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }

                            }
                            if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText && x.IsActive==true).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected" && x.Category == cid).OrderBy(x => x.PublishedDate).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                             
                            }
                            if (isRemark)
                            {
                                downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected" && x.AdminRemarks == SearchText).OrderBy(x => x.PublishedDate).ToList();
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
        public ActionResult Clone(int noteid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNote cloningdata = db.SellerNotes.Where(x => x.ID == noteid).FirstOrDefault();
                cloningdata.Status = "draft";
                db.Entry(cloningdata).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("RejectedNotes");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Changepassword()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Changepassword(Changepassword changepassword)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    if (ModelState.IsValid)
                    {
                        var user = db.Users.FirstOrDefault(u => u.EmailID == User.Identity.Name);
                        if (changepassword.OldPassword == user.Password)
                        {
                            if (changepassword.NewPassword == changepassword.ConfirmPassword)
                            {
                                user.Password = changepassword.NewPassword;
                                db.SaveChanges();

                            }
                            else
                            {
                                ModelState.AddModelError("PassNotMatch", "Confirm password does not match");
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("IncorrectPass", "Incorrect password");
                        }
                    }
                }
            }
            return RedirectToAction("Login","Users");
        }
        [Authorize]
        public ActionResult AllowDownload(int buyerID,int _id)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                Download downloadingdata = db.Downloads.Where(x => x.DownloaderId == buyerID && x.ID==_id).FirstOrDefault();
                downloadingdata.IsActive = true;
                downloadingdata.IsSellerHasAllowedDownload = true;
                downloadingdata.ModifiedBy = downloadingdata.SellerId;
                downloadingdata.ModifiedDate = DateTime.Now;
                SellerNote seller = db.SellerNotes.Where(x => x.ID == downloadingdata.NoteID).FirstOrDefault();
                seller.total_Earnings = (decimal)(seller.total_Earnings + downloadingdata.PurchasedPrice);
                UserProfile userprofile = db.UserProfiles.Where(x => x.UserId == downloadingdata.SellerId).FirstOrDefault();
                userprofile.Total_earnings = (decimal)(userprofile.Total_earnings + downloadingdata.PurchasedPrice);
                db.Entry(downloadingdata).State = EntityState.Modified;
                db.Entry(userprofile).State = EntityState.Modified;
                db.SaveChanges();
                var sellerEmail = db.Users.FirstOrDefault(u => u.EmailID == User.Identity.Name);
                var buyerEmail = db.Users.FirstOrDefault(u => u.ID == buyerID);
                MailMessage mail = new MailMessage();
                mail.To.Add(buyerEmail.EmailID);
                mail.From = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya");
                mail.Subject = sellerEmail.FirstName + " " + sellerEmail.LastName + " Allows you to download a note";
                string Body = "<p><p style=" + "margin-bottom:0px;" + ">Hello," + buyerEmail.FirstName + " " + buyerEmail.LastName + "<br>We would like to inform you that, " + sellerEmail.FirstName + " " + sellerEmail.LastName + " Allows you to download a note,Please login and see My Download tabs to download particular note.<br><p><b>Regards,<br>NotesMarketplace</b></p></p>";
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
            return RedirectToAction("BuyerRequest");
        }
        [Authorize]
        public ActionResult DeleteNotes(int?noteid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                if (noteid!=null)
                {
                    SellerNote notes = db.SellerNotes.Where(n=>n.ID==noteid).FirstOrDefault();
                    notes.IsActive = false;
                    db.Entry(notes).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("SellyourNotes","Users");
        }
        [Authorize]
        public ActionResult Review(int? noteid, string Message, string rating,SellerNotesReview seller)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);

                seller.ReviewedByID = user.ID;
                seller.Comments = Message;
                seller.NoteID = (int)noteid;
                seller.Ratings = decimal.Parse(rating);
                seller.CreatedDate = DateTime.Now;
                seller.IsActive = true;
                db.SellerNotesReviews.Add(seller);
                db.SaveChanges();
                var review = db.SellerNotesReviews.Where(r => r.NoteID == noteid && r.IsActive==true).ToList();
                var count = 0;
                decimal star = 0;
                foreach(var r in review)
                {
                    star += r.Ratings;
                    count += 1;
                }
                decimal note_star = star / count;
                SellerNote sellerNote = db.SellerNotes.FirstOrDefault(n => n.ID == noteid);
                sellerNote.rating = note_star;
                db.Entry(sellerNote).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("DownloadNotes", "Users");
        }
        [Authorize]
        public ActionResult ReportIssue(int? noteid, string Issue, SellerNotesReportedIssue notesReportedIssue)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                var notedetail = db.SellerNotes.FirstOrDefault(s=>s.ID==noteid);
                notesReportedIssue.NoteID = (int)noteid;
                notesReportedIssue.ReportedBYID = user.ID;
                notesReportedIssue.Remarks = Issue;
                notesReportedIssue.CreatedDate = DateTime.Now;
                notesReportedIssue.IsActive = true;
                db.SellerNotesReportedIssues.Add(notesReportedIssue);
                db.SaveChanges();
                var sellerEmail = db.Users.FirstOrDefault(s => s.ID == notedetail.SellerID);
                var systemAdmin_Email = db.SystemConfigurations.Where(s => s.K_ey == "admin_Email").Select(s=>s.Value).FirstOrDefault();
                string[] sender_email = systemAdmin_Email.Split(',');
                foreach(var sender in sender_email)
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(sender);
                    mail.From = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya");
                    mail.Subject = user.FirstName + " " + user.LastName + " Reported an issue for " + notedetail.Title;
                    string Body = "<p><p style=" + "margin-bottom:0px;" + ">Hello Admins, <br>We want to inform you that, " + user.FirstName + " " + user.LastName + " Reported an issue for " + sellerEmail.FirstName + " " + sellerEmail.LastName + "'s Note with title " + notedetail.Title + ". Please look and take required actions.<br><p><b>Regards,<br>NotesMarketplace</b></p></p>";
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
                
            }
            return RedirectToAction("DownloadNotes", "Users");
        }
        [HttpGet]
        public ActionResult Faq()
        {
            Header();
            return View();
        }

        [HttpGet]
        public ActionResult ContactUs()
        {
            Header();
            return View();
        }
        [HttpPost]
        public ActionResult ContactUs(ContactUs contactUs)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    var system_Admin_Email = db.SystemConfigurations.Where(x => x.K_ey == "admin_Email").Select(x => x.Value).FirstOrDefault();
                    string[] sender_email = system_Admin_Email.Split(',');
                    foreach (var sender in sender_email)
                    {
                        MailMessage mail = new MailMessage();
                        mail.To.Add(sender);
                        mail.From = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya");
                        mail.Subject = contactUs.firstName + " -Query";
                        string Body = "<p><p>Hello,</p>" + contactUs.comment + "<p><b>Regards,<br>" + contactUs.firstName + "</b></p></p>";
                        mail.Body = Body;
                        mail.IsBodyHtml = true;
                        mail.BodyEncoding = UTF8Encoding.UTF8;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("parejiyadevanshi@gmail.com", "jhmswewvcofgnyfr"); // Enter senders User name and password  
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                
            }
            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult AddNotes(int ?noteid)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {

                    var getCategoryList = db.NoteCategories.Where(x=>x.IsActive==true).ToList();
                    var getTypeList = db.NoteTypes.Where(x => x.IsActive == true).ToList();
                    var getCountryList = db.Countries.Where(x => x.IsActive == true).ToList();
                    SelectList CategoryList, TypeList, CountryList;
                    if(noteid !=null)
                    {
                        User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                        bool isrecordCreated = db.SellerNotes.Any(n => n.ID == noteid && n.SellerID == user.ID);
                        if(isrecordCreated)
                        {
                            var notedetail = db.SellerNotes.FirstOrDefault(n => n.ID == noteid && n.SellerID == user.ID);
                            Addnotes addnotes = new Addnotes();
                            addnotes.Title = notedetail.Title;
                            addnotes.NoOfPages = (int)notedetail.NumberofPages;
                            addnotes.Description = notedetail.Description;
                            addnotes.InstitutionName = notedetail.UniversityName;
                            addnotes.CourseName = notedetail.Course;
                            addnotes.CourseCode = notedetail.CourseCode;
                            addnotes.ProfessorName = notedetail.Professor;
                            addnotes.SellPrice = (float)notedetail.SellingPrice;
                            addnotes.noteid = (int)noteid;
                            if (notedetail.IsPaid == false)
                            {
                                addnotes.SellMode = "free";
                            }
                            else if (notedetail.IsPaid == true)
                            {
                                addnotes.SellMode = "paid";
                            }
                            NoteCategory category = db.NoteCategories.FirstOrDefault(c => c.CategoryID == notedetail.Category && c.IsActive==true);
                            object catid = category.CategoryID;
                            CategoryList = new SelectList(getCategoryList, "CategoryID", "CategoryName",catid);
                            ViewBag.categoryname = CategoryList;
                            NoteType noteType = db.NoteTypes.FirstOrDefault(t => t.TypeID == notedetail.NoteType && t.IsActive==true);
                            object tid = noteType.TypeID;
                            TypeList = new SelectList(getTypeList, "TypeID", "TypeName",tid);
                            ViewBag.typename = TypeList;
                            Country country = db.Countries.FirstOrDefault(co => co.CountryID == notedetail.Country && co.IsActive==true);
                            object counId = country.CountryID;
                            CountryList = new SelectList(getCountryList, "CountryID", "CountryName",counId);
                            ViewBag.countryname = CountryList;
                            addnotes.uploadPictureGet = notedetail.DisplayPicture;
                            string picfile = Path.GetFileName(notedetail.DisplayPicture);
                            ViewBag.picname = picfile;
                            addnotes.uploadNotesGet = notedetail.FilePath;
                            string notefile = Path.GetFileName(notedetail.FilePath);
                            ViewBag.notename = notefile;
                            addnotes.uploadPreviewGet = notedetail.NotesPreview;
                            string previewfile = Path.GetFileName(notedetail.NotesPreview);
                            ViewBag.previewname = previewfile;
                            return View(addnotes);
                        }
                    }
                    CategoryList = new SelectList(getCategoryList, "CategoryID", "CategoryName");
                    ViewBag.categoryname = CategoryList;
                    TypeList = new SelectList(getTypeList, "TypeID", "TypeName");
                    ViewBag.typename = TypeList;
                    CountryList = new SelectList(getCountryList, "CountryID", "CountryName");
                    ViewBag.countryname = CountryList;
                    ViewBag.countrydefvalue = "Select your country";
                    ViewBag.categorydefvalue = "Select your category";
                    ViewBag.typedefvalue = "Select your type";
                    ViewBag.picname = null;
                    ViewBag.notename = null;
                    ViewBag.previewname = null;
                }
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult AddNotes(Addnotes addnotes,SellerNote sellerNote,string submit)
        {
                using (var db = new Notes_MarketplaceEntities())
                {
                    int sellernoteTemp = 0;

                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var systemUpload_image = db.SystemConfigurations.Where(s => s.K_ey == "Note_image").Select(s => s.Value).FirstOrDefault();
                if (addnotes.noteid != null)
                    {
                        sellerNote = db.SellerNotes.Where(u => u.SellerID == user.ID && u.ID == addnotes.noteid).FirstOrDefault();
                        if (sellerNote != null)
                        {
                            sellernoteTemp = 1;
                        }
                    }
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
                    sellerNote.PublishedDate = DateTime.Now;
                    sellerNote.IsActive = false;
                    sellerNote.total_Earnings = 0;
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

                    var path = Server.MapPath("~\\UploadedFiles\\") + sellerNote.SellerID;
                    if (!(Directory.Exists(path)))
                    {
                        Directory.CreateDirectory(path);
                        ViewBag.Mesage = "Directory created";
                    }
                    else
                    {
                        ViewBag.Mesage = "Directory exist";
                    }
                    string temp_path = "~\\UploadedFiles\\" + sellerNote.SellerID + "\\";
                    if (addnotes.uploadPicture != null && addnotes.uploadPicture.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(addnotes.uploadPicture.FileName);
                        string imgext = Path.GetExtension(_FileName);
                        if (imgext == ".jpg" || imgext == ".gif" || imgext == ".jpeg" || imgext == ".png")
                        {

                            string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                            addnotes.uploadPicture.SaveAs(_path);
                            string upload_picpath = temp_path + _FileName;
                            sellerNote.DisplayPicture = upload_picpath;
                        }
                        else
                        {
                            ViewBag.Extensionerror = "Please select image file";
                            return View();
                        }
                    }
                    else if (addnotes.uploadPictureGet != null)
                    {
                        sellerNote.DisplayPicture = addnotes.uploadPictureGet;
                    }
                else
                {
                    sellerNote.DisplayPicture = systemUpload_image;
                }
                if (addnotes.uploadNotes != null && addnotes.uploadNotes.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(addnotes.uploadNotes.FileName);
                        string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                        addnotes.uploadNotes.SaveAs(_path);
                        string upload_notespath = temp_path + _FileName;
                        sellerNote.FileName = _FileName;
                        sellerNote.FilePath = upload_notespath;
                    }
                    else if (addnotes.uploadNotesGet != null)
                    {
                        sellerNote.FilePath = addnotes.uploadNotesGet;
                    }
                    if (addnotes.uploadPreview != null && addnotes.uploadPreview.ContentLength > 0)
                    {
                        string _FileName = Path.GetFileName(addnotes.uploadPreview.FileName);
                        string _path = Path.Combine(Server.MapPath(temp_path), _FileName);
                        addnotes.uploadPreview.SaveAs(_path);
                        string upload_previewpath = temp_path + _FileName;
                        sellerNote.NotesPreview = upload_previewpath;
                    }
                    else if (addnotes.uploadPreviewGet != null)
                    {
                        sellerNote.NotesPreview = addnotes.uploadPreviewGet;
                    }


                    switch (submit)
                    {
                        case "save":
                            sellerNote.Status = "draft";
                            break;
                        case "publish":
                            sellerNote.Status = "submitted";
                        var sellerEmail = db.Users.FirstOrDefault(u => u.EmailID == User.Identity.Name);
                        var systemAdmin_Email = db.SystemConfigurations.Where(s => s.K_ey == "admin_Email").Select(s => s.Value).FirstOrDefault();
                        string[] sender_email = systemAdmin_Email.Split(',');
                        foreach (var sender in sender_email)
                        {
                            MailMessage mail = new MailMessage();
                            mail.To.Add(sender);
                            mail.From = new MailAddress("parejiyadevanshi@gmail.com", "Devanshi Parejiya");
                            mail.Subject = sellerEmail.FirstName + " " + sellerEmail.LastName + " Sent his note for review";
                            string Body = "<p><p style=" + "margin-bottom:0px;" + ">Hello Admins,<br>We want to inform you that, " + sellerEmail.FirstName + " " + sellerEmail.LastName + " sent his note " + addnotes.Title + " for review. Please look at the notes and take required actions.<br><p><b>Regards,<br>NotesMarketplace</b></p></p>";
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
                        break;
                    }

                    if (sellernoteTemp == 1)
                    {
                        db.Entry(sellerNote).State = EntityState.Modified;
                    }
                    else
                    {
                        db.SellerNotes.Add(sellerNote);
                    }
                    db.SaveChanges();
                    return RedirectToAction("SellyourNotes", "Users");
                }

            return View();
        }
        [Authorize]
        public FileResult DownloadFile(int noteid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                SellerNote notedetail = db.SellerNotes.FirstOrDefault(d => d.ID == noteid);
                string path = Server.MapPath("~\\UploadedFiles\\")+notedetail.SellerID.ToString();
                string filename = Path.GetFileName(notedetail.FileName);
                string fullpath = Path.Combine(path, filename);
                return File(fullpath, "application/pdf", notedetail.FileName);
            }
        }
        
    }
}
