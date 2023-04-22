namespace dotnet_rpg.DTOs.Character
{
    public class GetCharacterResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Aladdin";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        // Relates to the Enum created for Character classes
        public RpgClass Class { get; set; } = RpgClass.Knight;
    }
}