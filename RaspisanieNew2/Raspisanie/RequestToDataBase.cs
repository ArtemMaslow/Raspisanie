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
            conn.Open();
            return conn.State == System.Data.ConnectionState.Open;
        }

        public bool Close()
        {
            conn.Close();
            return conn.State == System.Data.ConnectionState.Closed;
        }

        public IEnumerable<Faculty> ReadFaculty()
        {
            if (conn.State == System.Data.ConnectionState.Open)
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

            if (conn.State == System.Data.ConnectionState.Open)
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

            if (conn.State == System.Data.ConnectionState.Open)
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
            if (conn.State == System.Data.ConnectionState.Open)
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
            if (conn.State == System.Data.ConnectionState.Open)
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

            if (conn.State == System.Data.ConnectionState.Open)
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
            if (conn.State == System.Data.ConnectionState.Open)
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
            if (conn.State == System.Data.ConnectionState.Open)
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from department where id_department = {0}", ccontext[index].CodeOfDepartment), conn);
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
            if (conn.State == System.Data.ConnectionState.Open)
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
    }
}
