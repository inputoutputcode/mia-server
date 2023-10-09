using System.Text;


namespace Game.Cluster.Gateway
{
    public class RoutingManager
    {
        public void ReceiveCommand(byte[] receivedBytes)
        {
            string receivedCommand = Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length);
        }
    }
}
