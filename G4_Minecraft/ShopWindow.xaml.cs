using G4_Minecraft;
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

namespace G4_WPF_PROJECT
{
    /// <summary>
    /// ShopWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShopWindow : Page
    {
        private Shop shop = new();
        private Character character = ((App)Application.Current).mainCharacter;

        public ShopWindow()
        {
            InitializeComponent();
            InitializeProp();

            equipmentList.ItemsSource = shop.EquipmentList;
            foodList.ItemsSource = shop.FoodList;
            inventoryList.ItemsSource = character.Inventory;
            money.Text = character.Money.ToString();
        }

        private void InitializeProp()
        {
            //장비 추가
            shop.EquipmentList.Add(new Equipment("장비1", 5000, 10, "resource\\ex1.jpg", 100, 10));
            shop.EquipmentList.Add(new Equipment("장비2", 7000, 5, "resource\\ex2.jpg", 200, 5));
            shop.EquipmentList.Add(new Equipment("장비3", 9000, 2, "resource\\ex1.jpg", 300, 2));

            //음식 추가
            shop.FoodList.Add(new Food("음식1", 1000, 5, "resource\\ex1.jpg", 10));
            shop.FoodList.Add(new Food("음식2", 500, 3, "resource\\ex1.jpg", 5));
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            Equipment selectedEquipment = equipmentList.SelectedItem as Equipment;
            Food selectedFood = foodList.SelectedItem as Food;

            Prop prop = null;

            if (selectedEquipment != null)
            {
                prop = selectedEquipment;
            }
            else if (selectedFood != null)
            {
                prop = selectedFood;
            }

            if (prop == null)
            {
                MessageBox.Show("아이템을 선택해주세요.");
                return;
            }

            bool result = character.BuyProp(shop, prop);
            if (result)
            {
                MessageBox.Show($"{prop.Name} 을(를) 구매했습니다!");
                money.Text = character.Money.ToString(); // 돈 갱신

            }
            else
            {
                MessageBox.Show($"{prop.Name} 을(를) 구매할 수 없습니다.");
            }

            // 선택 해제
            equipmentList.SelectedItem = null;
            foodList.SelectedItem = null;

        }

        private void MyPage_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyPageWindow.xaml", UriKind.Relative));
        }

        private void sell_Click(object sender, RoutedEventArgs e)
        {

            Prop prop = inventoryList.SelectedItem as Prop;
            if (prop == null)
            {
                MessageBox.Show("아이템을 선택해주세요.");
                return;
            }

            if (prop is Resource resource)
            {
                bool result = character.SellResource(resource, shop);
                if (result)
                {
                    MessageBox.Show($"{resource.Name} 을(를) 판매했습니다!");
                    money.Text = character.Money.ToString(); // 돈 갱신
                }
                else
                {
                    MessageBox.Show($"{resource.Name} 을(를) 판매할 수 없습니다.");
                }
            }
            else
            {
                MessageBox.Show("자원(Resource)만 판매할 수 있습니다.");
            }

            // 선택 해제
            inventoryList.SelectedItem = null;
        }
    }
}
