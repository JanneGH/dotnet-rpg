using System.Security.Claims;
using dotnet_rpg.Data;
using dotnet_rpg.DTOs.Character;

namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContentAccessor;

        // Inject mapper & context so they are available in CharacterService
        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor httpContentAccessor)
        {
            _mapper = mapper;
            _context = context;
            _httpContentAccessor = httpContentAccessor;
        }

        private int GetuserId() => int.Parse(_httpContentAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// In the CRUD operations below the Characters table is accessed with _context. 
        /// This is possible because the DataContext was injected in the constructor above.

        public async Task<ServiceResponse<List<GetCharacterResponseDto>>> AddCharacter(AddCharacterRequestDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterResponseDto>>();

            try
            {
                var character = _mapper.Map<Character>(newCharacter);
                character.User = await _context.Users.FirstOrDefaultAsync(user =>
                    user.Id == GetuserId());

                _context.Characters.Add(character);

                // writes characters to db and writes a new id for the character.
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters
                    .Where(character => character.User!.Id == GetuserId())
                    .Select(character => _mapper.Map<GetCharacterResponseDto>(character))
                    .ToListAsync();
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
                var character = await _context.Characters.FirstOrDefaultAsync(character =>
                    character.Id == id && character.User!.Id == GetuserId()
                );

                if (character is null)
                    throw new Exception($"Character with Id '{id}' not found");

                /// remove with EF
                _context.Characters.Remove(character);
                /// save above Remove() change to db
                await _context.SaveChangesAsync();

                // Get response with updated list of characters
                serviceResponse.Data = await _context.Characters
                    .Where(character => character.User!.Id == GetuserId())
                    .Select(character => _mapper.Map<GetCharacterResponseDto>(character))
                    .ToListAsync();
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
                // only get characters related to a specific user
                /// passing the current user from the controller to the service
                /// Entity Framework enables access to the user object and its ID
                /// and get the characters that have the proper ID set in the db table.
                var dbCharacters = await _context.Characters
                    // included added later when weapons and skills were available to present the complete character picture
                    .Include(character => character.Weapon)
                    .Include(character => character.Skills)
                    .Where(character =>
                    character.User!.Id == GetuserId()).ToListAsync();

                serviceResponse.Data = dbCharacters.Select(character =>
                    _mapper.Map<GetCharacterResponseDto>(character)).ToList();
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
                var dbCharacter = await _context.Characters
                // included added later when weapons and skills were available to present the complete character picture
                    .Include(character => character.Weapon)
                    .Include(character => character.Skills)
                    .FirstOrDefaultAsync(character =>
                            character.Id == id && character.User!.Id == GetuserId()
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
                var character = await _context.Characters
                // to access related objects Include them
                .Include(character => character.User)
                .FirstOrDefaultAsync(character => character.Id == updatedCharacter.Id
                );

                if (character is null || character.User!.Id != GetuserId())
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

        public async Task<ServiceResponse<GetCharacterResponseDto>> AddCharacterSkill(AddCharacterSkillRequestDto newCharacterSkill)
        {
            // initialize serviceresponse
            var serviceResponse = new ServiceResponse<GetCharacterResponseDto>();

            try
            {
                // recieve correct character given by the character ID through the CharacterSkill DTO

                // access character:
                var character = await _context.Characters
                    .Include(character => character.Weapon)
                    .Include(character => character.Skills) // if you want to include much more (like side effects and stuff), use .thenInclude to continue including
                    .FirstOrDefaultAsync(character => character.Id == newCharacterSkill.CharacterId &&
                    character.User!.Id == GetuserId());

                if (character is null)
                {
                    serviceResponse.IsSuccess = false;
                    serviceResponse.Message = "Character not found";

                    return serviceResponse;
                }

                var skill = await _context.Skills
                    .FirstOrDefaultAsync(skill => skill.Id == newCharacterSkill.SkillId);

                if (skill is null)
                {
                    serviceResponse.IsSuccess = false;
                    serviceResponse.Message = "Skill not found";

                    return serviceResponse;
                }

                character.Skills!.Add(skill);
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterResponseDto>(character);

            }
            catch (Exception ex)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = ex.Message;
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

/// before a db connection is set up:
/*For only developing in Swagger (not db) create a static character list 
       private static List<Character> characters = new List<Character>() {
           new Character(),
           new Character { Id = 1, Name = "Ali Baba" }
       };
       */

/// In a specific controller method it is possible to
/// pass the current user from the controller to the service.
/// This was changed in the code because a specific user is needed for more than one operation.
/// This was the code in the GetAll request before that:
/// only get characters related to a specific user
/// passing the current user from the controller to the service

/// Entity Framework enables access to the user object and its ID
/// and get the characters that have the proper ID set in the db table.

/// var dbCharacters = await _context.Characters.Where(character =>
///    character.User!.Id == userId).ToListAsync();