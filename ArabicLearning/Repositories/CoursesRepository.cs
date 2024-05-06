using ArabicLearning.Repositories.Interfaces;
using ArabicLearning.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
//mysql
using MySql.Data;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;

namespace ArabicLearning.Repositories
{
    public class CoursesRepository : ICoursesRepository
    {   
        private String queryStr;
        private MySqlConnection myConnection;
        //private IEnumerable<Course> Collection;
        public CoursesRepository(IConfiguration configuration)
        {
            //this.Collection = InMemoryCourseCollection;
            try{
                myConnection = new MySqlConnection();
                Console.WriteLine("About to connect to DB");
                myConnection.ConnectionString = configuration.GetConnectionString("DefaultConnection");
                myConnection.Open();
            }
            catch(MySqlException ex) {
                Console.WriteLine(ex.Message);
            }
        }

        public IEnumerable<Course> GetAllCourses()
        {
            queryStr = "SELECT * From Class Join Person on Person.person_id=Class.teacher_id";
            //return this.Collection;
            return GetResultFromDB(queryStr);
        }
        public IEnumerable<Course> GetByType(string courseType)
        {
            queryStr = string.Format("SELECT * From Class Join Person on Person.person_id=Class.teacher_id WHERE name='{0}'", courseType);
            //return this.Collection.GroupBy(course => course.Type==courseType).FirstOrDefault();
            return GetResultFromDB(queryStr);
        }

        public IEnumerable<Course> GetByLevel(string level)
        {
            queryStr = string.Format("SELECT * From Class Join Person on Person.person_id=Class.teacher_id WHERE name='{0}'", level);
            //return this.Collection.GroupBy(course => course.Level == level).FirstOrDefault();
            return GetResultFromDB(queryStr);
        }
        public IEnumerable<Course> GetAllNew()
        {
            queryStr = "SELECT * From Class Join Person on Person.person_id=Class.teacher_id ORDER BY class_id ASC;";
            return GetResultFromDB(queryStr);
        }
        public IEnumerable<Course> GetAllPopular()
        {
            queryStr = "SELECT * From Class Join Person on Person.person_id=Class.teacher_id ORDER BY class_id DESC;";
            return GetResultFromDB(queryStr);
        }
        public IEnumerable<Course> GetFullCourse(int id)
        {
            queryStr = string.Format("SELECT * From Class Join Person on Person.person_id=Class.teacher_id WHERE class_id={0}", id);
            return GetResultFromDB(queryStr);
        }
        private IEnumerable<Course> GetResultFromDB(string queryStr)
        {
            Console.WriteLine("Getting results....");
            List<Course> resultList = new List<Course>();
            using (MySqlCommand myCommand = new MySqlCommand(queryStr, myConnection))
            {
                try
                {
                    // myConnection.Open();
                    MySqlDataReader dbData = myCommand.ExecuteReader();
                    if (dbData.HasRows)
                    {
                        while (dbData.Read())
                        {
                            Course course = new Course();
                            course.Id = (int) dbData["class_id"];
                            course.Name = (string) dbData["name"];
                            course.StartDate = (DateTime)dbData["start_date"];
                            course.EndDate = (DateTime)dbData["end_date"];
                            course.Level = "";//(string)dbData["name"];
                            course.Teacher = (string)dbData["first_name"] +" "+ (string)dbData["last_name"];
                            course.Type = "";// (string)dbData["name"];

                            resultList.Add(course);
                            System.Diagnostics.Debug.WriteLine("-- Database query successful --");
                            //System.Diagnostics.Debug.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}", dbData.GetInt32(0), dbData.GetString(1), dbData.GetString(2), dbData.GetString(3), dbData.GetString(4));
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No rows found.");
                    }
                    dbData.Close();
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
                finally
                {
                    if (myConnection.State == ConnectionState.Open)
                    {
                        myConnection.Close();
                    }
                }
            }
            return resultList;
        }
        private IEnumerable<Course> InMemoryCourseCollection { get; } = new List<Course>
        {
            new Course { Name = "Classical Arabic 1", Level = "Beginner", Teacher = "Aq", Type = "Grammar" },
            new Course { Name = "Classical Arabic 2", Level = "Elementary", Teacher = "Aq", Type = "Morphology" },
            new Course { Name = "Classical Arabic 3", Level = "Lower Intermediate", Teacher = "Aq", Type = "Language" },
            new Course { Name = "Classical Arabic 4", Level = "Upper Intermediate", Teacher = "Aq", Type = "Eloquence" },
            new Course { Name = "Classical Arabic 5", Level = "Advanced", Teacher = "Aq", Type = "Poetry" }
        };

    }
}
