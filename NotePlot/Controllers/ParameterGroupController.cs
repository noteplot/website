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
            return View("List", repo.GetParameterGroups());
        }

        // GET: ParameterGroup/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ParameterGroup/Create
        public ActionResult Create()
        {
            ParameterGroup pg = new ParameterGroup { ParameterGroupID = -1,LoginID = -1 };
            return PartialView(pg);
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
                    Claim claimLoginId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "LoginID");
                    if (claimLoginId != null)
                    {
                        pg.LoginID = Convert.ToInt64(claimLoginId.Value);
                        try
                        {
                            repo.SetParameterGroup(pg);
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
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ParameterGroup/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ParameterGroup/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ParameterGroup/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}