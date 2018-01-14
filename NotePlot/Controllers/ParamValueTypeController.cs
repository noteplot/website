using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Controllers
{
    public class ParamValueTypeController : Controller
    {
        IRepositoryParamValueType repo;
        public ParamValueTypeController(IRepositoryParamValueType r)
        {
            repo = r;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("ParamValueTypeView", repo.GetParamValueTypes());
        }
        /*
        [HttpGet]
        public ActionResult ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", repo.GetParamValueTypes());
            else
                return BadRequest("Нет аутентификации!");

        }
        */
        [HttpGet]
        public async Task<ActionResult> ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", await repo.GetParamValueTypesAsync());
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}