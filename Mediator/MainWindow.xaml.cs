using System;
using System.Collections.Generic;
using System.Globalization;
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
using Infralution.Localization.Wpf;
using Mediator.DB;
using Mediator.Editors;

namespace Mediator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Core Core;

        public List<IDbViewer<IData>> Viewers;
        public List<UIElement> Editors;

        public MainWindow(Core core)
        {
            //CultureManager.UICulture = new CultureInfo("en");
            Core = core;
            DataContext = this;
            InitializeComponent();

            createTabs();

            core.GetCache();
            
        }
        /// <summary>
        /// Create db viewers and editors tabs
        /// </summary>
        private void createTabs()
        {
            Viewers = new List<IDbViewer<IData>>();
            Editors = new List<UIElement>();

            Viewers.Add(new TextsDB());
            //Add new viewer here

            foreach (var viewer in Viewers)
            {
                //Viewer
                TabItem ti = new TabItem();
                ti.Header = locale.Main.ResourceManager.GetString(viewer.DbType.ToString());
                ti.Content = viewer;
                DbViewersTabs.Items.Add(ti);
                //Editor
                ti = new TabItem();
                ti.Header = locale.Main.ResourceManager.GetString(viewer.DbType.ToString());
                ti.Content = viewer.GetEditor();
                Editors.Add(viewer.GetEditor());
                DbEditorsTabs.Items.Add(ti);
            }
        }

    }
}
