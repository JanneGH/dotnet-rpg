namespace dotnet_rpg.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        /// With a DbSet you are able to query and save.
        /// The name is the name of the table.
        /// Whenever you want to see a representation of your model in the database you have to add a DbSet of that model.
        public DbSet<Character> Characters => Set<Character>();

        public DbSet<User> Users => Set<User>();

        public DbSet<Weapon> Weapons => Set<Weapon>();
    }
}