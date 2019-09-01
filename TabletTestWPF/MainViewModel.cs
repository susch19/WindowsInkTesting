using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace TabletTestWPF
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public static SimpleStylus SimpleStylus;

        public double PenSize
        {
            get => penSize;
            set
            {
                if (value == penSize)
                    return;

                PenSizeSlider(value);
                penSize = value;
                OnPropertyChanged();
            }
        }
        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                if (value == selectedColor)
                    return;

                PenColorChange(value);
                selectedColor = value;
                OnPropertyChanged();
            }
        }
        public bool HighlighterActive
        {
            get => highlighterActive;
            set
            {
                if (value == highlighterActive)
                    return;

                PenHighlighterChanged(value);
                highlighterActive = value;
                OnPropertyChanged();
            }
        }

        public double DrawingWidth
        {
            get => drawingWidth;
            set => SetProperty(ref drawingWidth, value);
        }
        public double DrawingHeigth
        {
            get => drawingHeigth;
            set => SetProperty(ref drawingHeigth, value);
        }

        public Action OpenClicked => ()=>OpenClickedExecuted(null,null);

        public event PropertyChangedEventHandler PropertyChanged;
        private double penSize;
        private Color selectedColor;
        private bool highlighterActive;
        private double drawingWidth;
        private double drawingHeigth;

        public MainViewModel(SimpleStylus simpleStylus)
        {
            SimpleStylus = simpleStylus;
            simpleStylus.StrokeAdded += SimpleStylus_StrokeAdded;
            selectedColor = simpleStylus.CurrentAttributes.Color;
        }

        private void SimpleStylus_StrokeAdded(object sender, System.Windows.Ink.StrokeCollection e)
        {
            var maxX = e.Max(x => x.StylusPoints.Max(y => y.X)); 
            var maxY = e.Max(x => x.StylusPoints.Max(y => y.Y));

            if (DrawingHeigth < maxY)
                DrawingHeigth = maxY;

            if (DrawingWidth < maxX)
                DrawingWidth = maxX;
        }

        private void PenSizeSlider(double newSize)
        {
            if (SimpleStylus == null)
                return;

            System.Windows.Ink.DrawingAttributes dr = SimpleStylus.CurrentAttributes.Clone();
            dr.Width = newSize;
            dr.Height = newSize;
            SimpleStylus.CurrentAttributes = dr;
        }

        private void PenColorChange(Color value)
        {
            if (SimpleStylus == null)
                return;

            System.Windows.Ink.DrawingAttributes dr = SimpleStylus.CurrentAttributes.Clone();
            dr.Color = value;
            SimpleStylus.CurrentAttributes = dr;
        }
        private void PenHighlighterChanged(bool value)
        {
            if (SimpleStylus == null)
                return;

            System.Windows.Ink.DrawingAttributes dr = SimpleStylus.CurrentAttributes.Clone();
            dr.IsHighlighter = value;
            SimpleStylus.CurrentAttributes = dr;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (value.Equals(field))
                return;
            field = value;
            OnPropertyChanged(name);
        }

        private void OpenClickedExecuted(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void OpenClickedCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}