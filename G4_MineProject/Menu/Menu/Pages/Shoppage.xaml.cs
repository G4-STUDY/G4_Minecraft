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
using Menu.Classes;

namespace Menu.Pages
{
    /// <summary>
    /// Shoppage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Shoppage : Page
    {
        public Shoppage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.SwitchPage(MainWindow.mainWindow.mainpage);
        }
        private void Inventory_Send(object sender, RoutedEventArgs e)
        {
           // Prop exprop = new Prop();//프랍생성
            //MainWindow.mainWindow.SendProp(exprop);
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
