using dotnet_rpg.DTOs.Character;

namespace dotnet_rpg.Services.CharacterService
{
    public interface ICharacterService
    {
        /// These methods are tasks to be able to make them async (Tasks are always async)
        /// meaning if one thread is busy you don't have to wait for it to be finished, threads can excecute asynchronously.
        /// You'd do this if you know that you'll have a large application.
        /// Make methods asynchronous by adding the await operator. (in this case in the CharacterController)

        /// They get ServiceResponse to implement that wrapper and send additional info to the frontend.

        Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacters();

        Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id);

        Task<ServiceResponse<List<GetCharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter);

        Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter);

        Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id);

        Task<ServiceResponse<GetCharacterResponseDto>> AddCharacterSkill(AddCharacterSkillRequestDto newCharacterSkill);
    }
}

/// On DTO's:

/// BEFORE: the Character Entity was used
///  Task<ServiceResponse<List<Character>>> AddCharacter(Character newCharacter);();

/// AFTER: the Dto was used
/// Task<ServiceResponse<List<GetCharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter);

