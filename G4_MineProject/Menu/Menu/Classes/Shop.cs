using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Menu.Classes
{
    public class Shop
    {
        private List<Resource> resourceList = new();
        private List<Equipment> equipmentList = new();
        private List<Food> foodList = new();

        public List<Resource> ResourceList
        {
            get { return resourceList; }
            set { resourceList = value; }
        }
        public List<Equipment> EquipmentList
        {
            get { return equipmentList; }
            set { equipmentList = value; }
        }
        public List<Food> FoodList
        {
            get { return foodList; }
            set { foodList = value; }
        }

        // 상품 판매 가능 여부 확인
        public bool CanSell(Prop prop, Character character)
        {
            // 상품 재고 확인
            if (prop.Amount < 1) return false;
            // 구매자 돈 확인
            if (character.Money < prop.Price) return false;
            return true;
        }

        // 실제 판매 처리
        public bool Sell(Prop prop, Character character)
        {
            if (!CanSell(prop, character)) return false;

            if(prop.Name == "탈출 보트")
            {
                MessageBox.Show("탈출 ~");
                var mainWindow = System.Windows.Application.Current.MainWindow as Menu.MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainFrame.Navigate(new Menu.Pages.EscapePage());
                }
                return true;
            }

            prop.Amount -= 1;
            character.Money -= prop.Price;
            //character.AddProp(prop); // 캐릭터 인벤토리에 추가
            MainWindow.mainWindow.SendProp(prop, 1);
            return true;
        }

        // 자원 구매
        public void AddResource(Resource resource)
        {
            Prop existing = resourceList.Find(p => p.Name == resource.Name);

            if (existing == null || existing.Amount >= 64)
            {
                //새로 만들어서 추가
                resourceList.Add((Resource)resource.CloneWithAmount(1));
            }
            else
            {
                existing.Amount += 1;
            }
        }
    }
}

