using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Threading;
using DrawingTablet.Core;
using Microsoft.AspNetCore.SignalR.Client;

namespace TabletTestWPF
{
    public class SignalRConnection
    {
        public static HubConnection Connection { get; set; }
        public SignalRConnection()
        {
            DoStuff();
        }

        async void DoStuff()
        {
            var dt = DateTime.Now;

            Connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:55332/")
                .Build();

            while (true)
            {
                try
                {
                    await Connection.StartAsync();
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                }
                if (Connection.State == HubConnectionState.Connected)
                    break;
            }

            Connection.On<byte[]>("NewDrawing", (arr) =>
            {
                lock (MainWindow.LockObject)

                    using (var ms = new MemoryStream(arr))
                        SimpleStylus.Dispatcher.Invoke(() =>
                        MainWindow.SimpleStylus.InkPresent.Strokes.Add(new StrokeCollection(ms)));
            });

            var allDrawings = await Connection.InvokeAsync<List<byte[]>>("GetAllDrawing");
            lock (MainWindow.LockObject)
                foreach (var drawing in allDrawings)
                    using (var ms = new MemoryStream(drawing))
                        SimpleStylus.Dispatcher.Invoke(() =>
                    MainWindow.SimpleStylus.InkPresent.Strokes.Add(new StrokeCollection(ms)));

        }
    }
}
