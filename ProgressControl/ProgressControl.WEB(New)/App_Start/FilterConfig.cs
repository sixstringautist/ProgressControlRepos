﻿using System.Web;
using System.Web.Mvc;

namespace ProgressControl.WEB_New_
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
