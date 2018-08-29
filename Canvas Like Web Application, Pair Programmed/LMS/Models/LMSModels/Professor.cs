using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Professor
    {
        public Professor()
        {
            Class = new HashSet<Class>();
        }

        public string UId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string Department { get; set; }

        public DateTime DOB { get; set; }

        public Department DepartmentNavigation { get; set; }
        public ICollection<Class> Class { get; set; }
    }
}
