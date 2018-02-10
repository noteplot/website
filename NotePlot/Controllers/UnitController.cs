using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Controllers
{
    public class UnitController : Controller
    {
        IRepositoryParameterUnit repo;
        public UnitController(IRepositoryParameterUnit r)
        {
            repo = r;
        }

        // GET: ParameterGroup - диалог выбора группы
        /*
        public ActionResult ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", repo.GetParameterUnits(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }
        */
        public async Task<ActionResult> ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", await repo.GetParameterUnitsAsync(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }

        public async Task<IActionResult> UnitList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("UnitList", await repo.GetParameterUnitsAsync(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }


    }
}