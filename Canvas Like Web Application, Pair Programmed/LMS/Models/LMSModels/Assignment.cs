using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Assignment
    {
        public Assignment()
        {
            Submission = new HashSet<Submission>();
        }

        public int AId { get; set; }
        public string Name { get; set; }
        public string Contents { get; set; }
        public sbyte? HandinType { get; set; }
        public DateTime? DueDateTime { get; set; }
        public int? Points { get; set; }
        public int Category { get; set; }

        public AssignmentCategory CategoryNavigation { get; set; }
        public ICollection<Submission> Submission { get; set; }
    }
}
