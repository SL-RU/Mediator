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

namespace Mediator.elements
{
    /// <summary>
    /// Логика взаимодействия для TextDataElement.xaml
    /// </summary>
    public partial class TextDataElement : UserControl
    {
        public TextDataElement()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            status_field.Text = "ID: " + ((TextData) DataContext).Id;
        }
    }
}
