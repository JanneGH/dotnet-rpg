using dotnet_rpg.DTOs.Character;

namespace dotnet_rpg
{
    // TODO: Create a profile for every mapping. Currently all profiles in 1 class.
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterResponseDto>();

            CreateMap<AddCharacterRequestDto, Character>();
        }
    }
}