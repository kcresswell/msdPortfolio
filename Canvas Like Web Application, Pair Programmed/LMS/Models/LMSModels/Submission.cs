using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public DateTime Time { get; set; }
        public int Score { get; set; }
        public string Contents { get; set; }
        public string UId { get; set; }
        public int AId { get; set; }

        public Assignment A { get; set; }
        public Student U { get; set; }
    }
}
