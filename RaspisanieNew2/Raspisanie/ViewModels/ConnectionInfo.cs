namespace Raspisanie.ViewModels
{
    public class ConnectionInfo
    {
        public string DB { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public static ConnectionInfo Default =>
            new ConnectionInfo
            {
                DB = "",
                Login = "SYSDBA",
                Password = "masterkey"
            };
    }
}
