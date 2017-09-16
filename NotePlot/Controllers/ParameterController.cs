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
            ViewBag.Action = "/Parameter/New";
            ViewBag.ListType = ParameterType.ParameterTypeList; // для отображения типа параметра
            Parameter pr = new Parameter { ParameterID = -1, ParameterGroupID = -1, ParameterTypeID = 0, ParameterUnitID = -1, ParameterValueTypeID = -1, LoginID = -1 };
            return View("Edit",pr);
        }

        // POST: Parameter/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Parameter/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Parameter/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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

        //загружаем ViewComponent на форме параметра
        public IActionResult LoadRelations(long id)
        {
            return ViewComponent("ParamRelations", id);
        }
    }
}