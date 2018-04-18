using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Raspisanie.ViewModels;
using FirebirdSql.Data.FirebirdClient;

namespace Raspisanie
{

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

            var context = new MainVM(builder.ConnectionString);
            var app = new MainWindow { DataContext = context };

            this.ShutdownMode = ShutdownMode.OnLastWindowClose;
            app.Show();
        }
    }
}
