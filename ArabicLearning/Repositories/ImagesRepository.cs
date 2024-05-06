using ArabicLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArabicLearning.Repositories
{
    public class ImagesRepository : IImagesRepository
    {
        public ImagesRepository()
        {
            this.Collection = InMemoryCollection;
        }
        private IEnumerable<Image> Collection { get; }

        public IEnumerable<Image> GetAll()
        {
            return this.Collection;
        }
        public Image GetImage(int Id)
        {
            if (Id < 0 || Id > this.Collection.Count()) return null;

            return this.Collection.SingleOrDefault(p=>p.id==Id);
        }

        private static IEnumerable<Image> InMemoryCollection { get; } = new List<Image>
        {
            new Image {id = 0, author = "Alejandro Escamilla", url="https://picsum.photos/id/0/5616/3744"},
            new Image {id = 1, author = "Alejandro Escamilla", url="https://picsum.photos/id/1/5616/3744"},
            new Image {id = 2, author = "Paul Jarvis", url="https://picsum.photos/id/10/2500/1667"},
            new Image {id = 3, author = "Tina Rataj", url="https://picsum.photos/id/100/2500/1656"},
            new Image {id = 4, author = "Danielle MacInnes", url="https://picsum.photos/id/1001/5616/3744"}
        };
    }
}
