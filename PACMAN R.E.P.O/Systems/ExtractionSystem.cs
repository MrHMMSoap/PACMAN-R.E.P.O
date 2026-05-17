using PACMAN_R.E.P.O.Items;
using PlayerEntity = PACMAN_R.E.P.O.Entities.Player;

namespace PACMAN_R.E.P.O.Systems
{
    public class ExtractionSystem
    {
        public int RequiredValue { get; set; } = 500;
        public int ExtractedValue { get; set; } = 0;

        public void ExtractItem(PlayerEntity player, Item item)
        {
            player.Money += item.Value;
            player.CarriedWeight -= item.Weight;

            if (player.CarriedWeight < 0)
            {
                player.CarriedWeight = 0;
            }

            ExtractedValue += item.Value;
            item.IsCarried = false;
        }

        public void AddExtractedValue(int value)
        {
            ExtractedValue += value;
        }

        public bool IsGoalComplete()
        {
            return ExtractedValue >= RequiredValue;
        }
    }
}