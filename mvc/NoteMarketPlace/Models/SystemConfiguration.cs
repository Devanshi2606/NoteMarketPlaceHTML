using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class SystemConfiguration
    {
        [Required]
        [EmailAddress]
        public string emailAddress { get; set; }
        [Required]
        public string phoneNo { get; set; }
        public string adminEmails { get; set; }
        [Required]
        public string FacebookUrl { get; set; }
        [Required]
        public string TwitterUrl { get; set; }
        [Required]
        public string LinkedInUrl { get; set; }
        public HttpPostedFileBase uploadPicture { get; set; }
        public string uploadPictureGet { get; set; }
        public HttpPostedFileBase profilePicture { get; set; }
        public string profilePictureGet { get; set; }

    }
}