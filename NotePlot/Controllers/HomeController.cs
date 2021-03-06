﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;

namespace NotePlot.Controllers
{
    public class HomeController : Controller
    {
        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
        public IActionResult Index()
        {
            return View();
        }

        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 3600)]
        public IActionResult Help()
        {
            return View();
        }

        [NonAction]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [NonAction]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            string errorMessage = string.Empty;
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionFeature.Error is System.Data.SqlClient.SqlException)
                errorMessage = "Cервер базы данных";
            else
                errorMessage = "Web-сервер";
            return View("Error", errorMessage);
        }

    }
}
