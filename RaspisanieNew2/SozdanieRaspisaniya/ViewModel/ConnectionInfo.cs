using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SozdanieRaspisaniya.ViewModel
{
    public class ConnectionInfo
    {
        public string DB { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        //public static ConnectionInfo Default =>
        //    new ConnectionInfo
        //    {
        //        DB = "",
        //        Login = "SYSDBA",
        //        Password = "masterkey"
        //    };
    }
}
