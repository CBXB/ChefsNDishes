using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChefsNDishes.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ChefsNDishes.Controllers
{
    public class HomeController : Controller
    {
        private ChefDishContext DbContext { get; set; }
        public HomeController(ChefDishContext context)
        {
            DbContext = context;
        }

        // http://localhost:5000/
        // The method below query all chefs to be display in the above link
        [HttpGet("")]
        public IActionResult Index()
        {
            List<Chef> AllChefs = DbContext.Chefs.Include(dish => dish.Dishes).ToList();
            ViewBag.allchefs = AllChefs;
            return View();
        }

        // http://localhost:5000/Dishes
        // The method below query all dishes to be display in the above link
        [HttpGet("Dishes")]
        public IActionResult Dishes()
        {
            List<Dish> AllDishes = DbContext.Dishes.Include(chef => chef.Creator).ToList();
            ViewBag.alldishes = AllDishes;
            return View("Dishes");
        }

        // http://localhost:5000/AddChefView
        [HttpGet("AddChefView")]
        public IActionResult AddChefView()
        {
            return View("AddChef");
        }

        [HttpPost("AddChef")]
        public IActionResult AddChef(Chef chef)
        {
            if(ModelState.IsValid)
            {
                if(chef.Birthday >= DateTime.Today)
                {
                    ModelState.AddModelError("Birthday", "Birthday must be from the past!");
                    return View("AddChef");
                }
                Chef newChef = new Chef
                {
                    FirstName = chef.FirstName,
                    LastName = chef.LastName,
                    Birthday = chef.Birthday,
                };
                DbContext.Add(newChef);
                DbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View("AddChef");
            }
        }

        [HttpGet("AddDishView")]
        public IActionResult AddDishView()
        {
            List<Chef> AllChefs = DbContext.Chefs.ToList();
            ViewBag.allchefs = AllChefs;
            return View("AddDish");
        }

        [HttpPost("AddDish")]
        public IActionResult AddDish(Dish dish)
        {
            if(ModelState.IsValid)
            {
                DbContext.Add(dish);
                DbContext.SaveChanges();
                return RedirectToAction("AddDishView");
            }
            else
            {
                List<Chef> AllChefs = DbContext.Chefs.ToList();
                ViewBag.allchefs = AllChefs;
                return View("AddDish", dish);
            }
        }

// ==================================================================================

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
