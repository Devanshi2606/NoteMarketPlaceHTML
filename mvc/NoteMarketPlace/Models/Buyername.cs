using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NoteMarketPlace.Models
{
    public class Buyername
    {
        public SelectList BuyerName { get; set; }
        public SelectList SellerName { get; set; }
        public SelectList NoteTitle { get; set; }
        public SellerNote[] published_list{get;set;}
    }
}