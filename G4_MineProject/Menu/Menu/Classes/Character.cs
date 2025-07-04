using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Menu.Classes
{
    public class Character
    {
        private int hp;
        private int health;
        private int attack;
        private int money = 50000;
        private List<Prop> inventory = new();
        private List<Equipment> equipped = new(); //착용한 장비
        public int Hp
        {
            get { return hp; }
            set { hp = value; }
        }
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        public int Attack
        {
            get { return attack; }
            set { attack = value; }
        }
        public int Money
        {
            get { return money; }
            set { money = value; }
        }
        public List<Prop> Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }
        public List<Equipment> Equipped
        {
            get { return equipped; }
            set { equipped = value; }
        }
        public Character()
        {
            hp = 100;
            health = 10;
            attack = 10;
            money = 50000;

        }

        public bool BuyProp(Shop shop, Prop prop)
        {
            return shop.Sell(prop, this);
        }

        // 인벤토리에 추가
        public void AddProp(Prop prop)
        {
            Prop existing = inventory.Find(p => p.Name == prop.Name);

            if (existing == null || existing.Amount >= 64)
            {
                //새로 만들어서 추가
                inventory.Add(prop.CloneWithAmount(1));
            }
            else
            {
                existing.Amount += 1;
            }
        }

        // 자원 판매
        public bool SellResource(Resource resource, Shop shop)
        {
            if (resource.Amount < 1) return false;

            resource.Amount -= 1;
            Money += resource.Price;
            shop.AddResource(resource);
            return true;
        }

        public void FindProp(Prop prop)
        {
            Prop existing = inventory.Find(p => p.Name == prop.Name);
        }

        // 인벤토리에서 제거
        public void RemoveProp(Prop prop)
        {
            Prop existing = inventory.Find(p => p.Name == prop.Name);

            if (existing.Amount == 1)
            {
                Inventory.Remove(existing);
            }
            else
            {
                existing.Amount -= 1;
            }
        }



    }
}
        
