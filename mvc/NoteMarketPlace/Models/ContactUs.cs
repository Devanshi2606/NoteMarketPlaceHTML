using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class ContactUs
    {
        [Required]
        public string firstName { get; set; }
        [Required][EmailAddress]
        public string emailId { get; set; }
        [Required][StringLength(50)]
        public string subject { get; set; }
        [Required]
        public string comment { get; set; }
    }
}