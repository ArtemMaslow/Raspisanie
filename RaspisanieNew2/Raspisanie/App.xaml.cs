using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Raspisanie.ViewModels;
using FirebirdSql.Data.FirebirdClient;
using System.Diagnostics;

namespace Raspisanie
{

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            FbConnectionStringBuilder builderString;
            do
            {
                var path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "XMLConfig.xml";
                var ci = ConnectionInfo.Default;
                if (System.IO.File.Exists(path))
                {
                    ci = XMLConfig.ReadDatabaseValue(path);
                }
                else
                {
                    ci = ConnectionInfo.Default;
                }
                var connectVm = new ConnectVM(ci);
                var connect = new ConnectToDataBase { DataContext = connectVm };
                connect.ShowDialog();
                if (!connect.Result) { Environment.Exit(0); }
                FbConnectionStringBuilder builder = new FbConnectionStringBuilder
                {
                    Database = ci.DB,
                    Password = ci.Password,
                    UserID = ci.Login,
                };
                builderString = builder;
            }
            while (!testConnection(builderString));

            RequestToDataBase req = RequestToDataBase.getOrCreateInstance(builderString.ConnectionString);
            req.Open();

            var context = new MainVM();
            var app = new MainWindow { DataContext = context };

            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            app.Show();
            req.Close();
        }

        private bool testConnection(FbConnectionStringBuilder connectionString)
        {
            try
            {
                FbConnection test = new FbConnection(connectionString.ConnectionString);
                test.Open();
                test.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("Проверьте правильность введенных данных !\n" + e.Message);
                return false;
            }
        }

    }


}

