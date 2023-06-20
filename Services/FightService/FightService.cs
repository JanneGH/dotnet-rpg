using dotnet_rpg.Data;
using dotnet_rpg.DTOs.Fight;

namespace dotnet_rpg.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public FightService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<ServiceResponse<FightResultResponseDto>> Fight(FightRequestDto request)
        {
            var serviceResponse = new ServiceResponse<FightResultResponseDto>
            {
                Data = new FightResultResponseDto()
            };

            try
            {
                // Get all characters

                // Access context
                var characters = await _context.Characters
                // Include weapon and skills,
                    .Include(character => character.Weapon)
                    .Include(character => character.Skills)
                    // Use "Where" to get all characters from the db that match the given ID's 
                    .Where(character => request.CharacterIds.Contains(character.Id))
                .ToListAsync();

                bool defeated = false;

                // While loop should stop after the first character is defeated.
                while (!defeated)
                {
                    // every character will partake in order
                    foreach (var attacker in characters)
                    {
                        // Filter all characters that don't have the ID of the attacker to get opponents.
                        var opponents = characters.Where(character => character.Id != attacker.Id).ToList();

                        // Randomly choose one opponent. Then pass opponents.Count to get random number to use as an index for the opponents list.
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        // vars to use for the result log
                        int damage = 0;
                        string attackUsed = string.Empty;

                        // bool to determine using eihter weapon or skill
                        bool useWeapon = new Random().Next(2) == 0;

                        // For using Weapons
                        if (useWeapon && attacker.Weapon is not null)
                        {
                            // set the name of attack
                            attackUsed = attacker.Weapon.Name;
                            //calculate damage
                            damage = DoWeaponAttack(attacker, opponent);

                        }
                        // for using Skills
                        else if (!useWeapon && attacker.Skills is not null)
                        {
                            // Choose skill randomly from available attacker skills
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }
                        else
                        {
                            serviceResponse.Data.FightLog.Add($"{attacker.Name} wasn't able to attack!");
                            continue;
                        }

                        serviceResponse.Data.FightLog.Add(
                            $"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage"
                        );

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;

                            attacker.Victories++;
                            opponent.Defeats++;

                            serviceResponse.Data.FightLog.Add(
                                $"{opponent.Name} has been defeated!");
                            serviceResponse.Data.FightLog.Add(
                                $"{attacker.Name} is victorious with {attacker.HitPoints} HP left!");

                            break;
                        }
                    }
                }

                characters.ForEach(character =>
                {
                    character.Fights++;
                    character.HitPoints = 100;
                });

                await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                serviceResponse.IsSuccess = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<AttackResultResponseDto>> SkillAttack(SkillAttackRequestDto request)
        {
            var serviceResponse = new ServiceResponse<AttackResultResponseDto>();

            try
            {
                /// TODO: alternative: characters needed. Could have used the CharacterService to receive the attacker or access the context characters directly. 
                /// Only include the skill and find the character who's ID matches the Request Attacker ID.

                var attacker = await _context.Characters
                    .Include(character => character.Skills)
                    .FirstOrDefaultAsync(character => character.Id == request.AttackerId);

                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(character => character.Id == request.OpponentId);

                if (attacker is null || opponent is null || attacker.Skills is null)
                {
                    throw new Exception("Something fishy is going on here");
                }

                /// Up to here was a copy of the WeaponAttack method.
                /// Get the correct Skill. Since the skills of the rpg character are included in the var above, there is no need to access the skills separately via the context. Instead verify if the attacker really has the specific called skill. 
                /// So: Initialize a new skill opbject and look through the attacker skills to find the one where the skill ID of the skill equals the request skill ID
                var attackerSkill = attacker.Skills.FirstOrDefault(skill => skill.Id == request.SkillId);

                if (attackerSkill is null)
                {
                    serviceResponse.IsSuccess = false;
                    serviceResponse.Message = $"{attacker.Name} does not have that skill";

                    return serviceResponse;
                }

                int damage = DoSkillAttack(attacker, opponent, attackerSkill);

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

                int damage = DoWeaponAttack(attacker, opponent);
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

        public async Task<ServiceResponse<List<HighScoreResultResponseDto>>> GetHighScore()
        {
            var characters = await _context.Characters
                .Where(character => character.Fights > 0)
                .OrderByDescending(character => character.Victories)
                .ThenBy(character => character.Defeats)
            .ToListAsync();

            var serviceResponse = new ServiceResponse<List<HighScoreResultResponseDto>>()
            {
                Data = characters.Select(character => _mapper.Map<HighScoreResultResponseDto>(character)).ToList()
            };

            return serviceResponse;
        }
        /// Extracted method. Made non-nullable.  
        /// No null warnings because nullcheck is done in the implementing SkillAttack method.
        private static int DoSkillAttack(Character attacker, Character opponent, Skill attackerSkill)
        {
            int damage = attackerSkill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defence);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        /// Extracted method. Made non-nullable. 
        /// No null warnings because nullcheck is done in the implementing WeaponAttack method.
        private static int DoWeaponAttack(Character attacker, Character opponent)
        {
            if (attacker.Weapon is null)
            {
                throw new Exception("Attacker has no weapon");
            }

            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defence);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }


    }
}

/// Sidenote: There is already a CharacterService that gets a character by Id, which is needed here.
/// It would be possible to inject the ICharacterService and use the method GetCharacterById(id) here.
/// But it would require an authenticated user, which is time consuming in the tests, which is why this is not done here.