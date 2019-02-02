using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TabletTestWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static SimpleStylus SimpleStylus;
        public static object LockObject { get; internal set; }
        public MainWindow()
        {
            var con = new SignalRConnection();
            LockObject = new object();
            InitializeComponent();

            SimpleStylus = simpleStylus;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            using (var fs = File.OpenWrite("save_" + DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss") + ".drawing"))
            {
                SimpleStylus.InkPresent.Strokes.Save(fs);
            }
        }

        private void DrawerButton_Click(object sender, RoutedEventArgs e)
        {
            DrawerGrid.Visibility = Visibility.Hidden;
            DrawerOpenButton.Visibility = Visibility.Visible;
            ParentDrawerGrid.Width = 40;
            ParentDrawerGrid.Height = 40;
        }

        private void DrawerOpenButton_Click(object sender, RoutedEventArgs e)
        {
            DrawerGrid.Visibility = Visibility.Visible;
            DrawerOpenButton.Visibility = Visibility.Hidden;
            ParentDrawerGrid.Width = 200;
            ParentDrawerGrid.Height = Height;
        }

        private void PenSizeSlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SimpleStylus == null)
                return;
            var dr = SimpleStylus.PenRenderer.Clone();
            dr.DrawingAttributes.Width = e.NewValue;
            dr.DrawingAttributes.Height = e.NewValue;
            SimpleStylus.PenRenderer = dr;
        }
        private void OpacitySlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SimpleStylus == null)
                return;
            var dr = SimpleStylus.PenRenderer.Clone();
            SimpleStylus.PenRenderer = dr;
        }
        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (SimpleStylus == null || !e.NewValue.HasValue)
                return;
            var dr = SimpleStylus.PenRenderer.Clone();
            dr.DrawingAttributes.Color = e.NewValue.Value;
            SimpleStylus.PenRenderer = dr;
        }

        private void HighlighterCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (SimpleStylus == null)
                return;

            var dr = SimpleStylus.PenRenderer.Clone();
            dr.DrawingAttributes.IsHighlighter = ((CheckBox)sender).IsChecked.Value;
            SimpleStylus.PenRenderer = dr;
        }

    }
}
