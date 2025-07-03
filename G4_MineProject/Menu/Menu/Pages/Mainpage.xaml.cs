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
namespace Menu.Pages
{
    /// <summary>
    /// Mainpage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Mainpage : Page
    {
        public Mainpage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.SwitchPage(MainWindow.mainWindow.shoppage);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.SwitchPage(MainWindow.mainWindow.minepage);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow.MyCharacter.Hp = 100;
        }
    }
}
