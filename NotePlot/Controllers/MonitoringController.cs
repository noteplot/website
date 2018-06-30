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
            long loginID = LoginController.GetLogin(HttpContext.User); // ��� �������� �� ������� ������ � �������
            MonitoringFilter mf = repo.GetMonitoringFilter(id, loginID);
            return View("MonitoringList", mf);
        }
        */
        public async Task<IActionResult> MonitoringList(long id)
        {
            long loginID = LoginController.GetLogin(HttpContext.User); // ��� �������� �� ������� ������ � �������
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
                // ��������� �������� �������� - ���������� ������ ������� ������
                RepositoryMonitor repoMon = new RepositoryMonitor((repo as RepositoryMonitoring).GetConnection()); // TODO: ���������� �� ������
                Monitor mr = repoMon.GetMonitor(id, loginID);
                // ����� ���������
                Monitoring mt = new Monitoring() { MonitorID = id, MonitorShortName = mr.MonitorShortName };
                return View("MonitoringEdit", mt);
            }
            else
                return BadRequest("��� ��������������!"); // TODO: ���������� ������ ��������������
        }
        */
        //id - MonitorID
        public async Task<IActionResult> MonitoringNew(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                long loginID = LoginController.GetLogin(HttpContext.User);
                // ��������� �������� �������� - ���������� ������ ������� ������
                RepositoryMonitor repoMon = new RepositoryMonitor((repo as RepositoryMonitoring).GetConnection()); // TODO: ���������� �� ������
                Monitor mr = await repoMon.GetMonitorAsync(id, loginID);
                // ����� ���������
                Monitoring mt = new Monitoring() { MonitorID = id, MonitorShortName = mr.MonitorShortName };
                return View("MonitoringEdit", mt);
            }
            else
                return BadRequest("��� ��������������!"); // TODO: ���������� ������ ��������������
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
                return BadRequest("��� ��������������!"); // TODO: ���������� ������ ��������������
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
                return BadRequest("��� ��������������!"); // TODO: ���������� ������ ��������������
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
                // �������������� ������ � DateTime
                mr.MonitoringDate = dateResult;
            else
            {
                return BadRequest("�������� ������ ���� � �������!");
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
                // �������������� ������ � DateTime
                mr.MonitoringDate = dateResult;
            else
            {
                return BadRequest("�������� ������ ���� � �������!");
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

        public IActionResult MonitoringDeleteByDate(long id)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return PartialView("MonitoringsDelete", id);
                
            }
            else
                return BadRequest("��� ��������������!"); // TODO: ���������� ������ ��������������
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
                        string dMes = String.Format("������� ���������: {0}", dRows);
                        return Ok(dMes); // ajax ������ ������ ������ ������
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

    }
}