﻿using System.Web;
using System.Web.Mvc;

namespace StaySeoul_SS5API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
