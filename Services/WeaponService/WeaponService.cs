using System.Security.Claims;
using dotnet_rpg.Data;
using dotnet_rpg.DTOs.Character;
using dotnet_rpg.DTOs.Weapon;

namespace dotnet_rpg.Services.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            // to get the currently authorized user:
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> AddWeapon(AddWeaponRequestDto newWeapon)
        {
            var response = new ServiceResponse<GetCharacterResponseDto>();

            try
            {
                // Get Character with the right Character ID in the Weapon object from the Character context
                // Check if Character is an object that belongs to the current authorized User
                var character = await _context.Characters
                    .SingleOrDefaultAsync(cha =>
                        cha.Id == newWeapon.CharacterId &&
                        // Get the ID of the current User by accessing the NameIdentifyer claims value from the JSON webtoken.
                        cha.User!.Id == int.Parse(_httpContextAccessor.HttpContext!.User
                            .FindFirstValue(ClaimTypes.NameIdentifier)!)
                    );

                if (character is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Character not found";

                    return response;
                }

                // new Weapon instance
                /// TODO: alternatively add a new mapping from the AddWeaponRequestDto to the Weapon type (now the properties are manually set here)
                var weapon = new Weapon
                {
                    // add the given Name and Damage value to the new Weapon instance
                    Name = newWeapon.Name,
                    Damage = newWeapon.Damage,
                    // set the Character property of this new Weapon instance to the Character object retrieved from the database
                    Character = character
                };

                // add to db
                _context.Weapons.Add(weapon);

                // save changes
                await _context.SaveChangesAsync();

                // return character
                /// Mapper: map the character (source) entity to the GetCharacterResponseDto (destination)
                /// So mapper transfers data from the general entity to the DTO, as the DTO contains selected information (no ID for example) as a DTO does.
                response.Data = _mapper.Map<GetCharacterResponseDto>(character);

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}