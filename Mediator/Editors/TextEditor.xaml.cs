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

namespace Mediator.Editors
{
    /// <summary>
    /// Логика взаимодействия для TextEditor.xaml
    /// </summary>
    public partial class TextEditor : UserControl, IDbEditor<TextData>
    {
        MainWindow mainWindow;
        Core Core;
        Db<TextData> db;
        TextData txt;

        public TextData CurrentlyEditingData => txt;

        public TextEditor()
        {
            InitializeComponent();

        }

        public void Edit(TextData td)
        {
            txt = td;
            grid.DataContext = td;
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            await db.Set(txt);
        }

        private async void revert_Click(object sender, RoutedEventArgs e)
        {
            var t = await db.ReceiveData(txt.Id, false);
            db.Update(t);
        }

        public void CloseEdit()
        {
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)DataContext;
            Core = mainWindow.Core;
            db = (Db<TextData>)Core.getDb(DbType.Texts);
            db.ConnectEditor(this);

            
        }
    }
}
