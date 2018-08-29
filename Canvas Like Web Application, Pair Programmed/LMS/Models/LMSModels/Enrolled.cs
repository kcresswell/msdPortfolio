using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Enrolled
    {
        public string Grade { get; set; }
        public string UId { get; set; }
        public int ClassId { get; set; }

        public Class Class { get; set; }
        public Student U { get; set; }
    }
}
