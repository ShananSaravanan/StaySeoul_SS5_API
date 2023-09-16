using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaySeoul_SS5API.Models
{
    public class ServiceDiscount
    {
        public decimal total { get; set; }
        public string couponCode { get; set; }
    }
}