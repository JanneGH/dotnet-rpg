using dotnet_rpg.Data;
using dotnet_rpg.DTOs.Character;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        /*For only developing in Swagger (not db) create a static character list 
        private static List<Character> characters = new List<Character>() {
            new Character(),
            new Character { Id = 1, Name = "Ali Baba" }
        };
        */

        private readonly IMapper _mapper;
        private readonly DataContext _context;

        // Inject mapper & context so they are available in CharacterService
        public CharacterService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        /// In the CRUD operations below the Characters table is accessed with _context. 
        /// This is possible because the DataContext was injected in the constructor above.

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

            try
            {
                var character = _mapper.Map<Character>(newCharacter);

                _context.Characters.Add(character);

                // writes characters to db and writes a new id for the character.
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters.Select(c =>
                    _mapper.Map<GetCharacterResponseDto>(c)).ToListAsync();
            }
            catch (Exception e)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;

            ///PS
            /// The below was added to assign the next highest value to the Character Id instead of it defaulting to 0 because the DTO no longer contains the Id.
            /// When using SQL Server it is no longer necessary (automatically done)
            ///character.Id = characters.Max(c => c.Id) + 1;
        }

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

            try
            {
                // retrieve the character
                var character = await _context.Characters.FirstOrDefaultAsync(c =>
                    c.Id == id
                );

                if (character is null)
                    throw new Exception($"Character with Id '{id}' not found");

                /// remove with EF
                _context.Characters.Remove(character);
                /// save above Remove() change to db
                await _context.SaveChangesAsync();

                // Get response with updated list of characters
                serviceResponse.Data = await _context.Characters.Select(c =>
                _mapper.Map<GetCharacterResponseDto>(c)).ToListAsync();
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

            try
            {
                var dbCharacters = await _context.Characters.ToListAsync();
                serviceResponse.Data = dbCharacters.Select(c =>
                    _mapper.Map<GetCharacterResponseDto>(c)).ToList();
            }
            catch (Exception e)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = e.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

            try
            {
                /// Use LINQ to find character by Id. 
                var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c =>
                    c.Id == id
                );

                serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(dbCharacter);
            }
            catch (Exception e)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = e.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> UpdateCharacter(UpdateCharacterRequestDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

            try
            {
                // recieve character from db
                var character = await _context.Characters.FirstOrDefaultAsync(c =>
                    c.Id == updatedCharacter.Id
                );

                if (character is null)
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found");

                // change the values
                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Defence = updatedCharacter.Defence;
                character.Class = updatedCharacter.Class;

                // save to database 
                /// (no update method needed, all properties and calling the changes async is enough).
                await _context.SaveChangesAsync();

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

/// BEFORE 
/*  public async Task<List<Character>> GetAllCharacters()
    {
        return characters;
    }
    In swagger you see the data object as a list of characters.
    */


/// AFTER
/*  public async Task<ServiceResponse<List<Character>>> GetAllCharacters()
    {
        var serviceResponse = new ServiceResponse<List<Character>>();
        serviceResponse.Data = characters;
        return serviceResponse;
    }
    In Swagger you see a data object with the data (second level is the character info, was first level BEFORE.
    Plus the issuccess and message properties of the wrapper at first level (relating to ServiceResponse.cs)*/

/// More on DTO's and why Automapper is used:
/// Using DTO's as a type and having the Data as a different type causes errors.