using dotnet_rpg.DTOs.Character;
using dotnet_rpg.DTOs.Weapon;

namespace dotnet_rpg.Services.WeaponService
{
    public interface IWeaponService
    {
        /// This interface gets 1 method returning a GetCharacter DTO and taking an AddWeaponDto
        Task<ServiceResponse<GetCharacterResponseDto>> AddWeapon(AddWeaponRequestDto newWeapon);
    }
}