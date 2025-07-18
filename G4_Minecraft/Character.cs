﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace G4_WPF_PROJECT
{
    public class Character
    {
        private int hp;
        private int attack;
        private int money = 50000;
        private List<Prop> inventory = new();
        private List<Equipment> equipped = new(); //착용한 장비
        public int Hp
        {
            get { return hp; }
            set { hp = value; }
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

        public void useProp(Prop prop)
        {
            if(prop is Resource)
            {
                MessageBox.Show("자원 착용 불가");
                return;
            }

            if(prop is Food)
            {
                Food food = (Food)prop;
                Hp += food.Healnum;
            }
            else if(prop is Equipment)
            {
                Equipment equipment = (Equipment)prop;
                attack = equipment.Force;
                //hp 적게 주는 것 반영 필요
            }
            
            RemoveProp(prop);
        }

    }
}
