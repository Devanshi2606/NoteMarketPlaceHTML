using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class Login
    {
        [Required][EmailAddress]
        public string emailId { get; set; }
        [Required]
        public string password { get; set; }
    }
}