using Raspisanie.Models;
using System;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;
using System.Windows;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Win32;

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

            connect = this.Factory.CommandSyncParam<Window>(Connection);
            this.connectionInfo = ci;
            getDataBaseFile = this.Factory.CommandSync(SetDataBaseFile);      
        }

        public void SetDataBaseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                dataBase.Value = openFileDialog.FileName;
            }
        }

        public void Connection(Window obj)
        {
            connectionInfo.Login = Loggin;
            connectionInfo.Password = Password;
            connectionInfo.DB = DataBase;

            
            if (!string.IsNullOrWhiteSpace(DataBase) && !string.IsNullOrWhiteSpace(Loggin) && !string.IsNullOrWhiteSpace(Password))
            {
                FbConnection  fb = new FbConnection();
                try
                {
                    fb.Open();
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                if (fb.State == System.Data.ConnectionState.Open)
                {
                    obj.Close();
                    Console.WriteLine("Подключение работает");
                }
                else
                {
                    Console.WriteLine(fb.State.ToString());
                }                
            }
           
        }

        public ICommand Connect => connect;
        public ICommand GetFileDataBase => getDataBaseFile;
        public string DataBase { get { return dataBase.Value ; } set { dataBase.Value = value; } }
        public string Loggin { get { return loggin.Value; } set { loggin.Value = value; } }
        public string Password { get { return password.Value; } set { password.Value = value; } }
    }
}
