using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using Newtonsoft.Json;

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

        public IActionResult Create(long id)
        {
            ViewBag.Action = "/Unit/Create";
            ViewBag.Mode = "new";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                // считываем список групп - используем методы другого класса
                //UnitCategoryRepository repoGroup = new UnitCategoryRepository(repo.GetConnection()); // TODO: обработать на ошибку
                //ViewBag.UnitGroups = await repoGroup.GetCategoriesAsync(loginID);
                // новое измерение
                Unit ut= new Unit() { LoginID = loginID };
                return PartialView("UnitEdit", ut);
            }
            else
                return BadRequest("Нет аутентификации!"); // TODO: обработать ошибку аутентификации
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Unit ut)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    long loginID = LoginController.GetLogin(HttpContext.User);
                    if (loginID >= 0)
                    {
                        ut.LoginID = loginID;
                        try
                        {
                            await repo.SetUnitAsync(ut, 0);
                            string jsn = JsonConvert.SerializeObject(ut);
                            return Ok(jsn); // ajax диалог просто пустая строка
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("Нет аутентификации");
                    }
                }
                else
                {
                    return BadRequest("Нет аутентификации");
                }
            }
            else
            {
                return BadRequest("Не все обязательные поля заполнены!");
            }
        }

        public async Task<ActionResult> Edit(long id)
        {
            ViewBag.Action = "/Unit/Edit";// POST
            ViewBag.Mode = "edit";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    //UnitCategoryRepository repoGroup = new UnitCategoryRepository(repo.GetConnection()); // TODO: обработать на ошибку
                    //ViewBag.UnitGroups = await repoGroup.GetCategoriesAsync(loginID);
                    return PartialView("UnitEdit", await repo.GetParameterUnitAsync(id, loginID));
                }
                else
                    return BadRequest("Нет аутентификации!");
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Unit ut)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    try
                    {
                        long loginID = LoginController.GetLogin(HttpContext.User);
                        if (loginID >= 0)
                        {
                            ut.LoginID = loginID;
                            await repo.SetUnitAsync(ut, 1);
                            string jsn = JsonConvert.SerializeObject(ut);
                            return Ok(jsn); // ajax диалог просто пустая строка
                        }
                        else
                        {
                            return BadRequest("Нет аутентификации");
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    return BadRequest("Нет аутентификации");
                }
            }
            else
            {
                return BadRequest("Не все обязательные поля заполнены!");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {

                long lgID = LoginController.GetLogin(HttpContext.User);
                if (lgID >= 0)
                {
                    Unit ut = new Unit { UnitID = id, LoginID = lgID };
                    await repo.SetUnitAsync(ut, 2);
                    return Ok();
                }
                else
                    return BadRequest("Нет аутентификации!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}