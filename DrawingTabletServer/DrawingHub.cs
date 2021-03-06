﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AspNetCore.SignalR;
using static DrawingTablet.Core.StrokesAndTime;

namespace DrawingTabletServer
{
    internal class DrawingHub : Hub
    {
        public DrawingHub()
        {
            Program.DrawingManager.DrawingChanged += DrawingManager_DrawingChanged;
        }

        private void DrawingManager_DrawingChanged(object sender, List<(ActionType, byte[], byte[])> e) => 
            Clients.Others.SendAsync("DrawingChanged", e);
        public List<byte[]> GetLatestDrawing(DateTime time) => Program.DrawingManager.GetLatest(time);

        public List<byte[]> GetAllDrawing() => Program.DrawingManager.GetAll();

        public async void StoreNewestDrawing(byte[] drawings)
        {
            Program.DrawingManager.StoreNewest(drawings, DateTime.Now);
            await Clients.Others.SendAsync("NewDrawing", drawings);
        }

        public async void AddStroke(byte[] strokes)
        {
            ChangeStroke(strokes, new byte[0], ActionType.Add);
            //Program.DrawingManager.StoreNewStroke(strokes);
            //await Clients.Others.SendAsync("NewDrawing", strokes);
        }
        public async void ChangeStroke(byte[] strokes, byte[] changed, ActionType actionType)
        {
            Program.DrawingManager.ChangeStrokes(strokes, changed, actionType);
            await Clients.Others.SendAsync("ChangedStroke", strokes, changed, actionType);
        }

        public List<(ActionType, byte[], byte[])> GetStrokeCollection() => Program.DrawingManager.GetStrokeCollection();
    }
}