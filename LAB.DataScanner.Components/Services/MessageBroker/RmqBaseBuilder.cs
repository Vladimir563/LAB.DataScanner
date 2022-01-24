using Microsoft.Extensions.Configuration;

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

        public RmqBaseBuilder<T> UsingConfigConnectionSettings(IConfigurationSection configurationSection)
        {
            UserName = configurationSection.GetSection("UserName").Value;

            Password = configurationSection.GetSection("Password").Value;

            HostName = configurationSection.GetSection("HostName").Value;

            Port = int.Parse(configurationSection.GetSection("Port").Value);

            VirtualHost = configurationSection.GetSection("VirtualHost").Value;

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
