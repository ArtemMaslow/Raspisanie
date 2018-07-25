using Raspisanie.Models;
using System;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;
using System.IO;

namespace Raspisanie.ViewModels
{
   
    class ConnectVM : ViewModelBase
    {
        private  readonly INotifyingValue<string> dataBase;
        private  readonly INotifyingValue<string> loggin;
        private  readonly INotifyingValue<string> password;

        private readonly INotifyCommand connect;
        private readonly INotifyCommand getDataBaseFile;
        private readonly INotifyCommand saveDataBaseFile;

        ConnectionInfo connectionInfo;

        public ConnectVM(ConnectionInfo ci)
        {
            dataBase = this.Factory.Backing(nameof(DataBase), ci.DB);
            loggin = this.Factory.Backing(nameof(Loggin),ci.Login);
            password = this.Factory.Backing(nameof(Password), ci.Password);

            connect = this.Factory.CommandSync(Connection);
            this.connectionInfo = ci;
            getDataBaseFile = this.Factory.CommandSync(SetDataBaseFile);
            saveDataBaseFile = this.Factory.CommandSync(CreateDataBase);      
        }
///////////////////////////////////////  
        public void SetDataBaseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                    dataBase.Value = openFileDialog.FileName;
            }
        }

        public void CreateDataBase()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Файл fdb|*.FDB";
            if (saveFileDialog.ShowDialog() == true)
            {
               
                dataBase.Value = saveFileDialog.FileName;
            }

            connectionInfo.Login = Loggin;
            connectionInfo.Password = Password;
            connectionInfo.DB = DataBase;

            string conStr = new FbConnectionStringBuilder
            {
                Database = connectionInfo.DB,
                Password = connectionInfo.Password,
                UserID = connectionInfo.Login,
                Dialect = 3,
                
            }.ToString();
               
            int pageSize = 4096;
            bool forcedWrites = true;
            bool overwrite = false;
            if (!File.Exists(connectionInfo.DB))
                FbConnection.CreateDatabase(conStr, pageSize, forcedWrites, overwrite);

            FbConnection con = new FbConnection(conStr);
            con.Open();

            if (con.State == System.Data.ConnectionState.Open)
            {
                FbCommand createFacultyTable = new FbCommand("Create table Faculty (id_faculty integer, name_of_faculty varchar(35),  primary key(id_faculty))", con);
                FbCommand createDepartmentsTable = new FbCommand("Create table Departments(id_department integer,  name_of_department varchar(50),  id_faculty integer,  primary key(id_department),  foreign key(id_faculty) references faculty(id_faculty) ON DELETE CASCADE)", con);
                FbCommand createGroupsTable = new FbCommand("Create table Groups(id_group integer,    name_of_group varchar(50),    id_department integer,    primary key(id_group),    foreign key(id_department) references Departments(id_department) ON DELETE CASCADE)", con);
                FbCommand createSubjectsTable = new FbCommand("Create table Subjects(id_subject integer,    name_of_subject varchar(50),    id_department integer,    specific varchar(15),    primary key(id_subject),    foreign key(id_department) references Departments(id_department) ON DELETE CASCADE)",con);
                FbCommand createTeachersTable = new FbCommand("Create table Teachers(id_teacher integer,    fio varchar(50),    post varchar(25),    primary key(id_teacher))", con);
                FbCommand createClassroomsTable = new FbCommand("Create table Classrooms(id_classroom integer,    number_of_classroom varchar(10),    id_department integer,    specific varchar(20),    primary key(id_classroom),    foreign key(id_department) references Departments(id_department) ON DELETE CASCADE)", con);
                FbCommand createTeachersAndDepartmentsTable = new FbCommand("Create table TeachersAndDepartments(id_teacher integer,    id_department integer,    primary key(id_teacher, id_department),    foreign key(id_teacher) references Teachers(id_teacher) ON DELETE CASCADE,    foreign key(id_department) references Departments(id_department) ON DELETE CASCADE)", con);
                FbCommand createSubjectAndGroups = new FbCommand("Create table SubjectAndGroups(id_group integer,    id_subject integer,    primary key(id_group, id_subject),    foreign key(id_group) references Groups(id_group) ON DELETE CASCADE,    foreign key(id_subject) references Subjects(id_subject) ON DELETE CASCADE)", con);

                FbTransaction dbtran = con.BeginTransaction();
                createFacultyTable.Transaction = dbtran;
                createDepartmentsTable.Transaction = dbtran;
                createGroupsTable.Transaction = dbtran;
                createSubjectsTable.Transaction = dbtran;
                createTeachersTable.Transaction = dbtran;
                createClassroomsTable.Transaction = dbtran;
                createTeachersAndDepartmentsTable.Transaction = dbtran;
                createSubjectAndGroups.Transaction = dbtran;
                try
                {
                    int resultFaculty = createFacultyTable.ExecuteNonQuery();
                    int resultDepartments = createDepartmentsTable.ExecuteNonQuery();
                    int resultGroups = createGroupsTable.ExecuteNonQuery();
                    int resultSubjects = createSubjectsTable.ExecuteNonQuery();
                    int resultTeachers = createTeachersTable.ExecuteNonQuery();
                    int resultClassrooms = createClassroomsTable.ExecuteNonQuery();
                    int resultTeachersAndDepartments = createTeachersAndDepartmentsTable.ExecuteNonQuery();
                    int resultSubjectAndGroups = createSubjectAndGroups.ExecuteNonQuery();

                    dbtran.Commit();
                    createFacultyTable.Dispose();
                    createDepartmentsTable.Dispose();
                    createGroupsTable.Dispose();
                    createSubjectsTable.Dispose();
                    createTeachersTable.Dispose();
                    createClassroomsTable.Dispose();
                    createTeachersAndDepartmentsTable.Dispose();
                    createSubjectAndGroups.Dispose();

                    Console.WriteLine(resultFaculty + resultDepartments+ resultGroups+ resultSubjects+ resultTeachers+ resultClassrooms+ resultTeachersAndDepartments+ resultSubjectAndGroups);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        /////////////////////////////////////////

        public void Connection()
        {
            connectionInfo.Login = Loggin;
            connectionInfo.Password = Password;
            connectionInfo.DB = DataBase;


          //  Console.WriteLine("подключение есть");
        }

        public ICommand Connect => connect;
        public ICommand GetFileDataBase => getDataBaseFile;
        public ICommand SaveFileDataBase => saveDataBaseFile;
        public string DataBase { get { return dataBase.Value ; } set { dataBase.Value = value; } }
        public string Loggin { get { return loggin.Value; } set { loggin.Value = value; } }
        public string Password { get { return password.Value; } set { password.Value = value; } }
    }
}
