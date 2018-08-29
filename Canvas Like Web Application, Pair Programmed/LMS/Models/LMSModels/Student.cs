using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Student
    {
        public Student()
        {
            Enrolled = new HashSet<Enrolled>();
            Submission = new HashSet<Submission>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Major { get; set; }

        public DateTime DOB { get; set; }

        public Department MajorNavigation { get; set; }
        public ICollection<Enrolled> Enrolled { get; set; }
        public ICollection<Submission> Submission { get; set; }
    }
}
