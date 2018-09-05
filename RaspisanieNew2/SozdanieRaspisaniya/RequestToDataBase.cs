using System.Xml.Linq;
using Raspisanie.Models;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.ObjectModel;
using SozdanieRaspisaniya.ViewModel;
using System;
using System.Windows;

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
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select * from faculty";
                        selectCommand.Connection = conn;
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
                    }
                    dbtran.Commit();
                }
            }

        }

        public IEnumerable<Department> ReadDepartments()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select * from(departments join faculty using (id_faculty))";
                        selectCommand.Connection = conn;
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
                    }
                    dbtran.Commit();
                }
            }
        }

        public IEnumerable<ClassRoom> ReadClassrooms()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_classroom, number_of_classroom, specific, id_department, name_of_department from (classrooms join departments using (id_department))";
                        selectCommand.Connection = conn;
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
                    }
                    dbtran.Commit();
                }

            }
        }

        public IEnumerable<Subject> ReadSubjects()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_subject,name_of_subject, specific, id_department ,name_of_department from (subjects join departments using(id_department))";
                        selectCommand.Connection = conn;
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
                    }
                    dbtran.Commit();
                }
            }
        }

        public IEnumerable<Teacher> ReadTeachers()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_teacher, fio, post, id_department, name_of_department from (teachers join teachersanddepartments using(id_teacher) join departments using(id_department))";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            yield return new Teacher
                            {
                                CodeOfTeacher = reader.GetInt32(0),
                                FIO = reader.GetString(1),
                                Post = reader.GetString(2),
                                Department = new Department
                                {
                                    CodeOfDepartment = reader.GetInt32(3),
                                    NameOfDepartment = reader.GetString(4)
                                }
                            };
                        }
                    }
                    dbtran.Commit();

                }
            }
        }

        public IEnumerable<Group> ReadGroups()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_group, name_of_group, id_department, name_of_department from (groups join departments using(id_department))";
                        selectCommand.Connection = conn;
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
                    }
                    dbtran.Commit();
                }
            }
        }

        public bool clearClasses()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = " execute block as begin "+
                                "EXECUTE STATEMENT 'delete from Classes';"+
                                "EXECUTE STATEMENT 'set GENERATOR classes_id to 0'; end ";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            
                            int result = deleteCommand.ExecuteNonQuery();
                            dbtran.Commit();
                            return result > 0;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        dbtran.Rollback();
                        return false;
                    }
                }
            }
            return false;
        }

        public bool requestInsertIntoClassesItemOne(ObservableCollection<ObservableCollection<DropItem>> Filtered, int i, int j)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into Classes(id_teacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@CodeOfTeacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher",Filtered[i][j].Item.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", Filtered[i][j].Item.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", Filtered[i][j].Item.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", Filtered[i][j].Item.Group.CodeOfGroup);
                            insertCommand.Parameters.AddWithValue("@Specifics", Filtered[i][j].Item.Specifics);
                            insertCommand.Parameters.AddWithValue("@Day", Filtered[i][j].Info.Day);
                            insertCommand.Parameters.AddWithValue("@Time", Filtered[i][j].Info.Pair);
                            insertCommand.Parameters.AddWithValue("@Num_den", Filtered[i][j].Item.ndindex);
                            insertCommand.Parameters.AddWithValue("@Key", Filtered[i][j].Key);
                            insertCommand.Parameters.AddWithValue("@KeyType", Filtered[i][j].KeyType);

                            int result = insertCommand.ExecuteNonQuery();
                            dbtran.Commit();
                            return result > 0;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        dbtran.Rollback();
                        return false;
                    }
                }
            }
            return false;
        }

        public bool requestInsertIntoClassesItemTwo(ObservableCollection<ObservableCollection<DropItem>> Filtered, int i, int j)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into Classes(id_teacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@CodeOfTeacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher", Filtered[i][j].ItemTwo.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", Filtered[i][j].ItemTwo.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", Filtered[i][j].ItemTwo.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", Filtered[i][j].ItemTwo.Group.CodeOfGroup);
                            insertCommand.Parameters.AddWithValue("@Specifics", Filtered[i][j].ItemTwo.Specifics);
                            insertCommand.Parameters.AddWithValue("@Day", Filtered[i][j].Info.Day);
                            insertCommand.Parameters.AddWithValue("@Time", Filtered[i][j].Info.Pair);
                            insertCommand.Parameters.AddWithValue("@Num_den", Filtered[i][j].ItemTwo.ndindex);
                            insertCommand.Parameters.AddWithValue("@Key", Filtered[i][j].Key);
                            insertCommand.Parameters.AddWithValue("@KeyType", Filtered[i][j].KeyType);
                            
                            int result = insertCommand.ExecuteNonQuery();
                            dbtran.Commit();
                            return result > 0;
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                        dbtran.Rollback();
                        return false;
                    }
                }
            }
            return false;
        }

        //public IEnumerable<DropItem> ReadClasses()
        //{
        //    if (Open())
        //    {
        //        using (FbTransaction dbtran = conn.BeginTransaction())
        //        {
        //            using (FbCommand selectCommand = new FbCommand())
        //            {
        //                selectCommand.CommandText = "";
        //                selectCommand.Connection = conn;
        //                selectCommand.Transaction = dbtran;
        //                FbDataReader reader = selectCommand.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    //yield return new DropItem
        //                    //{
        //                    //};
        //                }
        //            }
        //            dbtran.Commit();
        //        }
        //    }
        //}

    }
}
