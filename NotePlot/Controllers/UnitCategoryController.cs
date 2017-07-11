using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NotePlot.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotePlot.Controllers
{
    public class UnitCategoryController : Controller
    {
        IUnitCategoryRepository repo;
        public UnitCategoryController(IUnitCategoryRepository repo)
        {
            this.repo = repo;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View(repo.GetCategories());
        }

        public IActionResult Edit(int id)
        {
            return View(repo.GetCategory(id));
        }
    }
}
