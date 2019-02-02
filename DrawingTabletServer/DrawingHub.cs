using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR;

namespace DrawingTabletServer
{
    internal class DrawingHub : Hub
    {
        public List<byte[]> GetLatestDrawing(DateTime time)
        {
            return Program.DrawingManager.GetLatest(time);
        }

        public List<byte[]> GetAllDrawing()
        {
            return Program.DrawingManager.GetAll();
        }

        public void StoreNewestDrawing(byte[] drawings)
        {
            Program.DrawingManager.StoreNewest(drawings, DateTime.Now);
            Clients.Others.SendAsync("NewDrawing", drawings);
        }
    }
}