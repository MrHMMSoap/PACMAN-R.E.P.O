using PACMAN_R.E.P.O.Items;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    /// <summary>
    /// Manages item extraction and tracks progress toward the round goal.
    /// </summary>
    public class ExtractionSystem
    {
        /// <summary>Gets or sets the required total item value to complete the round.</summary>
        public int RequiredValue { get; set; } = 500;

        /// <summary>Gets or sets the total value of items extracted so far.</summary>
        public int ExtractedValue { get; set; } = 0;

        /// <summary>
        /// Extracts an item at an extraction point, converting it to money.
        /// Updates the player's money, weight, and extraction progress.
        /// </summary>
        /// <param name="player">The player extracting the item.</param>
        /// <param name="item">The item being extracted.</param>
        public void ExtractItem(PlayerEntity player, Item item)
        {
            // Convert item value to money
            player.Money += item.Value;

            // Remove weight from player's carried load
            player.CarriedWeight -= item.Weight;

            if (player.CarriedWeight < 0)
            {
                player.CarriedWeight = 0;
            }

            // Track extraction progress
            ExtractedValue += item.Value;
            item.IsCarried = false;
        }

        /// <summary>
        /// Directly adds to the extracted value total.
        /// </summary>
        /// <param name="value">The value to add to the extraction progress.</param>
        public void AddExtractedValue(int value)
        {
            ExtractedValue += value;
        }

        /// <summary>
        /// Checks if the player has extracted enough value to complete the round.
        /// </summary>
        /// <returns>True if extracted value meets or exceeds the requirement; otherwise, false.</returns>
        public bool IsGoalComplete()
        {
            return ExtractedValue >= RequiredValue;
        }
    }
}