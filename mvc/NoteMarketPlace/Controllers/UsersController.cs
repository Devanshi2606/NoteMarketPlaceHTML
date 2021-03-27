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
                try
                {
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    var userdata = db.UserProfiles.FirstOrDefault(s => s.UserId == user.ID);

                    if (userdata != null)
                    {
                        //ViewBag.userdetails = userdata.Profile_Picture;
                        TempData["Userdetails"] = userdata.Profile_Picture;
                    }
                    else
                    {
                        TempData["Userdetails"] = "~/ UploadedFiles / defaultfile / user - 1.jpg";
                    }
                }
                catch
                {
                    TempData["Userdetails"] = "";
                }
                return View("~/Views/Shared/Header.cshtml");
            }
            
        }

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
        public ActionResult Login(Models.Login l)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    bool isValid = db.Users.Any(x => x.EmailID == l.emailId && x.Password == l.password && x.IsEmailVerified);
                    
                    if (isValid)
                    {
                        User user = db.Users.FirstOrDefault(x => x.EmailID == l.emailId);
                        FormsAuthentication.SetAuthCookie(l.emailId, false);
                        if (user.RoleID == 1)
                        {
                            bool isRecordcreated = db.UserProfiles.Any(p => p.UserId == user.ID);
                            if (isRecordcreated)
                            {
                                return RedirectToAction("Home");
                            }
                            return RedirectToAction("Userprofile", "Users");
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
            }
            return View();
        }
        
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
                    var getCountryList = db.Countries.ToList();
                    SelectList CountryList, CountryNameList;
                    User user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    bool isRecordcreated = db.UserProfiles.Any(p => p.UserId == user.ID);
                    if(isRecordcreated)
                    {
                        var userList = db.UserProfiles.FirstOrDefault(p => p.UserId == user.ID);
                        User_Profile up = new User_Profile();
                        up.Firstname = user.FirstName;
                        up.Lastname = user.LastName;
                        up.emailId = user.EmailID;
                        up.DOB = Convert.ToDateTime(userList.DOB);
                        up.phoneNo = userList.Phone_Number;
                        up.address1 = userList.Address_Line1;
                        up.address2 = userList.Address_Line2;
                        up.city = userList.City;
                        up.state = userList.State;
                        up.University = userList.University;
                        up.College = userList.College;
                        up.zipCode = userList.Zip_Code;
                        Country country = db.Countries.FirstOrDefault(c => c.CountryID == userList.Country);
                        object counId = country.CountryID;
                        CountryList = new SelectList(getCountryList, "CountryID", "CountryCode",counId);
                        ViewBag.countrycode = CountryList;
                        CountryNameList = new SelectList(getCountryList, "CountryID", "CountryName",counId);
                        ViewBag.countryname = CountryNameList;
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
                    
                }
            }
            return View();

        }
        [HttpPost]
        public ActionResult Userprofile(User_Profile userProfilemodel)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    int userProfileTemp = 0;
                    int userTemp = 0;
                    
                    var user = db.Users.Where(x => x.EmailID == User.Identity.Name).FirstOrDefault();
                    var userProfileDb = db.UserProfiles.Where(u => u.UserId == user.ID).FirstOrDefault();
                    if(user!=null)
                    {
                        userTemp = 1;
                    }
                    if(userProfileDb!=null)
                    {
                        userProfileTemp = 1;
                    }
                    user.FirstName = userProfilemodel.Firstname;
                    user.LastName = userProfilemodel.Lastname;
                    userProfileDb.UserId = user.ID;
                    userProfileDb.DOB = userProfilemodel.DOB;
                    userProfileDb.Gender = userProfilemodel.Gender;
                    userProfileDb.Country_Code = userProfilemodel.CountryCode;
                    userProfileDb.Phone_Number = userProfilemodel.phoneNo;
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
                        userProfileDb.Profile_Picture = "~\\UploadedFiles\\defaultfile\\user-1.jpg";
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
                    userProfileDb.CreatedDate = DateTime.Now;

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
                    var bookdata = db.SellerNotes.Where(s => s.Status == "approved").ToList();
                    var inappropriate_message = db.SellerNotesReportedIssues.ToList();
                    SelectList ReportedList = new SelectList(inappropriate_message, "ID", "NoteID");
                    ViewBag.inappropriateMsg = ReportedList;
                    var note_review = db.SellerNotesReviews.ToList();
                    SelectList ReviewList = new SelectList(note_review, "Ratings", "NoteID");
                    ViewBag.reviewMsg = ReviewList;
                    var getCountryList = db.Countries.ToList();
                    SelectList CountryList = new SelectList(getCountryList, "CountryID", "CountryName");
                    ViewBag.countryname = CountryList;
                    var getCategoryList = db.NoteCategories.ToList();
                    SelectList CategoryList = new SelectList(getCategoryList, "CategoryID", "CategoryName");
                    ViewBag.categoryname = CategoryList;
                    var getTypeList = db.NoteTypes.ToList();
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
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Title == changeInTitle).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    if (changeInType!=null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.NoteType==changeInType).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    else if (changeINCategory != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Category == changeINCategory).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    else if (changeInUniversity != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.UniversityName == changeInUniversity).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    else if (changeInCourse != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Course == changeInCourse).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    else if (changeInCountry != null)
                    {
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.Country == changeInCountry).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    else if (changeInRating != null)
                    {
                        var r = db.SellerNotes.Where(s => s.Status == "approved" ).Select(s=>s.rating).ToList();
                        bookdata = db.SellerNotes.Where(s => s.Status == "approved" && s.rating >=changeInRating).ToList();
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    else
                    {
                        if (bookdata.Count != 0)
                        {
                            ViewBag.bookdetails = bookdata;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    


                }
            }
            return View();
        }

        
        [Authorize]
        public ActionResult SellyourNotes(string progressText,string publishedText)
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
                    var progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && (x.Status == "draft" || x.Status == "inReview" || x.Status == "submitted")).ToList();
                    var publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved").ToList();
                    ViewBag.progressdetails = null;
                    ViewBag.publisheddetails = null;
                    var SoldNotes = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == true).ToList();
                    ViewBag.Soldnotes = SoldNotes.Count;
                    var earning = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == true).Sum(x => x.PurchasedPrice);
                    ViewBag.Earning = earning;
                    var downloadNotes = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsPaid == true).ToList();
                    ViewBag.downloadNotes = downloadNotes.Count;
                    var rejectedNotes = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected").ToList();
                    ViewBag.rejectedNotes = rejectedNotes.Count;
                    var buyerNotes = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == false).ToList();
                    ViewBag.buyerNotes = buyerNotes.Count;
                    var categorydata = db.NoteCategories.ToList();
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
                                progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && (x.Status == "draft" || x.Status == "inReview" || x.Status == "submitted") && x.Title == progressText).ToList();
                                if (progressList.Count != 0)
                                {
                                    ViewBag.progressdetails = progressList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == progressText).Select(x => x.CategoryID).FirstOrDefault();
                                progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && (x.Status == "draft" || x.Status == "inReview" || x.Status == "submitted") && x.Category == cid).ToList();
                                if (progressList.Count != 0)
                                {
                                    ViewBag.progressdetails = progressList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isStatus)
                            {
                                progressList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == progressText).ToList();
                                if (progressList.Count != 0)
                                {
                                    ViewBag.progressdetails = progressList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            
                            else
                            {
                                ViewBag.NorecordDetail = "No Records Found";
                            }
                        }
                    }
                    else
                    {
                        if (progressList.Count != 0)
                        {
                            ViewBag.progressdetails = progressList;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
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
                                publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.Title == publishedText).ToList();
                                if (publishedList.Count != 0)
                                {
                                    ViewBag.publisheddetails = publishedList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == publishedText).Select(x => x.CategoryID).FirstOrDefault();
                                publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.Category == cid).ToList();
                                if (publishedList.Count != 0)
                                {
                                    ViewBag.publisheddetails = publishedList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isPrice)
                            {

                                bool Price = db.SellerNotes.Any(d => d.SellingPrice == price);
                                if (Price)
                                {
                                    publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.SellingPrice == price).ToList();
                                    if (publishedList.Count != 0)
                                    {
                                        ViewBag.publisheddetails = publishedList;
                                    }
                                    else
                                    {
                                        ViewBag.NorecordDetail = "No Records Found";
                                    }
                                }
                            }
                            else if (isSellType)
                            {
                                if (publishedText == "free")
                                {
                                    publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.IsPaid == false).ToList();
                                }
                                 if (publishedText == "paid")
                                {
                                    publishedList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "approved" && x.IsPaid == true).ToList();
                                }
                                if (publishedList.Count != 0)
                                {
                                    ViewBag.publisheddetails = publishedList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else
                            {
                                ViewBag.NorecordDetail = "No Records Found";
                            }
                        }
                    }
                    else
                    {
                        if (publishedList.Count != 0)
                        {
                            ViewBag.publisheddetails = publishedList;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                    
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

                    var bookdata = db.SellerNotes.Where(x => x.ID == noteid).ToList();
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
                    var bookdata = db.SellerNotes.Where(x => x.ID == noteid).ToList();
                    var sellernote = db.SellerNotes.FirstOrDefault(s => s.ID == noteid);
                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                    if (ModelState.IsValid)
                    {
                        download.NoteID = noteid;
                        download.SellerId = sellernote.SellerID;
                        download.DownloaderId = user.ID;
                        download.IsSellerHasAllowedDownload = false;
                        download.AttachmentName = sellernote.FileName;
                        download.AttachmentPath = sellernote.FilePath;
                        download.IsAttachmentDownloaded = false;
                        download.AttachmentDownloadDate = DateTime.Now;
                        download.IsPaid = false;
                        download.PurchasedPrice = sellernote.SellingPrice;
                        download.NoteTitle = sellernote.Title;
                        download.NoteCategory = Convert.ToInt32(sellernote.Category);
                        db.Downloads.Add(download);
                        db.SaveChanges();
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
                    var downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == false).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var buyerdata = db.Users.ToList();
                    SelectList BuyerList = new SelectList(buyerdata, "EmailID", "ID");
                    ViewBag.buyerdetails = BuyerList;
                    var countrydata = db.Countries.ToList();
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
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == false && x.NoteTitle == SearchText).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == false && x.NoteCategory == cid).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isBuyer)
                            {
                                var bname = db.Users.Where(b => b.FirstName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == false && bname.Contains(x.DownloaderId)).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isphoneno)
                            {
                                var pno = db.UserProfiles.Where(b => b.Phone_Number == SearchText).Select(b => b.UserId).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == false && x.DownloaderId == pno).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isPrice)
                            {

                                bool Price = db.Downloads.Any(d => d.PurchasedPrice == price);
                                if (Price)
                                {
                                    downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == false && x.PurchasedPrice == price).ToList();
                                    if (downloadList.Count != 0)
                                    {
                                        ViewBag.downloaddetails = downloadList;
                                    }
                                    else
                                    {
                                        ViewBag.NorecordDetail = "No Records Found";
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.NorecordDetail = "No Records Found";
                            }
                        }
                    }
                    else
                    {
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
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
                    var downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsPaid == true).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var buyerdata = db.Users.ToList();
                    SelectList BuyerList = new SelectList(buyerdata, "EmailID", "ID");
                    ViewBag.buyerdetails = BuyerList;
                    var countrydata = db.Countries.ToList();
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
                                downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsPaid == true && x.NoteTitle == SearchText).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsPaid == true && x.NoteCategory == cid).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }

                            }
                            else if (isBuyer)
                            {
                                var bname = db.Users.Where(b => b.FirstName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsPaid == true && bname.Contains(x.DownloaderId)).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isPrice)
                            {

                                bool Price = db.Downloads.Any(d => d.PurchasedPrice == price);
                                if (Price)
                                {
                                    downloadList = db.Downloads.Where(x => x.DownloaderId == user.ID && x.IsPaid == true && x.PurchasedPrice == price).ToList();
                                    if (downloadList.Count != 0)
                                    {
                                        ViewBag.downloaddetails = downloadList;
                                    }
                                    else
                                    {
                                        ViewBag.NorecordDetail = "No Records Found";
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.NorecordDetail = "No Records Found";
                            }
                        }
                    }
                    else
                    {
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
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
                    var downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == true).ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.ToList();
                    SelectList CategoryList = new SelectList(categorydata, "CategoryName", "CategoryID");
                    ViewBag.categorydetails = CategoryList;
                    var buyerdata = db.Users.ToList();
                    SelectList BuyerList = new SelectList(buyerdata, "EmailID", "ID");
                    ViewBag.buyerdetails = BuyerList;
                    var countrydata = db.Countries.ToList();
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
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == true && x.NoteTitle == SearchText).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == true && x.NoteCategory == cid).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }

                            }
                            else if (isBuyer)
                            {
                                var bname = db.Users.Where(b => b.FirstName == SearchText).Select(b => b.ID).ToList();
                                downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == true && bname.Contains(x.DownloaderId)).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isPrice)
                            {

                                bool Price = db.Downloads.Any(d => d.PurchasedPrice == price);
                                if (Price)
                                {
                                    downloadList = db.Downloads.Where(x => x.SellerId == user.ID && x.IsPaid == true && x.PurchasedPrice == price).ToList();
                                    if (downloadList.Count != 0)
                                    {
                                        ViewBag.downloaddetails = downloadList;
                                    }
                                    else
                                    {
                                        ViewBag.NorecordDetail = "No Records Found";
                                    }
                                }
                            }
                            else
                            {
                                ViewBag.NorecordDetail = "No Records Found";
                            }
                        }
                    }
                    else
                    {
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
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
                    var downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected").ToList();
                    ViewBag.downloaddetails = null;
                    var categorydata = db.NoteCategories.ToList();
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
                                downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected" && x.Title == SearchText).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isCat)
                            {
                                var cid = db.NoteCategories.Where(x => x.CategoryName == SearchText).Select(x => x.CategoryID).FirstOrDefault();
                                downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected" && x.Category == cid).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else if (isRemark)
                            {
                                downloadList = db.SellerNotes.Where(x => x.SellerID == user.ID && x.Status == "rejected" && x.AdminRemarks == SearchText).ToList();
                                if (downloadList.Count != 0)
                                {
                                    ViewBag.downloaddetails = downloadList;
                                }
                                else
                                {
                                    ViewBag.NorecordDetail = "No Records Found";
                                }
                            }
                            else
                            {
                                ViewBag.NorecordDetail = "No Records Found";
                            }
                        }
                    }
                    else
                    {
                        if (downloadList.Count != 0)
                        {
                            ViewBag.downloaddetails = downloadList;
                        }
                        else
                        {
                            ViewBag.NorecordDetail = "No Records Found";
                        }
                    }
                }
            }
            return View();
        }
        
        public ActionResult Clone(int noteid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                var cloningdata = db.SellerNotes.Where(x => x.ID == noteid).FirstOrDefault();
                cloningdata.Status = "draft";
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
            return View();
        }
        public ActionResult AllowDownload(int buyerID,int _id)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                var downloadingdata = db.Downloads.Where(x => x.DownloaderId == buyerID && x.ID==_id).FirstOrDefault();
                downloadingdata.IsPaid = true;
                downloadingdata.IsSellerHasAllowedDownload = true;
                db.SaveChanges();
            }
            return RedirectToAction("BuyerRequest");
        }
        public ActionResult DeleteNotes(int?noteid)
        {
            using (var db = new Notes_MarketplaceEntities())
            {
                if (noteid!=null)
                {
                    SellerNote notes = db.SellerNotes.Where(n=>n.ID==noteid).FirstOrDefault();
                    db.SellerNotes.Remove(notes);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("SellyourNotes","Users");
        }
        public ActionResult Review(SellerNotesReview seller,string rate,string comment,Review r)
        {
            
            using (var db=new Notes_MarketplaceEntities())
            {
                var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
                
                seller.ReviewedByID = user.ID;
                seller.Comments = comment;
                seller.Ratings = decimal.Parse(rate);
                seller.CreatedDate = DateTime.Now;
                db.SellerNotesReviews.Add(seller);
                db.SaveChanges();
            }
            return RedirectToAction("DownloadNotes");
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
        public ActionResult AddNotes(int ?noteid)
        {
            Header();
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {

                    var getCategoryList = db.NoteCategories.ToList();
                    var getTypeList = db.NoteTypes.ToList();
                    var getCountryList = db.Countries.ToList();
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
                            NoteCategory category = db.NoteCategories.FirstOrDefault(c => c.CategoryID == notedetail.Category);
                            object catid = category.CategoryID;
                            CategoryList = new SelectList(getCategoryList, "CategoryID", "CategoryName",catid);
                            ViewBag.categoryname = CategoryList;
                            NoteType noteType = db.NoteTypes.FirstOrDefault(t => t.TypeID == notedetail.NoteType);
                            object tid = noteType.TypeID;
                            TypeList = new SelectList(getTypeList, "TypeID", "TypeName",tid);
                            ViewBag.typename = TypeList;
                            Country country = db.Countries.FirstOrDefault(co => co.CountryID == notedetail.Country);
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

        [HttpPost]
        public ActionResult AddNotes(Addnotes addnotes,SellerNote sellerNote,string submit)
        {
            if (ModelState.IsValid)
            {
                using (var db = new Notes_MarketplaceEntities())
                {
                    int sellernoteTemp = 0;

                    var user = db.Users.FirstOrDefault(x => x.EmailID == User.Identity.Name);
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
                            sellerNote.PublishedDate = DateTime.Now;
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
            }

            return View();
        }
        
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
            User user = db.Users.Find(id
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
