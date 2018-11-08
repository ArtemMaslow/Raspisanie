using System.Xml.Linq;
using Raspisanie.Models;
using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.ObjectModel;
using SozdanieRaspisaniya.ViewModel;
using System;
using System.Windows;
using System.Collections;
using Newtonsoft.Json;

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
                        selectCommand.CommandText = "select id_teacher, fio, post, mail, id_department, name_of_department from (teachers join teachersanddepartments using(id_teacher) join departments using(id_department))";
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
                                Mail = reader.GetString(3),
                                Department = new Department
                                {
                                    CodeOfDepartment = reader.GetInt32(4),
                                    NameOfDepartment = reader.GetString(5)
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

        public bool clearClassesSpring()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = " execute block as begin " +
                                "EXECUTE STATEMENT 'delete from ClassesSpring';" +
                                "EXECUTE STATEMENT 'set GENERATOR classesSpring_id to 0'; end ";
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

        public bool requestInsertIntoClassesSpringItemOne(DropItem item)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into ClassesSpring(id_teacher,id_departmentsteacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@CodeOfTeacher,@id_departmentsteacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher", item.Item.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@id_departmentsteacher", item.Item.Teacher.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", item.Item.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", item.Item.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", item.Item.Group.CodeOfGroup);
                            insertCommand.Parameters.AddWithValue("@Specifics", item.Item.Specifics);
                            insertCommand.Parameters.AddWithValue("@Day", item.Info.Day);
                            insertCommand.Parameters.AddWithValue("@Time", item.Info.Pair);
                            insertCommand.Parameters.AddWithValue("@Num_den", item.Item.Ndindex);
                            insertCommand.Parameters.AddWithValue("@Key", item.Key);
                            insertCommand.Parameters.AddWithValue("@KeyType", item.KeyType);

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

        public bool requestInsertIntoClassesSpringItemTwo(DropItem itemTwo)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into ClassesSpring(id_teacher, id_departmentsteacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@CodeOfTeacher, @id_departmentsteacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher", itemTwo.ItemTwo.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@id_departmentsteacher", itemTwo.ItemTwo.Teacher.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", itemTwo.ItemTwo.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", itemTwo.ItemTwo.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", itemTwo.ItemTwo.Group.CodeOfGroup);
                            insertCommand.Parameters.AddWithValue("@Specifics", itemTwo.ItemTwo.Specifics);
                            insertCommand.Parameters.AddWithValue("@Day", itemTwo.Info.Day);
                            insertCommand.Parameters.AddWithValue("@Time", itemTwo.Info.Pair);
                            insertCommand.Parameters.AddWithValue("@Num_den", itemTwo.ItemTwo.Ndindex);
                            insertCommand.Parameters.AddWithValue("@Key", itemTwo.Key);
                            insertCommand.Parameters.AddWithValue("@KeyType", itemTwo.KeyType);

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

        public object ReturnObjectFromCollections(string name, Group[] classGroups)
        {
            for (int i = 0; i < classGroups.Length; i++)
            {
                if (name == classGroups[i].NameOfGroup)
                    return classGroups[i];
            }
            return null;
        }

        public IEnumerable<DropItem> ReadClassesSpring(Group[] groups)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_teacher, fio, post, id_departmentsteacher, d1.name_of_department," + //4
                            "id_subject, name_of_subject, subjects.specific, subjects.id_department, d2.name_of_department," + //9
                            "id_classroom, number_of_classroom, classrooms.specific, classrooms.id_department, d3.name_of_department," + //14
                            "id_group, name_of_group, groups.id_department, d4.name_of_department,specifics,"+//19
                            "NUMERATOR_DENOMINATOR, pair, daytime, keyy, typekey " +//24
                            " from((((ClassesSpring join teachers using (id_teacher) join departments d1 on d1.id_department = ClassesSpring.id_departmentsteacher)" +
                                "join subjects using (id_subject) join departments d2 on d2.id_department = subjects.id_department)" +
                                "join classrooms using (id_classroom) join departments d3 on d3.id_department = classrooms.id_department)" +
                                "join groups using (id_group) join departments d4 on d4.id_department = groups.id_department)";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;

                        FbDataReader reader = selectCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            PairInfo info = new PairInfo(reader.GetInt32(21), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader.GetString(22)));
                            string[] str = reader.GetString(24).Split('.');
                            var type = typeof(Group);
                            if (str[2] == "Group")
                            {
                                type = typeof(Group);
                            }
                            else if (str[2] == "Teacher")
                            {
                                type = typeof(Teacher);
                            }
                            else if (str[2] == "ClassRoom")
                            {
                                type = typeof(ClassRoom);
                            }
                            //Console.WriteLine(reader.GetString(23));

                            var key = ReturnObjectFromCollections(reader.GetString(23), groups);
                            Console.WriteLine(reader.GetInt32(20));
                            if ((reader.GetInt32(20) == 0) || (reader.GetInt32(20) == 1))
                            {
                                yield return new DropItem(key, type, info)
                                {
                                    Item = new DropInformation
                                    {
                                        Teacher = new Teacher
                                        {
                                            CodeOfTeacher = reader.GetInt32(0),
                                            FIO = reader.GetString(1),
                                            Post = reader.GetString(2),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(3),
                                                NameOfDepartment = reader.GetString(4)
                                            }
                                        },
                                        Subject = new Subject
                                        {
                                            CodeOfSubject = reader.GetInt32(5),
                                            NameOfSubject = reader.GetString(6),
                                            Specific = reader.GetString(7),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(8),
                                                NameOfDepartment = reader.GetString(9)
                                            }
                                        },
                                        NumberOfClassroom = new ClassRoom
                                        {
                                            CodeOfClassroom = reader.GetInt32(10),
                                            NumberOfClassroom = reader.GetString(11),
                                            Specifics = reader.GetString(12),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(13),
                                                NameOfDepartment = reader.GetString(14)
                                            }
                                        },
                                        Group = new Group
                                        {
                                            CodeOfGroup = reader.GetInt32(15),
                                            NameOfGroup = reader.GetString(16),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(17),
                                                NameOfDepartment = reader.GetString(18),
                                            }
                                        },
                                        Specifics = reader.GetString(19),
                                        Ndindex = reader.GetInt32(20)
                                    },
                                    Info = info,
                                    State = reader.GetInt32(20)
                                };
                            }
                            else if(reader.GetInt32(20) == -1)
                            {
                                yield return new DropItem(key, type, info)
                                {
                                    ItemTwo = new DropInformation
                                    {
                                        Teacher = new Teacher
                                        {
                                            CodeOfTeacher = reader.GetInt32(0),
                                            FIO = reader.GetString(1),
                                            Post = reader.GetString(2),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(3),
                                                NameOfDepartment = reader.GetString(4)
                                            }
                                        },
                                        Subject = new Subject
                                        {
                                            CodeOfSubject = reader.GetInt32(5),
                                            NameOfSubject = reader.GetString(6),
                                            Specific = reader.GetString(7),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(8),
                                                NameOfDepartment = reader.GetString(9)
                                            }
                                        },
                                        NumberOfClassroom = new ClassRoom
                                        {
                                            CodeOfClassroom = reader.GetInt32(10),
                                            NumberOfClassroom = reader.GetString(11),
                                            Specifics = reader.GetString(12),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(13),
                                                NameOfDepartment = reader.GetString(14)
                                            }
                                        },
                                        Group = new Group
                                        {
                                            CodeOfGroup = reader.GetInt32(15),
                                            NameOfGroup = reader.GetString(16),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(17),
                                                NameOfDepartment = reader.GetString(18),
                                            }
                                        },
                                        Specifics = reader.GetString(19),
                                        Ndindex = reader.GetInt32(20)
                                    },
                                    Info = info,
                                    State = reader.GetInt32(20)
                                };
                            }
                        }
                    }
                    dbtran.Commit();
                }
            }
        }

        public bool clearClassesAutumn()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = " execute block as begin " +
                                "EXECUTE STATEMENT 'delete from ClassesAutumn';" +
                                "EXECUTE STATEMENT 'set GENERATOR classesAutumn_id to 0'; end ";
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

        public bool requestInsertIntoClassesAutumnItemOne(DropItem item)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into ClassesAutumn(id_teacher,id_departmentsteacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@CodeOfTeacher,@id_departmentsteacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher", item.Item.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@id_departmentsteacher", item.Item.Teacher.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", item.Item.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", item.Item.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", item.Item.Group.CodeOfGroup);
                            insertCommand.Parameters.AddWithValue("@Specifics", item.Item.Specifics);
                            insertCommand.Parameters.AddWithValue("@Day", item.Info.Day);
                            insertCommand.Parameters.AddWithValue("@Time", item.Info.Pair);
                            insertCommand.Parameters.AddWithValue("@Num_den", item.Item.Ndindex);
                            insertCommand.Parameters.AddWithValue("@Key", item.Key);
                            insertCommand.Parameters.AddWithValue("@KeyType", item.KeyType);

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

        public bool requestInsertIntoClassesAutumnItemTwo(DropItem itemTwo)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into ClassesAutumn(id_teacher, id_departmentsteacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@CodeOfTeacher, @id_departmentsteacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher", itemTwo.ItemTwo.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@id_departmentsteacher", itemTwo.ItemTwo.Teacher.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", itemTwo.ItemTwo.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", itemTwo.ItemTwo.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", itemTwo.ItemTwo.Group.CodeOfGroup);
                            insertCommand.Parameters.AddWithValue("@Specifics", itemTwo.ItemTwo.Specifics);
                            insertCommand.Parameters.AddWithValue("@Day", itemTwo.Info.Day);
                            insertCommand.Parameters.AddWithValue("@Time", itemTwo.Info.Pair);
                            insertCommand.Parameters.AddWithValue("@Num_den", itemTwo.ItemTwo.Ndindex);
                            insertCommand.Parameters.AddWithValue("@Key", itemTwo.Key);
                            insertCommand.Parameters.AddWithValue("@KeyType", itemTwo.KeyType);

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

        public IEnumerable<DropItem> ReadClassesAutumn(Group[] groups)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_teacher, fio, post, id_departmentsteacher, d1.name_of_department," + //4
                            "id_subject, name_of_subject, subjects.specific, subjects.id_department, d2.name_of_department," + //9
                            "id_classroom, number_of_classroom, classrooms.specific, classrooms.id_department, d3.name_of_department," + //14
                            "id_group, name_of_group, groups.id_department, d4.name_of_department,specifics," +//19
                            "NUMERATOR_DENOMINATOR, pair, daytime, keyy, typekey " +//24
                            " from((((ClassesAutumn join teachers using (id_teacher) join departments d1 on d1.id_department = ClassesAutumn.id_departmentsteacher)" +
                                "join subjects using (id_subject) join departments d2 on d2.id_department = subjects.id_department)" +
                                "join classrooms using (id_classroom) join departments d3 on d3.id_department = classrooms.id_department)" +
                                "join groups using (id_group) join departments d4 on d4.id_department = groups.id_department)";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;

                        FbDataReader reader = selectCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            PairInfo info = new PairInfo(reader.GetInt32(21), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader.GetString(22)));
                            string[] str = reader.GetString(24).Split('.');
                            var type = typeof(Group);
                            if (str[2] == "Group")
                            {
                                type = typeof(Group);
                            }
                            else if (str[2] == "Teacher")
                            {
                                type = typeof(Teacher);
                            }
                            else if (str[2] == "ClassRoom")
                            {
                                type = typeof(ClassRoom);
                            }
                            //Console.WriteLine(reader.GetString(23));

                            var key = ReturnObjectFromCollections(reader.GetString(23), groups);
                            Console.WriteLine(reader.GetInt32(20));
                            if ((reader.GetInt32(20) == 0) || (reader.GetInt32(20) == 1))
                            {
                                yield return new DropItem(key, type, info)
                                {
                                    Item = new DropInformation
                                    {
                                        Teacher = new Teacher
                                        {
                                            CodeOfTeacher = reader.GetInt32(0),
                                            FIO = reader.GetString(1),
                                            Post = reader.GetString(2),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(3),
                                                NameOfDepartment = reader.GetString(4)
                                            }
                                        },
                                        Subject = new Subject
                                        {
                                            CodeOfSubject = reader.GetInt32(5),
                                            NameOfSubject = reader.GetString(6),
                                            Specific = reader.GetString(7),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(8),
                                                NameOfDepartment = reader.GetString(9)
                                            }
                                        },
                                        NumberOfClassroom = new ClassRoom
                                        {
                                            CodeOfClassroom = reader.GetInt32(10),
                                            NumberOfClassroom = reader.GetString(11),
                                            Specifics = reader.GetString(12),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(13),
                                                NameOfDepartment = reader.GetString(14)
                                            }
                                        },
                                        Group = new Group
                                        {
                                            CodeOfGroup = reader.GetInt32(15),
                                            NameOfGroup = reader.GetString(16),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(17),
                                                NameOfDepartment = reader.GetString(18),
                                            }
                                        },
                                        Specifics = reader.GetString(19),
                                        Ndindex = reader.GetInt32(20)
                                    },
                                    Info = info,
                                    State = reader.GetInt32(20)
                                };
                            }
                            else if (reader.GetInt32(20) == -1)
                            {
                                yield return new DropItem(key, type, info)
                                {
                                    ItemTwo = new DropInformation
                                    {
                                        Teacher = new Teacher
                                        {
                                            CodeOfTeacher = reader.GetInt32(0),
                                            FIO = reader.GetString(1),
                                            Post = reader.GetString(2),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(3),
                                                NameOfDepartment = reader.GetString(4)
                                            }
                                        },
                                        Subject = new Subject
                                        {
                                            CodeOfSubject = reader.GetInt32(5),
                                            NameOfSubject = reader.GetString(6),
                                            Specific = reader.GetString(7),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(8),
                                                NameOfDepartment = reader.GetString(9)
                                            }
                                        },
                                        NumberOfClassroom = new ClassRoom
                                        {
                                            CodeOfClassroom = reader.GetInt32(10),
                                            NumberOfClassroom = reader.GetString(11),
                                            Specifics = reader.GetString(12),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(13),
                                                NameOfDepartment = reader.GetString(14)
                                            }
                                        },
                                        Group = new Group
                                        {
                                            CodeOfGroup = reader.GetInt32(15),
                                            NameOfGroup = reader.GetString(16),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(17),
                                                NameOfDepartment = reader.GetString(18),
                                            }
                                        },
                                        Specifics = reader.GetString(19),
                                        Ndindex = reader.GetInt32(20)
                                    },
                                    Info = info,
                                    State = reader.GetInt32(20)
                                };
                            }
                        }
                    }
                    dbtran.Commit();
                }
            }
        }

        public IEnumerable<TeachersAndSubjects> ReadTeacherAndSubjects()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_teacher, fio, post, subjectlist, daylist, id_TAndS, id_department, name_of_department from (TeachersAndSubjects join Teachers using(id_teacher) join departments using(id_department))";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            var ls = JsonConvert.DeserializeObject<Subject[]>(reader.GetString(3));
                            var ld = JsonConvert.DeserializeObject<DayOfWeek[]>(reader.GetString(4));

                            yield return new TeachersAndSubjects
                            {
                                Teacher = new Teacher
                                {
                                    CodeOfTeacher = reader.GetInt32(0),
                                    FIO = reader.GetString(1),
                                    Post = reader.GetString(2),
                                    Department = new Department
                                    {
                                        CodeOfDepartment = reader.GetInt32(6),
                                        NameOfDepartment = reader.GetString(7)
                                    }
                                },
                                CodeOftands = reader.GetInt32(5),
                                SubjectList = ls,
                                DayList = ld
                            };
                        }
                    }
                    dbtran.Commit();
                }
            }
        }
    }
}
//public object Key { get; set; }
//public Type KeyType { get; set; }
//public PairInfo Info { get; set; }
//private DropInformation item;
//private DropInformation itemTwo;
//private int n_dIndex;
//private int state = 0;