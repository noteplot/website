using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotePlot.Controllers
{
    public class UnitCategoryController : Controller
    {
        IUnitCategoryRepository repo;
        public UnitCategoryController(IUnitCategoryRepository repo)
        {
            this.repo = repo;
        }

        // GET: /<controller>/
        //public IActionResult Index() => View(repo.GetCategories());
        //public async Task<IActionResult> Index() => View(await repo.GetCategoriesAsync());
        public async Task<IActionResult> UnitCategoryList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("UnitGroupList", await repo.GetCategoriesAsync(loginID));
            //return View("ParameterList", repo.GetParameters(loginID)); // несинхр
            else
                return BadRequest("Нет аутентификации!");
        }

        public async Task<IActionResult> SelectUnitCategoryList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("UnitGroupSelect", await repo.GetCategoriesAsync(loginID));
            //return View("ParameterList", repo.GetParameters(loginID)); // несинхр
            else
                return BadRequest("Нет аутентификации!");
        }

        public ActionResult Create()
        {
            ViewBag.Action = "/UnitCategory/Create";
            ViewBag.Mode = "new";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    UnitCategory uc = new UnitCategory() { LoginID = loginID };// {ParameterGroupID = -1, ParameterTypeID = 0, ParameterUnitID = -1, ParameterValueTypeID = -1, LoginID = -1 };
                    return PartialView("UnitGroupEdit", uc);
                }
                else
                    return BadRequest("Нет аутентификации!");
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UnitCategory uc)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    long loginID = LoginController.GetLogin(HttpContext.User);
                    if (loginID >= 0)
                    {
                        uc.LoginID = loginID;
                        try
                        {
                            await repo.SetCategoryAsync(uc, 0);
                            string jsn = JsonConvert.SerializeObject(uc);
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

        //public IActionResult Edit(int id) => View(repo.GetCategory(id));
        //public async Task<IActionResult> Edit(int id) => View(await repo.GetCategoryAsync(id));

        public async Task<ActionResult> Edit(long id)
        {
            ViewBag.Action = "/UnitCategory/Edit";// POST
            ViewBag.Mode = "edit";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {                    
                    return PartialView("UnitGroupEdit", await repo.GetCategoryAsync(id, loginID));
                }
                else
                    return BadRequest("Нет аутентификации!");
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UnitCategory uc)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    long loginID = LoginController.GetLogin(HttpContext.User);
                    if (loginID >= 0)
                    {
                        uc.LoginID = loginID;
                        try
                        {
                            await repo.SetCategoryAsync(uc, 1);
                            string jsn = JsonConvert.SerializeObject(uc);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                long lgID = LoginController.GetLogin(HttpContext.User);
                if (lgID >= 0)
                {
                    UnitCategory uc = new UnitCategory { UnitGroupID = id, LoginID = lgID };
                    await repo.SetCategoryAsync(uc, 2);
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

        //public IActionResult EditDialog(int id) => PartialView("Edit", repo.GetCategory(id));
        //public async Task<IActionResult> EditDialog(int id) => PartialView("Edit", await repo.GetCategoryAsync(id));

    }
}
