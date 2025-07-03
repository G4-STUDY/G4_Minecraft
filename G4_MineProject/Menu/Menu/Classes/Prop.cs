using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menu.Classes
{
    public class Prop
    {
        private string name;
        private int price;
        private int amount;
        private string imgsource;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int Price
        {
            get { return price; }
            set { price = value; }
        }
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        public string Imgsource
        {
            get { return imgsource; }
            set { imgsource = value; }
        }
        public Prop(string name, int price, int amount, string imgsource)
        {
            this.name = name;
            this.price = price;
            this.amount = amount;
            this.imgsource = imgsource;
        }

    }

    public class Resource : Prop
    {
        public Resource(string name, int price, int amount, string imgsource) : base(name, price, amount,imgsource)
        {
            
        }
        //??
    }
    public class Equipment : Prop
    {
        private int force; //공격력
        private int damage; //HP 깎이는 정도
        public int Force
        {
            get { return force; }
            set { force = value; }
        }
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        public Equipment(string name, int price, int amount, string imgsource, int force, int damage) : base(name, price, amount, imgsource)
        {
            this.force = force;
            this.damage = damage;
        }
        //구매
        //착용
        //공격력
    }
    public class Food : Prop
    {
        private int healnum;
        public int Healnum
        {
            get { return healnum; }
            set { healnum = value; }
        }
        public Food(string name, int price, int amount, string imgsource) : base(name, price, amount, imgsource)
        {
            //음식 먹기
        }
        //??
    }
}
