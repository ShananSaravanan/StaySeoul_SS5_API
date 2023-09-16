using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StaySeoul_SS5API.Models
{
    public class ServiceTypes
    {
        public long serviceID { get; set; }
        public string iconName { get; set; }
        public string serviceType { get; set; }
        public string description { get; set; }
    }
}