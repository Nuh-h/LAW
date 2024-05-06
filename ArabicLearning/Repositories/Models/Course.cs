using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArabicLearning.Repositories.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Teacher { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        IEnumerable<String> Students { get; set; }

    }
}
