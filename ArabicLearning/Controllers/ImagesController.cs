using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArabicLearning.Models;
using ArabicLearning.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ArabicLearning.Controllers
{
    [Route("Images")]
    public class ImagesController : Controller
    {
        private readonly IImagesRepository imagesRepo;
        [BindProperty] Image image { get; set; }
        public ImagesController(IImagesRepository imagerepo)
        {
            imagesRepo = imagerepo;
        }

        public IActionResult GetImage(int? id)
        {
            image = new Image();
            if (id == null)
            {
                image = imagesRepo.GetImage(0);
                return View(image);
            }
            
            image = imagesRepo.GetImage((int)id);

            if (image == null) return NotFound();

            return View(image);
        }
    }
}
