using dotnet_rpg.DTOs.Character;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>() {
            new Character(),
            new Character { Id = 1, Name = "Ali Baba" }
        };

        private readonly IMapper _mapper;
        public CharacterService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

            var character = _mapper.Map<Character>(newCharacter);
            // Add this to assign the next highest value to the Character Id instead of it defaulting to 0 because the DTO no longer contains the Id.
            // When using Entity Framework this is no longer necessary.
            character.Id = characters.Max(c => c.Id) + 1;

            characters.Add(character);

            serviceResponse.Data = characters.Select(c =>
                _mapper.Map<GetCharacterResponseDto>(c)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

            try
            {
                var character = characters.FirstOrDefault(c =>
                    c.Id == id
                );

                if (character is null)
                    throw new Exception($"Character with Id '{id}' not found");

                characters.Remove(character);

                serviceResponse.Data = characters.Select(c =>
                _mapper.Map<GetCharacterResponseDto>(c)).ToList();
            }
            catch (Exception e)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();
            serviceResponse.Data = characters.Select(c =>
                _mapper.Map<GetCharacterResponseDto>(c)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

            // Use LINQ to find character by Id. 
            var character = characters.FirstOrDefault(c =>
                c.Id == id
            );

            serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

            try
            {
                var character = characters.FirstOrDefault(c =>
                    c.Id == updatedCharacter.Id
                );

                if (character is null)
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found");

                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Defence = updatedCharacter.Defence;
                character.Class = updatedCharacter.Class;

                serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);
            }
            catch (Exception e)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }
    }
}

/// More on the ServiceResponse Wrapper:

// BEFORE 
/*  public async Task<List<Character>> GetAllCharacters()
    {
        return characters;
    }
    In swagger you see the data object as a list of characters.
    */


// AFTER
/*  public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
    {
        var serviceResponse = new ServiceResponse<List<Character>>();
        serviceResponse.Data = characters;
        return serviceResponse;
    }
    In Swagger you see a data object with the data (second level is the character info, was first level BEFORE.
    Plus the issuccess and message properties of the wrapper at first level (relating to ServiceResponse.cs)*/

// More on DTO's and why Automapper is used
// Using DTO's as a type and having the Data as a different type causes errors.