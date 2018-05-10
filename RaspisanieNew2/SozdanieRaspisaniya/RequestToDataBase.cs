using System.Xml.Linq;
using Raspisanie.Models;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;

namespace SozdanieRaspisaniya
{
    class RequestToDataBase
    {

        private FbConnection conn;

        private RequestToDataBase(string sconn)
        {
            this.conn = new FbConnection(sconn);
        }

        public bool IsOpen => conn.State == System.Data.ConnectionState.Open;

        public static RequestToDataBase Instance { get; private set; } = null;

        public static RequestToDataBase getOrCreateInstance(string sconn = null)
        {
            if (Instance == null && sconn != null)
            {
                Instance = new RequestToDataBase(sconn);
            }
            return Instance;
        }

        public bool Open()
        {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            return IsOpen;
        }

        public bool Close()
        {
            if (conn.State != System.Data.ConnectionState.Closed)
                conn.Close();
            return conn.State == System.Data.ConnectionState.Closed;
        }

        public IEnumerable<Faculty> ReadFaculty()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select * from faculty", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Faculty
                    {
                        CodeOfFaculty = reader.GetInt32(0),
                        NameOfFaculty = reader.GetString(1)
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }

        }



        public IEnumerable<Department> ReadDepartments()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select * from(departments join faculty using (id_faculty))", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Department
                    {
                        CodeOfDepartment = reader.GetInt32(0),
                        NameOfDepartment = reader.GetString(1),
                        Faculty = new Faculty
                        {
                            CodeOfFaculty = reader.GetInt32(2),
                            NameOfFaculty = reader.GetString(3)
                        }
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }




        public IEnumerable<ClassRoom> ReadClassrooms()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select id_classroom, number_of_classroom, specific, id_department, name_of_department from (classrooms join departments using (id_department))", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new ClassRoom
                    {
                        CodeOfClassroom = reader.GetInt32(0),
                        NumberOfClassroom = reader.GetString(1),
                        Specifics = reader.GetString(2),
                        Department = new Department
                        {
                            CodeOfDepartment = reader.GetInt32(3),
                            NameOfDepartment = reader.GetString(4)
                        }
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }


        public IEnumerable<Subject> ReadSubjects()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select id_subject,name_of_subject, specific, id_department ,name_of_department from (subjects join departments using(id_department))", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Subject
                    {
                        CodeOfSubject = reader.GetInt32(0),
                        NameOfSubject = reader.GetString(1),
                        Specific = reader.GetString(2),
                        Department = new Department
                        {
                            CodeOfDepartment = reader.GetInt32(3),
                            NameOfDepartment = reader.GetString(4)
                        }
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }

        public IEnumerable<Teacher> ReadTeachers()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select id_teacher, fio, post, id_department, name_of_department from (teachers join teachersanddepartments using(id_teacher) join departments using(id_department))", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Teacher
                    {
                        CodeOfTeacher = reader.GetInt32(0),
                        FIO = reader.GetString(1),
                        Post = reader.GetString(2)
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }


        public IEnumerable<Group> ReadGroups()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select id_group, name_of_group, id_department, name_of_department from (groups join departments using(id_department))", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Group
                    {
                        CodeOfGroup = reader.GetInt32(0),
                        NameOfGroup = reader.GetString(1),
                        Department = new Department
                        {
                            CodeOfDepartment = reader.GetInt32(2),
                            NameOfDepartment = reader.GetString(3)
                        }

                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }


    }
}
