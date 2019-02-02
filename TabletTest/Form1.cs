using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;

namespace TabletTest
{
    public partial class Form1 : Form
    {
      

        public Form1()
        {
            InitializeComponent();

            var ic = new InkControl();

            this.Controls.Add(ic);

        }
    }
}
