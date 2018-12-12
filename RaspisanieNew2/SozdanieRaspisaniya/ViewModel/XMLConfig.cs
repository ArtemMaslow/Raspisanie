using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SozdanieRaspisaniya.ViewModel
{
    class XMLConfig
    {
        public static ConnectionInfo ReadDatabaseValue(string path)
        {
            ConnectionInfo ci = new ConnectionInfo();
            if (System.IO.File.Exists(path))
            {
                var xmlconf = XDocument.Load(path);
                foreach (var database in xmlconf.Root.Elements("Database"))
                {
                    ci.DB = (string)database.Element("DatabasePath");
                    ci.Login = (string)database.Element("Login");
                    ci.Password = (string)database.Element("Password");
                    return ci;
                }
            }
            return null;
        }

        public static string ReadMailLoginValue(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xmlconf = XDocument.Load(path);
                foreach (var mail in xmlconf.Root.Elements("Mail"))
                {
                    string maillogin = (string)mail.Element("MailLogin");
                    return maillogin;
                }
            }
            return null;
        }

        public static string ReadMailPasswordValue(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xmlconf = XDocument.Load(path);
                foreach (var mail in xmlconf.Root.Elements("Mail"))
                {
                    string mailpassword = (string)mail.Element("MailPassword");
                    return mailpassword;
                }
            }
            return null;
        }

    }
}
