using FirebirdSql.Data.FirebirdClient;
using Raspisanie.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace Raspisanie.ViewModels
{
    class WindowFacultyVM : ViewModelBase
    {
        private readonly INotifyingValue<int> index;

        private readonly INotifyCommand addCommand;
        private readonly INotifyCommand removeCommand;
        private readonly INotifyCommand editCommand;
        
        public WindowFacultyVM(ObservableCollection<Faculty> classFaculty)
        {
            addCommand = this.Factory.CommandSync(Add);
            removeCommand = this.Factory.CommandSync(Remove);
            editCommand = this.Factory.CommandSync(Edit);

            ClassFaculty = classFaculty;   

            index = this.Factory.Backing(nameof(Index), -1);
        }

        private void Add()
        {
            var context = new FacultyVM();
            var wind = new NewFaculty()
            {
                DataContext = context
            };
            wind.ShowDialog();
            if (context.Faculty != null)
            {
                FbConnection db = new FbConnection(ConnectVM.ConnectionStr);
                try
                {
                    db.Open();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                if (db.State == System.Data.ConnectionState.Open)
                {
                    FbCommand insertCommand = new FbCommand(string.Format("insert into faculty(id_faculty, name_of_faculty) values({0},'{1}')",context.Faculty.CodeOfFaculty,context.Faculty.NameOfFaculty),db);
                    FbTransaction dbtran = db.BeginTransaction();
                    insertCommand.Transaction = dbtran;
                    try
                    {
                        int result = insertCommand.ExecuteNonQuery();
                        dbtran.Commit();
                        insertCommand.Dispose();
                        db.Close();
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                if (db.State == System.Data.ConnectionState.Closed)
                {
                    ClassFaculty.Add(context.Faculty);
                }
            }
        }

        private void Edit()
        {
            if (Index >= 0)
            {
                var faculty = ClassFaculty[Index];
                var context = new FacultyVM(faculty);
                var wind = new NewFaculty()
                {
                    DataContext = context
                };
                wind.ShowDialog();
                if (context.Faculty != null)
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
                        FbCommand updateCommand = new FbCommand(string.Format("update faculty set id_faculty={0}, name_of_faculty='{1}' where id_faculty = {2}", context.Faculty.CodeOfFaculty, context.Faculty.NameOfFaculty, Index+1), db);
                        FbTransaction dbtran = db.BeginTransaction();
                        updateCommand.Transaction = dbtran;
                        try
                        {
                            int result = updateCommand.ExecuteNonQuery();
                            dbtran.Commit();
                            updateCommand.Dispose();
                            db.Close();
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message);
                        }
                    }
                    if (db.State == System.Data.ConnectionState.Closed)
                    {
                        ClassFaculty[Index] = context.Faculty;
                    }
                }
            }
        }

        private void Remove()
        {
            if (Index >= 0)
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
                    FbCommand deleteCommand = new FbCommand(string.Format("delete from faculty where id_faculty = {0}", Index+1), db);
                    FbTransaction dbtran = db.BeginTransaction();
                    deleteCommand.Transaction = dbtran;
                    try
                    {
                        int result = deleteCommand.ExecuteNonQuery();
                        dbtran.Commit();
                        deleteCommand.Dispose();
                        db.Close();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                if (db.State == System.Data.ConnectionState.Closed)
                {

                    ClassFaculty.RemoveAt(Index);
                }
            }
        }

        public ObservableCollection<Faculty> ClassFaculty { get; }
        public ICommand AddCommand => addCommand;
        public ICommand RemoveCommand => removeCommand;
        public ICommand EditCommand => editCommand;

        public int Index { get { return index.Value; } set { index.Value = value; } }
    }
}
