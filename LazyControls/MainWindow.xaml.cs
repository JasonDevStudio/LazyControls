using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> Items { get; set; } = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            for (int i = 0; i < 10000; i++)
            {
                Items.Add($"Test abcdefghjkl {i}");
            }

            this.DataContext = this;
        }
    }
}
