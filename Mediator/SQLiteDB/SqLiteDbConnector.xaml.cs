using System;
using System.Collections.Generic;
using System.Data;
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

namespace Mediator.SQLiteDB
{
    /// <summary>
    /// Логика взаимодействия для SqLiteDbConnector.xaml
    /// </summary>
    public partial class SqLiteDbConnector : UserControl
    {
        public SqLiteDbConnector()
        {
            InitializeComponent();
        }

        SqliteDbInterface iSqliteDbInterface;

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                iSqliteDbInterface = new SqliteDbInterface(Path.Text);
                iSqliteDbInterface.Create();
                await iSqliteDbInterface.Connect();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private async void connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                iSqliteDbInterface = new SqliteDbInterface(Path.Text);
                await iSqliteDbInterface.Connect();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }
            MainWindow mw = new MainWindow(new Core(iSqliteDbInterface));
            mw.Show();
        }

        private async void disconnect_Click(object sender, RoutedEventArgs e)
        {
            await iSqliteDbInterface.Close();
        }
    }
}
