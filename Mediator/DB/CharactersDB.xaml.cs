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
using Mediator.Editors;

namespace Mediator.DB
{
    /// <summary>
    /// Логика взаимодействия для TextsDB.xaml
    /// </summary>
    public partial class CharactersDB : UserControl, IDbViewer<CharacterData>
    {
        Core _core;
        Db<CharacterData> _db;
        MainWindow _mainWindow;
        CharacterEditor _textEditor;

        public DbType DbType => DbType.Texts;

        public CharactersDB()
        {
            InitializeComponent();

        }

        public UIElement GetEditor()
        {
            if(_textEditor == null)
                _textEditor = new CharacterEditor();
            return _textEditor;
        }

        public void Clear()
        {
            
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var d = await _db.Create();
            await _db.Edit(d.Id);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow = (MainWindow)DataContext;
            _core = _mainWindow.Core;
            _db = (Db<CharacterData>)_core.getDb(DbType.Texts);
            _db.ConnectViewer(this);

            dataGrid.ItemsSource = _db.Cache;
        }

        private async void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                await _db.Edit(((CharacterData)dataGrid.SelectedItem).Id);
            }
        }
    }
}
