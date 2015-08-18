using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace SingalRTest
{
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
            return base.OnConnected();
        }

        public void Notify(string name, string id)
        {
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
            return Clients.All.disconnected(name.Key);
        }
    }
}