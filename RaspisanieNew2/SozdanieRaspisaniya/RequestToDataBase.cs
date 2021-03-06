﻿using FirebirdSql.Data.FirebirdClient;
using Models;
using Newtonsoft.Json;
using SozdanieRaspisaniya.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
                        selectCommand.CommandText = "select id_classroom, number_of_classroom, specific, id_department, name_of_department, id_faculty, name_of_faculty from (classrooms join departments using (id_department) join faculty using (id_faculty))";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            yield return new ClassRoom
                            {
                                CodeOfClassroom = reader.GetInt32(0),
                                NumberOfClassroom = reader.GetString(1),
                                Specific = reader.GetString(2),
                                Department = new Department
                                {
                                    CodeOfDepartment = reader.GetInt32(3),
                                    NameOfDepartment = reader.GetString(4),
                                    Faculty = new Faculty
                                    {
                                        CodeOfFaculty = reader.GetInt32(5),
                                        NameOfFaculty = reader.GetString(6)

                                    }
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
                        selectCommand.CommandText = "select id_subject, name_of_subject, id_department , name_of_department, id_faculty, name_of_faculty from (subjects join departments using(id_department) join faculty using (id_faculty)) order by name_of_subject";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            yield return new Subject
                            {
                                CodeOfSubject = reader.GetInt32(0),
                                NameOfSubject = reader.GetString(1),
                                Department = new Department
                                {
                                    CodeOfDepartment = reader.GetInt32(2),
                                    NameOfDepartment = reader.GetString(3),
                                    Faculty = new Faculty
                                    {
                                        CodeOfFaculty = reader.GetInt32(4),
                                        NameOfFaculty = reader.GetString(5)

                                    }
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
                        selectCommand.CommandText = "select id_teacher, fio, post, mail, isreadlecture, id_department, name_of_department, id_faculty, name_of_faculty from (teachers join teachersanddepartments using(id_teacher) join departments using(id_department) join faculty using (id_faculty)) order by fio";
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
                                IsReadLecture = Convert.ToBoolean(reader.GetInt32(4)),
                                Department = new Department
                                {
                                    CodeOfDepartment = reader.GetInt32(5),
                                    NameOfDepartment = reader.GetString(6),
                                    Faculty = new Faculty
                                    {
                                        CodeOfFaculty = reader.GetInt32(7),
                                        NameOfFaculty = reader.GetString(8)

                                    }
                                }
                            };
                        }
                    }
                    dbtran.Commit();

                }
            }
        }

        public IEnumerable<Group> ReadGroups(int term)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_group, name_of_group, term, id_department, name_of_department, id_faculty, name_of_faculty from (groups join departments using(id_department) join faculty using (id_faculty)) where term = @Term order by name_of_group";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        selectCommand.Parameters.AddWithValue("@Term",term);
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            yield return new Group
                            {
                                CodeOfGroup = reader.GetInt32(0),
                                NameOfGroup = reader.GetString(1),
                                Term = reader.GetInt32(2),
                                Department = new Department
                                {
                                    CodeOfDepartment = reader.GetInt32(3),
                                    NameOfDepartment = reader.GetString(4),
                                    Faculty = new Faculty
                                    {
                                        CodeOfFaculty = reader.GetInt32(5),
                                        NameOfFaculty = reader.GetString(6)
                                    }
                                },
                            };
                        }
                    }
                    dbtran.Commit();
                }
            }
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

        public List<string> ReadFromClasses()
        {
            List<string> nameofschedule = new List<string>();
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select nameofschedule from Classes group by nameofschedule";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            nameofschedule.Add(reader.GetString(0));
                        }
                    }
                    dbtran.Commit();
                    return nameofschedule;
                }
            }
            return null;
            
        }

        public bool clearClasses(string nameOfSchedule)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from Classes where nameofschedule = @nameofschedule";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;

                            deleteCommand.Parameters.AddWithValue("@nameofschedule", nameOfSchedule);
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

        public bool requestInsertIntoClassesItemOne(DropItem item,string nameOfSchedule)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into Classes(nameofschedule, id_teacher, id_departmentsteacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@Nameofschedule, @CodeOfTeacher, @id_departmentsteacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@Nameofschedule", nameOfSchedule);
                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher", item.Item.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@id_departmentsteacher", item.Item.Teacher.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", item.Item.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", item.Item.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", item.Item.Group.Single().CodeOfGroup);
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

        public bool requestInsertIntoClassesItemTwo(DropItem itemTwo, string nameOfSchedule)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into Classes(nameofschedule, id_teacher, id_departmentsteacher, id_subject, id_classroom, id_group, specifics, daytime, pair, numerator_denominator, keyy, typekey) values (@Nameofschedule, @CodeOfTeacher, @id_departmentsteacher, @CodeOfSubject, @CodeOfClassroom, @CodeOfGroup, @Specifics, @Day, @Time, @Num_den, @Key, @KeyType)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@Nameofschedule", nameOfSchedule);
                            insertCommand.Parameters.AddWithValue("@CodeOfTeacher", itemTwo.ItemTwo.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@id_departmentsteacher", itemTwo.ItemTwo.Teacher.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@CodeOfSubject", itemTwo.ItemTwo.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@CodeOfClassroom", itemTwo.ItemTwo.NumberOfClassroom.CodeOfClassroom);
                            insertCommand.Parameters.AddWithValue("@CodeOfGroup", itemTwo.ItemTwo.Group.Single().CodeOfGroup);
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

        public IEnumerable<DropItem> ReadClasses(Group[] groups, string nameofschedule)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select Classes.id_teacher, fio, post, mail, id_departmentsteacher, d1.name_of_department, f1.id_faculty, f1.name_of_faculty," + //7
                            "id_subject, name_of_subject, subjects.id_department, d2.name_of_department, f2.id_faculty, f2.name_of_faculty," + //13                           
                            "id_classroom, number_of_classroom, classrooms.specific, classrooms.id_department, d3.name_of_department, f3.id_faculty, f3.name_of_faculty," + //20
                            "id_group, name_of_group, groups.id_department, d4.name_of_department, f4.id_faculty, f4.name_of_faculty," +//26
                            "specifics, NUMERATOR_DENOMINATOR, pair, daytime, keyy, typekey, isreadlecture" +//33
                            " from ((((Classes join teachers using (id_teacher) join departments d1 on d1.id_department = Classes.id_departmentsteacher join faculty f1 on d1.id_faculty = f1.id_faculty join teachersanddepartments on teachersanddepartments.id_department = d1.id_department and teachersanddepartments.id_teacher = Classes.id_teacher)" +
                                "join subjects using (id_subject) join departments d2 on d2.id_department = subjects.id_department join faculty f2 on d2.id_faculty = f2.id_faculty)" +
                                "join classrooms using (id_classroom) join departments d3 on d3.id_department = classrooms.id_department join faculty f3 on d3.id_faculty = f3.id_faculty)" +
                                "join groups using (id_group) join departments d4 on d4.id_department = groups.id_department join faculty f4 on d4.id_faculty = f4.id_faculty) where nameofschedule = @nameofschedule";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        selectCommand.Parameters.AddWithValue("@nameofschedule", nameofschedule);
                        FbDataReader reader = selectCommand.ExecuteReader();

                        while (reader.Read())
                        {
                            PairInfo info = new PairInfo(reader.GetInt32(29), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), reader.GetString(30)));
                            var type = typeof(Group);
                            var key = ReturnObjectFromCollections(reader.GetString(31), groups);
                            if ((reader.GetInt32(28) == 0) || (reader.GetInt32(28) == 1))
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
                                            Mail = reader.GetString(3),
                                            IsReadLecture = Convert.ToBoolean(reader.GetInt32(33)),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(4),
                                                NameOfDepartment = reader.GetString(5),
                                                Faculty = new Faculty
                                                {
                                                    CodeOfFaculty = reader.GetInt32(6),
                                                    NameOfFaculty = reader.GetString(7)
                                                }
                                            }
                                        },
                                        Subject = new Subject
                                        {
                                            CodeOfSubject = reader.GetInt32(8),
                                            NameOfSubject = reader.GetString(9),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(10),
                                                NameOfDepartment = reader.GetString(11),
                                                Faculty = new Faculty
                                                {
                                                    CodeOfFaculty = reader.GetInt32(12),
                                                    NameOfFaculty = reader.GetString(13)
                                                }
                                            }
                                        },
                                        NumberOfClassroom = new ClassRoom
                                        {
                                            CodeOfClassroom = reader.GetInt32(14),
                                            NumberOfClassroom = reader.GetString(15),
                                            Specific = reader.GetString(16),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(17),
                                                NameOfDepartment = reader.GetString(18),
                                                Faculty = new Faculty
                                                {
                                                    CodeOfFaculty = reader.GetInt32(19),
                                                    NameOfFaculty = reader.GetString(20)
                                                }
                                            }
                                        },
                                        Group = new List<Group> {
                                        {
                                                new Group
                                                {
                                                    CodeOfGroup = reader.GetInt32(21),
                                                    NameOfGroup = reader.GetString(22),
                                                    Department = new Department
                                                    {
                                                        CodeOfDepartment = reader.GetInt32(23),
                                                        NameOfDepartment = reader.GetString(24),
                                                        Faculty = new Faculty
                                                        {
                                                            CodeOfFaculty = reader.GetInt32(25),
                                                            NameOfFaculty = reader.GetString(26)
                                                        }
                                                    }
                                                }

                                        }
                                        },
                                        Specifics = reader.GetString(27),
                                        Ndindex = reader.GetInt32(28)
                                    },
                                    Info = info,
                                    State = reader.GetInt32(28)
                                };
                            }
                            else if (reader.GetInt32(28) == -1)
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
                                            Mail = reader.GetString(3),
                                            IsReadLecture = Convert.ToBoolean(reader.GetInt32(33)),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(4),
                                                NameOfDepartment = reader.GetString(5),
                                                Faculty = new Faculty
                                                {
                                                    CodeOfFaculty = reader.GetInt32(6),
                                                    NameOfFaculty = reader.GetString(7)
                                                }
                                            }
                                        },
                                        Subject = new Subject
                                        {
                                            CodeOfSubject = reader.GetInt32(8),
                                            NameOfSubject = reader.GetString(9),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(10),
                                                NameOfDepartment = reader.GetString(11),
                                                Faculty = new Faculty
                                                {
                                                    CodeOfFaculty = reader.GetInt32(12),
                                                    NameOfFaculty = reader.GetString(13)
                                                }
                                            }
                                        },
                                        NumberOfClassroom = new ClassRoom
                                        {
                                            CodeOfClassroom = reader.GetInt32(14),
                                            NumberOfClassroom = reader.GetString(15),
                                            Specific = reader.GetString(16),
                                            Department = new Department
                                            {
                                                CodeOfDepartment = reader.GetInt32(17),
                                                NameOfDepartment = reader.GetString(18),
                                                Faculty = new Faculty
                                                {
                                                    CodeOfFaculty = reader.GetInt32(19),
                                                    NameOfFaculty = reader.GetString(20)
                                                }
                                            }
                                        },
                                        Group = new List<Group> {
                                        {
                                                new Group
                                                {
                                                    CodeOfGroup = reader.GetInt32(21),
                                                    NameOfGroup = reader.GetString(22),
                                                    Department = new Department
                                                    {
                                                        CodeOfDepartment = reader.GetInt32(23),
                                                        NameOfDepartment = reader.GetString(24),
                                                        Faculty = new Faculty
                                                        {
                                                            CodeOfFaculty = reader.GetInt32(25),
                                                            NameOfFaculty = reader.GetString(26)
                                                        }
                                                    }
                                                }

                                        }
                                        },
                                        Specifics = reader.GetString(27),
                                        Ndindex = reader.GetInt32(28)
                                    },
                                    Info = info,
                                    State = reader.GetInt32(28)
                                };
                            }
                        }
                    }
                    dbtran.Commit();
                }
            }
        }

        public TeachersAndSubjects[] ReadTeacherAndSubjects()
        {
            List<TeachersAndSubjects> tands = new List<TeachersAndSubjects>();
            List<Subject> subjlist = new List<Subject>();

            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select TeachersAndSubjects.id_teacher, fio, post, mail, TeachersAndSubjects.id_department, d1.name_of_department, id_subject, name_of_subject, subjects.id_department, d2.name_of_department, daylist, isreadlecture, f1.id_faculty, f1.name_of_faculty " +
                            " from (TeachersAndSubjects join Teachers on TeachersAndSubjects.id_teacher = teachers.id_teacher join departments d1 on d1.id_department = TeachersAndSubjects.id_department join Subjects using(id_subject) join departments d2 on d2.id_department = subjects.id_department join faculty f1 on d1.id_faculty = f1.id_faculty join teachersanddepartments on teachersanddepartments.id_department = d1.id_department and teachersanddepartments.id_teacher = TeachersAndSubjects.id_teacher)";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            if (!tands.Exists(t => t.Teacher.CodeOfTeacher == reader.GetInt32(0) && t.Teacher.Department.CodeOfDepartment == reader.GetInt32(4)))
                            {
                                var ld = JsonConvert.DeserializeObject<DayOfWeek[]>(reader.GetString(10));
                                var subj = new Subject
                                {
                                    CodeOfSubject = reader.GetInt32(6),
                                    NameOfSubject = reader.GetString(7),
                                    Department = new Department
                                    {
                                        CodeOfDepartment = reader.GetInt32(8),
                                        NameOfDepartment = reader.GetString(9)
                                    }
                                };
                                subjlist.Add(subj);

                                var TeacherSubjects = new TeachersAndSubjects
                                {
                                    Teacher = new Teacher
                                    {
                                        CodeOfTeacher = reader.GetInt32(0),
                                        FIO = reader.GetString(1),
                                        Post = reader.GetString(2),
                                        Mail = reader.GetString(3),
                                        IsReadLecture = Convert.ToBoolean(reader.GetInt32(11)),
                                        Department = new Department
                                        {
                                            CodeOfDepartment = reader.GetInt32(4),
                                            NameOfDepartment = reader.GetString(5),
                                            Faculty = new Faculty
                                            {
                                                CodeOfFaculty = reader.GetInt32(12),
                                                NameOfFaculty = reader.GetString(13)
                                            }
                                        }
                                    },
                                    SubjectList = subjlist.ToArray(),
                                    DayList = ld
                                };
                                tands.Add(TeacherSubjects);
                                subjlist.Clear();
                            }
                            else
                            {
                                var subj = new Subject
                                {
                                    CodeOfSubject = reader.GetInt32(6),
                                    NameOfSubject = reader.GetString(7),
                                    Department = new Department
                                    {
                                        CodeOfDepartment = reader.GetInt32(8),
                                        NameOfDepartment = reader.GetString(9)
                                    }
                                };
                                foreach (var teacher in tands)
                                {
                                    if (teacher.Teacher.CodeOfTeacher == reader.GetInt32(0) && teacher.Teacher.Department.CodeOfDepartment == reader.GetInt32(4))
                                    {
                                        var temp = teacher.SubjectList.Append(subj);
                                        teacher.SubjectList = temp.ToArray();
                                    }

                                }
                            }
                        }
                    }
                    dbtran.Commit();
                    return tands.ToArray();
                }
            }
            return null;
        }

        public GroupsAndSubjects[] ReadGroupsAndSubjects(int term)
        {
            List<GroupsAndSubjects> grandsb = new List<GroupsAndSubjects>();
            List<SubjectInform> subjlist = new List<SubjectInform>();
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_group, name_of_group, term, groups.id_department, d1.name_of_department, id_subject, name_of_subject, subjects.id_department, d2.name_of_department, lecturehour, exercisehour, labhour,  f1.id_faculty, f1.name_of_faculty " +
                            "from (GroupsAndSubjects join Groups using(id_group) join departments d1 on d1.id_department = groups.id_department join Subjects using(id_subject) join departments d2 on d2.id_department = subjects.id_department join faculty f1 on d1.id_faculty = f1.id_faculty)  where term = @Term";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
                        selectCommand.Parameters.AddWithValue("@Term", term);
                        FbDataReader reader = selectCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            if (!grandsb.Exists(g => g.Group.CodeOfGroup == reader.GetInt32(0)))
                            {
                                var sbinf = new SubjectInform
                                {
                                    Subject = new Subject
                                    {
                                        CodeOfSubject = reader.GetInt32(5),
                                        NameOfSubject = reader.GetString(6),
                                        Department = new Department
                                        {
                                            CodeOfDepartment = reader.GetInt32(7),
                                            NameOfDepartment = reader.GetString(8)
                                        }
                                    },
                                    LectureHour = reader.GetInt32(9),
                                    ExerciseHour = reader.GetInt32(10),
                                    LaboratoryHour = reader.GetInt32(11)
                                };
                                subjlist.Add(sbinf);

                                var GroupsSubjects = new GroupsAndSubjects
                                {
                                    Group = new Group
                                    {
                                        CodeOfGroup = reader.GetInt32(0),
                                        NameOfGroup = reader.GetString(1),
                                        Term = reader.GetInt32(2),
                                        Department = new Department
                                        {
                                            CodeOfDepartment = reader.GetInt32(3),
                                            NameOfDepartment = reader.GetString(4),
                                            Faculty = new Faculty
                                            {
                                                CodeOfFaculty = reader.GetInt32(12),
                                                NameOfFaculty = reader.GetString(13)
                                            }
                                        }
                                    },
                                    InformationAboutSubjects = subjlist.ToArray()
                                };
                                grandsb.Add(GroupsSubjects);
                                subjlist.Clear();
                            }
                            else
                            {
                                var sbinf = new SubjectInform
                                {
                                    Subject = new Subject
                                    {
                                        CodeOfSubject = reader.GetInt32(5),
                                        NameOfSubject = reader.GetString(6),
                                        Department = new Department
                                        {
                                            CodeOfDepartment = reader.GetInt32(7),
                                            NameOfDepartment = reader.GetString(8)
                                        }
                                    },
                                    LectureHour = reader.GetInt32(9),
                                    ExerciseHour = reader.GetInt32(10),
                                    LaboratoryHour = reader.GetInt32(11)
                                };
                                foreach (var group in grandsb)
                                {
                                    if (group.Group.CodeOfGroup == reader.GetInt32(0))
                                    {
                                        var temp = group.InformationAboutSubjects.Append(sbinf);
                                        group.InformationAboutSubjects = temp.ToArray();
                                    }

                                }

                            }
                        }
                    }
                    dbtran.Commit();
                    return grandsb.ToArray();
                }
            }
            return null;
        }
    }
}
