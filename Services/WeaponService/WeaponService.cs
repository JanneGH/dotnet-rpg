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
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<GetCharacterResponseDto>> AddWeapon(AddWeaponRequestDto newWeapon)
        {
            var response = new ServiceResponse<GetCharacterResponseDto>();

            try
            {
                // Get Character with the right Character ID in the Weapon object
                // Check if Character is an object that belongs to the current authenticated User
                var character = await _context.Characters
                    .FirstOrDefaultAsync(cha =>
                        cha.Id == newWeapon.CharacterId &&
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