using Microsoft.Extensions.Configuration;

namespace LAB.DataScanner.Components.Services.MessageBroker
{
    public abstract class RmqBaseBuilder<T> where T : class
    {
        protected string userName;

        protected string password;

        protected string hostName;

        protected int port;

        protected string virtualHost;

        public RmqBaseBuilder<T> UsingDefaultConnectionSetting()
        {
            userName = "guest";

            password = "guest";

            hostName = "localhost";

            port = 5672;

            virtualHost = "/";

            return this; 
        }

        public RmqBaseBuilder<T> UsingConfigConnectionSettings(IConfigurationSection configurationSection)
        {
            userName = configurationSection.GetSection("UserName").Value;

            password = configurationSection.GetSection("Password").Value;

            hostName = configurationSection.GetSection("HostName").Value;

            port = int.Parse(configurationSection.GetSection("Port").Value);

            virtualHost = configurationSection.GetSection("VirtualHost").Value;

            return this;
        }

        public RmqBaseBuilder<T> UsingCustomHost(string hostName)
        {
            this.hostName = hostName;

            return this;
        }

        public RmqBaseBuilder<T> UsingCustomCredentials(string userName, string userPassword)
        {
            this.userName = userName;

            this.password = userPassword;

            return this;
        }

        public abstract T Build();
    }
}
