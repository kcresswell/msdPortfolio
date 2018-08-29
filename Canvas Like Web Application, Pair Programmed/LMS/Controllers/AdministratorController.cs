using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Administrator")]
  public class AdministratorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Department(string subject)
    {
      ViewData["subject"] = subject;
      return View();
    }

    public IActionResult Course(string subject, string num)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      return View();
    }

    /// <summary>
    /// Returns a JSON array of all the courses in the given department.
    /// Each object in the array should have the following fields:
    /// "number" - The course number (as in 6016 for this course)
    /// "name" - The course name (as in "Database Systems..." for this course)
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns></returns>
    public IActionResult GetCourses(string subject)
    {
        using (Team9Context db = new Team9Context())
        {
            var queryCourses =
                from cr in db.Course
                where cr.DId == subject
                select new { number = cr.CourseNum, name = cr.Name };

            return Json(queryCourses.ToArray());
        }
    }

    /// <summary>
    /// Returns a JSON array of all the professors working in a given department.
    /// Each object in the array should have the following fields:
    /// "lname" - The professor's last name
    /// "fname" - The professor's first name
    /// "uid" - The professor's uid
    /// </summary>
    /// <param name="subject">The department subject abbreviation</param>
    /// <returns></returns>
    public IActionResult GetProfessors(string subject)
    {
        using (Team9Context db = new Team9Context())
        {
            var queryProf =
                from prof in db.Professor
                where prof.Department == subject
                select new { lname = prof.LastName, fname = prof.FirstName, uid = prof.UId};
            return Json(queryProf.ToArray());
        }
    }

    /// <summary>
    /// Creates a course.
    /// </summary>
    /// <param name="subject">The subject abbreviation for the department in which the course will be added</param>
    /// <param name="number">The course number</param>
    /// <param name="name">The course name</param>
    /// <returns>A JSON object containing {success = true/false}. False if the course already exists, true otherwise.</returns>
    public IActionResult CreateCourse(string subject, int number, string name)
    {
        using (Team9Context db = new Team9Context())
        {
            var queryAllCourse =
                from cr in db.Course orderby cr.CatId descending
                select cr;

                int maxCorId = queryAllCourse.ToArray()[0].CatId;

            var queryCourses =
                from cr in db.Course
                where cr.CourseNum == number && cr.Name == name && cr.DId == subject
                select new { courseNumber = cr.CourseNum, courseName = cr.Name, courseSubject = cr.DId };

            if (queryCourses.Count() == 0)
            {
                Course newCourse = new Course();
                newCourse.DId = subject;
                newCourse.CourseNum = number;
                newCourse.Name = name;
                newCourse.CatId = maxCorId + 1;

                db.Course.Add(newCourse);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Problem saving changes: " + e);
                }


                    return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }

        /// <summary>
        /// Creates a class offering of a given course.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="number">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="start">The start time</param>
        /// <param name="end">The end time</param>
        /// <param name="location">The location</param>
        /// <param name="instructor">The uid of the professor</param>
        /// <returns>A JSON object containing {success = true/false}. 
        ///     False if another class occupies the same location during any time within the start-end range in the same semester.</returns>
        public IActionResult CreateClass(string subject, int number, string season, int year, DateTime start, DateTime end, string location, string instructor)
        {
            using (Team9Context db = new Team9Context())
            {
                var queryAllClasses =
                    from cl in db.Class
                    orderby cl.ClassId descending
                    select cl;

                int maxClassID = queryAllClasses.ToArray()[0].ClassId;

                var queryClass =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId

                    ////////////////////////////////////////////////////////// fix timing for class time range
                    where cr.DId == subject && cr.CourseNum == number && cl.Season == season && cl.Year == year && cl.Location == location
                        && ((cl.StartTime <= start.TimeOfDay && cl.EndTime  >= end.TimeOfDay) //overlap fully
                            //|| (cl.StartTime >= start.TimeOfDay && cl.EndTime <= end.TimeOfDay) // inside fully
                            || (cl.EndTime > start.TimeOfDay && cl.EndTime <= end.TimeOfDay) // end in middle
                            || (cl.StartTime > start.TimeOfDay && cl.StartTime < end.TimeOfDay) // start in middle
                            )
                    select cl;

                var queryCourse =
                    from cr in db.Course
                    where cr.DId == subject && cr.CourseNum == number
                    select cr.CatId;


                if (queryClass.Count() == 0)
                {
                    Class newClass = new Class();
                    newClass.Season = season;
                    newClass.Year = year;
                    newClass.StartTime = start.TimeOfDay;
                    newClass.EndTime = end.TimeOfDay;
                    newClass.Instructor = instructor;
                    newClass.Location = location;
                    newClass.ClassId = maxClassID + 1;
                    newClass.CatId = queryCourse.ToArray()[0];

                    db.Class.Add(newClass);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Problem saving changes: " + e);
                    }


                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
        }

  }
}