﻿using System;
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

namespace Mediator.DB
{
    /// <summary>
    /// Логика взаимодействия для TextsDB.xaml
    /// </summary>
    public partial class TextsDB : UserControl, IDbViewer<TextData>
    {
        Core Core;
        Db<TextData> db;
        MainWindow mainWindow;

        public TextsDB()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            var d = await db.Create();
            db.Editor.Edit(d);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)DataContext;
            Core = mainWindow.Core;
            db = (Db<TextData>)Core.getDb(DbType.Texts);
            db.ConnectViewer(this);

            dataGrid.ItemsSource = db.Cache;
        }
    }
}
