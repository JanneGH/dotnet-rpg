namespace dotnet_rpg.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; } = "Aladdin";
        public int HitPoints { get; set; } = 100;
        public int Strength { get; set; } = 10;
        public int Defence { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        // Relates to the Enum created for classes
        public RpgClass Class { get; set; } = RpgClass.Knight;

        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }

        /// Add relationship: a character can have one user & one weapon
        public User? User { get; set; }
        /// See Weapons model for explanation for determining main and dependent
        public Weapon? Weapon { get; set; }

        // A character can have many skills
        public List<Skill>? Skills { get; set; }

    }
}