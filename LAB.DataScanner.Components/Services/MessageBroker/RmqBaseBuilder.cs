using LAB.DataScanner.Components.Settings.SettingsLibrary;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public abstract class RmqBaseBuilder<T> where T : class
    {
        protected string UserName;

        protected string Password;

        protected string HostName;
          
        protected int Port;

        protected string VirtualHost;

        public RmqBaseBuilder<T> UsingDefaultConnectionSetting()
        {
            UserName = "guest";

            Password = "guest";

            HostName = "localhost";

            Port = 5672;

            VirtualHost = "/";

            return this; 
        }

        public RmqBaseBuilder<T> UsingConfigConnectionSettings(BaseSettings settings)
        {
            UserName = settings.UserName;

            Password = settings.Password;

            HostName = settings.HostName;

            Port = settings.Port;

            VirtualHost = settings.VirtualHost;

            return this;
        }

        public RmqBaseBuilder<T> UsingCustomHost(string hostName)
        {
            HostName = hostName;

            return this;
        }

        public RmqBaseBuilder<T> UsingCustomCredentials(string userName, string userPassword)
        {
            UserName = userName;

            Password = userPassword;

            return this;
        }

        public abstract T Build();
    }
}
