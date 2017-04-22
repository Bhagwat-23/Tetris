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

namespace Tetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainViewModel viewModel;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
            viewModel = this.DataContext as MainViewModel;
        }

        private void Window_KeyDownEvents(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                viewModel.LeftCommandHandler();
            }
            if (e.Key == Key.Right)
            {
                viewModel.RightCommandHandler();
            }
            if (e.Key == Key.Up)
            {
                // Do Nothing...
            }
            if (e.Key == Key.Down)
            {
                viewModel.LandCommandHandler();
            }
            if (e.Key == Key.A)
            {
                viewModel.RotateLeftCommandHandler();
            }
            if (e.Key == Key.S)
            {
                viewModel.RotateRightCommandHandler();
            }
        }
    }
}
