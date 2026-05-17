using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACMAN_R.E.P.O.Items
{
    public class Item
    {
        public string Name { get; set; }
        public float Weight { get; set; }
        public int Value { get; set; }
        public bool IsCarried { get; set; }

        public Item(string name, float weight, int value)
        {
            Name = name;
            Weight = weight;
            Value = value;
            IsCarried = false;
        }
    }
}
