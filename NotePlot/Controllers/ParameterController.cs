using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Controllers
{
    public class ParameterController : Controller
    {
        IRepositoryParameter repo;
        public ParameterController(IRepositoryParameter r)
        {
            repo = r;
        }

        // GET: ParameterGroup/Create
        public ActionResult New()
        {
            long? loginID = null;
            ViewBag.Action = "/Parameter/Edit";
            ViewBag.ListType = ParameterType.ParameterTypeList; // ��� ����������� ���� ���������
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                loginID = LoginController.GetLogin(HttpContext.User);
            }
            Parameter pr = new Parameter() { ParameterTypeID = 0, LoginID = loginID };// {ParameterGroupID = -1, ParameterTypeID = 0, ParameterUnitID = -1, ParameterValueTypeID = -1, LoginID = -1 };
            return View("Edit",pr);
        }

        // GET: Parameter/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Parameter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Parameter pr)
        {            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    pr.LoginID = loginID;
                    if (ModelState.IsValid)
                    {
                        int md = (pr.ParameterID == null) ? 0 : 1;
                        try
                        {
                            repo.SetParameter(pr, md);
                            return Ok(); // ajax ������ ������ ������ ������
                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message);
                        }
                    }
                    else
                    {
                        string errmes = string.Empty;
                        foreach (var error in ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors))
                        {
                            errmes = errmes + error.ErrorMessage + ' ';
                        }
                        //return BadRequest("�� ��� ������������ ���� ���������!");
                        return BadRequest(errmes);
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

        // GET: Parameter/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Parameter/Delete/5
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

        //��������� ViewComponent �� ����� ���������
        public IActionResult LoadRelations(long id)
        {
            return ViewComponent("ParamRelations", id);
        }
    }
}