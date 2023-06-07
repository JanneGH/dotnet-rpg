namespace dotnet_rpg.Models
{
    public class Weapon
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Damage { get; set; }
        public Character? Character { get; set; }

        /// EF Error & convention alert!
        /// To handle migration error "The dependent side could not be determined for the one-to-one relationship between 'Character.Weapon' and 'Weapon.Character'. To identify the dependent side of the relationship, configure the foreign key property."
        /// An explicit relation is needed. In this case, Weapon is the dependent: a Character can exist without a Weapon but not the other way around.
        /// With the help of that convention using the C# classname "Character" and then "Id", EF knows that this is the corresponding FK for the Character property. 
        /// Verification available in the Migrations Design file: Weapon is defined as a one-to-one relationship with Character ("HasOne()" and "WithOne()")
        public int CharacterId { get; set; }
    }
}