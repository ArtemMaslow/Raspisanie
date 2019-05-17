using FirebirdSql.Data.FirebirdClient;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Raspisanie
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

        public bool requestInsertIntoFaculty(Faculty faculty)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into faculty(name_of_faculty) values(@NameOfFaculty) returning id_faculty";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;

                            insertCommand.Parameters.AddWithValue("@NameOfFaculty", faculty.NameOfFaculty);
                            insertCommand.Parameters.Add(new FbParameter() { Direction = System.Data.ParameterDirection.Output });

                            int result = insertCommand.ExecuteNonQuery();
                            dbtran.Commit();

                            if (result > 0)
                                faculty.CodeOfFaculty = (int)insertCommand.Parameters[1].Value;
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

        public bool requestUpdateFaculty(Faculty faculty, ObservableCollection<Faculty> context, int index)
        {

            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand updateCommand = new FbCommand())
                        {
                            updateCommand.CommandText = "update faculty set id_faculty=@CodeOfFaculty, name_of_faculty=@NameOfFaculty where id_faculty = @contextCodeOfFaculty";
                            updateCommand.Connection = conn;
                            updateCommand.Transaction = dbtran;
                            updateCommand.Parameters.AddWithValue("@contextCodeOfFaculty", context[index].CodeOfFaculty);
                            updateCommand.Parameters.AddWithValue("@CodeOfFaculty", faculty.CodeOfFaculty);
                            updateCommand.Parameters.AddWithValue("@NameOfFaculty", faculty.NameOfFaculty);

                            int result = updateCommand.ExecuteNonQuery();
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

        public bool requestDeleteFromFaculty(ObservableCollection<Faculty> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from faculty where id_faculty = @contextCodeOfFaculty";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            deleteCommand.Parameters.AddWithValue("@contextCodeOfFaculty", context[index].CodeOfFaculty);

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

        public bool requestInsertIntoDepartment(Department department)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into departments(name_of_department, id_faculty) values(@NameOfDepartment, @FacultyCodeOfFaculty) returning id_department";//
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;
                            insertCommand.Parameters.AddWithValue("@NameOfDepartment", department.NameOfDepartment);
                            insertCommand.Parameters.AddWithValue("@FacultyCodeOfFaculty", department.Faculty.CodeOfFaculty);
                            insertCommand.Parameters.Add(new FbParameter() { Direction = System.Data.ParameterDirection.Output });

                            int result = insertCommand.ExecuteNonQuery();
                            dbtran.Commit();

                            if (result > 0)
                                Console.WriteLine(insertCommand.Parameters[2].Value);
                            department.CodeOfDepartment = (int)insertCommand.Parameters[2].Value;

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

        public bool requestUpdateDepartment(Department department, ObservableCollection<Department> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand updateCommand = new FbCommand())
                        {
                            updateCommand.CommandText = "update departments set id_department = @CodeOfDepartment, name_of_department = @NameOfDepartment, id_faculty = @FacultyCodeOfFaculty where id_department = @contextCodeOfDepartment";
                            updateCommand.Connection = conn;
                            updateCommand.Transaction = dbtran;
                            updateCommand.Parameters.AddWithValue("@CodeOfDepartment", department.CodeOfDepartment);
                            updateCommand.Parameters.AddWithValue("@NameOfDepartment", department.NameOfDepartment);
                            updateCommand.Parameters.AddWithValue("@FacultyCodeOfFaculty", department.Faculty.CodeOfFaculty);
                            updateCommand.Parameters.AddWithValue("@contextCodeOfDepartment", context[index].CodeOfDepartment);

                            int result = updateCommand.ExecuteNonQuery();
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

        public bool requestDeleteFromDepartment(ObservableCollection<Department> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from departments where id_department = @contextCodeOfDepartment";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            deleteCommand.Parameters.AddWithValue("@contextCodeOfDepartment", context[index].CodeOfDepartment);

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

        public bool requestInsertIntoClassroom(ClassRoom classroom)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into classrooms( number_of_classroom, id_department, specific) values( @NumberOfClassroom, @DepartmentCodeOfDepartment, @Specifics) returning id_classroom";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;
                            insertCommand.Parameters.AddWithValue("@NumberOfClassroom", classroom.NumberOfClassroom);
                            insertCommand.Parameters.AddWithValue("@DepartmentCodeOfDepartment", classroom.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@Specifics", classroom.Specific);
                            insertCommand.Parameters.Add(new FbParameter() { Direction = System.Data.ParameterDirection.Output });

                            int result = insertCommand.ExecuteNonQuery();
                            dbtran.Commit();

                            if (result > 0)
                                classroom.CodeOfClassroom = (int)insertCommand.Parameters[3].Value;
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

        public bool requestUpdateClassroom(ClassRoom classroom, ObservableCollection<ClassRoom> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand updateCommand = new FbCommand())
                        {
                            updateCommand.CommandText = "update classrooms set id_classroom = @CodeOfClassroom, number_of_classroom = @NumberOfClassroom, id_department = @DepartmentCodeOfDepartment, specific = @Specifics where id_classroom = @contextCodeOfClassroom";
                            updateCommand.Connection = conn;
                            updateCommand.Transaction = dbtran;
                            updateCommand.Parameters.AddWithValue("@CodeOfClassroom", classroom.CodeOfClassroom);
                            updateCommand.Parameters.AddWithValue("@NumberOfClassroom", classroom.NumberOfClassroom);
                            updateCommand.Parameters.AddWithValue("@DepartmentCodeOfDepartment", classroom.Department.CodeOfDepartment);
                            updateCommand.Parameters.AddWithValue("@Specifics", classroom.Specific);
                            updateCommand.Parameters.AddWithValue("@contextCodeOfClassroom", context[index].CodeOfClassroom);

                            int result = updateCommand.ExecuteNonQuery();
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

        public bool requestDeleteFromClassroom(ObservableCollection<ClassRoom> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from classrooms where id_classroom = @contextCodeOfClassroom";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            deleteCommand.Parameters.AddWithValue("@contextCodeOfClassroom", context[index].CodeOfClassroom);

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

        public IEnumerable<Subject> ReadSubjects()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_subject, name_of_subject, id_department, name_of_department from (subjects join departments using(id_department)) order by name_of_subject";
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
                                    NameOfDepartment = reader.GetString(3)
                                }
                            };
                        }
                    }
                    dbtran.Commit();
                }
            }
        }

        public bool requestInsertIntoSubject(Subject subject)
        {

            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into subjects( name_of_subject, id_department) values( @NameOfSubject, @DepartmentCodeOfDepartment) returning id_subject";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;
                            insertCommand.Parameters.AddWithValue("@NameOfSubject", subject.NameOfSubject);
                            insertCommand.Parameters.AddWithValue("@DepartmentCodeOfDepartment", subject.Department.CodeOfDepartment);
                            insertCommand.Parameters.Add(new FbParameter() { Direction = System.Data.ParameterDirection.Output });

                            int result = insertCommand.ExecuteNonQuery();
                            dbtran.Commit();

                            if (result > 0)
                                subject.CodeOfSubject = (int)insertCommand.Parameters[2].Value;
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

        public bool requestUpdateSubject(Subject subject, ObservableCollection<Subject> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand updateCommand = new FbCommand())
                        {
                            updateCommand.CommandText = "update subjects set id_subject=@CodeOfSubject, name_of_subject=@NameOfSubject, id_department = @DepartmentCodeOfDepartment where id_subject = @contextCodeOfSubject";
                            updateCommand.Connection = conn;
                            updateCommand.Transaction = dbtran;
                            updateCommand.Parameters.AddWithValue("@CodeOfSubject", subject.CodeOfSubject);
                            updateCommand.Parameters.AddWithValue("@NameOfSubject", subject.NameOfSubject);
                            updateCommand.Parameters.AddWithValue("@DepartmentCodeOfDepartment", subject.Department.CodeOfDepartment);
                            updateCommand.Parameters.AddWithValue("@contextCodeOfSubject", context[index].CodeOfSubject);

                            int result = updateCommand.ExecuteNonQuery();
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

        public bool requestDeleteFromSubject(ObservableCollection<Subject> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from subjects where id_subject = @contextCodeOfSubject";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            deleteCommand.Parameters.AddWithValue("@contextCodeOfSubject", context[index].CodeOfSubject);

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

        public IEnumerable<Teacher> ReadTeachers()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_teacher, fio, post, mail, isreadlecture, id_department, name_of_department from (teachers join teachersanddepartments using(id_teacher) join departments using(id_department)) ORDER BY fio";
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
                                    NameOfDepartment = reader.GetString(6)
                                }
                            };
                        }
                    }
                    dbtran.Commit();

                }
            }
        }

        public bool requestInsertIntoTeacher(Teacher teacher)
        {
            if (Open())
            {
                FbTransaction dbtran = conn.BeginTransaction();
                FbCommand insertCommand = new FbCommand();
                insertCommand.CommandText = "insert into teachers(fio, mail, isreadlecture) values( @FIO, @Mail, @Isreadlecture) returning id_teacher";
                insertCommand.Connection = conn;
                insertCommand.Transaction = dbtran;
                insertCommand.Parameters.AddWithValue("@FIO", teacher.FIO);
                insertCommand.Parameters.AddWithValue("@Mail", teacher.Mail);
                insertCommand.Parameters.AddWithValue("@Isreadlecture", Convert.ToInt32(teacher.IsReadLecture));
                insertCommand.Parameters.Add(new FbParameter() { Direction = System.Data.ParameterDirection.Output });

                try
                {
                    int result = insertCommand.ExecuteNonQuery();

                    if (result > 0)
                        teacher.CodeOfTeacher = (int)insertCommand.Parameters[3].Value;
                    dbtran.Commit();
                    insertCommand.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    dbtran.Rollback();
                    return false;
                }

                FbTransaction dbtran2 = conn.BeginTransaction();
                FbCommand insertCommand2 = new FbCommand();
                insertCommand2.CommandText = "insert into teachersanddepartments(id_teacher, id_department, post) values(@CodeOfTeacher, @DepartmentCodeOfDepartment, @Post)";
                insertCommand2.Connection = conn;
                insertCommand2.Transaction = dbtran2;
                insertCommand2.Parameters.AddWithValue("@CodeOfTeacher", teacher.CodeOfTeacher);
                insertCommand2.Parameters.AddWithValue("@DepartmentCodeOfDepartment", teacher.Department.CodeOfDepartment);
                insertCommand2.Parameters.AddWithValue("@Post", teacher.Post);
                try
                {
                    int result_2 = insertCommand2.ExecuteNonQuery();
                    dbtran2.Commit();
                    insertCommand2.Dispose();
                    return result_2 > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    dbtran.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool requestInsertIntoTeacherDepartmentTwo(Teacher teacher)
        {
            if (Open())
            {
                FbTransaction dbtran = conn.BeginTransaction();
                FbCommand insertCommand2 = new FbCommand();
                insertCommand2.CommandText = "insert into teachersanddepartments(id_teacher, id_department, post) values(@CodeOfTeacher, @DepartmentCodeOfDepartment, @Post)";
                insertCommand2.Connection = conn;
                insertCommand2.Transaction = dbtran;
                insertCommand2.Parameters.AddWithValue("@CodeOfTeacher", teacher.CodeOfTeacher);
                insertCommand2.Parameters.AddWithValue("@DepartmentCodeOfDepartment", teacher.DepartmentTwo.CodeOfDepartment);
                insertCommand2.Parameters.AddWithValue("@Post", teacher.PostTwo);
                try
                {
                    int result_2 = insertCommand2.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand2.Dispose();
                    return result_2 > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    dbtran.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool requestUpdateTeacher(Teacher teacher, ObservableCollection<Teacher> context, int index)
        {
            if (Open())
            {
                FbTransaction dbtran = conn.BeginTransaction();
                FbCommand updateCommand = new FbCommand();
                updateCommand.CommandText = "update teachers set id_teacher = @CodeOfTeacher, fio = @FIO,  mail = @Mail,  isreadlecture = @Isreadlecture where id_teacher = @contextCodeOfTeacher";
                updateCommand.Connection = conn;
                updateCommand.Transaction = dbtran;
                updateCommand.Parameters.AddWithValue("@CodeOfTeacher", teacher.CodeOfTeacher);
                updateCommand.Parameters.AddWithValue("@FIO", teacher.FIO);
                updateCommand.Parameters.AddWithValue("@Mail", teacher.Mail);
                updateCommand.Parameters.AddWithValue("@Isreadlecture", Convert.ToInt32(teacher.IsReadLecture));
                updateCommand.Parameters.AddWithValue("@contextCodeOfTeacher", context[index].CodeOfTeacher);
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    dbtran.Rollback();
                    return false;
                }

                FbTransaction dbtran2 = conn.BeginTransaction();
                FbCommand updateCommand2 = new FbCommand();
                updateCommand2.CommandText = "update teachersanddepartments set id_teacher = @CodeOfTeacher, id_department = @DepartmentCodeOfDepartment, post = @Post where id_teacher = @contextCodeOfTeacher and id_department = @contextCodeOfDepartment";
                updateCommand2.Connection = conn;
                updateCommand2.Transaction = dbtran2;
                updateCommand2.Parameters.AddWithValue("@CodeOfTeacher", teacher.CodeOfTeacher);
                updateCommand2.Parameters.AddWithValue("@DepartmentCodeOfDepartment", teacher.Department.CodeOfDepartment);
                updateCommand2.Parameters.AddWithValue("@contextCodeOfTeacher", context[index].CodeOfTeacher);
                updateCommand2.Parameters.AddWithValue("@contextCodeOfDepartment", context[index].Department.CodeOfDepartment);
                updateCommand2.Parameters.AddWithValue("@Post", teacher.Post);
                try
                {
                    int result2 = updateCommand2.ExecuteNonQuery();
                    dbtran2.Commit();
                    updateCommand2.Dispose();
                    return result2 > 0;

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    dbtran2.Rollback();
                    return false;
                }
            }
            return false;
        }

        public bool requestDeleteFromTeacher(ObservableCollection<Teacher> context, int index)
        {
            if (Open())
            {
                FbTransaction dbtran = conn.BeginTransaction();
                FbCommand deleteCommand = new FbCommand();
                deleteCommand.CommandText = "delete from teachers where id_teacher = @contextCodeOfTeacher";
                deleteCommand.Connection = conn;
                deleteCommand.Transaction = dbtran;
                deleteCommand.Parameters.AddWithValue("@contextCodeOfTeacher", context[index].CodeOfTeacher);
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    dbtran.Rollback();
                    return false;
                }
            }
            return false;
        }

        public IEnumerable<Group> ReadGroups()
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_group, name_of_group, term, id_department, name_of_department from (groups join departments using(id_department))";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
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
                                    NameOfDepartment = reader.GetString(4)
                                }
                            };
                        }
                    }
                    dbtran.Commit();
                }
            }
        }

        public bool requestInsertIntoGroup(Group group)
        {

            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "insert into groups( name_of_group, term, id_department) values( @NameOfGroup, @Term, @DepartmentCodeOfDepartment) returning id_group";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;
                            insertCommand.Parameters.AddWithValue("@NameOfGroup", group.NameOfGroup);
                            insertCommand.Parameters.AddWithValue("@Term", group.Term);
                            insertCommand.Parameters.AddWithValue("@DepartmentCodeOfDepartment", group.Department.CodeOfDepartment);
                            insertCommand.Parameters.Add(new FbParameter() { Direction = System.Data.ParameterDirection.Output });

                            int result = insertCommand.ExecuteNonQuery();
                            dbtran.Commit();

                            if (result > 0)
                                group.CodeOfGroup = (int)insertCommand.Parameters[3].Value;

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

        public bool requestUpdateGroup(Group group, ObservableCollection<Group> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand updateCommand = new FbCommand())
                        {
                            updateCommand.CommandText = "update groups set name_of_group = @NameOfGroup, term = @Term, id_department = @DepartmentCodeOfDepartment where id_group = @contextCodeOfGroup";
                            updateCommand.Connection = conn;
                            updateCommand.Transaction = dbtran;
                            updateCommand.Parameters.AddWithValue("@NameOfGroup", group.NameOfGroup);
                            updateCommand.Parameters.AddWithValue("@Term", group.Term);
                            updateCommand.Parameters.AddWithValue("@DepartmentCodeOfDepartment", group.Department.CodeOfDepartment);
                            updateCommand.Parameters.AddWithValue("@contextCodeOfGroup", context[index].CodeOfGroup);

                            int result = updateCommand.ExecuteNonQuery();
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

        public bool requestDeleteFromGroup(ObservableCollection<Group> context, int index)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from groups where id_group = @contextCodeOfGroup";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            deleteCommand.Parameters.AddWithValue("@contextCodeOfGroup", context[index].CodeOfGroup);

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
                        selectCommand.CommandText = "select TeachersAndSubjects.id_teacher, fio, post, mail, TeachersAndSubjects.id_department, d1.name_of_department, id_subject, name_of_subject, subjects.id_department, d2.name_of_department, daylist, isreadlecture " +
                            " from(TeachersAndSubjects join Teachers on TeachersAndSubjects.id_teacher = teachers.id_teacher join departments d1 on d1.id_department = TeachersAndSubjects.id_department join Subjects using (id_subject) join departments d2 on d2.id_department = subjects.id_department join teachersanddepartments on teachersanddepartments.id_department = d1.id_department and teachersanddepartments.id_teacher = TeachersAndSubjects.id_teacher)";
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
                                            NameOfDepartment = reader.GetString(5)
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

        public bool requestInsertIntoTeachersAndSubjects(TeachersAndSubjects tands, Subject subject, string dayList)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "update or insert into TeachersAndSubjects(id_teacher, id_department, id_subject, daylist) values( @id_teacher, @id_department, @id_subject, @Daylist) matching(id_teacher, id_department, id_subject)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;
                            insertCommand.Parameters.AddWithValue("@id_teacher", tands.Teacher.CodeOfTeacher);
                            insertCommand.Parameters.AddWithValue("@id_department", tands.Teacher.Department.CodeOfDepartment);
                            insertCommand.Parameters.AddWithValue("@id_subject", subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@Daylist", dayList);

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

        public bool requestDeleteFromTeachersAndSubjects(TeachersAndSubjects tands)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from TeachersAndSubjects where id_teacher = @id_teacher and id_department = @id_department";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            deleteCommand.Parameters.AddWithValue("@id_teacher", tands.Teacher.CodeOfTeacher);
                            deleteCommand.Parameters.AddWithValue("@id_department", tands.Teacher.Department.CodeOfDepartment);

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

        public GroupsAndSubjects[] ReadGroupsAndSubjects()
        {
            List<GroupsAndSubjects> grandsb = new List<GroupsAndSubjects>();
            List<SubjectInform> subjlist = new List<SubjectInform>();
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    using (FbCommand selectCommand = new FbCommand())
                    {
                        selectCommand.CommandText = "select id_group, name_of_group, term, groups.id_department, d1.name_of_department, id_subject, name_of_subject, subjects.id_department, d2.name_of_department, lecturehour, exercisehour, labhour " +
                            "from (GroupsAndSubjects join Groups using(id_group) join departments d1 on d1.id_department = groups.id_department join Subjects using(id_subject) join departments d2 on d2.id_department = subjects.id_department)";
                        selectCommand.Connection = conn;
                        selectCommand.Transaction = dbtran;
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
                                            NameOfDepartment = reader.GetString(4)
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

        public bool requestInsertIntoGroupsAndSubjects(GroupsAndSubjects gands, SubjectInform subjectInform)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand insertCommand = new FbCommand())
                        {
                            insertCommand.CommandText = "update or insert into GroupsAndSubjects(id_group, id_subject, lecturehour, exercisehour, labhour ) values( @id_group, @id_subject, @lecturehour, @exercisehour, @labhour ) matching(id_group, id_subject)";
                            insertCommand.Connection = conn;
                            insertCommand.Transaction = dbtran;
                            insertCommand.Parameters.AddWithValue("@id_group", gands.Group.CodeOfGroup);
                            insertCommand.Parameters.AddWithValue("@id_subject", subjectInform.Subject.CodeOfSubject);
                            insertCommand.Parameters.AddWithValue("@lecturehour", subjectInform.LectureHour);
                            insertCommand.Parameters.AddWithValue("@exercisehour", subjectInform.ExerciseHour);
                            insertCommand.Parameters.AddWithValue("@labhour", subjectInform.LaboratoryHour);

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

        public bool requestUpdateGroupsAndSubjects(GroupsAndSubjects gands, SubjectInform subjectInform)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand updateCommand = new FbCommand())
                        {
                            updateCommand.CommandText = "update GroupsAndSubjects set lecturehour = @lecturehour, exercisehour = @exercisehour, labhour = @labhour  where id_group = @id_group and id_subject = @id_subject";
                            updateCommand.Connection = conn;
                            updateCommand.Transaction = dbtran;
                            updateCommand.Parameters.AddWithValue("@id_group", gands.Group.CodeOfGroup);
                            updateCommand.Parameters.AddWithValue("@id_subject", subjectInform.Subject.CodeOfSubject);
                            updateCommand.Parameters.AddWithValue("@lecturehour", subjectInform.LectureHour);
                            updateCommand.Parameters.AddWithValue("@exercisehour", subjectInform.ExerciseHour);
                            updateCommand.Parameters.AddWithValue("@labhour", subjectInform.LaboratoryHour);

                            int result = updateCommand.ExecuteNonQuery();
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

        public bool requestDeleteElementFromGroupsAndSubjects(GroupsAndSubjects gands, SubjectInform subjectInform)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from GroupsAndSubjects where id_group = @id_group and id_subject = @id_subject";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            //     deleteCommand.Parameters.AddWithValue("@CodeOfGands", gands.CodeOfGands);
                            deleteCommand.Parameters.AddWithValue("@id_group", gands.Group.CodeOfGroup);
                            deleteCommand.Parameters.AddWithValue("@id_subject", subjectInform.Subject.CodeOfSubject);
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

        public bool requestDeleteAllElementsFromGroupsAndSubjects(GroupsAndSubjects gands)
        {
            if (Open())
            {
                using (FbTransaction dbtran = conn.BeginTransaction())
                {
                    try
                    {
                        using (FbCommand deleteCommand = new FbCommand())
                        {
                            deleteCommand.CommandText = "delete from GroupsAndSubjects where id_group = @id_group";
                            deleteCommand.Connection = conn;
                            deleteCommand.Transaction = dbtran;
                            deleteCommand.Parameters.AddWithValue("@id_group", gands.Group.CodeOfGroup);
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
    }
}
