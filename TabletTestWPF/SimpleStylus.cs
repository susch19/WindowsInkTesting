using System;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Windows.Threading;

namespace TabletTestWPF
{
    public class SimpleStylus : Label
    {
        public InkPresenter InkPresent { get; private set; }

        public DynamicRenderer PenRenderer
        {
            get => penRenderer; set
            {
                lock (lockObject)
                {
                    penRenderer.Enabled = false;
                    InkPresent.AttachVisuals(value.RootVisual, value.DrawingAttributes);
                    StylusPlugIns.Add(value);
                    InkPresent.DetachVisuals(penRenderer.RootVisual);
                    StylusPlugIns.Remove(penRenderer);
                    penRenderer = value;
                }
            }
        }
        public new static Dispatcher Dispatcher { get; private set; }

        private DynamicRenderer renderer => /*invertedPenMode ? eraserRenderer :*/ penRenderer;

        DynamicRenderer penRenderer;
        private object lockObject = new object();
        StylusPointCollection stylusPoints = null;
        bool invertedPenMode = false;
        IncrementalStrokeHitTester hitter;

        EllipseStylusShape eraserTip = new EllipseStylusShape(3, 3, 0);
        StrokeCollectionConverter converter = new StrokeCollectionConverter();

        public SimpleStylus()
        {
            Dispatcher = Dispatcher.CurrentDispatcher;
            InkPresent = new InkPresenter();

            //eraserRenderer = new DynamicRenderer();
            //eraserRenderer.DrawingAttributes.Color = Color.FromRgb(255, 255, 255);
            //eraserRenderer.DrawingAttributes.Width = 20;
            //eraserRenderer.DrawingAttributes.Height = 20;
            //InkPresent.AttachVisuals(eraserRenderer.RootVisual, eraserRenderer.DrawingAttributes);

            //eraserRenderer.Enabled = false;
            penRenderer = new DynamicRenderer();
            Random r = new Random();
            penRenderer.DrawingAttributes.Color = Color.FromRgb((byte)r.Next(100, 256), (byte)r.Next(100, 256), (byte)r.Next(100, 256));
            InkPresent.AttachVisuals(penRenderer.RootVisual, penRenderer.DrawingAttributes);

            StylusPlugIns.Add(penRenderer);
            //StylusPlugIns.Add(eraserRenderer);
            hitter = InkPresent.Strokes.GetIncrementalStrokeHitTester(eraserTip);

            StylusButtonDown += (s, e) => InvertLogic(e.Inverted, e);
            StylusEnter += (s, e) => InvertLogic(e.Inverted, e);
            StylusLeave += (s, e) => InvertLogic(e.Inverted, e);
            StylusMove += (s, e) => { if (hitter.IsValid) hitter.AddPoints(e.GetStylusPoints(this)); };
            Content = InkPresent;
        }

        private void Hitter_StrokeHit(object sender, StrokeHitEventArgs e)
        {
            if (!invertedPenMode)
                return;

            var eraseResult = e.GetPointEraseResults();
            var strokesToReplace = new StrokeCollection();
            strokesToReplace.Add(e.HitStroke);
            
            File.AppendAllText("eraser.txt", converter.ConvertToString(strokesToReplace)+"\r\n");
            if (eraseResult.Count > 0)
                InkPresent.Strokes.Replace(strokesToReplace, eraseResult);
            else
                InkPresent.Strokes.Remove(strokesToReplace);
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
                hitter = InkPresent.Strokes.GetIncrementalStrokeHitTester(eraserTip);
                hitter.AddPoints(e.GetStylusPoints(this));
                hitter.StrokeHit += Hitter_StrokeHit;
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

        private void SendNewStroke(Stroke s)
        {
            using (var ms = new MemoryStream())
            {
                new StrokeCollection(new Stroke[] { s }).Save(ms);
                new Task(async () =>
                {
                    await SignalRConnection.Connection.InvokeAsync<byte[]>("StoreNewestDrawing", ms.ToArray());
                }).Start();
            }
        }
        public void AddNewStroke(List<Stroke> s)
        {
            lock (MainWindow.LockObject)
                foreach (var item in s)
                    InkPresent.Strokes.Add(item);
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
                InkPresent.Strokes.Add(stroke);
            SendNewStroke(stroke);

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

            Stroke stroke = new Stroke(stylusPoints);
            stroke.DrawingAttributes = renderer.DrawingAttributes;

            lock (MainWindow.LockObject)
                InkPresent.Strokes.Add(stroke);
            SendNewStroke(stroke);

            stylusPoints = null;
        }
    }
}