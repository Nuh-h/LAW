using ArabicLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArabicLearning.Repositories
{
    public interface IImagesRepository
    {
        IEnumerable<Image> GetAll();
        Image GetImage(int Id);
    }
}
