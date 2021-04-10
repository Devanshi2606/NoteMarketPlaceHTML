using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class Admin_Profile
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; set; }
        [Required]
        [EmailAddress]
        public string emailId { get; set; }
        [EmailAddress]
        public string Secondary_emailId { get; set; }
        public string CountryCode { get; set; }
        public string phoneNo { get; set; }
        public HttpPostedFileBase profilePicture { get; set; }
        public string profilePictureGet { get; set; }

    }
}