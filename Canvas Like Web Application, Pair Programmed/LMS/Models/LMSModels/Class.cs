using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Class
    {
        public Class()
        {
            AssignmentCategory = new HashSet<AssignmentCategory>();
            Enrolled = new HashSet<Enrolled>();
        }

        public int ClassId { get; set; }
        public int Year { get; set; }
        public string Season { get; set; }
        public string Location { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Instructor { get; set; }
        public int CatId { get; set; }

        public Course Cat { get; set; }
        public Professor InstructorNavigation { get; set; }
        public ICollection<AssignmentCategory> AssignmentCategory { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
    }
}
