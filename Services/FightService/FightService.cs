using dotnet_rpg.Data;
using dotnet_rpg.DTOs.Fight;

namespace dotnet_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        public FightService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<AttackResultResponseDto>> WeaponAttack(WeaponAttackRequestDto request)
        {
            var serviceResponse = new ServiceResponse<AttackResultResponseDto>();

            try
            {
                /// TODO: alternative: characters needed. Could have used the CharacterService to receive the attacker or access the context characters directly. 
                /// Only include the weapon and find the character who's ID matches the Request Attacker ID.

                var attacker = await _context.Characters
                    .Include(character => character.Weapon)
                    .FirstOrDefaultAsync(character => character.Id == request.AttackerId);

                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(character => character.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Weapon is null)
                {
                    throw new Exception("Something fishy is going on here");
                }

                int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                damage -= new Random().Next(opponent.Defence);

                if (damage > 0)
                {
                    opponent.HitPoints -= damage;
                }
                if (opponent.HitPoints <= 0)
                {
                    serviceResponse.Message = $"{opponent.Name} has been defeated";
                }

                await _context.SaveChangesAsync();

                // write the response data
                serviceResponse.Data = new AttackResultResponseDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHp = attacker.HitPoints,
                    OpponentHp = opponent.HitPoints,
                    Damage = damage
                };
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

/// Sidenote: There is already a CharacterService that gets a character by Id, which is needed here.
/// It would be possible to inject the ICharacterService and use the method GetCharacterById(id) here.
/// But it would require an authenticated user, which is time consuming in the tests, which is why this is not done here.