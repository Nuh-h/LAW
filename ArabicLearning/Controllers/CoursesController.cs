using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArabicLearning.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArabicLearning.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICoursesRepository coursesRepo;
        public CoursesController(ICoursesRepository Courses)
        {
            this.coursesRepo = Courses;
        }
        public IActionResult Index()
        {
            return View(coursesRepo.GetAllCourses());
        }
        
        [HttpGet("Courses/New")]
        public IActionResult NewCourses()
        {
            ViewData["Title"] = "New Courses";
            return View("~/Views/Courses/CategoriesTemplate.cshtml", coursesRepo.GetAllNew());
        }

        [HttpGet("Courses/Popular")]
        public IActionResult PopularCourses()
        {
            ViewData["Title"] = "Popular Courses";
            return View("~/Views/Courses/CategoriesTemplate.cshtml", coursesRepo.GetAllPopular());
        }


        [HttpGet("Courses/{type} Type")]
        public IActionResult CoursesOfType(string type)
        {
            ViewData["Title"] = type+" Courses";
            return View("~/Views/Courses/CategoriesTemplate.cshtml", coursesRepo.GetByType(type));
        }

        [HttpGet("Courses/{level} Level")]
        public IActionResult CoursesOfLevel(string level)
        {
            ViewData["Title"] = level+" Courses";
            System.Diagnostics.Debug.Write(coursesRepo.GetByLevel(level));
            return View("~/Views/Courses/CategoriesTemplate.cshtml", coursesRepo.GetByLevel(level));
        }

        [HttpGet("Courses/{id}/Detail")]
        public IActionResult CourseDetails(int id)
        {
            return View(coursesRepo.GetFullCourse(id));
        }

    }
}
