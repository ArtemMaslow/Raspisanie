using System.Xml.Linq;

namespace Raspisanie.ViewModels
{
    class XMLConfig
    {
        public static XElement CreateDatabaseElement(string databasePath, string login, string password)
        {
            return new XElement("Database",
                new XElement("DatabasePath", databasePath),
                new XElement("Login", login),
                new XElement("Password", password));
        }

        public static XElement CreateMailElement(string mailloging, string mailpassword)
        {
            return new XElement("Mail",
                new XElement("MailLogin", mailloging),
                new XElement("MailPassword", mailpassword));
        }

        public static void SaveXMLConfig(string databasePath, string login, string password, string mailloging, string mailpassword, string path)
        {
            var xmlconfig = new XDocument(new XElement("root"));
            var eldatabase = CreateDatabaseElement(databasePath, login, password);
            var elmail = CreateMailElement(mailloging, mailpassword);
            xmlconfig.Root.Add(eldatabase);
            xmlconfig.Root.Add(elmail);
            var dir = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);
            xmlconfig.Save(path);
        }

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

    }
}

