using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LMS.Models.LMSModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.Controllers
{
  [Authorize(Roles = "Professor")]
  public class ProfessorController : CommonController
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Students(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
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

    public IActionResult Categories(string subject, string num, string season, string year)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      return View();
    }

    public IActionResult CatAssignments(string subject, string num, string season, string year, string cat)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
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

    public IActionResult Submissions(string subject, string num, string season, string year, string cat, string aname)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      return View();
    }

    public IActionResult Grade(string subject, string num, string season, string year, string cat, string aname, string uid)
    {
      ViewData["subject"] = subject;
      ViewData["num"] = num;
      ViewData["season"] = season;
      ViewData["year"] = year;
      ViewData["cat"] = cat;
      ViewData["aname"] = aname;
      ViewData["uid"] = uid;
      return View();
    }

    /*******Begin code to modify********/


    /// <summary>
    /// Returns a JSON array of all the students in a class.
    /// Each object in the array should have the following fields:
    /// "fname" - first name
    /// "lname" - last name
    /// "uid" - user ID
    /// "dob" - date of birth
    /// "grade" - the student's grade in this class
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetStudentsInClass(string subject, int num, string season, int year)
    {
            using (Team9Context db = new Team9Context())
            {
                var queryStudentsInClass =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId
                    join e in db.Enrolled on cl.ClassId equals e.ClassId
                    join s in db.Student on e.UId equals s.UId
                    where cr.DId == subject && cr.CourseNum == num && cl.Season == season && cl.Year == year
                    select new { fname = s.FirstName, lname = s.LastName, uid = s.UId, dob = s.DOB, grade = e.Grade };

                return Json(queryStudentsInClass.ToArray());
            }

        }

    /// <summary>
    /// Assume that a specific class can not have two categories with the same name.
    /// Returns a JSON array with all the assignments in an assignment category for a class.
    /// If the "category" parameter is null, return all assignments in the class.
    /// Each object in the array should have the following fields:
    /// "aname" - The assignment name
    /// "cname" - The assignment category name.
    /// "due" - The due DateTime
    /// "submissions" - The number of submissions to the assignment
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class, or null to return assignments from all categories</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentsInCategory(string subject, int num, string season, int year, string category)
    {
        using (Team9Context db = new Team9Context())
        {

                if (category == null)
                {
                    var queryAssignments =
                        from cr in db.Course
                        join cl in db.Class on cr.CatId equals cl.CatId
                        join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                        join a in db.Assignment on ac.CatId equals a.Category
                        where cr.CourseNum == num && cl.Season == season && cl.Year == year
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.DueDateTime,
                            submissions = (from sub in db.Submission
                                           where sub.AId == a.AId
                                           select sub).Count()
                        };
                    return Json(queryAssignments.ToArray());
                }
                else
                {
                    var queryAssignments =
                        from cr in db.Course
                        join cl in db.Class on cr.CatId equals cl.CatId
                        join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                        join a in db.Assignment on ac.CatId equals a.Category
                        where cr.CourseNum == num && cl.Season == season && cl.Year == year && ac.Name == category
                        select new
                        {
                            aname = a.Name,
                            cname = ac.Name,
                            due = a.DueDateTime,
                            submissions = (from sub in db.Submission
                                           where sub.AId == a.AId
                                           select sub).Count()
                        };
                    return Json(queryAssignments.ToArray());
                }
        }
    }


    /// <summary>
    /// Returns a JSON array of the assignment categories for a certain class.
    /// Each object in the array should have the folling fields:
    /// "name" - The category name
    /// "weight" - The category weight
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetAssignmentCategories(string subject, int num, string season, int year)
    {
        using (Team9Context db = new Team9Context())
        {
                var queryAssignCats =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId
                    join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                    where cr.CourseNum == num && cl.Season == season && cl.Year == year
                    select new { name = ac.Name, weight = ac.Weight };

                return Json(queryAssignCats.ToArray());
        }
        }

    /// <summary>
    /// Creates a new assignment category for the specified class.
    /// A class can not have two categories with the same name.
    /// If a category of the given class with the given name already exists, return success = false.
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The new category name</param>
    /// <param name="catweight">The new category weight</param>
    /// <returns>A JSON object containing {success = true/false} </returns>
    public IActionResult CreateAssignmentCategory(string subject, int num, string season, int year, string category, int catweight)
    {
        using (Team9Context db = new Team9Context())
        {
            var queryCatagory =
                from cr in db.Course
                join cl in db.Class on cr.CatId equals cl.CatId into join1
                from j1 in join1.DefaultIfEmpty()

                join ac in db.AssignmentCategory on j1.ClassId equals ac.ClassId into join2
                from j2 in join2.DefaultIfEmpty()

                where cr.CourseNum == num && j1.Season == season && j1.Year == year && cr.DId == subject
                select new { catName = j2.Name ?? null, classID = j1.ClassId };

            bool categoryAdded = false;

            for (int i = 0; i < queryCatagory.Count(); i++)
            {
                if (queryCatagory.ToArray()[i].catName != null && queryCatagory.ToArray()[i].catName.Equals(category))
                {
                    categoryAdded = true;
                }
            }

            if (!categoryAdded)
            {
                //insert into enroll, grade, uid, classID
                AssignmentCategory catToAdd = new AssignmentCategory();
                    catToAdd.Name = category;
                    catToAdd.Weight = catweight;
                    catToAdd.ClassId = queryCatagory.ToArray()[0].classID;

                db.AssignmentCategory.Add(catToAdd);
                try { db.SaveChanges();
                }
                catch ( Exception e) {
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
    /// Creates a new assignment for the given class and category.
    /// An assignment category (which belongs to a class) can not have two assignments with 
    /// the same name.
    /// If an assignment of the given category with the given name already exists, return success = false. 
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The new assignment name</param>
    /// <param name="asgpoints">The max point value for the new assignment</param>
    /// <param name="asgdue">The due DateTime for the new assignment</param>
    /// <param name="asgcontents">The contents of the new assignment</param>
    /// <returns>A JSON object containing success = true/false</returns>
    public IActionResult CreateAssignment(string subject, int num, string season, int year, string category, string asgname, int asgpoints, DateTime asgdue, string asgcontents)
    {
            using (Team9Context db = new Team9Context())
            {
                var queryAssignment =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId into join1
                    from j1 in join1.DefaultIfEmpty()

                    join ac in db.AssignmentCategory on j1.ClassId equals ac.ClassId into join2
                    from j2 in join2.DefaultIfEmpty()

                    join ass in db.Assignment on j2.CatId equals ass.Category into join3
                    from j3 in join3.DefaultIfEmpty()

                    where cr.CourseNum == num && j1.Season == season && j1.Year == year && cr.DId == subject
                    && j2.Name == category && j3.Name == asgname
                    select new { assignName = j3.Name ?? null, catID = j2.CatId, points = j3.Points, classID = j1.ClassId};

                var queryCatagory =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId into join1
                    from j1 in join1.DefaultIfEmpty()

                    join ac in db.AssignmentCategory on j1.ClassId equals ac.ClassId into join2
                    from j2 in join2.DefaultIfEmpty()

                    where cr.CourseNum == num && j1.Season == season && j1.Year == year && cr.DId == subject && j2.Name == category
                    select new { catName = j2.Name ?? null, classID = j1.ClassId, catID = j2.CatId };

                var queryStudents =
                    from s in db.Student
                    join e in db.Enrolled
                    on s.UId equals e.UId
                    where e.ClassId == queryAssignment.ToArray()[0].classID
                    select s; 


                bool assAdded = false;

                for (int i = 0; i < queryAssignment.Count(); i++)
                {
                    if (queryAssignment.ToArray()[i].assignName.Equals(asgname))
                    {
                        assAdded = true;
                    }
                }

                if (!assAdded)
                {
                    Assignment assToAdd = new Assignment();
                    assToAdd.Name = asgname;
                    assToAdd.DueDateTime = asgdue;
                    assToAdd.Contents = asgcontents;
                    assToAdd.Points = asgpoints;
                    assToAdd.Category = queryCatagory.ToArray()[0].catID;

                    db.Assignment.Add(assToAdd);
                    db.SaveChanges();

                    string grade = ""; 

                    for (int i = 0; i < queryStudents.Count(); i++) {
                        //calculate grade and submit to enroll
                        grade = CalculateGrade(queryCatagory.ToArray()[0].classID, queryStudents.ToArray()[i].UId);
                        var queryEnroll =
                        from e in db.Enrolled
                        where e.ClassId == queryAssignment.ToArray()[0].classID && e.UId == queryStudents.ToArray()[i].UId
                        select e;

                        Enrolled enroll = queryEnroll.ToArray()[0];
                        enroll.Grade = grade;
                        db.SaveChanges();
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
        /// Gets a JSON array of all the submissions to a certain assignment.
        /// Each object in the array should have the following fields:
        /// "fname" - first name
        /// "lname" - last name
        /// "uid" - user ID
        /// "time" - DateTime of the submission
        /// "score" - The score given to the submission
        /// 
        /// </summary>
        /// <param name="subject">The course subject abbreviation</param>
        /// <param name="num">The course number</param>
        /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
        /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
        /// <param name="category">The name of the assignment category in the class</param>
        /// <param name="asgname">The name of the assignment</param>
        /// <returns>The JSON array</returns>
        public IActionResult GetSubmissionsToAssignment(string subject, int num, string season, int year, string category, string asgname)
        {
            using (Team9Context db = new Team9Context())
            {
                var queryAssSubmissions =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId
                    join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                    join ass in db.Assignment on ac.CatId equals ass.Category
                    join sub in db.Submission on ass.AId equals sub.AId
                    join stud in db.Student on sub.UId equals stud.UId
                    where cr.CourseNum == num && cl.Season == season && cl.Year == year && cr.DId == subject
                    && ac.Name == category && ass.Name == asgname
                    select new { fname = stud.FirstName, lname = stud.LastName, uid = stud.UId, time = sub.Time, score = sub.Score };

                if (queryAssSubmissions.Count() != 0) {
                    return Json(queryAssSubmissions.ToArray());
                } 
                else
                {
                    return Json(new { });
                }

                
                //var queryAssSubmissions =
                //    from cr in db.Course
                //    join cl in db.Class on cr.CatId equals cl.CatId into join1
                //    from j1 in join1.DefaultIfEmpty()

                //    join ac in db.AssignmentCategory on j1.ClassId equals ac.ClassId into join2
                //    from j2 in join2.DefaultIfEmpty()

                //    join ass in db.Assignment on j2.CatId equals ass.Category into join3
                //    from j3 in join3.DefaultIfEmpty()

                //    join sub in db.Submission on j3.AId equals sub.AId into join4
                //    from j4 in join4.DefaultIfEmpty()

                //    join stud in db.Student on j4.UId equals stud.UId into join5
                //    from j5 in join5.DefaultIfEmpty()

                //    where cr.CourseNum == num && j1.Season == season && j1.Year == year && cr.DId == subject
                //    && j2.Name == category && j3.Name == asgname
                //    select new { fname = j5.FirstName ?? null, lname = j5.LastName ?? null, uid = j5.UId ?? null, time = j4.Time, score = j4.Score }; //(DateTime?) j4.Time == null ? null : j4.Time

                //if (queryAssSubmissions.ToArray().Count() <= 0) {
                //    return Json(new { });
                //}

            }
        }


    /// <summary>
    /// Set the score of an assignment submission
    /// </summary>
    /// <param name="subject">The course subject abbreviation</param>
    /// <param name="num">The course number</param>
    /// <param name="season">The season part of the semester for the class the assignment belongs to</param>
    /// <param name="year">The year part of the semester for the class the assignment belongs to</param>
    /// <param name="category">The name of the assignment category in the class</param>
    /// <param name="asgname">The name of the assignment</param>
    /// <param name="uid">The uid of the student who's submission is being graded</param>
    /// <param name="score">The new score for the submission</param>
    /// <returns>A JSON object containing success = true/false</returns> 
    public IActionResult GradeSubmission(string subject, int num, string season, int year, string category, string asgname, string uid, int score)
    {
            using (Team9Context db = new Team9Context())
            {
                if (score < 0) {
                    return Json(new { success = false });
                }

                var queryAssSubmissions =
                    from cr in db.Course
                    join cl in db.Class on cr.CatId equals cl.CatId
                    join ac in db.AssignmentCategory on cl.ClassId equals ac.ClassId
                    join ass in db.Assignment on ac.CatId equals ass.Category
                    join sub in db.Submission on ass.AId equals sub.AId
                    join stud in db.Student on sub.UId equals stud.UId

                    where cr.CourseNum == num && cl.Season == season && cl.Year == year && cr.DId == subject
                    && ac.Name == category && ass.Name == asgname && stud.UId == uid
                    select new { sub, aPoints = ass.Points, classID = cl.ClassId};

                Submission newSub = null;
                if (queryAssSubmissions.Count() != 0)
                {
                    newSub = queryAssSubmissions.ToArray()[0].sub;
                    newSub.Score = score;
                    //db.Submission.Update(newSub);
                    db.SaveChanges();
                }


                //calculate grade and submit to enroll
                string grade = CalculateGrade(queryAssSubmissions.ToArray()[0].classID, uid);

                var queryEnroll =
                    from e in db.Enrolled
                    where e.UId == uid && e.ClassId == queryAssSubmissions.ToArray()[0].classID
                    select e;

                Enrolled enroll = queryEnroll.ToArray()[0];
                enroll.Grade = grade;
                db.Enrolled.Update(enroll);

                db.SaveChanges();
                return Json(new { success = true });
            }
        }


    /// <summary>
    /// Returns a JSON array of the classes taught by the specified professor
    /// Each object in the array should have the following fields:
    /// "subject" - The subject abbreviation of the class (such as "CS")
    /// "number" - The course number (such as 6016)
    /// "name" - The course name
    /// "season" - The season part of the semester in which the class is taught
    /// "year" - The year part of the semester in which the class is taught
    /// </summary>
    /// <param name="uid">The professor's uid</param>
    /// <returns>The JSON array</returns>
    public IActionResult GetMyClasses(string uid)
    {
        using (Team9Context db = new Team9Context())
        {
                var queryProf =
                    from p in db.Professor
                    join cr in db.Course on p.Department equals cr.DId
                    join cl in db.Class on cr.CatId equals cl.CatId
                    where p.UId == uid && cl.Instructor == uid
                    select new { subject = cr.DId, number = cr.CourseNum, name = cr.Name, season = cl.Season, year = cl.Year };

            return Json(queryProf.ToArray());
        }
    }
    private string CalculateGrade(int classID, string uid)      
    {
            double allCatWeights = 0.0;
            double numericalGradeTotal = 0.0;
            double catPercent = 0.0;
            double catScore = 0.0;

            var queryCategories =
                from asCat in db.AssignmentCategory
                where asCat.ClassId == classID
                select asCat;

            for (int c = 0; c < queryCategories.Count(); c++)
            {
                int catTotalPoints = 0;
                int catEarned = 0;

                var queryAssign =
                        from ass in db.Assignment
                        where ass.Category == queryCategories.ToArray()[c].CatId
                        select ass;

                for (int a = 0; a < queryAssign.Count(); a++)
                {
                    catTotalPoints += (int)queryAssign.ToArray()[a].Points;
                    var querySubs =
                        from sub in db.Submission
                        where sub.UId == uid && sub.AId == queryAssign.ToArray()[a].AId
                        select sub;

                    if (querySubs.Count() == 0)
                    {
                        catEarned += 0;
                    }
                    else
                    {
                        catEarned += querySubs.ToArray()[0].Score;
                    }
                }

                if (catTotalPoints > 0)
                {
                    catPercent = ((float)catEarned / (float)catTotalPoints);
                    catScore = catPercent * queryCategories.ToArray()[c].Weight;
                    numericalGradeTotal += catScore;
                    allCatWeights += queryCategories.ToArray()[c].Weight;
                }
            }
            numericalGradeTotal *= (100 / (float) allCatWeights);
            numericalGradeTotal /= 100f;
        //takes in a student id and class id, so we can re-use it by getting the students in the class by getting all the uids in a class


        string gradeLetter = "E";

        if(numericalGradeTotal > 0.93)
        {
            gradeLetter = "A";
        }
        else if (numericalGradeTotal > 0.90)
        {
            gradeLetter = "A-";
        }
        else if (numericalGradeTotal > 0.87)
        {
            gradeLetter = "B+";
        }
        else if (numericalGradeTotal > 0.83)
        {
            gradeLetter = "B";
        }
        else if (numericalGradeTotal > 0.80)
        {
            gradeLetter = "B-";
        }
        else if (numericalGradeTotal > 0.77)
        {
            gradeLetter = "C+";
        }
        else if (numericalGradeTotal > 0.73)
        {
            gradeLetter = "C";
        }
        else if (numericalGradeTotal > 0.70)
        {
            gradeLetter = "C-";
        }

        else if (numericalGradeTotal > 0.67)
        {
            gradeLetter = "D+";
        }
        else if (numericalGradeTotal > 0.63)
        {
            gradeLetter = "D";
        }
        else if (numericalGradeTotal > 0.60)
        {
            gradeLetter = "D-";
        }
        else
        {
            gradeLetter = "E";
        }

        return gradeLetter;
    }

  }
}