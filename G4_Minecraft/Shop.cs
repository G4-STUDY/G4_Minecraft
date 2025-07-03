using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G4_WPF_PROJECT
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
        
    }
}
