using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using static DrawingTablet.Core.StrokesAndTime;

namespace TabletTestWPF
{
    public class SimpleStylus : Label
    {
        public InkPresenter InkPresenter { get; private set; }

        public DrawingAttributes CurrentAttributes
        {
            get => penRenderer.DrawingAttributes; set
            {
                penRenderer.DrawingAttributes = value;
                InkPresenter.DetachVisuals(penRenderer.RootVisual);
                InkPresenter.AttachVisuals(penRenderer.RootVisual, penRenderer.DrawingAttributes);
            }
        }

        public static new Dispatcher Dispatcher { get; private set; }

        private DynamicRenderer renderer => /*invertedPenMode ? eraserRenderer :*/ penRenderer;

        private DynamicRenderer penRenderer;
        private readonly object lockObject = new object();
        private StylusPointCollection stylusPoints = null;
        private bool invertedPenMode = false;
        private IncrementalStrokeHitTester hitter;
        private EllipseStylusShape eraserTip = new EllipseStylusShape(3, 3, 0);
        private readonly StrokeCollectionConverter converter = new StrokeCollectionConverter();

        public SimpleStylus()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            InkPresenter = new InkPresenter();

            penRenderer = new DynamicRenderer();
            Random r = new Random();
            penRenderer.DrawingAttributes.Color = Color.FromRgb((byte)r.Next(100, 256), (byte)r.Next(100, 256), (byte)r.Next(100, 256));
            InkPresenter.AttachVisuals(penRenderer.RootVisual, penRenderer.DrawingAttributes);

            StylusPlugIns.Add(penRenderer);
            hitter = InkPresenter.Strokes.GetIncrementalStrokeHitTester(eraserTip);

            StylusButtonDown += (s, e) => InvertLogic(e.Inverted, e);
            StylusEnter += (s, e) => InvertLogic(e.Inverted, e);
            StylusLeave += (s, e) => InvertLogic(e.Inverted, e);
            StylusMove += (s, e) => { if (hitter.IsValid) hitter.AddPoints(e.GetStylusPoints(this)); };
            StylusInRange += (s, e) => InvertLogic(e.Inverted, e);
            StylusInAirMove += SimpleStylus_StylusInAirMove;
            InkPresenter.Strokes.StrokesChanged += Strokes_StrokesChanged;
            Content = InkPresenter;
        }

        private void SimpleStylus_StylusInAirMove(object sender, StylusEventArgs e)
        {
            if (Parent is ScrollViewer sv)
            {
                var pos = e.StylusDevice.GetPosition(sv);
                if (pos.X + 40 > sv.ActualWidth)
                    if (sv.HorizontalOffset + sv.ActualWidth < ActualWidth)
                        sv.ScrollToHorizontalOffset(sv.HorizontalOffset + 10);
                    else
                        Width = ActualWidth + 10;

                if (pos.Y + 40 > sv.ActualHeight)
                    if (sv.VerticalOffset + sv.ActualHeight < ActualHeight)
                        sv.ScrollToVerticalOffset(sv.VerticalOffset + 10);
                    else
                        Height = ActualHeight + 10;

                if (pos.X - 40 < 0)
                    if (sv.HorizontalOffset - 20 < 0)
                        sv.ScrollToLeftEnd();
                    else
                        sv.ScrollToHorizontalOffset(sv.HorizontalOffset - 10);

                if (pos.Y - 40 < 0)
                    if (sv.VerticalOffset - 20 < 0)
                        sv.ScrollToTop();
                    else
                        sv.ScrollToVerticalOffset(sv.VerticalOffset - 10);
            }
        }

        private async void Hitter_StrokeHit(object sender, StrokeHitEventArgs e)
        {
            if (!invertedPenMode)
                return;

            var eraseResult = e.GetPointEraseResults();
            var strokesToReplace = new StrokeCollection
            {
                e.HitStroke
            };
            if (eraseResult.Count > 0)
                InkPresenter.Strokes.Replace(strokesToReplace, eraseResult);
            else
                InkPresenter.Strokes.Remove(strokesToReplace);
            var t = new Task(async () =>
            {
                using (var stroke = new MemoryStream())
                using (var erase = new MemoryStream())
                {
                    strokesToReplace.Save(stroke);
                    eraseResult.Save(erase);
                    if (SignalRConnection.Connection.State == HubConnectionState.Connected)
                        await SignalRConnection.Connection.InvokeAsync("ChangeStroke", stroke.ToArray(), erase.ToArray(), ActionType.Change);
                }
            });
            t.Start();
            await t;

        }

        public void ChangeFromServer(StrokeCollection strokeCollection, StrokeCollection changed, ActionType actionType)
        {
            InkPresenter.Strokes.StrokesChanged -= Strokes_StrokesChanged;
            switch (actionType)
            {
                case ActionType.Change:
                    var toChange = new List<int>();
                    for (int i = 0; i < MainViewModel.SimpleStylus.InkPresenter.Strokes.Count; i++)
                    {
                        var stroke = MainViewModel.SimpleStylus.InkPresenter.Strokes[i];
                        var o = strokeCollection.FirstOrDefault(x => x.StylusPoints.SequenceEqual(stroke.StylusPoints));
                        if (o == null)
                            continue;
                        toChange.Add(i);
                    }
                    if (toChange.Count > 0)
                    {
                        var toChangeCol = new StrokeCollection();
                        foreach (var i in toChange)
                        {
                            toChangeCol.Add(MainViewModel.SimpleStylus.InkPresenter.Strokes[i]);
                        }
                        MainViewModel.SimpleStylus.InkPresenter.Strokes.Replace(toChangeCol, changed);
                    }
                    break;
                case ActionType.Remove:
                    //var sc = new StrokeCollection(ms);
                    var toRemove = new List<int>();
                    for (int i = 0; i < MainViewModel.SimpleStylus.InkPresenter.Strokes.Count; i++)
                    {
                        var stroke = MainViewModel.SimpleStylus.InkPresenter.Strokes[i];
                        var o = strokeCollection.FirstOrDefault(x => x.StylusPoints.SequenceEqual(stroke.StylusPoints));
                        if (o == null)
                            continue;
                        toRemove.Add(i);
                    }
                    if (toRemove.Count > 0)
                    {
                        var toRemoveCol = new StrokeCollection();
                        foreach (var i in toRemove)
                        {
                            toRemoveCol.Add(MainViewModel.SimpleStylus.InkPresenter.Strokes[i]);
                        }
                        MainViewModel.SimpleStylus.InkPresenter.Strokes.Remove(toRemoveCol);
                    }
                    //.Remove(sc);
                    break;
                case ActionType.Add:
                    InkPresenter.Strokes.Add(strokeCollection);
                    break;
                default: break;
            }
            InkPresenter.Strokes.StrokesChanged += Strokes_StrokesChanged;
        }

        private async void Strokes_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            var t = new Task(async () =>
            {
                using (var stroke = new MemoryStream())
                {
                    if (e.Removed.Count > 0)
                    {
                        //e.Removed.Save(stroke);
                        //await SignalRConnection.Connection.InvokeAsync("ChangeStroke", stroke.ToArray(), ActionType.Remove);
                    }
                    else
                    {
                        e.Added.Save(stroke);
                        if (SignalRConnection.Connection.State == HubConnectionState.Connected)
                            await SignalRConnection.Connection.InvokeAsync("AddStroke", stroke.ToArray());
                    }
                }
            });
            t.Start();
            await t;
        }

        public void InvertLogic(bool inverted, StylusEventArgs e)
        {
            if (inverted == invertedPenMode)
                return;

            invertedPenMode = inverted;
            penRenderer.Enabled = !inverted;

            if (inverted)
            {
                eraserTip = new EllipseStylusShape(renderer.DrawingAttributes.Width, renderer.DrawingAttributes.Height);
                hitter = InkPresenter.Strokes.GetIncrementalStrokeHitTester(eraserTip);
                hitter.StrokeHit += Hitter_StrokeHit;
                hitter.AddPoints(e.GetStylusPoints(this));
            }
            else
            {
                hitter.StrokeHit -= Hitter_StrokeHit;
                hitter.EndHitTesting();
            }
        }

        static SimpleStylus()
        {
            Type owner = typeof(SimpleStylus);
        }

        //private void SendNewStroke(Stroke s)
        //{
        //    byte[] stroke;
        //    using (var ms = new MemoryStream())
        //    {
        //        new StrokeCollection(new Stroke[] { s }).Save(ms);
        //        stroke = ms.ToArray();
        //        new Task(async () =>
        //        {
        //            await SignalRConnection.Connection.InvokeAsync<byte[]>("StoreNewestDrawing", ms.ToArray());
        //        }).Start();
        //    }
        //    using (var ms = new MemoryStream(stroke))
        //    {
        //        var sc = new StrokeCollection(ms);

        //        if (sc == new StrokeCollection(new Stroke[] { s }))
        //        {
        //            ;
        //        }
        //        else
        //        {
        //            ;
        //        }

        //    }
        //}
        public void AddNewStroke(List<Stroke> s)
        {
            lock (MainWindow.LockObject)
                foreach (var item in s)
                    InkPresenter.Strokes.Add(item);
        }

        protected override void OnStylusDown(StylusDownEventArgs e)
        {
            if (invertedPenMode)
                return;
            Stylus.Capture(this);

            stylusPoints = new StylusPointCollection();
            stylusPoints.Add(e.GetStylusPoints(this, stylusPoints.Description));
            Debug.WriteLine("D: " + stylusPoints.Count);
        }

        protected override void OnStylusMove(StylusEventArgs e)
        {
            if (stylusPoints == null || invertedPenMode)
                return;

            stylusPoints.Add(e.GetStylusPoints(this, stylusPoints.Description));
            Debug.WriteLine("M: " + stylusPoints.Count);
        }

        protected override void OnStylusUp(StylusEventArgs e)
        {
            if (stylusPoints == null || invertedPenMode)
                return;

            StylusPointCollection newStylusPoints =
                e.GetStylusPoints(this, stylusPoints.Description);
            stylusPoints.Add(newStylusPoints);

            Stroke stroke = new Stroke(stylusPoints, renderer.DrawingAttributes);

            lock (MainWindow.LockObject)
                InkPresenter.Strokes.Add(stroke);
            //SendNewStroke(stroke);

            stylusPoints = null;
            Stylus.Capture(null);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (e.StylusDevice != null)
                return;

            stylusPoints = new StylusPointCollection();
            Point pt = e.GetPosition(this);
            stylusPoints.Add(new StylusPoint(pt.X, pt.Y));
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.StylusDevice != null ||
                e.LeftButton == MouseButtonState.Released ||
                stylusPoints == null)
                return;

            Point pt = e.GetPosition(this);
            stylusPoints.Add(new StylusPoint(pt.X, pt.Y));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {

            base.OnMouseLeftButtonUp(e);

            if (e.StylusDevice != null || stylusPoints == null)
                return;

            Point pt = e.GetPosition(this);
            stylusPoints.Add(new StylusPoint(pt.X, pt.Y));

            Stroke stroke = new Stroke(stylusPoints)
            {
                DrawingAttributes = renderer.DrawingAttributes
            };

            lock (MainWindow.LockObject)
                InkPresenter.Strokes.Add(stroke);
            //SendNewStroke(stroke);

            stylusPoints = null;
        }
    }
}