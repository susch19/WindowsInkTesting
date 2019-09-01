using System;
using System.Collections.Generic;
using System.Drawing;
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

        public static object LockObject { get; internal set; }

        private MainViewModel mainViewModel;
        public MainWindow()
        {
            LockObject = new object();
            InitializeComponent();

            mainViewModel = new MainViewModel(simpleStylus);
            this.DataContext = mainViewModel;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //using (var fs = File.OpenWrite("save_" + DateTime.Now.ToString("dd.MM.yyyy_hh.mm.ss") + ".drawing"))
            //{
            //    SimpleStylus.InkPresenter.Strokes.Save(fs);
            //}
        }

        private void OpacitySlider(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //if (SimpleStylus == null)
            //    return;
            //var dr = SimpleStylus.CurrentAttributes.Clone();
            //SimpleStylus.CurrentAttributes = dr;
        }

        private void DrawerScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scroll = ((ScrollViewer)sender);
            if (e.ExtentHeightChange > 0)
            {
                if (e.ExtentHeightChange + scroll.VerticalOffset >= scroll.ScrollableHeight)
                    scroll.ScrollToVerticalOffset(scroll.ScrollableHeight);
                else
                    scroll.ScrollToVerticalOffset(scroll.VerticalOffset + e.ExtentHeightChange);
            }
            if (e.ExtentWidthChange > 0)
            {
                if (e.ExtentWidthChange + scroll.HorizontalOffset >= scroll.ScrollableWidth)
                    scroll.ScrollToHorizontalOffset(scroll.ScrollableWidth);
                else
                    scroll.ScrollToHorizontalOffset(scroll.HorizontalOffset + e.ExtentWidthChange);

            }
        }
    }
}
