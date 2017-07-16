using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

namespace NotePlot.Controllers
{
    public class ParamValueTypeController : Controller
    {
        IRepositoryParamValueType repo;
        public ParamValueTypeController(IRepositoryParamValueType r)
        {
            repo = r;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View("ParamValueTypeView",repo.GetParamValueTypes());
        }
    }
}