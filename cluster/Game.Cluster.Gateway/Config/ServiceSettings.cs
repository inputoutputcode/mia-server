using System.Collections.ObjectModel;

using System.Fabric.Description;


namespace Game.Cluster.Gateway.Config
{
    public class ServiceSettings
    {
        private KeyedCollection<string, ConfigurationProperty> keyValues;

        public int ClientDisconnectTimeoutInMs 
        {
            get
            {
                int value;
                int.TryParse(keyValues["ClientDisconnectTimeoutInMs"].Value, out value);

                return value;
            }
        }

        public string ClientConnectionKey
        { 
            get
            {
                return keyValues["ClientConnectionKey"].Value;
            }
        }

        public ServiceSettings(ConfigurationSection configSection) 
        {
            keyValues = configSection.Parameters;
        }
    }
}
