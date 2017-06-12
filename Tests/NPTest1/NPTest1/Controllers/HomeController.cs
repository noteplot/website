using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NPTest1.Models;
using Microsoft.AspNetCore.Http;

namespace NPTest1.Controllers
{
    public class HomeController : Controller
    {
        /* Entity Framework 
        NPContext db;
        public HomeController(NPContext context)
        {
            db = context;
        }
        public IActionResult Index()
        {
            return View(db.Test_Parameters.ToList());
        }
        */
        IRepositoryParameter repo;
        public HomeController(IRepositoryParameter r)
        {
            repo = r;
        }
        public ActionResult Index()
        {            
            //return View(repo.GetParameters());
            return View("Index",repo.GetParameters());
        }

        /*
        public ActionResult Details(int id)
        {
            User user = repo.Get(id);
            if (user != null)
                return View(user);
            return NotFound();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(User user)
        {
            repo.Create(user);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            User user = repo.Get(id);
            if (user != null)
                return View(user);
            return NotFound();
        }
        */

        [HttpGet]
        public ActionResult Edit(int id)
        {
            Test_Parameter par = repo.Get(id);
            if (par != null)
                return PartialView("Parameter",par);
            return NotFound();
        }

        [HttpGet]
        public ActionResult EditValue(int id)
        {
            Test_Parameter par = repo.Get(id);
            if (par != null)
                return PartialView("ParameterValue", par);
            return NotFound();
        }


        [HttpPost]
        public ActionResult Edit(Test_Parameter par)
        {
            /* ИЗВРАТ
            if (HttpContext.Request.Form["IsNegative"] == "on")
                par.IsNegative = true;
            else
                par.IsNegative = false;
            */
            try {
                repo.Update(par);
            }
            catch
            {
                return BadRequest("Проверка не прошла!");
            }
            //return BadRequest("Проверка не прошла!");
            return Ok();                       
        }

        /*
        public void Edit(Test_Parameter par)
        {
            repo.Update(par);
            //return RedirectToAction("Index");
        }
        
        public EmptyResult Edit(Test_Parameter par)
        {
            repo.Update(par);
            return new EmptyResult();
            //return RedirectToAction("Index");
        }
        */

        // имитация редактирования значения параметра
        [HttpPost]
        public ActionResult EditValue(Test_Parameter par)
        {
            try
            {
                repo.UpdateValue(par);
            }
            catch
            {
                return BadRequest("Ошибка записи!");
            }            
            //var s = "{" + $"\"ParameterID\":\"{par.ParameterID}\",\"ParameterValue\":\"{par.ParameterValue}\"" + "}";
         // в виде JSON
         return Content("{" + $"\"ParameterID\":\"{par.ParameterID}\",\"ParameterValue\":\"{par.ParameterValue}\"" + "}");
            //return Content($"ParameterID: {par.ParameterID}  ParameterValue: {par.ParameterValue}");
        }

        /*   
        [HttpGet]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int id)
        {
            User user = repo.Get(id);
            if (user != null)
                return View(user);
            return NotFound();
        }
        */

        //[HttpPost]
        public ActionResult Delete(int id)
        {
            //var s = Request.Query.FirstOrDefault(p => p.Key == "ParameterID").Value;
            repo.Delete(id);
            return RedirectToAction("Index");
        }
        
    }
}
