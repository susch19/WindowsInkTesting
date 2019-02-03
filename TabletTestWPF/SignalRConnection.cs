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
using static DrawingTablet.Core.StrokesAndTime;

namespace TabletTestWPF
{
    public class SignalRConnection
    {
        public static HubConnection Connection { get; set; }
        public SignalRConnection() => DoStuff();

        private async void DoStuff()
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

            //Connection.On<byte[]>("NewDrawing", (arr) =>
            //{
            //    lock (MainWindow.LockObject)
            //        using (var ms = new MemoryStream(arr))
            //            SimpleStylus.Dispatcher.Invoke(() =>
            //            MainWindow.SimpleStylus.ChangeFromServer(new StrokeCollection(ms)));
            //});

            Connection.On("ChangedStroke", (Action<byte[], ActionType>)((strokes, actionType) =>
            {
                ChangeStrokeFromServer(strokes, actionType);
            }));

            //var allDrawings = await Connection.InvokeAsync<List<byte[]>>("GetAllDrawing");
            //lock (MainWindow.LockObject)
            //    foreach (var drawing in allDrawings)
            //        using (var ms = new MemoryStream(drawing))
            //            SimpleStylus.Dispatcher.Invoke(() =>
            //        MainWindow.SimpleStylus.InkPresenter.Strokes.Add(new StrokeCollection(ms)));
            var allDrawingsAndDeletes = await Connection.InvokeAsync<List<(ActionType, byte[])>>("GetStrokeCollection");
            lock (MainWindow.LockObject)
                foreach (var drawing in allDrawingsAndDeletes)
                {
                    switch (drawing.Item1)
                    {
                        case ActionType.Add:
                            using (var ms = new MemoryStream(drawing.Item2))
                                SimpleStylus.Dispatcher.Invoke(() =>
                            MainWindow.SimpleStylus.InkPresenter.Strokes.Add(new StrokeCollection(ms)));
                            break;
                        //case ActionType.Change:
                        //    using (var ms = new MemoryStream(drawing.Item2))
                        //        SimpleStylus.Dispatcher.Invoke(() =>
                        //    MainWindow.SimpleStylus.InkPresenter.Strokes.Remove(new StrokeCollection(ms)));
                        //    break;
                        case ActionType.Remove:
                            using (var ms = new MemoryStream(drawing.Item2))
                                SimpleStylus.Dispatcher.Invoke(() =>
                                {
                                    var sc = new StrokeCollection(ms);
                                    MainWindow.SimpleStylus.InkPresenter.Strokes.Remove(sc);
                                });
                            //using (var ms = new MemoryStream(drawing.Item2))
                            //    SimpleStylus.Dispatcher.Invoke(() =>
                            //MainWindow.SimpleStylus.InkPresenter.Strokes.Add(new StrokeCollection(ms)));
                            break;
                        default: break;
                    }
                }


            //GetStrokeCollection
        }

        private static void ChangeStrokeFromServer(byte[] strokes, ActionType actionType)
        {

            lock (MainWindow.LockObject)
                using (var ms = new MemoryStream(strokes))

                    SimpleStylus.Dispatcher.Invoke(() =>
                    MainWindow.SimpleStylus.ChangeFromServer(new StrokeCollection(ms), actionType));
            //switch (actionType)
            //    {
            //        //case ActionType.Change:
            //        //    using (var ms = new MemoryStream(drawing.Item2))
            //        //        SimpleStylus.Dispatcher.Invoke(() =>
            //        //    MainWindow.SimpleStylus.InkPresenter.Strokes.Remove(new StrokeCollection(ms)));
            //        //    break;
            //        case ActionType.Remove:
            //                SimpleStylus.Dispatcher.Invoke(() =>
            //            MainWindow.SimpleStylus.InkPresenter.Strokes.Remove(new StrokeCollection(ms)));
            //            break;
            //        case ActionType.Add:
            //            break;
            //        default: break;
            //    }
            //using (var ms = new MemoryStream(strokes))
            //using (var eraseMS = new MemoryStream(erase))
            //{
            //    var strokesToReplace = new StrokeCollection(ms);
            //    var eraseResult = new StrokeCollection(eraseMS);

            //    SimpleStylus.Dispatcher.Invoke(() =>
            //    {
            //        if (eraseResult.Count > 0)
            //            MainWindow.SimpleStylus.InkPresenter.Strokes.Replace(strokesToReplace, eraseResult);
            //        else
            //            MainWindow.SimpleStylus.InkPresenter.Strokes.Remove(strokesToReplace);
            //    });
            //}
        }
    }
}
