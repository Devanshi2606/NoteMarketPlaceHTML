using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class Signup
    {
        
        [Key]
        public int id { get; set; }
        public int roleId { get; set; }

        [Required]
        public string firstname { get; set; }

        [Required]
        public string lastname { get; set; }

        [Required][EmailAddress]
        public string emailId { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string confirmPassword { get; set; }

        public bool isEmailVerified { get; set; }

        
    }
}