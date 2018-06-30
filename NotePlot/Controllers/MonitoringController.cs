using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;
using NotePlot.Models;
using System.Globalization;

namespace NotePlot.Controllers
{
    public class MonitoringController : Controller
    {
        IRepositoryMonitoring repo;
        public MonitoringController(IRepositoryMonitoring r)
        {
            repo = r;
        }

        //Get
        /*
        public IActionResult MonitoringList(long id)
        {
            //IHeaderDictionary headersDictionary = HttpContext.Request.Headers;
            //string urlReferrer = headersDictionary[HeaderNames.Referer].ToString();
            //string url = "/" + RouteData.Values["Controller"] + "/" + RouteData.Values["Action"] + "/" + RouteData.Values["id"];
            long loginID = LoginController.GetLogin(HttpContext.User); // для настроек по каждому логину в будущем
            MonitoringFilter mf = repo.GetMonitoringFilter(id, loginID);
            return View("MonitoringList", mf);
        }
        */
        public async Task<IActionResult> MonitoringList(long id)
        {
            long loginID = LoginController.GetLogin(HttpContext.User); // для настроек по каждому логину в будущем
            MonitoringFilter mf = await repo.GetMonitoringFilterAsync(id, loginID);
            return View("MonitoringList", mf);
        }
        /*
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult MonitorMonitoringList(MonitoringFilter mf)
        {           
            //IHeaderDictionary headersDictionary = HttpContext.Request.Headers;
            //string urlReferrer = headersDictionary[HeaderNames.Referer].ToString();
            //string url = "/" + RouteData.Values["Controller"] + "/" + RouteData.Values["Action"] + "/" + RouteData.Values["id"];
            return View("Views/Shared/Components/MonitorMonitorings/MonitorMonitoringList.cshtml", repo.GetMonitorings(mf));
        }
        */

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> MonitorMonitoringList(MonitoringFilter mf)
        {
            return View("Views/Shared/Components/MonitorMonitorings/MonitorMonitoringList.cshtml", await repo.GetMonitoringsAsync(mf));
        }

        //Get
        //id - MonitorID
        /*
        public IActionResult MonitoringNew(long id)
        {            
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                // считываем название монитора - используем методы другого класса
                RepositoryMonitor repoMon = new RepositoryMonitor((repo as RepositoryMonitoring).GetConnection()); // TODO: обработать на ошибку
                Monitor mr = repoMon.GetMonitor(id, loginID);
                // новое измерение
                Monitoring mt = new Monitoring() { MonitorID = id, MonitorShortName = mr.MonitorShortName };
                return View("MonitoringEdit", mt);
            }
            else
                return BadRequest("Нет аутентификации!"); // TODO: обработать ошибку аутентификации
        }
        */
        //id - MonitorID
        public async Task<IActionResult> MonitoringNew(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                // считываем название монитора - используем методы другого класса
                RepositoryMonitor repoMon = new RepositoryMonitor((repo as RepositoryMonitoring).GetConnection()); // TODO: обработать на ошибку
                Monitor mr = await repoMon.GetMonitorAsync(id, loginID);
                // новое измерение
                Monitoring mt = new Monitoring() { MonitorID = id, MonitorShortName = mr.MonitorShortName };
                return View("MonitoringEdit", mt);
            }
            else
                return BadRequest("Нет аутентификации!"); // TODO: обработать ошибку аутентификации
        }

        //id - MonitoringID
        /*
        public IActionResult MonitoringEdit(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Monitoring mt = repo.GetMonitoring(id);
                System.Globalization.CultureInfo culture = CultureInfo.CurrentCulture;                
                mt.MonitoringDateDt = mt.MonitoringDate.ToString("d", culture);
                mt.MonitoringDateTm = mt.MonitoringDate.ToString("t", culture);

                return View("MonitoringEdit", mt);
            }
            else
                return BadRequest("Нет аутентификации!"); // TODO: обработать ошибку аутентификации
        }
        */
        public async Task<IActionResult> MonitoringEdit(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                Monitoring mt = await repo.GetMonitoringAsync(id);
                System.Globalization.CultureInfo culture = CultureInfo.CurrentCulture;
                mt.MonitoringDateDt = mt.MonitoringDate.ToString("d", culture);
                mt.MonitoringDateTm = mt.MonitoringDate.ToString("t", culture);

                return View("MonitoringEdit", mt);
            }
            else
                return BadRequest("Нет аутентификации!"); // TODO: обработать ошибку аутентификации
        }
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MonitoringEditJson(Monitoring mr)
        {
            //Parameter pr = new Parameter();
            //return Ok();
            System.Globalization.CultureInfo culture = CultureInfo.CurrentCulture;
            DateTime dateResult;
            if (DateTime.TryParse(mr.MonitoringDateDt + " " + mr.MonitoringDateTm, culture, DateTimeStyles.None, out dateResult))
                // преобразование строки в DateTime
                mr.MonitoringDate = dateResult;
            else
            {
                return BadRequest("Неверный формат даты и времени!");
            }

            if (mr.JSON != null)
            {
                mr.JSON = mr.JSON.Substring(1);
                mr.JSON = mr.JSON.Substring(0, mr.JSON.Length - 1); 
            }
            //Parameter pr = JsonConvert.DeserializeObject<Parameter>(JSON);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {                    
                    if (ModelState.IsValid)
                    {
                        int md = (mr.MonitoringID == null) ? 0 : 1;
                        try
                        {
                            repo.SetMonitoring(mr, md);
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
        public async Task<ActionResult> MonitoringEditJson(Monitoring mr)
        {
            //Parameter pr = new Parameter();
            //return Ok();
            System.Globalization.CultureInfo culture = CultureInfo.CurrentCulture;
            DateTime dateResult;
            if (DateTime.TryParse(mr.MonitoringDateDt + " " + mr.MonitoringDateTm, culture, DateTimeStyles.None, out dateResult))
                // преобразование строки в DateTime
                mr.MonitoringDate = dateResult;
            else
            {
                return BadRequest("Неверный формат даты и времени!");
            }
            /*
            if (mr.JSON != null)
            {
                mr.JSON = mr.JSON.Substring(1);
                mr.JSON = mr.JSON.Substring(0, mr.JSON.Length - 1);
            }
            */
            //Parameter pr = JsonConvert.DeserializeObject<Parameter>(JSON);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    if (ModelState.IsValid)
                    {
                        int md = (mr.MonitoringID == null) ? 0 : 1;
                        try
                        {
                            await repo.SetMonitoringAsync(mr, md);
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
        public ActionResult MonitoringDelete(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    try
                    {
                        repo.DeleteMonitoring(id);
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
        public async Task<ActionResult> MonitoringDelete(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    try
                    {
                        await repo.DeleteMonitoringAsync(id);
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

        public IActionResult MonitoringDeleteByDate(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return PartialView("MonitoringsDelete", id);
                
            }
            else
                return BadRequest("Нет аутентификации!"); // TODO: обработать ошибку аутентификации
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MonitoringDeleteByDate(MonitoringFilter mf)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                if (loginID >= 0)
                {
                    try
                    {
                        var dRows = await repo.DeleteMonitoringByDateAsync(mf);
                        string dMes = String.Format("Удалено измерений: {0}", dRows);
                        return Ok(dMes); // ajax диалог просто пустая строка
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