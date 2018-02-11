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

        public async Task<IActionResult> Create(long id)
        {
            ViewBag.Action = "/Unit/Create";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                // считываем список групп - используем методы другого класса
                UnitCategoryRepository repoGroup = new UnitCategoryRepository(repo.GetConnection()); // TODO: обработать на ошибку
                ViewBag.UnitGroups = await repoGroup.GetCategoriesAsync(loginID);
                // новое измерение
                Unit ut= new Unit() { LoginID = loginID };
                return PartialView("UnitEdit", ut);
            }
            else
                return BadRequest("Нет аутентификации!"); // TODO: обработать ошибку аутентификации
        }

        public async Task<ActionResult> Edit(long id)
        {
            ViewBag.Action = "/Unit/Edit";// POST
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    UnitCategoryRepository repoGroup = new UnitCategoryRepository(repo.GetConnection()); // TODO: обработать на ошибку
                    ViewBag.UnitGroups = await repoGroup.GetCategoriesAsync(loginID);
                    return PartialView("UnitEdit", await repo.GetParameterUnitAsync(id, loginID));
                }
                else
                    return BadRequest("Нет аутентификации!");
            }
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}