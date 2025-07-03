using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menu.Classes
{
    class Monster
    {
        public string Name { get; set; } //썪은 나무, 은 ...
        public int Hp { get; set; } //대상 체력
        public int RewardAmout { get; set; }


        public Monster(string name, int hp, int rewardamout)
        {
            Name = name;
            Hp = hp;
            RewardAmout = rewardamout;
        }
    }
}
