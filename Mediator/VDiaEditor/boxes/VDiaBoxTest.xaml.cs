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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mediator.VDiaEditor.boxes
{
    /// <summary>
    /// Логика взаимодействия для VDiaBoxTest.xaml
    /// </summary>
    public partial class VDiaBoxTest : UserControl
    {
        public VDiaBoxTest()
        {
            InitializeComponent();
        }

        public void setText(string s)
        {
            text.Text = s;
        }
    }
}
