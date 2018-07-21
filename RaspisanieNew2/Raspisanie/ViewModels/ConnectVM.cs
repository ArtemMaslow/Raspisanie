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

        ConnectionInfo connectionInfo;

        public ConnectVM(ConnectionInfo ci)
        {
            dataBase = this.Factory.Backing(nameof(DataBase), ci.DB);
            loggin = this.Factory.Backing(nameof(Loggin),ci.Login);
            password = this.Factory.Backing(nameof(Password), ci.Password);

            connect = this.Factory.CommandSync(Connection);
            this.connectionInfo = ci;
            getDataBaseFile = this.Factory.CommandSync(SetDataBaseFile);      
        }
///////////////////////////////////////  
        public void SetDataBaseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                if (!File.Exists(openFileDialog.FileName))
                {
                    File.Create(openFileDialog.FileName);
                }
                else
                {
                    dataBase.Value = openFileDialog.FileName;
                }
            }
        }

/////////////////////////////////////////

        public void Connection()
        {
            connectionInfo.Login = Loggin;
            connectionInfo.Password = Password;
            connectionInfo.DB = DataBase;

            string conStr = new FbConnectionStringBuilder
            {
                Database = connectionInfo.DB,
                Password = connectionInfo.Password,
                UserID = connectionInfo.Login

            }.ToString();
            

            int pageSize = 4096;
            bool forcedWrites = true;
            bool overwrite = false;
            if (!File.Exists(connectionInfo.DB))
                FbConnection.CreateDatabase(conStr, pageSize, forcedWrites, overwrite);
            // RequestToDataBase.Instance.CreateTables();

          //  Console.WriteLine("подключение есть");
        }

        public ICommand Connect => connect;
        public ICommand GetFileDataBase => getDataBaseFile;
        public string DataBase { get { return dataBase.Value ; } set { dataBase.Value = value; } }
        public string Loggin { get { return loggin.Value; } set { loggin.Value = value; } }
        public string Password { get { return password.Value; } set { password.Value = value; } }
    }
}
