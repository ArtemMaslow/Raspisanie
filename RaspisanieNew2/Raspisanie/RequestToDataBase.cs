using FirebirdSql.Data.FirebirdClient;
using Raspisanie.Models;
using Raspisanie.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public bool requestInsertIntoFaculty(Faculty faculty)
        {
            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into faculty(id_faculty, name_of_faculty) values({0},'{1}')", faculty.CodeOfFaculty, faculty.NameOfFaculty), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }


        //public bool CreateTables()
        //{
        //    if (Open())
        //    {
        //        FbCommand createCommand = new FbCommand("Create table Faculty (id_faculty integer, name_of_faculty char(35),  primary key(id_faculty)).", conn);
        //        FbTransaction dbtran = conn.BeginTransaction();
        //        createCommand.Transaction = dbtran;
        //        try
        //        {
        //            int result = createCommand.ExecuteNonQuery();
        //            dbtran.Commit();
        //            createCommand.Dispose();
        //            return result > 0;
        //        }
        //        catch (Exception e)
        //        {
        //            MessageBox.Show(e.Message);
        //            return false;
        //        }
        //    }
        //    return false;
        //}


        public bool requestUpdateFaculty(Faculty faculty,ObservableCollection<Faculty> context, int index)
        {

            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update faculty set id_faculty={0}, name_of_faculty='{1}' where id_faculty = {2}", faculty.CodeOfFaculty, faculty.NameOfFaculty, context[index].CodeOfFaculty), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestDeleteFromFaculty(ObservableCollection<Faculty> context, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from faculty where id_faculty = {0}", context[index].CodeOfFaculty), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
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
                    return false;
                }               
            }
            return false;
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

        public bool requestInsertIntoDepartment(Department department)
        {
            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into departments(id_department, name_of_department, id_faculty) values({0},'{1}',{2})", department.CodeOfDepartment, department.NameOfDepartment, department.Faculty.CodeOfFaculty), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestUpdateDepartment(Department department,ObservableCollection<Department> context, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update departments set id_department={0}, name_of_department='{1}', id_faculty = {2} where id_department = {3}", department.CodeOfDepartment, department.NameOfDepartment, department.Faculty.CodeOfFaculty,context[index].CodeOfDepartment), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestDeleteFromDepartment(ObservableCollection<Department> context, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from departments where id_department = {0}", context[index].CodeOfDepartment), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    return result >0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
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



        public bool requestInsertIntoClassroom(ClassRoom classroom)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into classrooms(id_classroom, number_of_classroom, id_department, specific) values({0},'{1}',{2},'{3}')", classroom.CodeOfClassroom, classroom.NumberOfClassroom, classroom.Department.CodeOfDepartment, classroom.Specifics), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestUpdateClassroom(ClassRoom classroom, ObservableCollection<ClassRoom> context, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update classrooms set id_classroom={0}, number_of_classroom='{1}', id_department = {2}, specific = '{3}' where id_classroom = {4}", classroom.CodeOfClassroom, classroom.NumberOfClassroom, classroom.Department.CodeOfDepartment, classroom.Specifics, context[index].CodeOfClassroom), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestDeleteFromClassroom(ObservableCollection<ClassRoom> context, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from classrooms where id_classroom = {0}", context[index].CodeOfClassroom), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
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
                    return false;
                }
            }
            return false;
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

        public bool requestInsertIntoSubject(Subject subject)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into subjects(id_subject, name_of_subject, id_department,specific) values({0},'{1}',{2},'{3}')", subject.CodeOfSubject, subject.NameOfSubject, subject.Department.CodeOfDepartment,subject.Specific), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return result>0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestUpdateSubject(Subject subject, ObservableCollection<Subject> context, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update subjects set id_subject={0}, name_of_subject='{1}', id_department = {2}, specific = '{3}' where id_subject = {4}", subject.CodeOfSubject, subject.NameOfSubject, subject.Department.CodeOfDepartment,subject.Specific, context[index].CodeOfSubject), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestDeleteFromSubject(ObservableCollection<Subject> context, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from subjects where id_subject = {0}", context[index].CodeOfSubject), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
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
                    return false;
                }
            }
            return false;
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
                        Post = reader.GetString(2),
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


        public bool requestInsertIntoTeacher(Teacher teacher)
        {
            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into teachers(id_teacher, fio, post) values({0},'{1}','{2}')", teacher.CodeOfTeacher, teacher.FIO, teacher.Post), conn);
                FbCommand insertCommand2 = new FbCommand(string.Format("insert into teachersanddepartments(id_teacher, id_department) values({0},{1})", teacher.CodeOfTeacher, teacher.Department.CodeOfDepartment), conn);
                
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                insertCommand2.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    int result_2 = insertCommand2.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    insertCommand2.Dispose();
                    return result > 0 && result_2>0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                

            }
            return false;
        }

        public bool requestUpdateTeacher(Teacher teacher, ObservableCollection<Teacher> context, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update teachers set id_teacher={0},fio='{1}', post = '{2}' where id_teacher = {3}", teacher.CodeOfTeacher, teacher.FIO, teacher.Post, context[index].CodeOfTeacher), conn);
                FbCommand updateCommand2 = new FbCommand(string.Format("update teachersanddepartments set id_teacher={0},id_department = {1} where id_teacher = {2}", teacher.CodeOfTeacher, teacher.Department.CodeOfDepartment, context[index].CodeOfTeacher), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                updateCommand2.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    int result_2 = updateCommand2.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    updateCommand2.Dispose();
                    return result >0 && result_2>0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestDeleteFromTeacher(ObservableCollection<Teacher> context, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from teachers where id_teacher = {0}", context[index].CodeOfTeacher), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
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
                    return false;
                }
            }
            return false;
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


        public bool requestInsertIntoGroup(Group group)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into groups(id_group,name_of_group, id_department) values({0},'{1}',{2})", group.CodeOfGroup, group.NameOfGroup, group.Department.CodeOfDepartment), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestUpdateGroup(Group group, ObservableCollection<Group> context, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update groups set id_group={0},name_of_group='{1}', id_department = {2} where id_group = {3}", group.CodeOfGroup, group.NameOfGroup, group.Department.CodeOfDepartment, context[index].CodeOfGroup), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return result > 0;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
            }
            return false;
        }

        public bool requestDeleteFromGroup(ObservableCollection<Group> context, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from groups where id_group = {0}", context[index].CodeOfGroup), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
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
                    return false;
                }
            }
            return false;
        }

    }

}
