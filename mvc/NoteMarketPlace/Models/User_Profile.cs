using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Models
{
    public class User_Profile
    {   
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required][EmailAddress]
        public string emailId { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "dd/MMM/yyyy")]
        public DateTime DOB { get; set; }
        public string Gender { get; set; }
        [Key]
        public string CountryID { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string phoneNo { get; set; }
        public HttpPostedFileBase profilePicture { get; set; }
        public string profilePictureGet { get; set; }
        [Required]
        public string address1 { get; set; }
        [Required]
        public string address2 { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string state { get; set; }
        [Required]
        public string zipCode { get; set; }
        
        public string University { get; set; }
        public string College { get; set; }
        
    }
}