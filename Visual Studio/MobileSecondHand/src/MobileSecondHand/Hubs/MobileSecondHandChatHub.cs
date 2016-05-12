/*
 * In the Application_Start method, or somewhere in app's start
 * you'll need to add the namespace inclusion:
 * 
 * using Microsoft.AspNet.SignalR;
 * 
 * and the code:
 * 
 * RouteTable.Routes.MapHubs();
 * 
 * */

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MobileSecondHand.Hubs
{
	[HubName("MobileSecondHandChatHub")]
    public class MobileSecondHandChatHub : Hub
    {
        public void Register(string message)
        {
			var user = Context.User;
            base.Clients.All.UpdateChatMessage("Witaj. Pozdro ze strony serwera :)");
        }
    }
}