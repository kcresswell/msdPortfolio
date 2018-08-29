using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Student")]
  public class StudentController : CommonController
  {

        public IActionResult Index()
        {
          return View();
        }

        public IActionResult Catalog()
        {
          return View();
        }

        public IActionResult Class(string subject, string num, string season, string year)
        {
          ViewData["subject"] = subject;
          ViewData["num"] = num;
          ViewData["season"] = season;
          ViewData["year"] = year;
          return View();
        }

        public IActionResult Assignment(string subject, string num, string season, string year, string cat, string aname)
        {
          ViewData["subject"] = subject;
          ViewData["num"] = num;
          ViewData["season"] = season;
          ViewData["year"] = year;
          ViewData["cat"] = cat;
          ViewData["aname"] = aname;
          return View();
        }


        public IActionResult ClassListings(string subject, string num)
        {
          System.Diagnostics.Debug.WriteLine(subject + num);
          ViewData["subject"] = subject;
          ViewData["num"] = num;
          return View();
        }


        /*******Begin code to modify********/

        /// <summary>
        /// Returns a JSON array of the classes the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "subject" - The subject abbreviation of the class (such as "CS")
        /// "number" - The course number (such as 6016)
        /// "name" - The course name
        /// "season" - The season part of the semester
        /// "year" - The year part of the semester
        /// "grade" - The grade earned in the class, or "--" if one hasn't been assigned
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetMyClasses(string uid)
        {
            using (Team9Context db = new Team9Context())
            {
                var queryStudent =
                    from s in db.Student
                    join en in db.Enrolled on s.UId equals en.UId
                    join cl in db.Class on en.ClassId equals cl.ClassId
                    join cr in db.Course on cl.CatId equals cr.CatId
                    where s.UId == uid
                    select new { subject = cr.DId, number = cr.CourseNum, name = cr.Name, season = cl.Season, year = cl.Year, grade = en.Grade};
               
                //for (int i = 0; i < 10; i++)
                //{
                //    System.Diagnostics.Debug.WriteLine("");
                //}
                //foreach (var item in queryStudent.ToArray())
                //{
                //    System.Diagnostics.Debug.WriteLine(item);
                //}
                //System.Diagnostics.Debug.WriteLine(Json(queryStudent.ToArray()).ToString());
                return Json(queryStudent.ToArray());
            }
        }

        /// <summary>
        /// Returns a JSON array of all the assignments in the given class that the given student is enrolled in.
        /// Each object in the array should have the following fields:
        /// "aname" - The assignment name
        /// "cname" - The category name that the assignment belongs to
        /// "due" - The due Date/Time
        /// "score" - The score earned by the student, or null if the student has not submitted to this assignment.
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="uid"></param>
        /// <returns>The JSON array</returns>
        public IActionResult GetAssignmentsInClass (string subject, int num, string season, int year, string uid)
        {
            //var query = enrolled where e.Student == uid && season, year dept number
            //from a in db.Assignments
            //
            using (Team9Context db = new Team9Context())
            {
                var queryAssignments =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId into join1
                    from j1 in join1.DefaultIfEmpty()

                    join cat in db.AssignmentCategory on j1.ClassId equals cat.ClassId into join2
                    from j2 in join2.DefaultIfEmpty()

                    join ass in db.Assignment on j2.CatId equals ass.Category 
                    where cr.CourseNum == num && j1.Season == season && j1.Year == year && cr.DId == subject
                    select new { aID = ass.AId, aname = ass.Name, cname = j2.Name, due = ass.DueDateTime };

                //take that query result and  join it with the submission table aID|Name --- left join---> uID|aID and get the nulls where the uid matches
                var queryResult =
                    from qa in queryAssignments
                    join sub in db.Submission on new { aID = qa.aID, uID = uid} equals new { aID = sub.AId, uID = sub.UId } into join3
                    from j3 in join3.DefaultIfEmpty()
                    select new { aname = qa.aname, cname = qa.cname, due = qa.due, score = j3 == null ? null : (int?)j3.Score }; 

                return Json(queryResult.ToArray());
            }
        }

        /// <summary>
        /// Adds a submission to the given assignment for the given student
        /// The submission should use the current time as its DateTime
        /// You can get the current time with DateTime.Now
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The new assignment name</param>
        /// <param name="uid">The student submitting the assignment</param>
        /// <param name="contents">The text contents of the student's submission</param>
        /// <returns>A JSON object containing {success = true/false}</returns>
        public IActionResult SubmitAssignmentText(string subject, int num, string season, int year, string category, string asgname, string uid, string contents)
        {
            using (Team9Context db = new Team9Context())
            {
                if (contents == null)
                {
                    return Json(new { success = false });
                }

                var queryAssignments =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId into join1
                    from j1 in join1.DefaultIfEmpty()

                    join ac in db.AssignmentCategory on j1.ClassId equals ac.ClassId into join2
                    from j2 in join2.DefaultIfEmpty()

                    join ass in db.Assignment on j2.CatId equals ass.Category into join3
                    from j3 in join3.DefaultIfEmpty()

                    where cr.CourseNum == num && j1.Season == season && j1.Year == year && cr.DId == subject
                    && j2.Name == category && j3.Name == asgname
                    select new {aID = j3.AId};

                int assID = queryAssignments.ToArray()[0].aID;

                var querySubmission =
                    from sub in db.Submission
                    where sub.AId == assID && sub.UId == uid
                    select sub;


                Submission newSub = null;
                if (querySubmission.Count() != 0)
                {
                    newSub = querySubmission.ToArray()[0];
                    newSub.Contents = contents;
                    newSub.Time = DateTime.Now;

                    db.Submission.Update(newSub);
                } else
                {
                    newSub = new Submission();
                    newSub.Time = DateTime.Now;
                    newSub.Contents = contents;
                    newSub.Score = 0;
                    newSub.UId = uid;
                    newSub.AId = assID;
                    //no need to add score value into the table here

                    db.Submission.Add(newSub);
                    
                }
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
        }

        /// <summary>
        /// Enrolls a student in a class.
        /// </summary>
        /// <param name="subject">The department subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester</param>
        /// <param name="year">The year part of the semester</param>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing {success = {true/false}. False if the student is already enrolled in the class.</returns>
        public IActionResult Enroll(string subject, int num, string season, int year, string uid)
        {
            using (Team9Context db = new Team9Context())
            {
                //var queryEnrolled =
                //    from s in db.Student
                //    join e in db.Enrolled on s.UId equals e.UId
                //    join cl in db.Class on e.ClassId equals cl.ClassId
                //    join cr in db.Course on cl.CatId equals cr.CatId
                //    where e.UId == uid && cr.CourseNum == num && cl.Season == season && cl.Year == year && cr.DId == subject
                //    select new { uid = e.UId, classID = cl.ClassId };

                var queryEnrolled =
                    from cr in db.Course 
                    join cl in db.Class on cr.CatId equals cl.CatId into join1
                    from j1 in join1.DefaultIfEmpty()

                    join e in db.Enrolled on j1.ClassId equals e.ClassId into join3
                    from j3 in join3.DefaultIfEmpty()

                    where cr.CourseNum == num && j1.Season == season && j1.Year == year && cr.DId == subject
                    select new { uid = j3.UId ?? null, classID = j1.ClassId };

                bool uidEnrolled = false; 

                for (int i = 0; i < queryEnrolled.Count(); i++) {
                    if (queryEnrolled.ToArray()[0].uid == uid) {
                        uidEnrolled = true; 
                    }
                } 

                if (!uidEnrolled)
                {
                    //insert into enroll, grade, uid, classID
                    Enrolled enroll = new Enrolled();
                    enroll.UId = uid;
                    enroll.ClassId = queryEnrolled.ToArray()[0].classID;
                    enroll.Grade = "--";

                    db.Enrolled.Add(enroll);
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
                else {
                    return Json(new { success = false });
                }

            }
        }

        /// <summary>
        /// Calculates a student's GPA
        /// A student's GPA is determined by the grade-point representation of the average grade in all their classes.
        /// If a student does not have a grade in a class ("--"), that class is not counted in the average.
        /// Otherwise, the point-value of a letter grade for the UofU is determined by the table on this page:
        /// https://advising.utah.edu/academic-standards/gpa-calculator-new.php
        /// </summary>
        /// <param name="uid">The uid of the student</param>
        /// <returns>A JSON object containing a single field called "gpa" with the number value</returns>
        public IActionResult GetGPA(string uid)
        {
            var queryEnrolled =
                    from e in db.Enrolled
                    where e.UId == uid
                    select new { grade = e.Grade };
            
            //Letter Grade    Grade Points
            //A   4.0
            //A - 3.7
            //B + 3.3
            //B   3.0
            //B - 2.7
            //C + 2.3
            //C   2.0
            //C - 1.7
            //D + 1.3
            //D   1.0
            //D - 0.7
            //E   0.0

            Dictionary<string, double> gradeDict = new Dictionary<string, double>();

            gradeDict.Add("A", 4.0);
            gradeDict.Add("A-", 3.7);
            gradeDict.Add("B+", 3.3);
            gradeDict.Add("B", 3.0);
            gradeDict.Add("B-", 2.7);
            gradeDict.Add("C+", 2.3);
            gradeDict.Add("C", 2.0);
            gradeDict.Add("C-", 1.7);
            gradeDict.Add("D+", 1.3);
            gradeDict.Add("D", 1.0);
            gradeDict.Add("D-", 0.7);
            gradeDict.Add("E", 0.0);

            double gpaNum = 0;
            double totalGradePoints = 0;
            int numberOfGrades = 0; 

            foreach (var letterGrade in queryEnrolled.ToArray()) {
                if (!letterGrade.grade.Equals("--"))
                {
                    totalGradePoints += gradeDict[letterGrade.grade];
                    numberOfGrades++;
                }
            }

            gpaNum = totalGradePoints / numberOfGrades; 

            return Json(new { gpa = gpaNum });
        }
    }
}