using Menu.Classes;
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
using static System.Net.Mime.MediaTypeNames;
using static System.Reflection.Metadata.BlobBuilder;

namespace Menu.Pages
{
    /// <summary>
    /// Shoppage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Shoppage : Page
    {
        private Shop shop = new();
        private Character character;
        public Shoppage()
        {
            InitializeComponent();
            InitializeProp();
            equipmentList.ItemsSource = shop.EquipmentList;
            foodList.ItemsSource = shop.FoodList;
            character = MainWindow.MyCharacter;
            MessageBox.Show(character.Money.ToString());
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
                //money.Text = character.Money.ToString(); // 돈 갱신

            }
            else
            {
                MessageBox.Show($"{prop.Name} 을(를) 구매할 수 없습니다.");
            }

            // 선택 해제
            equipmentList.SelectedItem = null;
            foodList.SelectedItem = null;

        }

        private void sell_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InitializeProp()
        {
            //장비 추가
            shop.EquipmentList.Add(new Equipment("황금 헬멧", 5000, 1, "\\Images\\equiptment\\helmet.png", 100, 20));
            shop.EquipmentList.Add(new Equipment("황금 갑옷", 5000, 1, "\\Images\\equiptment\\tunic.png", 100, 20));
            shop.EquipmentList.Add(new Equipment("황금 바지", 5000, 1, "\\Images\\equiptment\\trousers.png", 100, 20));
            shop.EquipmentList.Add(new Equipment("황금 부츠", 5000, 1, "\\Images\\equiptment\\boots.png", 100, 20));
            shop.EquipmentList.Add(new Equipment("나무 곡괭이", 1000, 1, "\\Images\\equiptment\\pickaxe1.png", 10, 50));
            shop.EquipmentList.Add(new Equipment("돌 곡괭이", 2000, 1, "\\Images\\equiptment\\pickaxe2.png", 20, 40));
            shop.EquipmentList.Add(new Equipment("철 곡괭이", 3000, 1, "\\Images\\equiptment\\pickaxe3.png", 30, 30));
            shop.EquipmentList.Add(new Equipment("금 곡갱이", 5000, 1, "\\Images\\equiptment\\pickaxe4.png", 50, 20));
            shop.EquipmentList.Add(new Equipment("다이아 곡괭이", 8000, 1, "\\Images\\equiptment\\pickaxe5.png", 80, 5));
            shop.EquipmentList.Add(new Equipment("탈출 보트", 1000000, 1, "\\Images\\equiptment\\boat", 0, 0));


            //음식 추가
            shop.FoodList.Add(new Food("사과", 1000, 5, "\\Images\\food\\apple.png", 10));
            shop.FoodList.Add(new Food("케이크", 2000, 5, "\\Images\\food\\cake.png", 20));
            shop.FoodList.Add(new Food("당근", 1000, 5, "\\Images\\food\\carrot.png", 10));
            shop.FoodList.Add(new Food("후렴과", 3000, 5, "\\Images\\food\\chorusFruit.png", 30));
            shop.FoodList.Add(new Food("수박 조각", 1000, 5, "\\Images\\food\\melonSlice.png", 10));
            shop.FoodList.Add(new Food("감자", 1000, 5, "\\Images\\food\\Potato.png", 10));
            shop.FoodList.Add(new Food("연어", 2000, 5, "\\Images\\food\\salmon.png", 20));
            shop.FoodList.Add(new Food("꿀이 든 병", 3000, 5, "\\Images\\food\\honeyBottle.png", 30));
            shop.FoodList.Add(new Food("기본 물약", 3000, 5, "\\Images\\food\\basePotion.png", 30));
            shop.FoodList.Add(new Food("행운의 물약", 5000, 5, "\\Images\\food\\luckPotion.png", 50));
        }
    }
}
