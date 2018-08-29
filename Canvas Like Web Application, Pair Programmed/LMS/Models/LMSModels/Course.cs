using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Class = new HashSet<Class>();
        }

        public int CatId { get; set; }
        public string Name { get; set; }
        public int CourseNum { get; set; }
        public string DId { get; set; }

        public Department D { get; set; }
        public ICollection<Class> Class { get; set; }
    }
}
