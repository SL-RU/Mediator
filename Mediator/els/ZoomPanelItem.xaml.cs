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

namespace Mediator.els
{
    /// <summary>
    /// Логика взаимодействия для ZoomPanelItem.xaml
    /// </summary>
    public partial class ZoomPanelItem : UserControl
    {
        ZoomPanel parent;
        public ZoomPanelItem()
        {
            InitializeComponent();
        }

        public void setContent(UIElement el)
        {
            this.AddChild(el);
        }

        public void AttachToZoomPanel(ZoomPanel z)
        {
            parent = z;
            z.canvas.Children.Add(this);
            X = 0;
            Y = 0;
        }

        bool MoveMode = false;
        Point _MoveStartMouse, _MoveStartPos;

        public double X
        {
            get { return Canvas.GetLeft(this); }
            set { Canvas.SetLeft(this, value); }
        }
        public double Y
        {
            get { return Canvas.GetTop(this); }
            set { Canvas.SetTop(this, value); }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveMode = true;
            _MoveStartPos = new Point(X, Y);
            _MoveStartMouse = Mouse.GetPosition(parent);
            e.Handled = true;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveMode)
            {
                var c = Mouse.GetPosition(parent);
                X = _MoveStartPos.X + (- _MoveStartMouse.X + c.X) / parent.Zoom;
                Y = _MoveStartPos.Y + (- _MoveStartMouse.Y + c.Y) / parent.Zoom;
                e.Handled = true;
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MoveMode = false;
            e.Handled = true;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            MoveMode = false;
            e.Handled = true;
        }
    }
}
