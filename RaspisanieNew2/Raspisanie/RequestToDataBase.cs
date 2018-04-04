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
        public static System.Data.ConnectionState requestInsertIntoFaculty(FacultyVM context)
        {
            FbConnection db = new FbConnection(ConnectVM.ConnectionStr);//под вопросом
            try
            {
                db.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            if (db.State == System.Data.ConnectionState.Open)
            {
                FbCommand insertCommand = new FbCommand(string.Format("insert into faculty(id_faculty, name_of_faculty) values({0},'{1}')", context.Faculty.CodeOfFaculty, context.Faculty.NameOfFaculty), db);
                FbTransaction dbtran = db.BeginTransaction();
                insertCommand.Transaction = dbtran;
                try
                {
                    int result = insertCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    insertCommand.Dispose();
                    db.Close();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public static System.Data.ConnectionState requestUpdateFaculty(FacultyVM context, int index)
        {
            FbConnection db = new FbConnection(ConnectVM.ConnectionStr);
            try
            {
                db.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            if (db.State == System.Data.ConnectionState.Open)
            {
                FbCommand updateCommand = new FbCommand(string.Format("update faculty set id_faculty={0}, name_of_faculty='{1}' where id_faculty = {2}", context.Faculty.CodeOfFaculty, context.Faculty.NameOfFaculty, context.Faculty.CodeOfFaculty), db);
                FbTransaction dbtran = db.BeginTransaction();
                updateCommand.Transaction = dbtran;
                try
                {
                    int result = updateCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    updateCommand.Dispose();
                    db.Close();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }


        public static System.Data.ConnectionState requestDeleteFromFaculty(ObservableCollection<Faculty> context, int index)
        {
            FbConnection db = new FbConnection(ConnectVM.ConnectionStr);
            try
            {
                db.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            if (db.State == System.Data.ConnectionState.Open)
            {
                FbCommand deleteCommand = new FbCommand(string.Format("delete from faculty where id_faculty = {0}", context[index].CodeOfFaculty), db);
                FbTransaction dbtran = db.BeginTransaction();
                deleteCommand.Transaction = dbtran;
                try
                {
                    int result = deleteCommand.ExecuteNonQuery();
                    dbtran.Commit();
                    deleteCommand.Dispose();
                    db.Close();
                    return System.Data.ConnectionState.Closed;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return System.Data.ConnectionState.Open;
        }

        public static IEnumerable<Faculty> readFaculty(string connectionString)
        {
            Dictionary<int, string> faculty = new Dictionary<int, string>();
            FbConnection db = new FbConnection(connectionString);
            try
            {
                db.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            if (db.State == System.Data.ConnectionState.Open)
            {
                FbCommand selectCommand = new FbCommand("select * from faculty", db);
                FbTransaction dbtran = db.BeginTransaction();
                selectCommand.Transaction = dbtran;
                FbDataReader reader = selectCommand.ExecuteReader();

                try
                {
                    while (reader.Read())
                    {
                        faculty.Add(reader.GetInt32(0), reader.GetString(1));
                    }
                    dbtran.Commit();
                    selectCommand.Dispose();
                    db.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            foreach (KeyValuePair<int, string> facultyValue in faculty)
            {
                Console.WriteLine(facultyValue.Key + "-" + facultyValue.Value);

                yield return new Faculty
                {
                    CodeOfFaculty = facultyValue.Key,
                    NameOfFaculty = facultyValue.Value
                };
            }
        }
    }
}
