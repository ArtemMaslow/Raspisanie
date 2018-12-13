using FirebirdSql.Data.FirebirdClient;
using SozdanieRaspisaniya.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SozdanieRaspisaniya
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            var path = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "XMLConfig.xml";
            var ci = new ConnectionInfo();
            if (System.IO.File.Exists(path))
            {
                ci = XMLConfig.ReadDatabaseValue(path);
            }
            
            var connectVm = new ConnectVM(ci);
            var connect = new ConnectToDataBase { DataContext = connectVm };
            connect.ShowDialog();

            if (!connect.Result) { Environment.Exit(0); }
            Console.WriteLine(ci.Semestr);
            FbConnectionStringBuilder builder = new FbConnectionStringBuilder
            {
                Database = ci.DB,
                Password = ci.Password,
                UserID = ci.Login,
            };
            RequestToDataBase req = RequestToDataBase.getOrCreateInstance(builder.ConnectionString);
            req.Open();
            Console.WriteLine(ci.Semestr);
            var context = new MainVM(ci.Semestr);
            var app = new MainWindow { DataContext = context };

            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            app.Show();
            req.Close();

        }
    }
}
