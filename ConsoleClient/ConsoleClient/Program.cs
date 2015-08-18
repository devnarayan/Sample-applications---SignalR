using Microsoft.AspNet.SignalR.Client.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var hubConnection = new HubConnection("http://localhost:53748");
            var chat = hubConnection.CreateHubProxy("ChatHub");
            chat.On<string, string>("broadcastMessage", (name, message) => { Console.Write(name + ": "); Console.WriteLine(message); });
            hubConnection.Start().Wait();
            chat.Invoke("Notify", "Console app", hubConnection.ConnectionId);
            string msg = null;

            while ((msg = Console.ReadLine()) != null)
            {
                chat.Invoke("Send", "Console app", msg).Wait();
            }
        }
    }
}
