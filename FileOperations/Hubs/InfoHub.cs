using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileOperations.Hubs
{
    public class InfoHub:Hub
    {
        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("dinle", "Bağlantı sağlandı.");
        }
    }
}
