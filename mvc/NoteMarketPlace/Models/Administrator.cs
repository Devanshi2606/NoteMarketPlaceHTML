using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NoteMarketPlace.Models
{
    public class Administrator
    {
        public int? adminid { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string CountryCode { get; set; }
        public string phoneno { get; set; }
    }
}