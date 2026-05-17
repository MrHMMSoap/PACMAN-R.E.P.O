namespace PACMAN_R.E.P.O.Save
{
    public class SaveData
    {
        public int Round { get; set; } = 1;
        public int Money { get; set; } = 0;
        public int Health { get; set; } = 100;

        public int SpeedLevel { get; set; } = 0;
        public int StrengthLevel { get; set; } = 0;
        public int StaminaLevel { get; set; } = 0;
        public int HealthLevel { get; set; } = 0;

        public string MapFile { get; set; } = "";
    }
}
