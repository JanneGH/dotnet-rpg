using dotnet_rpg.DTOs.Fight;

namespace dotnet_rpg.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultResponseDto>> WeaponAttack(WeaponAttackRequestDto request);

        Task<ServiceResponse<AttackResultResponseDto>> SkillAttack(SkillAttackRequestDto request);
    }
}