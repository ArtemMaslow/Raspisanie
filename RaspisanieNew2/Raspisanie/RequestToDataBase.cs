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


        public System.Data.ConnectionState requestInsertIntoFaculty(FacultyVM context)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into faculty(id_faculty, name_of_faculty) values({0},'{1}')", context.Faculty.CodeOfFaculty, context.Faculty.NameOfFaculty), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return System.Data.ConnectionState.Closed;

            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestUpdateFaculty(FacultyVM context,ObservableCollection<Faculty> ccontext, int index)
        {

            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update faculty set id_faculty={0}, name_of_faculty='{1}' where id_faculty = {2}", context.Faculty.CodeOfFaculty, context.Faculty.NameOfFaculty, ccontext[index].CodeOfFaculty), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return System.Data.ConnectionState.Closed;
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestDeleteFromFaculty(ObservableCollection<Faculty> context, int index)
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
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                return System.Data.ConnectionState.Closed;
            }
            return System.Data.ConnectionState.Open;
        }

        public IEnumerable<Department> ReadDepartments()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select * from departments", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Department
                    {
                        CodeOfDepartment = reader.GetInt32(0),
                        NameOfDepartment = reader.GetString(1),
                        CodeOfFaculty = reader.GetInt32(2)
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }

        public System.Data.ConnectionState requestInsertIntoDepartment(DepartmentVM context)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into departments(id_department, name_of_department, id_faculty) values({0},'{1}',{2})", context.Department.CodeOfDepartment, context.Department.NameOfDepartment, context.Department.CodeOfFaculty), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestUpdateDepartment(DepartmentVM context,ObservableCollection<Department> ccontext, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update departments set id_department={0}, name_of_department='{1}', id_faculty = {2} where id_department = {3}", context.Department.CodeOfDepartment, context.Department.NameOfDepartment, context.Department.CodeOfFaculty,ccontext[index].CodeOfDepartment), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestDeleteFromDepartment(ObservableCollection<Department> ccontext, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from departments where id_department = {0}", ccontext[index].CodeOfDepartment), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public IEnumerable<ClassRoom> ReadClassrooms()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select * from classrooms", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new ClassRoom
                    {
                        CodeOfClassroom = reader.GetInt32(0),
                        NumberOfClassroom = reader.GetString(1),
                        CodeOfDepartment = reader.GetInt32(2),
                        Specifics = reader.GetString(3)
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }



        public System.Data.ConnectionState requestInsertIntoClassroom(ClassroomVM context)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into classrooms(id_classroom, number_of_classroom, id_department, specific) values({0},'{1}',{2},'{3}')", context.ClassRoom.CodeOfClassroom, context.ClassRoom.NumberOfClassroom, context.ClassRoom.CodeOfDepartment, context.ClassRoom.Specifics), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestUpdateClassroom(ClassroomVM context, ObservableCollection<ClassRoom> ccontext, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update classrooms set id_classroom={0}, number_of_classroom='{1}', id_department = {2}, specific = '{3}' where id_department = {4}", context.ClassRoom.CodeOfClassroom, context.ClassRoom.NumberOfClassroom, context.ClassRoom.CodeOfDepartment, context.ClassRoom.Specifics, ccontext[index].CodeOfClassroom), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestDeleteFromClassroom(ObservableCollection<ClassRoom> ccontext, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from classrooms where id_classroom = {0}", ccontext[index].CodeOfClassroom), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }


        public IEnumerable<Subject> ReadSubjects()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select * from subjects", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Subject
                    {
                        CodeOfSubject = reader.GetInt32(0),
                        NameOfSubject = reader.GetString(1),
                        CodeOfDepartment = reader.GetInt32(2)
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }

        public System.Data.ConnectionState requestInsertIntoSubject(SubjectVM context)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into subjects(id_subject, name_of_subject, id_department) values({0},'{1}',{2})", context.Subject.CodeOfSubject, context.Subject.NameOfSubject, context.Subject.CodeOfDepartment), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestUpdateSubject(SubjectVM context, ObservableCollection<Subject> ccontext, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update subjects set id_subject={0},name_of_subject='{1}', id_department = {2} where id_subject = {3}", context.Subject.CodeOfSubject, context.Subject.NameOfSubject, context.Subject.CodeOfDepartment, ccontext[index].CodeOfSubject), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestDeleteFromSubject(ObservableCollection<Subject> ccontext, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from subjects where id_subject = {0}", ccontext[index].CodeOfSubject), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public IEnumerable<Teacher> ReadTeachers()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select * from teachers", conn);
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


        public System.Data.ConnectionState requestInsertIntoTeacher(TeacherVM context)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into teachers(id_teacher, fio, post) values({0},'{1}','{2}')", context.Teacher.CodeOfTeacher, context.Teacher.FIO, context.Teacher.Post), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestUpdateTeacher(TeacherVM context, ObservableCollection<Teacher> ccontext, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update teachers set id_teacher={0},fio='{1}', post = '{2}' where id_teacher = {3}", context.Teacher.CodeOfTeacher, context.Teacher.FIO, context.Teacher.Post, ccontext[index].CodeOfTeacher), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestDeleteFromTeacher(ObservableCollection<Teacher> ccontext, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from teachers where id_teacher = {0}", ccontext[index].CodeOfTeacher), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }




        public IEnumerable<Group> ReadGroups()
        {
            if (Open())
            {
                FbCommand selectCommand = new FbCommand("select * from groups", conn);
                FbTransaction dbtran = conn.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();
                while (reader.Read())
                {
                    yield return new Group
                    {
                        CodeOfGroup = reader.GetInt32(0),
                        NameOfGroup = reader.GetString(1),
                        CodeOfDepartment = reader.GetInt32(2)
                    };
                }
                dbtran.Commit();
                selectCommand.Dispose();
            }
        }


        public System.Data.ConnectionState requestInsertIntoGroup(GroupVM context)
        {

            if (Open())
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into groups(id_group,name_of_group, id_department) values({0},'{1}',{2})", context.Group.CodeOfGroup, context.Group.NameOfGroup, context.Group.CodeOfDepartment), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestUpdateGroup(GroupVM context, ObservableCollection<Group> ccontext, int index)
        {
            if (Open())
            {
                FbCommand updateCommand = new FbCommand(string.Format("update groups set id_group={0},name_of_group='{1}', id_department = {2} where id_group = {3}", context.Group.CodeOfGroup, context.Group.NameOfGroup, context.Group.CodeOfDepartment, ccontext[index].CodeOfGroup), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public System.Data.ConnectionState requestDeleteFromGroup(ObservableCollection<Group> ccontext, int index)
        {
            if (Open())
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from groups where id_group = {0}", ccontext[index].CodeOfGroup), conn);
                FbTransaction dbtran = conn.BeginTransaction();
                deleteCommand.Transaction = dbtran;
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }


    }

}
