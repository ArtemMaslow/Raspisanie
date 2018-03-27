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
        public static string ConnectionStr;

        public ConnectVM()
        {
            dataBase = this.Factory.Backing(nameof(DataBase), "C:\\Users\\Artem\\Desktop\\kurs.fdb");
            loggin = this.Factory.Backing(nameof(Loggin), "SYSDBA");
            password = this.Factory.Backing(nameof(Password), "masterkey");
            
            connect = this.Factory.CommandSyncParam<Window>(Connection);
        }

        public string GetDataBaseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string fileName = "";
            if (openFileDialog.ShowDialog() == true)
            {
                fileName = openFileDialog.FileName;
            }
            return fileName;
        }

        public void Connection(Window obj)
        {
            FbConnectionStringBuilder fb_conStr = new FbConnectionStringBuilder();

            fb_conStr.UserID = Loggin;
            fb_conStr.Password = Password;
            fb_conStr.Database = DataBase;
            
            if (!string.IsNullOrWhiteSpace(DataBase) && !string.IsNullOrWhiteSpace(Loggin) && !string.IsNullOrWhiteSpace(Password))
            {
                FbConnection  fb = new FbConnection(fb_conStr.ToString());
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
                    ConnectionStr = fb_conStr.ToString();
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

        public string DataBase { get { return dataBase.Value ; } set { dataBase.Value = value; } }
        public string Loggin { get { return loggin.Value; } set { loggin.Value = value; } }
        public string Password { get { return password.Value; } set { password.Value = value; } }
    }
}
