namespace dotnet_rpg.DTOs.Character
{
    public class AddCharacterRequestDto
    {
        public string Name { get; set; } = "Aladdin";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        // Relates to the Enum created for classes
        public RpgClass Class { get; set; } = RpgClass.Knight;
    }
}