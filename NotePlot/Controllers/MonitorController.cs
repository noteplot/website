using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NotePlot.Models;


namespace NotePlot.Controllers
{
    public class MonitorController : Controller
    {
        IRepositoryMonitor repo;
        public MonitorController(IRepositoryMonitor r)
        {
            repo = r;
        }

        // GET:
        /*
        public ActionResult MonitorList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return View("MonitorList", repo.GetMonitors(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }
        */
        public async Task<ActionResult> MonitorList()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
            {
                return View("MonitorList", await repo.GetMonitorsAsync(loginID));
            }
            else
                return BadRequest("Нет аутентификации!");
        }

        // GET: Create
        public ActionResult MonitorNew()
        {
            long? loginID = null;
            ViewBag.Action = "/Monitor/MonitorEditJson";
            //ViewBag.ListType = ParameterType.ParameterTypeList; // для отображения типа параметра
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                loginID = LoginController.GetLogin(HttpContext.User);
                ViewBag.LoginID = loginID;
            }
            Monitor mt = new Monitor() { LoginID = loginID };// {ParameterGroupID = -1, ParameterTypeID = 0, ParameterUnitID = -1, ParameterValueTypeID = -1, LoginID = -1 };
            return View("MonitorEdit", mt);
        }

        // GET: Monitor/PacketEdit/5
        /*
        public ActionResult MonitorEdit(long id)
        {
            ViewBag.Action = "/Monitor/MonitorEditJson";// POST
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                return View("MonitorEdit", repo.GetMonitor(id, loginID));
            }
            else
                return BadRequest("Нет аутентификации!");
        }
        */

        public async Task<ActionResult> MonitorEdit(long id)
        {
            ViewBag.Action = "/Monitor/MonitorEditJson";// POST
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                ViewBag.LoginID = loginID;
                return View("MonitorEdit", await repo.GetMonitorAsync(id, loginID));
            }
            else
                return BadRequest("Нет аутентификации!");
        }
        /*    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonitorEditJson(Monitor mt)
        {
            //Parameter pr = new Parameter();
            //return Ok();
            if (mt.JSON != null)
            {
                mt.JSON = mt.JSON.Substring(1);
                mt.JSON = mt.JSON.Substring(0, mt.JSON.Length - 1);
            }
            //Parameter pr = JsonConvert.DeserializeObject<Parameter>(JSON);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    mt.LoginID = loginID;
                    if (ModelState.IsValid)
                    {
                        int md = (mt.MonitorID == null) ? 0 : 1;
                        try
                        {
                            repo.SetMonitor(mt, md);
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
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MonitorEditJson(Monitor mt)
        {
            //Parameter pr = new Parameter();
            //return Ok();
            /*
            if (mt.JSON != null)
            {
                mt.JSON = mt.JSON.Substring(1);
                mt.JSON = mt.JSON.Substring(0, mt.JSON.Length - 1);
            }
            */
            //Parameter pr = JsonConvert.DeserializeObject<Parameter>(JSON);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    mt.LoginID = loginID;
                    if (ModelState.IsValid)
                    {
                        int md = (mt.MonitorID == null) ? 0 : 1;
                        try
                        {
                            await repo.SetMonitorAsync(mt, md);
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
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonitorDelete(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    try
                    {
                        repo.DeleteMonitor(id, loginID);
                        //return View("MonitorList", repo.GetMonitors(loginID));
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
        */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MonitorDelete(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    try
                    {
                        await repo.DeleteMonitorAsync(id, loginID);
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

        public async Task<IActionResult> SelectMonitor()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("MonitorSelect", await repo.GetMonitorsAsync(loginID));
            else
                return BadRequest("Нет аутентификации!");
        }

    }
}