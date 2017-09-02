using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

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
        public ActionResult ListDialog()
        {
            long loginID = LoginController.GetLogin(HttpContext.User);
            if (loginID >= 0)
                return PartialView("ListDialog", repo.GetParameterUnits(loginID));
            else
                return BadRequest("��� ��������������!");
        }

    }
}