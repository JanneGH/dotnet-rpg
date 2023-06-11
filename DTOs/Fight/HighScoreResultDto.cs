namespace dotnet_rpg.DTOs.Fight
{
    /// TODO: only a result Dto, a request object is not needed. Dive into this.
    public class HighScoreResultDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}