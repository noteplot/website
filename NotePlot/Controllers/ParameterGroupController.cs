using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;

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
        /*
        public ActionResult List()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                    return View("List", repo.GetParameterGroups(loginID));
                else
                    return BadRequest("��� ��������������!");
        }
        */

        public async Task<ActionResult> List()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("List", await repo.GetParameterGroupsAsync(loginID));
            else
                return BadRequest("��� ��������������!");
        }

        // GET: ParameterGroup - ������ ������ ������
        /*
        public ActionResult ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", repo.GetParameterGroups(loginID));
            else
                return BadRequest("��� ��������������!");
        }
        */

        public async Task<ActionResult> ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", await repo.GetParameterGroupsAsync(loginID));
            else
                return BadRequest("��� ��������������!");
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
           ViewBag.Mode = "new";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    ParameterGroup pg = new ParameterGroup { ParameterGroupID = -1, LoginID = -1 };
                    return PartialView("Edit", pg);
                }
                else
                    return BadRequest("��� ��������������!");
            }
            else
                return BadRequest("��� ��������������!");
        }

        // POST: ParameterGroup/Create
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParameterGroup pg )//IFormCollection collection)
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
                            return Ok(); // ajax ������ ������ ������ ������
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("��� ��������������");
                    }
                }
                else
                {
                    return BadRequest("��� ��������������");
                }
            }
            else
            {
                return BadRequest("�� ��� ������������ ���� ���������!");
            }
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ParameterGroup pg)
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
                            await repo.SetParameterGroupAsync(pg, 0);
                            string jsn = JsonConvert.SerializeObject(pg);
                            return Ok(jsn); // ���������� ������
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("��� ��������������");
                    }
                }
                else
                {
                    return BadRequest("��� ��������������");
                }
            }
            else
            {
                return BadRequest("�� ��� ������������ ���� ���������!");
            }
        }

        // GET: ParameterGroup/Edit/5
        /*
        public ActionResult Edit(long id)
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0) {
                ViewBag.Action = "/ParameterGroup/Edit";
                return PartialView(repo.GetParameterGroup(id, loginID));
            }
            else
                return BadRequest("��� ��������������!");
        }
        */

        public async Task<ActionResult> Edit(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    ViewBag.Action = "/ParameterGroup/Edit";
                    ViewBag.Mode = "edit";
                    return PartialView(await repo.GetParameterGroupAsync(id, loginID));
                }
                else
                    return BadRequest("��� ��������������!");
            }
            else
                return BadRequest("��� ��������������!");
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ParameterGroup pg )//IFormCollection collection
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
                            return Ok(); // ajax ������ ������ ������ ������
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("��� ��������������");
                    }
                }
                else
                {
                    return BadRequest("��� ��������������");
                }
            }
            else
            {
                return BadRequest("�� ��� ������������ ���� ���������!");
            }
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ParameterGroup pg)
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
                            await repo.SetParameterGroupAsync(pg, 1);
                            string jsn = JsonConvert.SerializeObject(pg);
                            return Ok(jsn); // ajax ������ ������ ������ ������
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else
                    {
                        return BadRequest("��� ��������������");
                    }
                }
                else
                {
                    return BadRequest("��� ��������������");
                }
            }
            else
            {
                return BadRequest("�� ��� ������������ ���� ���������!");
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
                return BadRequest("��� ��������������!");
        }
        */

        /*
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
                return BadRequest("��� ��������������!");
        }
        */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(long id)
        {
            long lgID = LoginController.GetLogin(HttpContext.User);
            if (lgID >= 0)
            {
                ParameterGroup pg = new ParameterGroup { ParameterGroupID = id, LoginID = lgID };
                try
                {
                    await repo.SetParameterGroupAsync(pg, 2);
                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }                
            }
            else
                return BadRequest("��� ��������������!");
        }
    }
}