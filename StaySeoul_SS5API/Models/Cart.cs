using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaySeoul_SS5API.Models
{
    public class Cart
    {
        public long cartID { get;set; }
        public long UID { get;set; }
        public string serviceName { get;set; }
        public string addOnNotes { get; set; }
        public string couponCode { get; set; }
        public Guid guid { get; set; }
        public long addOnID { get; set; }
        public long serviceID{get;set;}
        public decimal payableAmount { get;set; }
        public DateTime fromDate { get;set; }
        public DateTime toDate { get;set; }
        public string iconName { get; set; }
        public int noOfPPl { get; set; }
        public decimal totalPayable { get; set; }

    }
}