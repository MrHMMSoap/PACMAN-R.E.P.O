using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PACMAN_R.E.P.O.Items
{
    /// <summary>
    /// Represents an item that can be collected by the player during gameplay.
    /// Items have weight (affects carrying capacity) and value (converted to money on extraction).
    /// </summary>
    public class Item
    {
        /// <summary>Gets or sets the display name of the item.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the weight of the item (affects carrying capacity).</summary>
        public float Weight { get; set; }

        /// <summary>Gets or sets the monetary value of the item (converted to money when extracted).</summary>
        public int Value { get; set; }

        /// <summary>Gets or sets whether the item is currently being carried by the player.</summary>
        public bool IsCarried { get; set; }

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="weight">The weight of the item.</param>
        /// <param name="value">The monetary value of the item.</param>
        public Item(string name, float weight, int value)
        {
            Name = name;
            Weight = weight;
            Value = value;
            IsCarried = false;
        }
    }
}
