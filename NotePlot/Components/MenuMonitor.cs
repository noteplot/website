using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace NotePlot.Components
{
    public class MenuMonitor : ViewComponent
    {
        public IViewComponentResult Invoke()
        {            
            return View("MenuMonitorView", HttpContext.User.Identity.IsAuthenticated);
        }

    }
}
