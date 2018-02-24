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

        // GET: ParameterGroup - ������ ������ ������
        /*
        public ActionResult ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", repo.GetParameterUnits(loginID));
            else
                return BadRequest("��� ��������������!");
        }
        */
        public async Task<ActionResult> ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", await repo.GetParameterUnitsAsync(loginID));
            else
                return BadRequest("��� ��������������!");
        }

        public async Task<IActionResult> UnitList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("UnitList", await repo.GetParameterUnitsAsync(loginID));
            else
                return BadRequest("��� ��������������!");
        }

        public IActionResult Create(long id)
        {
            ViewBag.Action = "/Unit/Create";
            ViewBag.Mode = "new";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                // ��������� ������ ����� - ���������� ������ ������� ������
                //UnitCategoryRepository repoGroup = new UnitCategoryRepository(repo.GetConnection()); // TODO: ���������� �� ������
                //ViewBag.UnitGroups = await repoGroup.GetCategoriesAsync(loginID);
                // ����� ���������
                Unit ut= new Unit() { LoginID = loginID };
                return PartialView("UnitEdit", ut);
            }
            else
                return BadRequest("��� ��������������!"); // TODO: ���������� ������ ��������������
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

        public async Task<ActionResult> Edit(long id)
        {
            ViewBag.Action = "/Unit/Edit";// POST
            ViewBag.Mode = "edit";
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    //UnitCategoryRepository repoGroup = new UnitCategoryRepository(repo.GetConnection()); // TODO: ���������� �� ������
                    //ViewBag.UnitGroups = await repoGroup.GetCategoriesAsync(loginID);
                    return PartialView("UnitEdit", await repo.GetParameterUnitAsync(id, loginID));
                }
                else
                    return BadRequest("��� ��������������!");
            }
            else
                return BadRequest("��� ��������������!");
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
                            return Ok(jsn); // ajax ������ ������ ������ ������
                        }
                        else
                        {
                            return BadRequest("��� ��������������");
                        }
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
                return BadRequest("�� ��� ������������ ���� ���������!");
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
                    return BadRequest("��� ��������������!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}