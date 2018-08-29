using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
  public class CommonController : Controller
  {

        // TODO: Add a protected database context variable once you have scaffolded your team database
        // for example: 
        protected Team9Context db;

        public CommonController()
    {
            // TODO: Initialize your context once you have scaffolded your team database
            // for example:
            db = new Team9Context();
    }

        /*
         * WARNING: This is the quick and easy way to make the controller
         *          use a different Context - good enough for our purposes.
         *          The "right" way is through Dependency Injection via the constructor (look this up if interested).
        */

        // TODO: Add a "UseContext" method if you wish to change the "db" context for unit testing
        //       See the lecture on testing

        // TODO: Uncomment this once you have created the variable "db"
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /*******Begin code to modify********/


        /// <summary>
        /// Retreive a JSON array of all departments from the database.
        /// Each object in the array should have a field called "name" and "subject",
        /// where "name" is the department name and "subject" is the subject abbreviation.
        /// </summary>
        /// <returns>The JSON array</returns>
        public IActionResult GetDepartments()
    {
            // TODO: Do not return this hard-coded array.
            using (Team9Context db = new Team9Context())
            {
                var queryDepartments =
                    from d in db.Department
                    select new {name = d.Name, subject = d.Abbreviation};

                return Json(queryDepartments.ToArray());
            }
    }


    /// <summary>
    /// Returns a JSON array representing the course catalog.
    /// Each object in the array should have the following fields:
    /// "subject": The subject abbreviation, (e.g. "CS")
    /// "dname": The department name, as in "Computer Science"
    /// "courses": An array of JSON objects representing the courses in the department.
    ///            Each field in this inner-array should have the following fields:
    ///            "number": The course number (e.g. 6016)
    ///            "cname": The course name (e.g. "Database Systems and Applications")
    /// </summary>
    /// <returns>The JSON array</returns>
    // Example JSON string:
    //[{"subject":"ART","dname":"Art","courses":[{"number":2200,"cname":"Beginning Drawing"},
    //{"number":2060,"cname":"Digital Photography"}]},
    //{"subject":"CS","dname":"Computer Science","courses":[{"number":1410,"cname":"Object-Oriented Programming"},
    //{"number":6016,"cname":"Database Systems and Applications"},
    //{"number":2420,"cname":"Introduction to Algorithms and Data Structures"},
    //{"number":3500,"cname":"Software Practice"},
    //{"number":3810,"cname":"Computer Organization"},
    //{"number":5300,"cname":"Artificial Intelligence"}]}
    public IActionResult GetCatalog()
    {
            using (Team9Context db = new Team9Context())
            {
                //find list of departments
                var queryDeptAbbr =
                    from d in db.Department
                    select new { subject = d.Abbreviation, dname = d.Name };

                //get courses for each department and add to an array same size as number of departments
                object[] queryCourses = new object[queryDeptAbbr.Count()]; 
                for (int i = 0; i < queryDeptAbbr.Count(); i++)
                {
                    var queryCatalog =
                            from d in db.Department
                            join c in db.Course on d.Abbreviation equals c.DId
                            where d.Abbreviation == queryDeptAbbr.ToArray()[i].subject
                            select new { number = c.CourseNum, cname = c.Name };

                    queryCourses[i] = queryCatalog.ToArray(); 
                }

                //make an array of department names, abbrv, and course using two arrays above.
                object[] catalog = new object[queryDeptAbbr.Count()];
                for(int i = 0; i < catalog.Count(); i++)
                {
                    catalog[i] = new { subject = queryDeptAbbr.ToArray()[i].subject, dname = queryDeptAbbr.ToArray()[i].dname, courses = queryCourses[i]};
                }
                  return Json(catalog);
            };
        }

        /// <summary>
        /// Returns a JSON array of all class offerings of a specific course.
        /// Each object in the array should have the following fields:
        /// "season": the season part of the semester, such as "Fall"
        /// "year": the year part of the semester
        /// "location": the location of the class
        /// "start": the start time in format "hh:mm:ss"
        /// "end": the end time in format "hh:mm:ss"
        /// "fname": the first name of the professor
        /// "lname": the last name of the professor
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public IActionResult GetClassOfferings(string subject, int number)
        {
            using (Team9Context db = new Team9Context())
            {
                var queryClass =
                    from cr in db.Course
                    join d in db.Class on cr.CatId equals d.CatId
                    join prof in db.Professor on d.Instructor equals prof.UId
                    where cr.CourseNum == number && subject == cr.DId
                    select new { season = d.Season, year = d.Year, location = d.Location, start = d.StartTime, end = d.EndTime, fname = prof.FirstName, lname = prof.LastName };

                return Json(queryClass.ToArray());
            }
        }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <returns>The assignment contents</returns>
    public IActionResult GetAssignmentContents(string subject, int num, string season, int year, string category, string asgname)
    {
        using (Team9Context db = new Team9Context())
        {
            var queryAssignmentContents =
                from cr in db.Course
                join cl in db.Class on cr.CatId equals cl.CatId
                join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                join a in db.Assignment on ac.CatId equals a.Category
                where cr.DId == subject && cr.CourseNum == num && cl.Season == season && cl.Year == year && ac.Name == category && a.Name == asgname
                select new { content = a.Contents };

                return Content(queryAssignmentContents.ToArray()[0].content); 
        }
    }

    /// <summary>
    /// This method does NOT return JSON. It returns plain text (containing html).
    /// Use "return Content(...)" to return plain text.
    /// Returns the contents of an assignment submission.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment in the category</param>
    /// <param name="uid">The uid of the student who submitted it</param>
    /// <returns>The submission text</returns>
    public IActionResult GetSubmissionText(string subject, int num, string season, int year, string category, string asgname, string uid)
    {
        using (Team9Context db = new Team9Context())
        {
            var queryAssignSubmissionContents =
                from cr in db.Course
                join cl in db.Class on cr.CatId equals cl.CatId into join1
                from j1 in join1.DefaultIfEmpty()

                join ac in db.AssignmentCategory on j1.ClassId equals ac.ClassId into join2
                from j2 in join2.DefaultIfEmpty()

                join a in db.Assignment on j2.CatId equals a.Category into join3
                from j3 in join3.DefaultIfEmpty()

                join sub in db.Submission on j3.AId equals sub.AId into join4
                from j4 in join4.DefaultIfEmpty()

                where cr.DId == subject && cr.CourseNum == num && j1.Season == season && j1.Year == year
                && j2.Name == category && j3.Name == asgname && j4.UId == uid
                select new { content = j4.Contents, uid = j4.UId, assName = j3.Name };

                if (queryAssignSubmissionContents.ToArray().Count() != 0)
                {
                    return Content(queryAssignSubmissionContents.ToArray()[0].content);
                } else
                {
                    return Content("");
                }
                

                //bool subTextAdded = false;

                //for (int i = 0; i < queryAssignSubmissionContents.Count(); i++)
                //{
                //    if (queryAssignSubmissionContents.ToArray()[i].uid.Equals(uid) && queryAssignSubmissionContents.ToArray()[i].assName.Equals(asgname))
                //    {
                //        subTextAdded = true;
                //    }
                //}

                //if (!subTextAdded)
                //{
                //    return Content(queryAssignSubmissionContents.ToArray()[0].content);
                //}
                //else
                //{
                //    return Content("");
                //}
            }
    }


    /// <summary>
    /// Gets information about a user as a single JSON object.
    /// The object should have the following fields:
    /// "fname": the user's first name
    /// "lname": the user's last name
    /// "uid": the user's uid
    /// "department": (professors and students only) the name (such as "Computer Science") of the department for the user. 
    ///               If the user is a Professor, this is the department they work in.
    ///               If the user is a Student, this is the department they major in.    
    ///               If the user is an Administrator, this field is not present in the returned JSON
    /// </summary>
    /// <param name="uid">The ID of the user</param>
    /// <returns>The user JSON object or an object containing {success: false} if the user doesn't exist</returns>
    public IActionResult GetUser(string uid)
    {
            using (Team9Context db = new Team9Context())
            {  
                var queryProfessors =
                    from d in db.Professor
                    join dpt in db.Department on d.Department equals dpt.Abbreviation
                    where d.UId == uid
                    select new { fname = d.FirstName, lname = d.LastName, uid = d.UId, department = dpt.Name};

                var queryStudent =
                    from d in db.Student
                    join mjr in db.Department on d.Major equals mjr.Abbreviation
                    where d.UId == uid
                    select new { fname = d.FirstName, lname = d.LastName, uid = d.UId, department = mjr.Name};

                var queryAdmin =
                    from d in db.Administrator
                    where d.UId == uid
                    select new { fname = d.FirstName, lname = d.LastName, uid = d.UId}; //admin does not have dept or major
                

                if (queryProfessors.Count() != 0) {
                   return Json(queryProfessors.ToArray()[0]);
                }
                else if (queryStudent.Count() != 0) {
                    return Json(queryStudent.ToArray()[0]);
                }
                else if (queryAdmin.Count() != 0) {
                    return Json(queryAdmin.ToArray()[0]);
                }

                return Json(new { success = false });
            }
    }
    /*******End code to modify********/
  }
}