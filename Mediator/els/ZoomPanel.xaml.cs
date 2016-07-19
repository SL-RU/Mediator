using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mediator.els
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class ZoomPanel : UserControl, INotifyPropertyChanged
    {
        public double ScaleRate = 1.1;
        public bool MoveMode = false;
        public double MaxZoom = 3;
        public double MinZoom = 0.7;
        public double Zoom
        {
            get { return canvas_st.ScaleX; }
            set
            {
                double z = value;
                if (z < MinZoom) z = MinZoom;
                if (z > MaxZoom) z = MaxZoom;
                canvas_st.ScaleX = z;
                canvas_st.ScaleY = z;
                OnPropertyChanged();
            }
        }

        public ZoomPanel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #region Children
        public int chCount = 0;
        public void AddVChild(UIElement ch)
        {
            var el = new ZoomPanelItem();
            el.setContent(ch);
            el.AttachToZoomPanel(this);
            el.X = chCount * 20;
            chCount++;
        }
        #endregion

        #region Move & Zoom
        public Point _moveStartMousePos,
            _moveStartCanvasPos;

        private void parent_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point c = Mouse.GetPosition(canvas),
                p = new Point(canvas_tt.X, canvas_tt.Y);

            double x = (c.X - p.X) / Zoom;
            double y = (c.Y - p.Y) / Zoom;
            if (e.Delta > 0)
            {
                Zoom *= ScaleRate;
            }
            else
            {
                Zoom /= ScaleRate;
            }
            canvas_tt.X = c.X - x * Zoom;
            canvas_tt.Y = c.Y - y * Zoom;
        }


        private void parent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveMode = true;
            _moveStartMousePos = Mouse.GetPosition(this);
            _moveStartCanvasPos = new Point(canvas_tt.X, canvas_tt.Y);
        }

        private void parent_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveMode)
            {
                Point c = Mouse.GetPosition(this);
                double x = _moveStartMousePos.X - c.X;
                double y = _moveStartMousePos.Y - c.Y;
                canvas_tt.X = _moveStartCanvasPos.X - x;
                canvas_tt.Y = _moveStartCanvasPos.Y - y;
            }
        }

        private void parent_MouseLeave(object sender, MouseEventArgs e)
        {
            MoveMode = false;
        }

        private void parent_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MoveMode = false;
        }
        #endregion
        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
