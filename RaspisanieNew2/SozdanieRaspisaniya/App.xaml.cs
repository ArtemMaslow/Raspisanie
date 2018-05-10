using FirebirdSql.Data.FirebirdClient;
using SozdanieRaspisaniya.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

            var ci = ConnectionInfo.Default;
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
            RequestToDataBase req = RequestToDataBase.getOrCreateInstance(builder.ConnectionString);
            req.Open();

            var context = new MainVM();
            var app = new MainWindow { DataContext = context };

            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            app.Show();
            req.Close();

        }
    }
}
