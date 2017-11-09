using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;
using System.IO;
using System.Text; // encoding
using Newtonsoft.Json;

namespace NotePlot.Controllers
{
    public class ParameterController : Controller
    {
        IRepositoryParameter repo;
        public ParameterController(IRepositoryParameter r)
        {
            repo = r;
        }

        // GET: Create
        public ActionResult New()
        {
            long? loginID = null;
            ViewBag.Action = "/Parameter/Edit";
            ViewBag.ListType = ParameterType.ParameterTypeList; // для отображения типа параметра
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                loginID = LoginController.GetLogin(HttpContext.User);
            }
            Parameter pr = new Parameter() { ParameterTypeID = 0, LoginID = loginID };// {ParameterGroupID = -1, ParameterTypeID = 0, ParameterUnitID = -1, ParameterValueTypeID = -1, LoginID = -1 };
            return View("Edit",pr);
        }

        // GET:
        public ActionResult ParameterList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("ParameterList", repo.GetParameters(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }

        public ActionResult ParameterListDialog(int md = 1)
        {
            if (md == 1)
                ViewBag.operations = repo.GetMathOperations(); // операции
            else
                ViewBag.operations = null;
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ParameterListDialog", repo.GetParameters(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }

        // GET: Parameter/Edit/5
        public ActionResult Edit(long id)
        {            
            ViewBag.Action = "/Parameter/EditJson";
            ViewBag.ListType = ParameterType.ParameterTypeList; // для отображения типа параметра
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                return View("Edit", repo.GetParameter(id, loginID));
            }
            else
                return BadRequest("Нет аутентификации!");
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
                            return Ok(); // ajax диалог просто пустая строка
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
                        //return BadRequest("Не все обязательные поля заполнены!");
                        return BadRequest(errmes);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditJson(Parameter pr)
        {
            //Parameter pr = new Parameter();
           //return Ok();
           if (pr.JSON != null)
            {
                pr.JSON = pr.JSON.Substring(1);
                pr.JSON = pr.JSON.Substring(0, pr.JSON.Length - 1);
            }
            //Parameter pr = JsonConvert.DeserializeObject<Parameter>(JSON);
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
                            return Ok(); // ajax диалог просто пустая строка
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
                        //return BadRequest("Не все обязательные поля заполнены!");
                        return BadRequest(errmes);
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


        // POST: Parameter/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    try
                    {
                        repo.DeleteParameter(id, loginID);
                        return View("ParameterList", repo.GetParameters(loginID));
                        //return Ok(); // ajax диалог просто пустая строка
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

        //загружаем ViewComponent на форме параметра
        public IActionResult LoadRelations(long id)
        {
            //return ViewComponent("ParamRelations", id);
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return ViewComponent("ParamRelations", new { ParameterID = id, LoginID = loginID });
            else
                return BadRequest("Нет аутентификации!");
        }

        // ПАКЕТ
        // GET:
        public ActionResult PacketList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("PacketList", repo.GetPackets(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }

        // GET: Create
        public ActionResult PacketNew()
        {
            long? loginID = null;
            ViewBag.Action = "/Parameter/PacketEditJson";
            //ViewBag.ListType = ParameterType.ParameterTypeList; // для отображения типа параметра
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                loginID = LoginController.GetLogin(HttpContext.User);
            }
            Packet pt = new Packet() {LoginID = loginID };// {ParameterGroupID = -1, ParameterTypeID = 0, ParameterUnitID = -1, ParameterValueTypeID = -1, LoginID = -1 };
            return View("PacketEdit", pt);
        }

        // GET: Parameter/PacketEdit/5
        public ActionResult PacketEdit(long id)
        {
            ViewBag.Action = "/Parameter/PacketEditJson"; // POST
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                return View("PacketEdit", repo.GetPacket(id, loginID));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PacketEditJson(Packet pt)
        {
            //Parameter pr = new Parameter();
            //return Ok();
            if (pt.JSON != null)
            {
                pt.JSON = pt.JSON.Substring(1);
                pt.JSON = pt.JSON.Substring(0, pt.JSON.Length - 1);
            }
            //Parameter pr = JsonConvert.DeserializeObject<Parameter>(JSON);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    pt.LoginID = loginID;
                    if (ModelState.IsValid)
                    {
                        int md = (pt.PacketID == null) ? 0 : 1;
                        try
                        {
                            repo.SetPacket(pt, md);
                            return Ok(); // ajax диалог просто пустая строка
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
                        //return BadRequest("Не все обязательные поля заполнены!");
                        return BadRequest(errmes);
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PacketDelete(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    try
                    {
                        repo.DeletePacket(id, loginID);
                        return View("PacketList", repo.GetPackets(loginID));
                        //return Ok(); // ajax диалог просто пустая строка
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


    }
}