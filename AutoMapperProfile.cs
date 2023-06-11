using dotnet_rpg.DTOs.Character;
using dotnet_rpg.DTOs.Fight;
using dotnet_rpg.DTOs.Skill;
using dotnet_rpg.DTOs.Weapon;

namespace dotnet_rpg
{
    /// Automapper: 
    /// Reusable way to transfer data from one object to another.
    /// instead of object1.prop = object2.prop to transfer manually each time.
    /// Map the entity to the Dto to select what information you want to pass.
    // TODO: Create a profile for every mapping. Currently all profiles in 1 class.
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterResponseDto>();

            CreateMap<AddCharacterRequestDto, Character>();

            /// Map the Weapon entity (source) to the WeaponresponseDto (destination)
            /// Get the complete character back with the related weapon. 
            /// Before creating it here, it was possible to add a weapon to a characer ID but the response Model did not show the WeaponResponseDto properties.
            CreateMap<Weapon, GetWeaponResponseDto>();

            CreateMap<Skill, GetSkillResponseDto>();

            CreateMap<Character, HighScoreResultDto>();
        }
    }
}