using Microsoft.AspNetCore.SignalR;

namespace Game.Server.Web.Hubs
{
    public class ScoreHub : Hub
    {
        public const string Url = "/scorehub";
    }
}
