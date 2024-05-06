using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArabicLearning.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArabicLearning.Controllers
{
    //[Route("/api/*")]
    public class ApiCourses : Controller
    {
        private readonly ICoursesRepository coursesRepo;
        public ApiCourses(ICoursesRepository coursesRepository)
        {
            this.coursesRepo = coursesRepository;
        }
        [HttpGet("api/courses")]
        public IActionResult GetAll()
        {
            return new OkObjectResult(coursesRepo.GetAllCourses());
        }

        [HttpGet("api/courses/{type} Type")]
        public IActionResult GetByType(string type)
        {
            return new OkObjectResult(coursesRepo.GetByType(type));
        }

        [HttpGet("api/courses/{level} Level")]
        public IActionResult GetCoursesOfLevel(string level)
        {
            return new OkObjectResult(coursesRepo.GetByLevel(level));
        }
    }
}
