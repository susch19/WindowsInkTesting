using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR;
using static DrawingTablet.Core.StrokesAndTime;

namespace DrawingTabletServer
{
    internal class DrawingHub : Hub
    {
        public List<byte[]> GetLatestDrawing(DateTime time) => Program.DrawingManager.GetLatest(time);

        public List<byte[]> GetAllDrawing() => Program.DrawingManager.GetAll();

        public async void StoreNewestDrawing(byte[] drawings)
        {
            Program.DrawingManager.StoreNewest(drawings, DateTime.Now);
            await Clients.Others.SendAsync("NewDrawing", drawings);
        }

        public async void AddStroke(byte[] strokes)
        {
            ChangeStroke(strokes, ActionType.Add);
            //Program.DrawingManager.StoreNewStroke(strokes);
            //await Clients.Others.SendAsync("NewDrawing", strokes);
        }
        public async void ChangeStroke(byte[] strokes, ActionType actionType)
        {
            Program.DrawingManager.ChangeStrokes(strokes, actionType);
            await Clients.Others.SendAsync("ChangedStroke", strokes, actionType);
        }

        public List<(ActionType, byte[])> GetStrokeCollection() => Program.DrawingManager.GetStrokeCollection();
    }
}