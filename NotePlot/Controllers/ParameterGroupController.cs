using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;

namespace NotePlot.Controllers
{
    public class ParameterGroupController : Controller
    {
        IRepositoryParameterGroup repo;
        public ParameterGroupController(IRepositoryParameterGroup r)
        {
            repo = r;
        }

        // GET: ParameterGroup
        public ActionResult List()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                    return View("List", repo.GetParameterGroups(loginID));
                else
                    return BadRequest("Нет аутентификации!");
        }

        // GET: ParameterGroup/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ParameterGroup/Create
        public ActionResult Create()
        {
           ViewBag.Action = "/ParameterGroup/Create";
           ParameterGroup pg = new ParameterGroup { ParameterGroupID = -1,LoginID = -1 };
           return PartialView("Edit",pg);
        }

        // POST: ParameterGroup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParameterGroup pg /*IFormCollection collection*/)
        {
            if (ModelState.IsValid)
            {

                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    long loginID = LoginController.GetLogin(HttpContext.User);
                    if (loginID >= 0)
                    {
                        pg.LoginID = loginID;
                        try
                        {
                            repo.SetParameterGroup(pg, 0);
                            return Ok(); // ajax диалог просто пустая строка
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

        // GET: ParameterGroup/Edit/5
        public ActionResult Edit(long id)
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0) {
                ViewBag.Action = "/ParameterGroup/Edit";
                return PartialView(repo.GetParameterGroup(id, loginID));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ParameterGroup pg /*IFormCollection collection*/)
        {
            if (ModelState.IsValid)
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    long loginID = LoginController.GetLogin(HttpContext.User);
                    if (loginID >= 0)
                    {
                        pg.LoginID = loginID;
                        try
                        {
                            repo.SetParameterGroup(pg, 1);
                            return Ok(); // ajax диалог просто пустая строка
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


        // POST: ParameterGroup/Delete/5
        /*
        [HttpGet]
        public ActionResult Delete(long id)
        {
            long lgID = LoginController.GetLogin(HttpContext.User);
            if (lgID >= 0)
            {
                ParameterGroup pg = new ParameterGroup { ParameterGroupID = id, LoginID = lgID };
                repo.SetParameterGroup(pg, 2);
                return Ok();
            }
            else
                return BadRequest("Нет аутентификации!");
        }
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            long lgID = LoginController.GetLogin(HttpContext.User);
            if (lgID >= 0)
            {
                ParameterGroup pg = new ParameterGroup { ParameterGroupID = id, LoginID = lgID };
                repo.SetParameterGroup(pg, 2);
                return Ok();
            }
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}