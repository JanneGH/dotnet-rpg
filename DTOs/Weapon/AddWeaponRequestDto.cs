namespace dotnet_rpg.DTOs.Weapon
{
    public class AddWeaponRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public int CharacterId { get; set; }
    }
}