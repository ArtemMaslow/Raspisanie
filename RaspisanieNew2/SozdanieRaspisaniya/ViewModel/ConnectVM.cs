using Microsoft.Win32;
using System;
using System.Windows.Input;
using ViewModule;
using ViewModule.CSharp;

namespace SozdanieRaspisaniya.ViewModel
{
    class ConnectVM : ViewModelBase 
    {
        private readonly INotifyingValue<string> dataBase;
        private readonly INotifyingValue<string> loggin;
        private readonly INotifyingValue<string> password;
        private readonly INotifyingValue<int> term;
        private readonly INotifyCommand connect;
        private readonly INotifyCommand getDataBaseFile;

        ConnectionInfo connectionInfo;

        public ConnectVM(ConnectionInfo ci)
        {
            Terms = new int[] { 1, 2 };
            dataBase = this.Factory.Backing(nameof(DataBase), ci.DB);
            loggin = this.Factory.Backing(nameof(Loggin), ci.Login);
            password = this.Factory.Backing(nameof(Password), ci.Password);
            term = this.Factory.Backing(nameof(Term), 1);

            connect = this.Factory.CommandSync(Connection);
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

        public void Connection()
        {
            connectionInfo.Login = Loggin;
            connectionInfo.Password = Password;
            connectionInfo.DB = DataBase;
            Console.WriteLine("подключение есть");
        }

        public ICommand Connect => connect;
        public ICommand GetFileDataBase => getDataBaseFile;

        public string DataBase { get { return dataBase.Value; } set { dataBase.Value = value; } }
        public string Loggin { get { return loggin.Value; } set { loggin.Value = value; } }
        public string Password { get { return password.Value; } set { password.Value = value; } }
        public int Term { get { return term.Value; } set { term.Value = value; } }

        public int[] Terms { get; set; }
    }
}
