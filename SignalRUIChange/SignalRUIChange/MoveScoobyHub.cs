using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRUIChange
{
    public class MoveScoobyHub : Hub
    {
        public void moveScoobyForward()
        {
            Clients.All.runScoobyForward();
        }

        public void moveScoobyBackward()
        {
            Clients.All.runScoobyBackward();
        }

        public void moveScoobyUpward()
        {
            Clients.All.runScoobyUpward();
        }
    }
}