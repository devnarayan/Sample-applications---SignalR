using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SingalRTest
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
    public class ChatHub : Hub
    {
        static ConcurrentDictionary<string, string> dictionary = new ConcurrentDictionary<string, string>();
       
        public override Task OnConnected()
        {
            var version = Context.QueryString["chatversion"];
            if (version != "1.0")
            {
                Clients.Caller.notifyWrongVersion();
            }

            // Count clinet connection
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }

        // Get Authentication info.
        protected object GetAuthInfo()
        {
            var user = Context.User;
            return new
            {
                IsAuthenticated = user.Identity.IsAuthenticated,
                IsAdmin = user.IsInRole("Admin"),
                UserName = user.Identity.Name
            };
        }
        public void Notify(string name, string id)
        {
            string uname= Clients.Caller.userName;
            if (dictionary.ContainsKey(name))
            {
                Clients.Caller.differentName();
            }
            else
            {
                dictionary.TryAdd(name, id);

                foreach (KeyValuePair<String, String> entry in dictionary)
                {
                    Clients.Caller.online(entry.Key);
                }

                Clients.Others.enters(name);
            }
        }

        public void Send(string name, string message)
        {
            if (message.Contains("<script>"))
            {
                throw new HubException("This message will flow to the client", new { user = Context.User.Identity.Name, message = message });
            }
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public void sendToSpecific(string name, string message, string to)
        {
            // Call the broadcastMessage method to update clients.
            Clients.Caller.broadcastMessage(name, message);
            Clients.Client(dictionary[to]).broadcastMessage(name, message);
        }

        public override Task OnDisconnected(bool bl)
        {
            var name = dictionary.FirstOrDefault(x => x.Value == Context.ConnectionId.ToString());
            string s;
            dictionary.TryRemove(name.Key, out s);
           
            // Discrease counter of client.
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            return Clients.All.disconnected(name.Key);
        }
    }
}