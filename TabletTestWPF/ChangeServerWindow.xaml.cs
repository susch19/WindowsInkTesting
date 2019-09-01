using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TabletTestWPF
{
    /// <summary>
    /// Interaction logic for ChangeServerWindow.xaml
    /// </summary>
    public partial class ChangeServerWindow : Window
    {
        public string LabelText { get; private set; }
        public string Answer { get; set; }

        public ChangeServerWindow()
        {
            InitializeComponent();
        }

        public void GiveRequiredData(string labelText)
        {
            LabelText = labelText;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            Answer = answertxtBox.Text;
            DialogResult = true;
        }
    }
}
